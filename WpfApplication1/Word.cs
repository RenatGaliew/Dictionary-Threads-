using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Word
    {
        /// <summary>
        /// Слово
        /// </summary>
        public string Name  { get; set; }

        /// <summary>
        /// Количество слов
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name"> Слово</param>
        public Word (string name)
        {
            Name = name;
            Count = 1;
        }
        public bool Equals(Word other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
    }
}
