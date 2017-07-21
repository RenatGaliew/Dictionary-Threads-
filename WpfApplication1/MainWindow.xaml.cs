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
using System.Threading;

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
        Mutex MyMutex = new Mutex();
        Thread t;
        Thread WordThread;
        bool pause = false;
        public MainWindow()
        {
            FilesNames = new List<string>();
            InitializeComponent();
            InitialPath();
            InitialFiles();
            MyDict = new Dictionary(Path,FilesNames,MyMutex);
            t = new Thread(new ThreadStart(InitialWords));
            WordThread = new Thread(new ThreadStart(RandomizeWord));
            //Запуск потока с отображением случайного слова каждую секунду
            WordThread.Start();
            //Запуск потока с отображением сколько слов в словаре всего
            t.Start();
        }

        /// <summary>
        /// Инициализация имен файлов
        /// </summary>
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

        /// <summary>
        /// Инициализация пути к папке
        /// </summary>
        private void InitialPath()
        {
            tbDirectoryName.Text = Directory.GetCurrentDirectory();
            Path = Directory.GetCurrentDirectory();

        }

        /// <summary>
        /// Инициализация слов
        /// </summary>
        private void InitialWords()
        {
            while (!pause)
            {
                MyMutex.WaitOne();
                foreach (Word s in MyDict.ListWords)
                {
                    this.Dispatcher.Invoke((ThreadStart)delegate {
                        if (lboxWords.Items.IndexOf(s.Name) == -1)
                            lboxWords.Items.Add(s.Name);
                        tbCount_Copy.Text = lboxWords.Items.Count.ToString();
                    });
                }
                MyMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Выбор случаного слова из словаря
        /// </summary>
        private void RandomizeWord()
        {
            while(!pause)
            {
                Thread.Sleep(1000);
                MyMutex.WaitOne();
                if (MyDict.ListWords.Count > 0)
                {
                    int i = MyDict.ListWords.Count;
                    Random r = new Random();
                    int n = r.Next(0, i);
                    lboxWords.Dispatcher.Invoke((ThreadStart)delegate
                    {
                        lboxWords.SelectedIndex = n;
                        tbWord.Text = MyDict.ListWords[lboxWords.SelectedIndex].Name.ToString();
                        tbCount.Text = MyDict.ListWords[lboxWords.SelectedIndex].Count.ToString();
                    });
                }
                MyMutex.ReleaseMutex();
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
            MyDict = new Dictionary(Path, FilesNames, MyMutex);
            MyDict.Start();
            //InitialWords(MyDict);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MyDict.ReadFile(@"Тестовое задание соискателю на должность «программист С#»

Написать программу, которая читает в несколько программных потоков текстовые файлы из указанной папки и составляет словарь слов с указанием сколько раз встретилось каждое слово.
В отдельном потоке каждую секунду выбирается случайное слово из словаря и узнаётся сколько раз оно встретилось на текущий момент.
");
            //InitialWords(MyDict);
        }

        private void SelectionIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lboxWords.SelectedIndex != -1)
            {
                tbWord.Text = MyDict.ListWords[lboxWords.SelectedIndex].Name.ToString();
                tbCount.Text = MyDict.ListWords[lboxWords.SelectedIndex].Count.ToString();
            }
        }

        private void lboxNameFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var text = "";
            if(lboxNameFiles.SelectedIndex != -1)
                text = System.IO.File.ReadAllText((string)FilesNames[lboxNameFiles.SelectedIndex], System.Text.Encoding.GetEncoding(1251));
            tbContain.Text = text;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            pause = true;
            MyDict.pause = true;
        }

        /// <summary>
        /// Удаление из памяти и завершение потоков
        /// </summary>
        ~MainWindow()
        {
            pause = true;
            t.Abort();
            WordThread.Abort();
            FilesNames.Clear();
        }
    }
}
