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
        public List<Word> ListWords;
        List<string> ListFiles;
        List<Thread> ListThreads;
        Regex r = new Regex(@"[а-яА-Яa-zA-ZёЁ]");
        string NameDirectory { get; set; }
        public Dictionary(string Path, List<string> files)
        {
            this.NameDirectory = Path;
            ListWords = new List<Word>();
            ListFiles = files;
            ListThreads = new List<Thread>();
        }

        public void Start()
        {
            foreach (string s in ListFiles)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ReadFile));
                ListThreads.Add(t);
            }
        }
        public void ReadFile(object FileName)
        {
            var text = System.IO.File.ReadAllText((string)FileName);
            string stroka = "";
            bool Begin = true;
            int i = 0;
            text = text.ToLower();
            foreach (char c in text)
            {
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
                    if (ListWords.FindIndex(x => x.Equals(w)) >= 0)
                    {
                        ListWords[ListWords.FindIndex(x => x.Equals(w))].Count++;
                    }
                    else
                        ListWords.Add(w);
                    stroka = "";
                    Begin = true;
                }
                i++;
            }
        }

        public void ReadFile(string text)
        {
            string stroka = "";
            bool Begin = true;
            int i = 0;
            text = text.ToLower();
            foreach (char c in text)
            {
                if (Begin)
                {
                    Begin = false;
                    if (c!= '\n' && c != '-' && Char.IsLetter(c))
                        stroka += c.ToString();
                }
                else
                {
                    if (c != '\n' && c == '-' || Char.IsLetter(c))
                        stroka += c.ToString();
                }
                if (c != '\n' && i == text.Length-1 || c != '-' && !Char.IsLetter(c) && stroka != "")
                {
                    if (stroka != "" && stroka[stroka.Length - 1] == '-')
                        stroka = stroka.Replace("-", "");
                    Word w = new Word(stroka);
                    if (ListWords.FindIndex(x => x.Equals(w)) >= 0)
                    {
                        ListWords[ListWords.FindIndex(x => x.Equals(w))].Count++;
                    }
                    else
                        ListWords.Add(w);
                    stroka = "";
                    Begin = true;
                }
                i++;
            }
        }
    }
}
