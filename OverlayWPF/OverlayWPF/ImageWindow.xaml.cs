using Microsoft.Win32;
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
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public ImageWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            this.Topmost = true;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100ms
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.Q))
                this.Close();

            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.H))
            {
                if (this.Visibility == Visibility.Visible)
                    this.Visibility = Visibility.Hidden;
                else
                    this.Visibility = Visibility.Visible;
            }
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

                    int width = imageSource.PixelWidth;
                    int height = imageSource.PixelHeight;

                    imageControl.Width = width;
                    imageControl.Height = height;

                    this.Height = 400;
                    this.Width = 400;

                    //this.MaxHeight = 2000;
                    //this.MaxWidth = 2000;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
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
            dispatcherTimer.Stop();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            this.Close();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = (Brush)FindResource("onMouseEnterBackgroundColor");
                button.FontWeight = FontWeights.Bold;
                button.Foreground = Brushes.White;
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                button.Background = (Brush)FindResource("onMouseLeaveBackgroundColor");
                button.FontWeight = FontWeights.Normal;
                button.Foreground = Brushes.Black;
            }
        }

        private void Close_App_Click(object sender, RoutedEventArgs e) => this.Close();

    }
}
