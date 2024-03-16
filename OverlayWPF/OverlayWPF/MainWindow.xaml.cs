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

            KeyGesture quitKeyGesture = new KeyGesture(Key.Q, ModifierKeys.Alt);

            // Vytvoření vazby pro klávesovou kombinaci
            InputBinding quitInputBinding = new InputBinding(ApplicationCommands.Close, quitKeyGesture);

            // Přidání vazby do kolekce vazeb okna
            this.InputBindings.Add(quitInputBinding);


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
            //slider.ValueChanged += Slider_ValueChanged;
        }

        private void ShowFileContent(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                // Clear existing content
                textblockText.Inlines.Clear();

                // Define default color
                Brush defaultColor = Brushes.White;

                // Process each line in the file content
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        // Process each character in the line
                        for (int i = 0; i < line.Length; i++)
                        {
                            char currentChar = line[i];

                            // Check for special markers
                            if (currentChar == '\\')
                            {
                                if (i + 1 < line.Length)
                                {
                                    char nextChar = line[i + 1];

                                    // Handle color markers
                                    if (nextChar == 'r')
                                    {
                                        // Set color to orangeRed until the next occurrence of '\r'
                                        defaultColor = Brushes.OrangeRed;

                                        // Find the index of the next occurrence of '\r'
                                        int endIndex = line.IndexOf("\\r", i + 2);

                                        // If '\r' is found, process text until the end index
                                        if (endIndex != -1)
                                        {
                                            for (int j = i + 2; j < endIndex; j++)
                                            {
                                                Run run = new Run(line[j].ToString());
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
                                        defaultColor = Brushes.SpringGreen;

                                        // Find the index of the next occurrence of '\g'
                                        int endIndex = line.IndexOf("\\g", i + 2);

                                        // If '\g' is found, process text until the end index
                                        if (endIndex != -1)
                                        {
                                            for (int j = i + 2; j < endIndex; j++)
                                            {
                                                Run run = new Run(line[j].ToString());
                                                run.Foreground = defaultColor;
                                                textblockText.Inlines.Add(run);
                                            }
                                            i = endIndex + 1; // Skip the '\g' and continue processing
                                            defaultColor = Brushes.White; // Reset color to default
                                            continue;
                                        }
                                    }
                                    else if (nextChar == 'b')
                                    {
                                        // Set color to skyblue until the next occurrence of '\b'
                                        defaultColor = Brushes.SkyBlue;

                                        // Find the index of the next occurrence of '\b'
                                        int endIndex = line.IndexOf("\\b", i + 2);

                                        // If '\b' is found, process text until the end index
                                        if (endIndex != -1)
                                        {
                                            for (int j = i + 2; j < endIndex; j++)
                                            {
                                                Run run = new Run(line[j].ToString());
                                                run.Foreground = defaultColor;
                                                textblockText.Inlines.Add(run);
                                            }
                                            i = endIndex + 1; // Skip the '\b' and continue processing
                                            defaultColor = Brushes.White; // Reset color to default
                                            continue;
                                        }
                                    }
                                    else if (nextChar == 'y')
                                    {
                                        // Set color to yellow until the next occurrence of '\y'
                                        defaultColor = Brushes.Yellow;

                                        // Find the index of the next occurrence of '\y'
                                        int endIndex = line.IndexOf("\\y", i + 2);

                                        // If '\y' is found, process text until the end index
                                        if (endIndex != -1)
                                        {
                                            for (int j = i + 2; j < endIndex; j++)
                                            {
                                                Run run = new Run(line[j].ToString());
                                                run.Foreground = defaultColor;
                                                textblockText.Inlines.Add(run);
                                            }
                                            i = endIndex + 1; // Skip the '\y' and continue processing
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

                    // Add a line break after each processed line
                    textblockText.Inlines.Add(new LineBreak());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
                Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
