using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitleBoutClone.Utils;

namespace TitleBoutClone.Actions
{
    public class PunchLanded : IAction
    {
        public PunchType Type { get; }
        public int Value { get; }

        public PunchLanded(PunchType type, int value)
        {
            Type = type;
            Value = value;
        }
    }
}
