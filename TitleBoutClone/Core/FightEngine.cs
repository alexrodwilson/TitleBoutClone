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
     
        private FightInfo _fightInfo = new FightInfo();
        private Fighter _redCorner;
        private Fighter _blueCorner;
        IEnumerable<Fighter> _fighters;
   
        private const int LastPunchValue = 78;
        private const int TimeUnitsInRound = 50;
        private const int Rounds = 15;

        public FightEngine(Fighter redCorner, Fighter blueCorner)
        {
            _redCorner = redCorner;
            _blueCorner = blueCorner;
            _fighters = new List<Fighter> { _redCorner, _blueCorner };
            _fightInfo = new FightInfo();
        }

        private void WriteLastRoundScore((int redPoints, int bluePoints) lastRoundScore)
        {
            if (lastRoundScore.redPoints > lastRoundScore.bluePoints) System.Console.WriteLine($"{_redCorner.Surname} won the last round {lastRoundScore.redPoints} to {lastRoundScore.bluePoints}");
            else if (lastRoundScore.bluePoints > lastRoundScore.redPoints) System.Console.WriteLine($"{_blueCorner.Surname} won the last round {lastRoundScore.bluePoints} to {lastRoundScore.redPoints}");
            else System.Console.WriteLine($"The last round was drawn {lastRoundScore.redPoints} {lastRoundScore.bluePoints}");
        }
        private void SimulateInterval()
        {
            AdjustEndurance();
            AdjustChin();
            (int redPoints, int bluePoints) lastRoundScore = ScoreLastRound();
            WriteLastRoundScore(lastRoundScore);
            PrepareInfoForNextRound(lastRoundScore);
           
        }

        private (int redPoints, int bluePoints) ScoreLastRound()
        {
            int movedVersusClinchedRed = (_redCorner.Info.TimesMoved > _redCorner.Info.TimesClinched) ? 1 : -1;
            int movedVersusClinchedBlue = (_blueCorner.Info.TimesMoved > _blueCorner.Info.TimesClinched) ? 1 : -1;
            int redScore = _redCorner.Info.PointsScoredCurrentRound + _blueCorner.Info.MissedPunches + movedVersusClinchedRed;
            int blueScore = _blueCorner.Info.PointsScoredCurrentRound + _redCorner.Info.MissedPunches + movedVersusClinchedBlue;
            int timesBlueKd = _blueCorner.Info.TimesLeftKnockedDownLastRound.Count();
            int timesRedKd = _redCorner.Info.TimesLeftKnockedDownLastRound.Count();
            //System.Console.WriteLine($"{_redCorner.Surname} was knocked down {timesRedKd} times last round, {_blueCorner.Surname} was knocked down {timesBlueKd} times.");
            int numberOfKnockdowns = timesBlueKd + timesRedKd;
            if (numberOfKnockdowns == 0 || timesBlueKd == timesRedKd)
            {
                if (redScore > blueScore) return (10, 9);
                else if (blueScore > redScore) return (9, 10);
                else return (10, 10);
            }
            else
            {
                (int redPoints, int bluePoints) prelimScore = (timesBlueKd > timesRedKd) ? (10, 10 - (timesBlueKd + 1)) : (10 - (timesRedKd + 1), 10);
                int difference = Math.Abs(redScore - blueScore);
                if (difference < 20 ) return prelimScore;
                else
                {
                    if (redScore > blueScore && timesBlueKd > timesRedKd) return prelimScore;
                    else if (blueScore > redScore && timesRedKd > timesBlueKd) return prelimScore;
                    else if (redScore > blueScore) return (prelimScore.redPoints + 1, prelimScore.bluePoints);
                    else return (prelimScore.redPoints, prelimScore.bluePoints + 1);
                }

            }

        }

        private void PrepareInfoForNextRound((int redPoints, int bluePoints) lastRoundScore)
        {
            _fightInfo.Scores.Add(lastRoundScore);
            if(_redCorner.Info.PointsScoredCurrentRound == _blueCorner.Info.PointsScoredCurrentRound)
            {
                _fightInfo.LastRoundWinner = null;
            }
            else
            {
                _fightInfo.LastRoundWinner = (_redCorner.Info.PointsScoredCurrentRound > _blueCorner.Info.PointsScoredCurrentRound)?
                    _redCorner : _blueCorner;
            }
            _fightInfo.CurrentRound++;
            _fightInfo.TimeUnitsLeft = TimeUnitsInRound;
            foreach(var fighter in _fighters)
            {
                fighter.Info.TimesClinched = 0;
                fighter.Info.TimesMoved = 0;
                fighter.Info.PointsScoredCurrentRound = 0;
                fighter.Info.MissedPunches = 0;
                fighter.Info.TimesLeftKnockedDownLastRound = new List<int>();
            }
        }
        private void AdjustChin()
        {
            foreach(var fighter in _fighters)
            {
                if (fighter.Info.TimesLeftKnockedDownLastRound.Count() == 1)
                {
                    fighter.CurrentChin = (fighter.Info.TimesLeftKnockedDownLastRound.First() + fighter.Recovery <= 20)?
                     fighter.Chin + fighter.Recovery : fighter.Chin;

                }
                else if (fighter.Info.TimesLeftKnockedDownLastRound.Count() > 1)
                {
                    fighter.CurrentChin = fighter.Chin + fighter.Recovery;
                }
                else
                {
                    fighter.CurrentChin = fighter.Chin;
                }
            }
           
        }

        private void AdjustEndurance()
        {
            foreach (var fighter in _fighters)
            {
                fighter.Endurance -= GetOtherFighter(fighter).Info.PointsScoredCurrentRound;
                if ((fighter.Endurance <= 0) && (fighter.Info.Fatigued == false))
                {
                    fighter.CurrentControl -= 2;
                    fighter.Info.Fatigued = true;
                }
            }
        }

        public void SimulateFight()
        {
            (int redConvertedControl, int blueConvertedControl) = ConvertControl(_redCorner, _blueCorner);
            _redCorner.CurrentControl = redConvertedControl;
            _blueCorner.CurrentControl = blueConvertedControl;
            (int redAdjustedEndurance, int blueAdjustedEndurance) = AdjustEnduranceForFightLength(Rounds, _redCorner, _blueCorner);
            _redCorner.Endurance = redAdjustedEndurance;
            _blueCorner.Endurance = blueAdjustedEndurance;
            while (_fightInfo.CurrentRound <= Rounds && ! _fightInfo.FightIsOver)
            {
                SimulateRound();
                SimulateInterval();
                System.Console.ReadLine();
            }
            WriteFinalScores(_fightInfo.Scores, _redCorner, _blueCorner);
            System.Console.ReadLine();
        }
        private void WriteFinalScores(List<(int,int)> scores, Fighter redCorner, Fighter blueCorner)
        {
            int redTotal = scores.Select(score => score.Item1).Sum();
            int blueTotal = scores.Select(score => score.Item2).Sum();
            if (redTotal == blueTotal)
            {
                System.Console.WriteLine($"The scores are tied at scores of {blueTotal}, ladies and gentleman. The result is a draw.");
                return;
            }
            else
            {
                string winner = (redTotal > blueTotal) ? redCorner.Surname : blueCorner.Surname;
                System.Console.WriteLine($"Ladies and gentleman, your winner by a score of {Math.Max(redTotal, blueTotal)} to {Math.Min(redTotal, blueTotal)}, is the winner {winner}.");
            }
            
        }

        private (int redEndurance, int blueEndurance)AdjustEnduranceForFightLength(int rounds, Fighter redCorner, Fighter blueCorner)
        {
            int redEndurance = (redCorner.Endurance / 10) * rounds;
            int blueEndurance = (blueCorner.Endurance / 10) * rounds;
            return (redEndurance, blueEndurance);
        }
        public void SimulateRound()
        {
            System.Console.WriteLine($"This is the {_fightInfo.CurrentRound}th round");
            System.Console.WriteLine($"{_redCorner.Surname} Endurance has {_redCorner.Endurance}, {_blueCorner.Surname} has {_blueCorner.Endurance}, ");

            (Fighter leading, Fighter reacting) = DetermineAggressor(_redCorner, _blueCorner);
            System.Console.WriteLine($"{_redCorner.Surname} is set out as a {_redCorner.CurrentStyle} this round, while {_blueCorner.Surname} is being a {_blueCorner.CurrentStyle}.");
            System.Console.WriteLine($"{_redCorner.Surname} has a CF of {_redCorner.CurrentControl} and {_blueCorner.Surname} one of {_blueCorner.CurrentControl}.");
            while(_fightInfo.TimeUnitsLeft > 0 )
            {
                SimulateAction(leading, reacting);
                if (_fightInfo.FightIsOver) return;
                (leading, reacting) = DetermineControl(leading, reacting, _fightInfo);  
            }
            System.Console.WriteLine($"The round ends with {_redCorner.Surname} scoring {_redCorner.Info.PointsScoredCurrentRound} points and {_blueCorner.Surname} on {_blueCorner.Info.PointsScoredCurrentRound} points.");
            System.Console.WriteLine();
        }
        
        private (Fighter leading, Fighter reacting) DetermineAggressor(Fighter fighterA, Fighter fighterB)
        {
            if (fighterA.Aggression != fighterB.Aggression)
            {
                return (fighterA.Aggression > fighterB.Aggression) ? (fighterA, fighterB) : (fighterB, fighterA);
            }
            else if (_fightInfo.LastRoundWinner != null)
            {
                return (_fightInfo.LastRoundWinner, GetOtherFighter(_fightInfo.LastRoundWinner));
            }
            else return (_boxRandom.CoinLandsHeads()) ? (fighterA, fighterB) : (fighterB, fighterA);
        }

        private (Fighter leading, Fighter reacting) DetermineControl(Fighter leading, Fighter reacting, FightInfo roundState)
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

        private void SimulateAction(Fighter leading, Fighter reacting)
        {
            _fightInfo.TimeUnitsLeft--;
            int RN = _boxRandom.DieOf(80);
            IAction action = GetFighterAction(leading, reacting, RN);
            if(action is PunchLanded)
            {
                var punchLanded = (PunchLanded)action;
                if (_boxRandom.DieOf(80) <= leading.KnockdownChance && _boxRandom.DieOf(20) <= leading.Power)
                {
                    _fightInfo.TimeUnitsLeft--;
                    SimulateHeavyShot(leading, reacting, _fightInfo);
                }
                else
                {
                    
                    RegisterPunch(leading, reacting, punchLanded, _fightInfo);
                    System.Console.WriteLine($"{leading.Surname} lands a {punchLanded.Value}-point {punchLanded.Type} to {reacting.Surname}.");
                }

            }
            else if (action is PunchMissed)
            {
                int newRandom = _boxRandom.DieOf(80);
                if ( newRandom <= (leading.Predictability + reacting.Counterpunching))
                {
                    if (_boxRandom.DieOf(80) <= leading.KnockdownChance && _boxRandom.DieOf(20) <= leading.Power)
                    {
                        _fightInfo.TimeUnitsLeft--;
                        SimulateHeavyShot(leading, reacting, _fightInfo);
                    }
                    else
                    {
                        (var type, var value) = GetPunchTypeAndValue(_boxRandom.DieOf(80), reacting.HittingValuesTable);
                        _fightInfo.TimeUnitsLeft--;
                        RegisterPunch(reacting, leading, new PunchLanded(type, value), _fightInfo);
                        System.Console.WriteLine($"{reacting.Surname} lands a beautiful {value}-point {type} counterpunch.");
                    }

                }
                else
                {
                    leading.Info.MissedPunches++;
                    System.Console.WriteLine($"{leading.Surname} misses a punch.");
                }
            }
            else if (action is Clinching)
            {
                leading.Info.TimesClinched++;
                System.Console.WriteLine($"{leading.Surname} holds on.");
            }
            else if(action is Movement)
            {
                leading.Info.TimesMoved++;
                System.Console.WriteLine($"{leading.Surname} moves around the ring.");
            }

        }
        private int EndOfJabs(IDictionary<PunchType, (Range, Range)>hitTable)
        {
            var ranges = (new Range(1, 1), new Range(1, 1));
            hitTable.TryGetValue(PunchType.Jab, out ranges );
            return Math.Max(ranges.Item1.End, ranges.Item2.End) + 1;
        }

        private void SimulateHeavyShot(Fighter leading, Fighter reacting, FightInfo roundState)
        {
            int rn = _boxRandom.DieOf(20);
            int chanceOfK = reacting.CurrentChin;
            int chanceOf5 = reacting.CurrentChin + 4;
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
                SimulateKillerInstinct(leading, reacting, roundState);
            }
            else
            {
                RegisterPunch(leading, reacting, new PunchLanded(punchType, 4), roundState);
                System.Console.WriteLine($"{leading.Surname} lands a heavy {punchType}.");
            }
        }

        private void SimulateKnockdown(Fighter puncher, Fighter punchee, FightInfo roundState)
        {
            roundState.TimeUnitsLeft--;
            int rn = _boxRandom.DieOf(20);
            int chanceOfKnockout = punchee.Recovery;
            punchee.Info.TimesLeftKnockedDownLastRound.Add(roundState.TimeUnitsLeft);
            //int chanceOf5 = reacting.Chin + 4;
            (PunchType punchType, _) = GetPunchTypeAndValue(_boxRandom.NInRange(EndOfJabs(puncher.HittingValuesTable), LastPunchValue),
                puncher.HeavyShotsTable);
            if (rn <= chanceOfKnockout)
            {
                roundState.FightIsOver = true;
                System.Console.WriteLine($"That's it! The fight is over! {punchee.Surname} won't get up from that. {puncher.Surname} is victorious.");
            }
            else
            {
                roundState.TimeUnitsLeft--;
                int count = _boxRandom.DieOf(9);
                for(int i = 1;  i<= count; i++)
                {
                    System.Console.WriteLine(i);
                }
                System.Console.WriteLine($"{punchee.Surname} rises on shaky legs.");
                punchee.CurrentChin = punchee.Chin + punchee.Recovery;
                SimulateKillerInstinct(puncher, punchee, roundState);
            }
        }

        private void SimulateKillerInstinct(Fighter leading, Fighter reacting, FightInfo roundState)
        {
            System.Console.WriteLine($"{leading.Surname} is going in for the kill.");
            int timeUnitsLeftAfterKillerInstinct = roundState.TimeUnitsLeft - leading.Finishing;
            while((roundState.TimeUnitsLeft > 0) && (roundState.TimeUnitsLeft > timeUnitsLeftAfterKillerInstinct))
            {
                SimulateAction(leading, reacting);
            }
        }



        private void RegisterPunch(Fighter puncher, Fighter punchee, PunchLanded punch, FightInfo roundState)
        {
            if (IsRedCornerFighter(puncher))
            {
                _redCorner.Info.PointsScoredCurrentRound += punch.Value;
            }
            else
            {
                _blueCorner.Info.PointsScoredCurrentRound += punch.Value;
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
