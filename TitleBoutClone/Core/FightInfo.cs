using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleBoutClone.Core
{
    public class FightInfo
    {

        public int TimeUnitsLeft { get; set; }
        public bool FightIsOver { get; set; }
        public Fighter LastRoundWinner { get; set; }
        public int CurrentRound { get; set; }


        public FightInfo()
        {
            TimeUnitsLeft = 50;
            FightIsOver = false;
            LastRoundWinner = null;
            CurrentRound = 1;
        }
    }
}
