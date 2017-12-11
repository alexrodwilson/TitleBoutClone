﻿using System;
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
        private bool isStopped = false;
        private BoutState _boutState = new BoutState();
        private Fighter _redCorner;
        private Fighter _blueCorner;

        public FightEngine(Fighter redCorner, Fighter blueCorner)
        {
            _redCorner = redCorner;
            _blueCorner = blueCorner;
        }
        public void SimulateRound()
        {
            RoundState roundState = new RoundState();
            (int redConvertedControl, int blueConvertedControl) = ConvertControl(_redCorner.Control, _blueCorner.Control);
            _redCorner.Control = redConvertedControl;
            _blueCorner.Control = blueConvertedControl;
            (Fighter leading, Fighter reacting) = DetermineAggressor(_redCorner, _blueCorner);
            while(roundState.TimeUnitsLeft > 0 && ! isStopped)
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
            return (leading.Control > _boxRandom.DieOf(20)) ? (leading, reacting) : (reacting, leading);
        }

        private void SimulateAction(Fighter leading, Fighter reacting, RoundState roundState)
        {
            roundState.TimeUnitsLeft--;
            int RN = _boxRandom.DieOf(80);
            IAction action = GetFighterAction(leading, reacting);
            if(action is PunchLanded)
            {
                var punchLanded = (PunchLanded)action;
                if (IsRedCornerFighter(leading))
                {
                    roundState.PointsScoredRed += punchLanded.Value;
                }
                else
                {
                    roundState.PointsScoredBlue += punchLanded.Value;
                }
                System.Console.WriteLine($"{leading.Surname} lands a {punchLanded.Type} worth {punchLanded.Value} to {reacting.Surname}.");
                
            }
            else if (action is PunchMissed)
            {
                
            }
            else if (action is Clinching)
            {

            }
            else if(action is Movement)
            {
                
            }

        }

        private void SimulatePunchLanding(Fighter leading, Fighter reacting)
        {
            
        }

        private Fighter GetOtherFighter(Fighter fighter)
        {
            return (fighter.Equals(_redCorner)) ? _blueCorner : _redCorner;
        }
        private bool IsRedCornerFighter(Fighter fighter)
        {
            return fighter.Equals(_redCorner);
        }
        private IAction GetFighterAction(Fighter leading, Fighter reacting)
        {
            int rn = _boxRandom.DieOf(80);
            if (leading.PunchLandedRange.HasWithinIt(rn))
            {
                (PunchType punchType, int value) = GetPunchTypeAndValue(_boxRandom.DieOf(80), leading.HittingValuesTable);
                return new PunchLanded(punchType, value);
            }
            else if (leading.PunchMissedRange.HasWithinIt(rn))
            {
                return new PunchMissed();
            }
            else if (leading.ClinchingRange.HasWithinIt(rn))
            {
                return new Clinching();
            }
            else if (leading.MovementRange.HasWithinIt(rn))
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

        private static (int FighterAControl, int FighterBControl) ConvertControl(int fighterAControl, int fighterBControl)
        {
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