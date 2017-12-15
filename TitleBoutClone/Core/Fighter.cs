using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Utils;

namespace TitleBoutClone.Core
{
    
    public class Fighter
    {
        public int Id { get; }
        public string Surname { get; }
        public Style CurrentStyle { get; set; }
        public Style NaturalStyle { get; set; }
        public int? CurrentControl{get;set;}
        public int ControlAgainstB { get; }
        public int ControlAgainstS { get; }
        public int Power { get; set; }
        public int Aggression { get; set; }
        public int Chin { get; set; }
        public int CurrentChin { get; set; }
        public int Finishing { get; set; }
        public int Endurance { get; set; }
        public int Defence { get; set; }
        public int Recovery { get; set; }
        public int KnockdownChance { get; set; }
        public Range PunchLandedRange { get; }
        public Range PunchMissedRange { get; }
        public Range ClinchingRange { get; }
        public Range MovementRange { get; }
        public IDictionary<PunchType, (Range, Range)> HeavyShotsTable { get; }
        public IDictionary<PunchType, (Range, Range)>  HittingValuesTable { get;}
        public int Counterpunching { get; }
        public int Predictability { get; }
        public (int PointsScoredCurrentRound, int MissedPunches, int TimesMoved, int TimesClinched, bool Fatigued, List<int> TimesLeftKnockedDownLastRound) Info;

        public Fighter(int id, string surname, int power, int controlAgainstS, int controlAgainstB, int aggression, 
            int endurance, int defence, int counterpunching, int finishing, int openToCounterpunch, int recovery,int chin, int knockdownChance, Style currentStyle, Range punchLandedRange,
            Range punchMissedRange, Range clinchingRange, Range movementRange, 
            IDictionary<PunchType, (Range, Range)> hittingValuesTable)
        {
            Id = id;
            Surname = surname;
            ControlAgainstB = controlAgainstB;
            ControlAgainstS = controlAgainstS;
            Aggression = aggression;
            Endurance = endurance;
            Chin = chin;
            CurrentChin = Chin;
            Recovery = recovery;
            Defence = defence;
            KnockdownChance = knockdownChance;
            PunchLandedRange = punchLandedRange;
            PunchMissedRange = punchMissedRange;
            ClinchingRange = clinchingRange;
            MovementRange = movementRange;
            HittingValuesTable = hittingValuesTable;
            Predictability = openToCounterpunch;
            Counterpunching = counterpunching;
            CurrentStyle = currentStyle;
            HeavyShotsTable = HittingValuesTable.Where(kvp => kvp.Key != PunchType.Jab).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Finishing = finishing;
            Power = power;
            Info = (0, 0, 0, 0, false, new List<int>());
        }

        public override bool Equals(Object otherFighter)
        {
            return Id == ((Fighter)otherFighter).Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }


}
