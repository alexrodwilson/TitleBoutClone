using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Utils;

namespace TitleBoutClone.Core
{
    public class Cut
    {
        public Action<Fighter> Consequence { get; private set; }
        public CutLocation Location { get; private set; }
        public string Description{ get; private set; }
        public Severity Extent { get; private set; }
        private static BoxingRandom _boxRandom = new BoxingRandom();
        private static int? n;
        private static Cut severeOverLeftEye = new Cut(CutLocation.AboveTheLeftEye, Severity.Severe, fighter => fighter.CurrentControl -= 1);
        private static Cut mediumOverLeftEye = new Cut(CutLocation.AboveTheLeftEye, Severity.Medium, fighter => fighter.CurrentControl -= 1);
        private static Cut lightOverLeftEye = new Cut(CutLocation.AboveTheLeftEye, Severity.Light, fighter => { });
        private static Cut severeOverRightEye = new Cut(CutLocation.AboveTheRightEye, Severity.Severe, fighter => fighter.CurrentControl -= 1);
        private static Cut mediumOverRightEye = new Cut(CutLocation.AboveTheRightEye, Severity.Medium, fighter => fighter.CurrentControl -= 1);
        private static Cut lightOverRightEye = new Cut(CutLocation.AboveTheRightEye, Severity.Light, fighter => { });


        public Cut(CutLocation location,  Severity severity, Action<Fighter> consequence)
        {
            Location = location;
            Extent = severity;
            Consequence = consequence;
            Description = $"A {Extent.ToString().ToLower()} cut {getLocationDescription(Location)}.";
        }

        public static string getLocationDescription(CutLocation location)
        {
            StringBuilder sb = new StringBuilder();
            foreach(Char ch in location.ToString())
            {
                if(Char.IsUpper(ch))
                {
                    sb.Append(' ');
                    sb.Append(ch);
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString().ToLower().Trim();
        }

        private static Dictionary<Cut, int> cutToChance = new Dictionary<Cut, int>() {
            { severeOverLeftEye, 2},
            { mediumOverLeftEye, 1},
            { lightOverLeftEye, 3 },
            { severeOverRightEye, 2},
            { mediumOverRightEye, 1},
            { lightOverRightEye, 3 },
        };
        public static Cut GetCut(Fighter fighter)
        {
            if(n == null)
            {
                n = cutToChance.Values.Sum();
                int total = 0;
                var tempDict = new Dictionary<Cut, int>();
                foreach (var pair in cutToChance)
                {
                    //cutToChance.Remove(pair.Key);
                    tempDict.Add(pair.Key, pair.Value + total);
                    total += pair.Value;
                }
                cutToChance = tempDict;
            }
            int rn = _boxRandom.DieOf((int)n);
            return PickKey(rn, cutToChance);

        }

        private static T PickKey<T>(int rn, Dictionary<T, int> dict)
        {
            foreach(var pair in dict)
            {
                if(rn <= pair.Value)
                {
                    return pair.Key;
                }
            }
            throw new ArgumentOutOfRangeException($"The value of {nameof(rn)} does not correspond to any value in the dictionary", nameof(rn));
        }

        public override bool Equals(Object other)
        {
            var otherCut = (Cut)other;
            bool sameLocation = (this.Location == otherCut.Location);
            bool sameExtent = this.Extent == otherCut.Extent;
            return sameLocation && sameExtent;
        }

        public override int GetHashCode()
        {
            var hashCode = -1070376209;
            hashCode = hashCode * -1521134295 + Location.GetHashCode();
            hashCode = hashCode * -1521134295 + Extent.GetHashCode();
            return hashCode;
        }
    }
}
