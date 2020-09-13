using System;
using System.Collections.Generic;
using AllLaunchLibrary.Arguments;

namespace AllLaunchLibrary
{
    public static class ArgumentProcessor
    {
        private static List<IArgument> _argOperators = new List<IArgument>
        {
            new NoMenuArgument()

        };

        public static void Process(string[] args)
        {
            foreach (string arg in args)
            {
                foreach (var argOperator in _argOperators)
                {
                    if (arg == argOperator.Signature)
                    {
                        argOperator.Operate();
                    }
                }
            }
        }

    }

    
}

