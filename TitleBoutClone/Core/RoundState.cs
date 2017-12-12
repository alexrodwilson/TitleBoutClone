using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleBoutClone.Core
{
    public class RoundState
    {
        public int PointsScoredRed { get; set; }
        public int PointsScoredBlue { get; set; }
        public int TimeUnitsLeft { get; set; }
        public bool FightIsOver { get; set; }

        public RoundState()
        {
            TimeUnitsLeft = 50;
            FightIsOver = false;
        }
    }
}
