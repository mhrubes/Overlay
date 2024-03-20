using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Data.SQLite;
using Application = System.Windows.Application;

namespace OverlayWPF
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public SQLiteConnection connection;
        DatabaseManager dbManager = new DatabaseManager();

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimerKeybinds = new DispatcherTimer();

        Dictionary<int, string> dataKeys = new Dictionary<int, string>();
        Keybinds keybinds = new Keybinds();
        KeyConverter converter = new KeyConverter();

        public ImageWindow()
        {
            InitializeComponent();

            connection = new SQLiteConnection("Data Source=WpfDatabase.sqlite;Version=3;");
            try
            {
                connection.Open();

                dataKeys = dbManager.GetDataFromIdMoreThanTwo();

                foreach (var item in dataKeys)
                {
                    if (item.Key == 2)
                        keybinds.WhenActive = (Key)converter.ConvertFromString(item.Value);
                    else if (item.Key == 3)
                        keybinds.WhenNotActive = (Key)converter.ConvertFromString(item.Value);
                    else if (item.Key == 4)
                        keybinds.HideShow = (Key)converter.ConvertFromString(item.Value);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            Loaded += MainWindow_Loaded;

            this.Topmost = true;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100ms
            dispatcherTimer.Start();

            dispatcherTimerKeybinds.Tick += new EventHandler(dispatcherTimerKeybinds_Tick);
            dispatcherTimerKeybinds.Interval = new TimeSpan(0, 0, 5); // 5s
            dispatcherTimerKeybinds.Start();
        }

        private void dispatcherTimerKeybinds_Tick(object sender, EventArgs e)
        {
            try
            {
                dataKeys = dbManager.GetDataFromIdMoreThanTwo();

                foreach (var item in dataKeys)
                {
                    if (item.Key == 2)
                        keybinds.WhenActive = (Key)converter.ConvertFromString(item.Value);
                    else if (item.Key == 3)
                        keybinds.WhenNotActive = (Key)converter.ConvertFromString(item.Value);
                    else if (item.Key == 4)
                        keybinds.HideShow = (Key)converter.ConvertFromString(item.Value);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(keybinds.WhenNotActive))
                Application.Current.Shutdown();

            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(keybinds.HideShow))
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
            {
                if (Application.Current.Windows.OfType<SettingWindow>().Any())
                {
                    SettingWindow settingWindow = new SettingWindow();
                    settingWindow.Close();
                }

                Application.Current.Shutdown();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Change_Window_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            dispatcherTimerKeybinds.Stop();

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

        private void Close_App_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void Setting_Open_Click(object sender, RoutedEventArgs e)
        {
            bool isSettingWindowOpen = Application.Current.Windows.OfType<SettingWindow>().Any();

            if (!isSettingWindowOpen)
            {
                SettingWindow settingWindow = new SettingWindow();
                settingWindow.Show();
            }
            else
                MessageBox.Show("Setting Window is already open.");
        }
    }
}
