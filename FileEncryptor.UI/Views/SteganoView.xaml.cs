using FileEncryptor.Core.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileEncryptor.UI.Views
{
    /// <summary>
    /// Interaction logic for SteganoView.xaml
    /// </summary>
    public partial class SteganoView : UserControl
    {
        private Bitmap _loadedBitmap;
        public SteganoView()
        {
            InitializeComponent();
        }

        private void TxtFileName_DoubleClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var tempBitmap = new Bitmap(dlg.FileName);
                    _loadedBitmap = new Bitmap(tempBitmap);
                    tempBitmap.Dispose();

                    ImgPreview.Source = BitmapToImageSource(_loadedBitmap);

                    TxtFileName.Text = System.IO.Path.GetFileName(dlg.FileName);
                    TxtStatus.Text = "The image was saved succesfully.";
                    TxtMessage.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading the picture: " + ex.Message);
                }
            }
        }

        private void TxtFileName_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;

                var txtBox = sender as TextBox;
                if (txtBox != null)
                {
                    txtBox.BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4CC2FF"));
                    txtBox.BorderThickness = new Thickness(2);
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void TxtFileName_PreviewDrop(object sender, DragEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0 && txtBox != null)
                {
                    txtBox.Text = files[0];
                }
            }

            if (txtBox != null)
            {
                ResetTextBoxStyle(txtBox);
            }
        }
        private void TxtFileName_DragLeave(object sender, DragEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox != null)
                ResetTextBoxStyle(txtBox);
        }
        private void ResetTextBoxStyle(TextBox txtBox)
        {
            txtBox.ClearValue(Control.BorderBrushProperty);
            txtBox.ClearValue(Control.BorderThicknessProperty);
        }



        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Choose an Image";
            dlg.Filter = "Képfájlok|*.png;*.jpg;*.jpeg;*.bmp";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var tempBitmap = new Bitmap(dlg.FileName);
                    _loadedBitmap = new Bitmap(tempBitmap);
                    tempBitmap.Dispose();

                    ImgPreview.Source = BitmapToImageSource(_loadedBitmap);

                    TxtFileName.Text = System.IO.Path.GetFileName(dlg.FileName);
                    TxtStatus.Text = "The image was saved succesfully.";
                    TxtMessage.Clear(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading the picture: " + ex.Message);
                }
            }
        }

        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (_loadedBitmap == null)
            {
                MessageBox.Show("Please upload an image!");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtMessage.Text))
            {
                MessageBox.Show("Please fill in the text field!");
                return;
            }

            try
            {
                TxtStatus.Text = "Proccessing...";

                Bitmap encryptedImage = SteganoService.EmbedText(_loadedBitmap, TxtMessage.Text);

                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "PNG image|*.png";
                saveDlg.Title = "Saving the encrypted image";
                saveDlg.FileName = "secret_image.png"; 

                if (saveDlg.ShowDialog() == true)
                {
                    encryptedImage.Save(saveDlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    TxtStatus.Text = "Saving was succesful: " + saveDlg.FileName;
                    MessageBox.Show("Done. The message was encrypted in the picture.");

                    _loadedBitmap = encryptedImage;
                    ImgPreview.Source = BitmapToImageSource(_loadedBitmap);
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Something went wrong! Please try again.";
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            if (_loadedBitmap == null)
            {
                MessageBox.Show("Upload the encrypted picture");
                return;
            }

            try
            {
                TxtStatus.Text = "Decrypting...";
                
                string secretMessage = SteganoService.ExtractText(_loadedBitmap);

                TxtMessage.Text = secretMessage;
                TxtStatus.Text = "The messege was extracted succesfully.";

                if (string.IsNullOrEmpty(secretMessage))
                {
                    MessageBox.Show("Something went wrong. Please try again.");
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Error in the reading.";
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad; 
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }    
    }
}
