using System;
using System.Collections.Generic;
using AllLaunchLibrary.Arguments;

namespace AllLaunchLibrary
{
    // Process input arguments
    public static class ArgumentProcessor
    {
        // Includes all arguments
        private static List<IArgument> _argOperators = new List<IArgument>
        {
            new NoMenuArgument()

        };

        // Loop through each of received arguments and process them
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

