using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
using System.Windows.Shapes;

namespace OverlayWPF
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SQLiteConnection connection;

        Dictionary<int, string> dataKeys = new Dictionary<int, string>();

        private Key capturedKey;
        private string buttonClick = "";

        KeyConverter converter = new KeyConverter();
        Keybinds keybinds = new Keybinds();

        public SettingWindow()
        {
            InitializeComponent();

            connection = new SQLiteConnection("Data Source=WpfDatabase.sqlite;Version=3;");
            try
            {
                connection.Open();

                DatabaseManager dbManager = new DatabaseManager();
                dataKeys = dbManager.GetDataFromIdMoreThanTwo();

                foreach (var item in dataKeys)
                {
                    if (item.Key == 2)
                    {
                        When_Active_Block.Text = item.Value;
                        keybinds.WhenActive = (Key)converter.ConvertFromString(item.Value);
                    }
                    else if (item.Key == 3)
                    {
                        When_Not_Active_Block.Text = "Alt + " + item.Value;
                        keybinds.WhenNotActive = (Key)converter.ConvertFromString(item.Value);

                    }
                    else if (item.Key == 4)
                    {
                        Show_Hide_Block.Text = "Alt + " + item.Value;
                        keybinds.HideShow = (Key)converter.ConvertFromString(item.Value);
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            this.Topmost = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            capturedKey = e.Key;

            if (buttonClick == "when_active")
            {
                When_Active_Block.Text = capturedKey.ToString();
                keybinds.WhenActive = capturedKey;
            }
            else if (buttonClick == "when_not_active")
            {
                if (capturedKey != keybinds.HideShow)
                {
                    When_Not_Active_Block.Text = "Alt + " + capturedKey.ToString();
                    keybinds.WhenNotActive = capturedKey;
                }
                else
                    MessageBox.Show($"{capturedKey} is already set for another shortcut");
            }
            else if (buttonClick == "hide_show")
            {
                if (capturedKey != keybinds.WhenNotActive)
                {
                    Show_Hide_Block.Text = "Alt + " + capturedKey.ToString();
                    keybinds.HideShow = capturedKey;
                }
                else
                    MessageBox.Show($"{capturedKey} is already set for another shortcut");
            }

            this.KeyDown -= Window_KeyDown;
        }

        private void When_Active_Button_Click(object sender, RoutedEventArgs e)
        {
            buttonClick = "when_active";
            this.KeyDown += Window_KeyDown;
        }

        private void When_Not_Active_Button_Click(object sender, RoutedEventArgs e)
        {
            buttonClick = "when_not_active";
            this.KeyDown += Window_KeyDown;
        }

        private void Show_Hide_Button_Click(object sender, RoutedEventArgs e)
        {
            buttonClick = "hide_show";
            this.KeyDown += Window_KeyDown;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, string> keys = new Dictionary<int, string>();

            try
            {
                keys.Add(2, keybinds.WhenActive.ToString());
                keys.Add(3, keybinds.WhenNotActive.ToString());
                keys.Add(4, keybinds.HideShow.ToString());

                DatabaseManager dbManager = new DatabaseManager();
                dbManager.SaveHotkeys(keys);

                MessageBox.Show("Saved");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseManager dbManager = new DatabaseManager();
                dbManager.ResetHotkeys(dataKeys);

                keybinds.WhenActive = Key.Q;
                keybinds.WhenNotActive = Key.Q;
                keybinds.HideShow = Key.H;

                When_Active_Block.Text = keybinds.WhenActive.ToString();
                When_Not_Active_Block.Text = "Alt + " + keybinds.WhenNotActive.ToString();
                Show_Hide_Block.Text = "Alt + " + keybinds.HideShow.ToString();

                MessageBox.Show("Reset was successful");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_App_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
