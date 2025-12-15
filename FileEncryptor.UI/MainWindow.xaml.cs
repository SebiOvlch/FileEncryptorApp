using FileEncryptor.Core;
using FileEncryptor.Core.Interfaces;
using FileEncryptor.Core.Models;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace FileEncryptor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICryptoService _cryptoService = new FileCryptoService();

        private CancellationTokenSource _cts;
        public MainWindow()
        {
            InitializeComponent();
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            cbAlgorithm.ItemsSource = Enum.GetValues(typeof(SupportedAlgorithm));
            cbCipherMode.ItemsSource = Enum.GetValues(typeof(CipherMode));
            cbPaddingMode.ItemsSource = Enum.GetValues(typeof(PaddingMode));

            cbAlgorithm.SelectedItem = SupportedAlgorithm.Aes;
            cbCipherMode.SelectedItem = CipherMode.CBC;
            cbPaddingMode.SelectedItem = PaddingMode.PKCS7;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                txtFilePath.Text = openFileDialog.FileName;
                lblStatus.Text = "The file is ready..";
                progressBar.Value = 0;
            }
        }

        private async void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteCryptoOperation(CryptoAction.Encrypt);
        }

        private async void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteCryptoOperation(CryptoAction.Decrypt);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private async Task ExecuteCryptoOperation(CryptoAction action)
        {
            if(string.IsNullOrEmpty(txtFilePath.Text) || !File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Please choose a valid and existing file!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please choose a password!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ToggleControls(false);

            string suffix = action == CryptoAction.Encrypt ? "_encrypted" : "_decrypted";
            string outputPath = Path.Combine(
                Path.GetDirectoryName(txtFilePath.Text),
                Path.GetFileNameWithoutExtension(txtFilePath.Text) + suffix + Path.GetExtension(txtFilePath.Text)
            );

            var options = new CryptoOptions
            {
                InputFilePath = txtFilePath.Text,
                OutputFilePath = outputPath,
                Password = txtPassword.Text,
                Algorithm = (SupportedAlgorithm)cbAlgorithm.SelectedItem,
                Mode = (CipherMode)cbCipherMode.SelectedItem,
                Padding = (PaddingMode)cbPaddingMode.SelectedItem,
                Action = action
            };

            var progressHandler = new Progress<double>(percent =>
            {
                progressBar.Value = percent;
                lblStatus.Text = $"{action} in progress... {percent:F0}%";
            });

            _cts = new CancellationTokenSource();

            try
            {
                await _cryptoService.ProcessFileAsync(options, progressHandler, _cts.Token);

                lblStatus.Text = "Done! ✅";
                MessageBox.Show($"Succes!\nFile saved: {outputPath}", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Cancelled 🛑";
                if (File.Exists(outputPath)) File.Delete(outputPath);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Something went wrong! ❌";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ToggleControls(true);
                _cts.Dispose();
                _cts = null;
            }
        }

        private void ToggleControls(bool isEnabled)
        {
            btnEncrypt.IsEnabled = isEnabled;
            btnDecrypt.IsEnabled = isEnabled;
            btnCancel.IsEnabled = !isEnabled;
            txtFilePath.IsEnabled = isEnabled;
            txtPassword.IsEnabled = isEnabled;
            this.Cursor = isEnabled ? System.Windows.Input.Cursors.Arrow : System.Windows.Input.Cursors.Wait;
        }
    }
}