using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace WpfApplication1
{
    class Dictionary
    {
        /// <summary>
        /// Переменная обозначающая паузу
        /// </summary>
        public bool pause = false;
        /// <summary>
        /// Список слов
        /// </summary>
        public List<Word> ListWords;

        /// <summary>
        /// Список файлов
        /// </summary>
        List<string> ListFiles;

        /// <summary>
        /// Список потоков
        /// </summary>
        List<Thread> ListThreads;

        /// <summary>
        /// Мьютекс
        /// </summary>
        Mutex MyMutex;

        /// <summary>
        /// Имя директории
        /// </summary>
        string NameDirectory { get; set; }

        /// <summary>
        /// Конструктор для класса словаря
        /// </summary>
        /// <param name="Path">Путь к папке</param>
        /// <param name="files">Список файлов</param>
        /// <param name="m">Мьютекс из внешнего потока</param>
        public Dictionary(string Path, List<string> files, Mutex m)
        {
            this.NameDirectory = Path;
            ListWords = new List<Word>();
            ListFiles = files;
            ListThreads = new List<Thread>();
            MyMutex = m;
        }

        /// <summary>
        /// Запускает алгоритм создания и запусков потоков
        /// </summary>
        public void Start()
        {
            //создание и запуск потоков под каждый файл
            foreach (string s in ListFiles)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ReadFile));
                t.Start(s);
                ListThreads.Add(t);
            }
        }

        /// <summary>
        /// Чтение текста из FileName
        /// </summary>
        /// <param name="FileName">Полный путь к файлу</param>
        public void ReadFile(object FileName)
        {
            var text = System.IO.File.ReadAllText((string)FileName, System.Text.Encoding.GetEncoding(1251));
            text = text.Replace("\r", " ");
            text = text.Replace("\n", " ");
            text = text.Replace("\t", " ");
            text = text.Replace("\a", " ");
            text = text.Replace("\b", " ");
            text = text.Replace("\v", " ");
            text = text.Replace("\f", " ");
            text = text.Replace("\0", " ");
            string stroka = "";
            bool Begin = true;
            int i = 0;
            text = text.ToLower();
            foreach (char c in text)
            {
                if(pause)
                    return;
                if (Begin)
                {
                    Begin = false;
                    if (c != '\n' && c != '-' && Char.IsLetter(c))
                        stroka += c.ToString();
                }
                else
                {
                    if (c != '\n' && c == '-' || Char.IsLetter(c))
                        stroka += c.ToString();
                }
                if (c != '\n' && i == text.Length - 1 || c != '-' && !Char.IsLetter(c) && stroka != "")
                {
                    if (stroka != "" && stroka[stroka.Length - 1] == '-')
                        stroka = stroka.Replace("-", "");
                    Word w = new Word(stroka);
                    Thread.Sleep(300);
                    MyMutex.WaitOne();
                    if (ListWords.FindIndex(x => x.Equals(w)) >= 0)
                    {
                        ListWords[ListWords.FindIndex(x => x.Equals(w))].Count++;
                    }
                    else
                        ListWords.Add(w);
                    MyMutex.ReleaseMutex();
                    stroka = "";
                    Begin = true;
                }
                i++;
            }
        }

        //public void ReadFile(string text)
        //{
        //    string stroka = "";
        //    bool Begin = true;
        //    int i = 0;
        //    text = text.ToLower();
        //    foreach (char c in text)
        //    {
        //        if (Begin)
        //        {
        //            Begin = false;
        //            if (c!= '\n' && c != '-' && Char.IsLetter(c))
        //                stroka += c.ToString();
        //        }
        //        else
        //        {
        //            if (c != '\n' && c == '-' || Char.IsLetter(c))
        //                stroka += c.ToString();
        //        }
        //        if (c != '\n' && i == text.Length-1 || c != '-' && !Char.IsLetter(c) && stroka != "")
        //        {
        //            if (stroka != "" && stroka[stroka.Length - 1] == '-')
        //                stroka = stroka.Replace("-", "");
        //            Word w = new Word(stroka);
        //            if (ListWords.FindIndex(x => x.Equals(w)) >= 0)
        //            {
        //                ListWords[ListWords.FindIndex(x => x.Equals(w))].Count++;
        //            }
        //            else
        //                ListWords.Add(w);
        //            stroka = "";
        //            Begin = true;
        //        }
        //        i++;
        //    }
        //}
        ~Dictionary()  // destructor
        {
            pause = true;
            ListFiles.Clear();
            ListWords.Clear();
            foreach (Thread t in ListThreads)
                t.Abort();
            ListThreads.Clear();
            // cleanup statements...
        }
    }
}
