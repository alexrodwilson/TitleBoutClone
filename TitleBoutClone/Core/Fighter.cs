using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Utils;

namespace TitleBoutClone.Core
{
    public enum Style { Boxer, Slugger, Either}
    public class Fighter
    {
        public int Id { get; }
        public string Surname { get; }
        public Style CurrentStyle { get; set; }
        public Style NaturalStyle { get; set; }
        public int Control{get;set;}
        public int Aggression { get; set; }
        public int Finishing { get; set; }
        public int Endurance { get; set; }
        public int Defence { get; set; }
        public Range PunchLandedRange { get; }
        public Range PunchMissedRange { get; }
        public Range ClinchingRange { get; }
        public Range MovementRange { get; }
        public IDictionary<PunchType, (Range, Range)>  HittingValuesTable { get;}

        public Fighter(int id, string surname, int control, int aggression, 
            int endurance, int defence, Range punchLandedRange,
            Range punchMissedRange, Range clinchingRange, Range movementRange, 
            IDictionary<PunchType, (Range, Range)> hittingValuesTable)
        {
            Id = id;
            Surname = surname;
            Control = control;
            Aggression = aggression;
            Endurance = endurance;
            Defence = defence;
            PunchLandedRange = punchLandedRange;
            PunchMissedRange = punchMissedRange;
            ClinchingRange = clinchingRange;
            MovementRange = movementRange;
            HittingValuesTable = hittingValuesTable;
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
