using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitleBoutClone.Core
{
    public class BoutState
    {

        public int CurrentRound { get; set; }
        public Fighter LastRoundWinner { get; set; }

        public BoutState()
        {
            CurrentRound = 1;
            LastRoundWinner = null;
        }

        
    }
}
