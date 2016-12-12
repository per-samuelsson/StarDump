using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarDump
{
    public class Output
    {
        public virtual void WriteErrorLine(string message = null)
        {
            DefaultWriteLine(message);
        }

        public virtual void WriteErrorLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                DefaultWriteLine(message);
            }
        }

        public virtual void WriteWarningLine(string message = null)
        {
            DefaultWriteLine(message);
        }

        public virtual void WriteWarningLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                DefaultWriteLine(message);
            }
        }

        public virtual void WriteLine(string message = null)
        {
            DefaultWriteLine(message);
        }
        
        public virtual void WriteLine(List<string> messages)
        {
            foreach (string message in messages)
            {
                DefaultWriteLine(message);
            }
        }

        private void DefaultWriteLine(string message)
        {
            if (message == null)
            {
                System.Console.WriteLine();
                return;
            }

            System.Console.WriteLine(message);
        }
    }
}
