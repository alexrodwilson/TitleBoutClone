using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleBoutClone.Utils
{
    public class Range
    {
        public int Start { get; set; }
        public int End { get; set; }

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }


        public bool HasWithinIt(int n)
        {
            return (n >= Start && n <= End);
        }
    }
}
