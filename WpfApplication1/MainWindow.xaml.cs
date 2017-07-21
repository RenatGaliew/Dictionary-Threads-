using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
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
using Microsoft.Win32;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary MyDict;
        string Path { get; set; }
        List<string> FilesNames { get; set; }
        public MainWindow()
        {
            FilesNames = new List<string>();
            InitializeComponent();
            InitialPath();
            InitialFiles();
            MyDict = new Dictionary(Path,FilesNames);
            InitialWords(MyDict);
        }

        private void InitialFiles()
        {
            FilesNames.Clear();
            lboxNameFiles.Items.Clear();
            lboxWords.Items.Clear();
            string[] files = Directory.GetFiles(Path, "*.txt");
            foreach (string s in files)
            {
                lboxNameFiles.Items.Add(System.IO.Path.GetFileName(s));
                FilesNames.Add(s);
            }
        }

        private void InitialPath()
        {
            tbDirectoryName.Text = Directory.GetCurrentDirectory();
            Path = Directory.GetCurrentDirectory();

        }
        
        private void InitialWords(Dictionary D)
        {
            lboxWords.Items.Clear();
            foreach(Word s in D.ListWords)
            {
                lboxWords.Items.Add(s.Name);
            }

        }

        private void btnViewDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openDirectoryDialog = new FolderBrowserDialog();
            if (openDirectoryDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbDirectoryName.Text = openDirectoryDialog.SelectedPath;
                Path = openDirectoryDialog.SelectedPath;
                string [] files = Directory.GetFiles(openDirectoryDialog.SelectedPath, "*.txt");
                foreach (string s in files)
                {
                    lboxNameFiles.Items.Add(System.IO.Path.GetFileName(s));
                    FilesNames.Add(s);
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            MyDict = new Dictionary(Path, FilesNames);
            MyDict.Start();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MyDict.ReadFile(@"Тестовое задание соискателю на должность «программист С#»

Написать программу, которая читает в несколько программных потоков текстовые файлы из указанной папки и составляет словарь слов с указанием сколько раз встретилось каждое слово.
В отдельном потоке каждую секунду выбирается случайное слово из словаря и узнаётся сколько раз оно встретилось на текущий момент.
");
            InitialWords(MyDict);
        }

        private void SelectionIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lboxWords.SelectedIndex != -1)
            {
                tbCount.Text = MyDict.ListWords[lboxWords.SelectedIndex].Count.ToString();
            }
        }
    }
}
