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
            Fighter ali = new Fighter(id: 1, surname: "Ali", controlAgainstS: 11, controlAgainstB: 12,
                aggression: 6, power: 6, endurance: 85, knockdownChance: 16, defence: 0, chin: 2, finishing: 7, recovery: 3, counterpunching: 6, openToCounterpunch: 42,
                punchLandedRange: new Range(1, 40), currentStyle: Style.Boxer,
                punchMissedRange: new Range(41, 50), clinchingRange: new Range(51, 70),
                movementRange: new Range(71, 80), hittingValuesTable: hitTable1);

            Fighter foreman = new Fighter(id: 2, surname: "Foreman", controlAgainstB: 11, controlAgainstS: 12,
                aggression: 8, power: 12, endurance: 78, defence: 2, knockdownChance: 16, chin: 1, finishing: 7, recovery: 5, counterpunching: 4, openToCounterpunch: 42, punchLandedRange: new Range(1, 37),
                punchMissedRange: new Range(38, 50), clinchingRange: new Range(51, 70), currentStyle: Style.Slugger,
                movementRange: new Range(71, 80), hittingValuesTable: hitTable1);

            Fighter price = new Fighter(id: 3, surname: "Price", controlAgainstB: 7, controlAgainstS: 6,
    aggression: 5, power: 12, endurance: 70, defence: 3, knockdownChance: 8, chin: 6, finishing: 5, recovery: 6, counterpunching: 2, openToCounterpunch: 60, punchLandedRange: new Range(1, 37),
    punchMissedRange: new Range(38, 50), clinchingRange: new Range(51, 70), currentStyle: Style.Boxer,
    movementRange: new Range(71, 80), hittingValuesTable: hitTable1);

            var fightEngine = new FightEngine(ali, foreman);
            fightEngine.SimulateFight();
        }
    }
}
