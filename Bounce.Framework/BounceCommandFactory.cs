﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bounce.Framework
{
    public class BounceCommandFactory
    {
        public static IBounceCommand GetCommandByName(string name)
        {
            var commandParser = new BounceCommandParser();
            return commandParser.Parse(name);
        }
    }
}
