using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Core;
using TitleBoutClone.Utils;

namespace TitleBoutClone
{
    class Program
    {
        static void Main(string[] args)
        {
            TrySimulatingBout();
        }

        private static void TrySimulatingBout()
        {
            var hitTable1 = new Dictionary<PunchType, (Range, Range)>()
            {
                {PunchType.Jab, (new Range(1,3), new Range(4, 14))},
                {PunchType.Hook, (new Range(15,18), new Range(19, 35))},
                {PunchType.Cross, (new Range(36,38), new Range(39, 50))},
                {PunchType.Combination, (new Range(51,53), new Range(54, 65))},
                {PunchType.Uppercut, (new Range(66,68), new Range(69, 80))}
            };
            Fighter redCorner = new Fighter(id: 1, surname: "Ali", control: 11,
                aggression: 6, endurance: 10, defence: 0, counterpunching: 6, openToCounterpunch: 42, 
                punchLandedRange: new Range(1, 40),
                punchMissedRange: new Range(41, 50), clinchingRange: new Range(51, 70),
                movementRange: new Range(71, 80), hittingValuesTable: hitTable1);

            Fighter blueCorner = new Fighter(id: 2, surname: "Foreman", control: 12,
                aggression: 8, endurance: 10, defence: 3, counterpunching: 4, openToCounterpunch: 42, punchLandedRange: new Range(1, 37),
                punchMissedRange: new Range(38, 50), clinchingRange: new Range(51, 70),
                movementRange: new Range(71, 80), hittingValuesTable: hitTable1);

            var fightEngine = new FightEngine(redCorner, blueCorner);
            fightEngine.SimulateRound();
        }
    }
}
