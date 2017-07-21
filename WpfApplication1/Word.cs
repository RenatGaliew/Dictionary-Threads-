using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1
{
    class Word
    {
        public string Name  { get; set; }
        public int Count { get; set; }
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
