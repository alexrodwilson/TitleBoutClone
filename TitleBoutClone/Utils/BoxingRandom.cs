using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleBoutClone.Utils
{
    public class BoxingRandom
    {
        private Random _random;
        public BoxingRandom()
        {
            _random = new Random();
        }
        public int DieOf(int n)
        {
            return _random.Next(n) + 1;
        }
        public bool CoinLandsHeads()
        {
            return _random.Next(2) == 0;
        }
    }
}
