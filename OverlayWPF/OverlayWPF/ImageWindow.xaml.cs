﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace OverlayWPF
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();

            widthBox.Text = this.Width.ToString();
            heightBox.Text  = this.Height.ToString();

            this.Topmost = true;
        }

        private void Image_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                try
                {
                    BitmapImage imageSource = new BitmapImage(new Uri(selectedFileName));
                    imageControl.Source = imageSource;

                    this.MinHeight = 250;
                    this.Height = 500;
                    this.Width = 500;

                    widthBox.Text = this.Width.ToString();
                    heightBox.Text = this.Height.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void WidthBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWindowValues("width");
        }

        private void WidthBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateWindowValues("width");
        }

        private void HeightBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateWindowValues("height");
        }

        private void HeightBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateWindowValues("height");
        }

        private void UpdateWindowValues(string boxType)
        {
            if (boxType == "width")
            {
                if (double.TryParse(widthBox.Text, out double width))
                    this.Width = width;
                else
                    MessageBox.Show("Error");
            }

            if (boxType == "height")
            {
                if (double.TryParse(heightBox.Text, out double height))
                    this.Height = height;
                else
                    MessageBox.Show("Error");
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
                System.Windows.Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Change_Window_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }
    }
}
