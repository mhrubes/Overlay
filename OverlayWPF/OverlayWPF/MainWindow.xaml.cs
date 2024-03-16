using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.IO;
using System.Data.SQLite;

namespace OverlayWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragging;
        private Point lastMousePosition;

        public SQLiteConnection connection;
        public string FilePath;

        public MainWindow()
        {
            InitializeComponent();

            connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            try
            {
                connection.Open();

                DatabaseManager dbManager = new DatabaseManager();
                string path = dbManager.GetFirstFilePath();
                FilePath = path;

                if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
                {
                    PathName.Text = FilePath;
                    ShowFileContent(FilePath);
                }
                else
                {
                    PathName.Text = "Not Selected .txt file";
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }

            this.Topmost = true;
            slider.ValueChanged += Slider_ValueChanged;
        }
        private void ShowFileContent(string filePath)
        {
            try
            {
                string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                //Clipboard.SetText(fileContent);

                // Clear existing content
                textblockText.Inlines.Clear();

                // Define default color
                Brush defaultColor = Brushes.White;

                // Process each character in the file content
                for (int i = 0; i < fileContent.Length; i++)
                {
                    char currentChar = fileContent[i];

                    // Check for special markers
                    if (currentChar == '\\')
                    {
                        if (i + 1 < fileContent.Length)
                        {
                            char nextChar = fileContent[i + 1];

                            // Handle color markers
                            if (nextChar == 'r')
                            {
                                // Set color to red until the next occurrence of '\r'
                                defaultColor = Brushes.Red;

                                // Find the index of the next occurrence of '\r'
                                int endIndex = fileContent.IndexOf("\\r", i + 2);

                                // If '\r' is found, process text until the end index
                                if (endIndex != -1)
                                {
                                    for (int j = i + 2; j < endIndex; j++)
                                    {
                                        Run run = new Run(fileContent[j].ToString());
                                        run.Foreground = defaultColor;
                                        textblockText.Inlines.Add(run);
                                    }
                                    i = endIndex + 1; // Skip the '\r' and continue processing
                                    defaultColor = Brushes.White; // Reset color to default
                                    continue;
                                }
                            }
                            else if (nextChar == 'g')
                            {
                                // Set color to green until the next occurrence of '\g'
                                defaultColor = Brushes.Green;

                                // Find the index of the next occurrence of '\g'
                                int endIndex = fileContent.IndexOf("\\g", i + 2);

                                // If '\g' is found, process text until the end index
                                if (endIndex != -1)
                                {
                                    for (int j = i + 2; j < endIndex; j++)
                                    {
                                        Run run = new Run(fileContent[j].ToString());
                                        run.Foreground = defaultColor;
                                        textblockText.Inlines.Add(run);
                                    }
                                    i = endIndex + 1; // Skip the '\g' and continue processing
                                    defaultColor = Brushes.White; // Reset color to default
                                    continue;
                                }
                            }
                        }
                    }

                    // Create a run for the current character with the determined color
                    Run defaultRun = new Run(currentChar.ToString());
                    defaultRun.Foreground = defaultColor;
                    textblockText.Inlines.Add(defaultRun);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void File_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Textové soubory (*.txt)|*.txt|Všechny soubory (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;
                try
                {
                    DatabaseManager dbManager = new DatabaseManager();
                    dbManager.InsertFilePath(selectedFileName);

                    PathName.Text = selectedFileName;
                    ShowFileContent(selectedFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nastala chyba: {ex.Message}");
                }
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
