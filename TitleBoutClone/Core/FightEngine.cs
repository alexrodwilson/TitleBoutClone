using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Actions;
using TitleBoutClone.Utils;

namespace TitleBoutClone.Core
{
    public class FightEngine
    {
        private BoxingRandom _boxRandom = new BoxingRandom();
     
        private BoutState _boutState = new BoutState();
        private Fighter _redCorner;
        private Fighter _blueCorner;
        private const int LastPunchValue = 78;

        public FightEngine(Fighter redCorner, Fighter blueCorner)
        {
            _redCorner = redCorner;
            _blueCorner = blueCorner;
        }
        public void SimulateRound()
        {
            RoundState roundState = new RoundState();
            (int redConvertedControl, int blueConvertedControl) = ConvertControl(_redCorner, _blueCorner);
            _redCorner.CurrentControl = redConvertedControl;
            _blueCorner.CurrentControl = blueConvertedControl;
            (Fighter leading, Fighter reacting) = DetermineAggressor(_redCorner, _blueCorner);
            System.Console.WriteLine($"{_redCorner.Surname} is set out as a {_redCorner.CurrentStyle} this round, while {_blueCorner.Surname} is being a {_blueCorner.CurrentStyle}.");
            System.Console.WriteLine($"{_redCorner.Surname} has a CF of {_redCorner.CurrentControl} and {_blueCorner.Surname} one of {_blueCorner.CurrentControl}.");
            while(roundState.TimeUnitsLeft > 0 && ! roundState.FightIsOver)
            {
                SimulateAction(leading, reacting, roundState);
                (leading, reacting) = DetermineControl(leading, reacting, roundState);  
            }
            System.Console.WriteLine($"The round ends with {_redCorner.Surname} on {roundState.PointsScoredRed} points and {_blueCorner.Surname} on {roundState.PointsScoredBlue} points.");
            System.Console.ReadLine();
        }
        
        private (Fighter leading, Fighter reacting) DetermineAggressor(Fighter fighterA, Fighter fighterB)
        {
            if (fighterA.Aggression != fighterB.Aggression)
            {
                return (fighterA.Aggression > fighterB.Aggression) ? (fighterA, fighterB) : (fighterB, fighterA);
            }
            else if (_boutState.LastRoundWinner != null)
            {
                return (_boutState.LastRoundWinner, GetOtherFighter(_boutState.LastRoundWinner));
            }
            else return (_boxRandom.CoinLandsHeads()) ? (fighterA, fighterB) : (fighterB, fighterA);
        }

        private (Fighter leading, Fighter reacting) DetermineControl(Fighter leading, Fighter reacting, RoundState roundState)
        {
            roundState.TimeUnitsLeft--;
            if (leading.CurrentControl == null)
            {
                throw new ArgumentException("The value of the CurrentControl of leading has not been set correctly", nameof(leading));
            }
            if (leading.CurrentControl > _boxRandom.DieOf(20))
            {
                return (leading, reacting);
            }
            else
            {
                System.Console.WriteLine($"{reacting.Surname} takes control.");
                return (reacting, leading);
            }
        }

        private void SimulateAction(Fighter leading, Fighter reacting, RoundState roundState)
        {
            roundState.TimeUnitsLeft--;
            int RN = _boxRandom.DieOf(80);
            IAction action = GetFighterAction(leading, reacting, RN);
            if(action is PunchLanded)
            {
                var punchLanded = (PunchLanded)action;
                if (_boxRandom.DieOf(80) <= leading.KnockdownChance)
                {
                    SimulateHeavyShot(leading, reacting, roundState);
                }
                else
                {
                    
                    RegisterPunch(leading, reacting, punchLanded, roundState);
                    System.Console.WriteLine($"{leading.Surname} lands a {punchLanded.Value}-point {punchLanded.Type} to {reacting.Surname}.");
                }

            }
            else if (action is PunchMissed)
            {
                int newRandom = _boxRandom.DieOf(80);
                if ( newRandom <= (leading.OpenToCounterpunch + reacting.Counterpunching))
                {
                    (var type, var value) = GetPunchTypeAndValue(_boxRandom.DieOf(80), reacting.HittingValuesTable);
                    roundState.TimeUnitsLeft--;
                    RegisterPunch(reacting, leading, new PunchLanded(type, value),roundState);
                    System.Console.WriteLine($"{reacting.Surname} lands a beautiful {value}-point {type} counterpunch.");
                }
                else
                {
                    System.Console.WriteLine($"{leading.Surname} misses a punch.");
                }
            }
            else if (action is Clinching)
            {
                System.Console.WriteLine($"{leading.Surname} holds on.");
            }
            else if(action is Movement)
            {
                System.Console.WriteLine($"{leading.Surname} moves around the ring.");
            }

        }
        private int EndOfJabs(IDictionary<PunchType, (Range, Range)>hitTable)
        {
            var ranges = (new Range(1, 1), new Range(1, 1));
            hitTable.TryGetValue(PunchType.Jab, out ranges );
            return Math.Max(ranges.Item1.End, ranges.Item2.End) + 1;
        }

        private void SimulateHeavyShot(Fighter leading, Fighter reacting, RoundState roundState)
        {
            int rn = _boxRandom.DieOf(20);
            int chanceOfK = reacting.Chin;
            int chanceOf5 = reacting.Chin + 4;
            int randomPunchN = _boxRandom.NInRange(EndOfJabs(leading.HittingValuesTable), LastPunchValue);
            (PunchType punchType, _) = GetPunchTypeAndValue(randomPunchN,
                leading.HeavyShotsTable);
            if(rn <= chanceOfK)
            {
                roundState.TimeUnitsLeft--;
                System.Console.WriteLine($"Down goes {reacting.Surname}. {leading.Surname} with a heavy {punchType}");
                RegisterPunch(leading, reacting, new PunchLanded(punchType, 6), roundState);
                SimulateKnockdown(leading, reacting, roundState);
            }
            else if(rn <= chanceOf5)
            {
                RegisterPunch(leading, reacting, new PunchLanded(punchType, 5), roundState);
                System.Console.WriteLine($"{reacting.Surname} has been shaken to his boots by a {punchType} from {leading.Surname}.");
            }
            else
            {
                RegisterPunch(leading, reacting, new PunchLanded(punchType, 4), roundState);
                System.Console.WriteLine($"{leading.Surname} lands a heavy {punchType}.");
            }
        }

        private void SimulateKnockdown(Fighter leading, Fighter reacting, RoundState roundState)
        {
            roundState.TimeUnitsLeft--;
            int rn = _boxRandom.DieOf(20);
            int chanceOfKnockout = reacting.Recovery;
            int chanceOf5 = reacting.Chin + 4;
            (PunchType punchType, _) = GetPunchTypeAndValue(_boxRandom.NInRange(EndOfJabs(leading.HittingValuesTable), LastPunchValue),
                leading.HeavyShotsTable);
            if (rn <= chanceOfKnockout)
            {
                roundState.FightIsOver = true;
                System.Console.WriteLine($"That's it! The fight is over! {reacting.Surname} won't get up from that. {leading.Surname} is victorious.");
            }
            else
            {
                roundState.TimeUnitsLeft--;
                System.Console.WriteLine($"{reacting.Surname} rises on shaky legs.");
            }
        }



        private void RegisterPunch(Fighter puncher, Fighter punchee, PunchLanded punch, RoundState roundState)
        {
            if (IsRedCornerFighter(puncher))
            {
                roundState.PointsScoredRed += punch.Value;
            }
            else
            {
                roundState.PointsScoredBlue += punch.Value;
            }
        }

        private Fighter GetOtherFighter(Fighter fighter)
        {
            return (fighter.Equals(_redCorner)) ? _blueCorner : _redCorner;
        }

        private bool IsRedCornerFighter(Fighter fighter)
        {
            return fighter.Equals(_redCorner);
        }

        private IAction GetFighterAction(Fighter leading, Fighter reacting, int randomNumber)
        {
            if (randomNumber <= (leading.PunchLandedRange.End + reacting.Defence))
            {
                (PunchType punchType, int value) = GetPunchTypeAndValue(_boxRandom.DieOf(80), leading.HittingValuesTable);
                return new PunchLanded(punchType, value);
            }
            else if (leading.PunchMissedRange.HasWithinIt(randomNumber) || randomNumber < leading.PunchMissedRange.Start)
            {
                return new PunchMissed();
            }
            else if (leading.ClinchingRange.HasWithinIt(randomNumber))
            {
                return new Clinching();
            }
            else if (leading.MovementRange.HasWithinIt(randomNumber))
            {
                return new Movement();
            }
            else
            {
                throw new ArgumentException("Leading fighter's Ranges are non exhaustive", nameof(leading));
            }

        }



        private (PunchType punchType, int punchValue) GetPunchTypeAndValue(int randomNumber, IDictionary<PunchType, (Range , Range)> hittingValuesTable)
        {
            foreach(var pair in hittingValuesTable)
            {
                if (pair.Value.Item1.HasWithinIt(randomNumber))
                {
                    return (pair.Key, 3);
                }
                else if (pair.Value.Item2.HasWithinIt(randomNumber))
                {
                    return (pair.Key, 2);
                }
            }
            throw new ArgumentException("Exception caused by non-exhaustive ranges in HittingValuesTable", nameof(hittingValuesTable));
        }

        private static (int FighterAControl, int FighterBControl) ConvertControl(Fighter fighterA, Fighter fighterB)
        {
            int fighterAControl = (fighterB.CurrentStyle == Style.Slugger)? 
                fighterA.ControlAgainstS : fighterA.ControlAgainstB;
            int fighterBControl = (fighterA.CurrentStyle == Style.Slugger) ?
                fighterB.ControlAgainstS : fighterB.ControlAgainstB;

            int difference = Math.Abs(fighterAControl - fighterBControl);
            if (difference > 2)
            {
                return (fighterAControl, fighterBControl);
            }
            else
            {
                if(fighterAControl > fighterBControl)
                {
                    return (10, 10 - difference);
                }
                else
                {
                    return (10 - difference, 10);
                }
            }
        }

    }
}
