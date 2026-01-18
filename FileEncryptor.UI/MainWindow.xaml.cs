using FileEncryptor.Core;
using FileEncryptor.Core.Enums;
using FileEncryptor.Core.Interfaces;
using FileEncryptor.Core.Models;
using FileEncryptor.Core.Services;
using FileEncryptor.UI.Views;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Path = System.IO.Path;

namespace FileEncryptor.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isMenuOpen = true;
        public MainWindow()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                PageContainer.Content = new EncryptionView();
                BtnSteganography.BorderThickness = new Thickness(4, 0, 0, 0);
                BtnSteganography.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
                SetActiveMenuButton(BtnEncryption);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Menu_Encryption_Click(object sender, RoutedEventArgs e)
        {
            PageContainer.Content = new EncryptionView();
            SetActiveMenuButton(BtnEncryption);
        }
        private void Menu_Stegano_Click(object sender, RoutedEventArgs e)
        {
            PageContainer.Content = new SteganoView();
            SetActiveMenuButton(BtnSteganography);
        }

        private void BtnToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            double targetWidth = _isMenuOpen ? 0 : 200;

            DoubleAnimation animation = new DoubleAnimation();
            animation.To = targetWidth;
            animation.Duration = TimeSpan.FromSeconds(0.3); 
            animation.EasingFunction = new QuadraticEase(); 

            SideMenu.BeginAnimation(Border.WidthProperty, animation);

            _isMenuOpen = !_isMenuOpen;
        }

        private void SetActiveMenuButton(Button activeButton)
        {
            BtnEncryption.Background = Brushes.Transparent;
            BtnEncryption.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204));

            BtnSteganography.Background = Brushes.Transparent;
            BtnSteganography.Foreground = new SolidColorBrush(Color.FromRgb(204, 204, 204)); 

            activeButton.Background = new SolidColorBrush(Color.FromRgb(63, 63, 70));
            activeButton.Foreground = Brushes.White;

            activeButton.BorderThickness = new Thickness(4, 0, 0, 0);
            activeButton.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212)); 
        }
    }
}