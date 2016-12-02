using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarDump;

namespace StarDump.Console
{
    public class CommandLine
    {
        private const string COMMAND_UNLOAD = "unload";
        private const string COMMAND_RELOAD = "reload";
        private const string COMMAND_HELP = "help";
        private const string INDENT = "  ";

        private List<Command> Commands { get; set; } = new List<Command>();
        private Command CurrentCommand { get; set; }
        public StarDump.Configuration Configuration { get; set; } = new StarDump.Configuration();

        public CommandLine()
        {
            // Commands
            Commands.Add(new Command
            {
                Name = COMMAND_UNLOAD,
                Description = "Unload database",
                Usage = "stardump unload [<command options>]",
                CommandOptions = new List<Option>
                {
                    new Option
                    {
                        Name = "--database",
                        Description = "Name of starcounter database",
                        SetParameterValue = (value) =>  Configuration.DatabaseName = value
                    },
                    new Option
                    {
                        Name = "--dump",
                        Description = "output filename",
                        SetParameterValue = (value) =>  Configuration.FileName = value
                    }
                },
                Run = (x) => StarDump.Program.Unload(x)
            });

            Commands.Add(new Command
            {
                Name = COMMAND_RELOAD,
                Description = "Reload database",
                Usage = "stardump reload [<command options>]",
                CommandOptions = new List<Option>
                {
                    new Option
                    {
                        Name = "--database",
                        Description = "Name of starcounter database",
                        SetParameterValue = (value) => Configuration.DatabaseName = value
                    },
                    new Option
                    {
                        Name = "--dump",
                        Description = "Dump to load",
                        SetParameterValue = (value) => Configuration.FileName = value
                    }
                },
                Run = (x) => System.Console.WriteLine("Entry point for reload")
            });

            Commands.Add(new Command
            {
                Name = COMMAND_HELP,
                Description = "Show help of all commands",
                Usage = "stardump help [<command options>]",
                Run = (x) => { PrintHelp(); }
            });
        }

        public bool Parse(string[] args)
        {
            var enumerator = args.GetEnumerator();

            if (enumerator.MoveNext() == false)
            {
                CurrentCommand = Commands.Where(x => x.Name == COMMAND_HELP).FirstOrDefault();
                return true;
            }

            CurrentCommand = Commands.Where(x => x.Name == (string)enumerator.Current).FirstOrDefault();
            if (CurrentCommand != null)
            {
                Option option = null;
                while (enumerator.MoveNext())
                {
                    string[] currentOptionArgs = ((string)enumerator.Current).Split('=');

                    string optionArg;
                    string optionParamterArg;

                    if (currentOptionArgs.Count() == 1)
                    {
                        optionArg = currentOptionArgs[0];
                        optionParamterArg = null;
                    }
                    else if (currentOptionArgs.Count() == 2)
                    {
                        optionArg = currentOptionArgs[0];
                        optionParamterArg = currentOptionArgs[1];
                    }
                    else
                    {
                        System.Console.WriteLine("<{0}> may not contain more than 1 equal sign (=)", (string)enumerator.Current);
                        System.Console.WriteLine();
                        PrintCommand(CurrentCommand);
                        return false;
                    }

                    option = CurrentCommand.CommandOptions.Where(x => x.Name == optionArg).FirstOrDefault();

                    if (option == null)
                    {
                        System.Console.WriteLine("<{0}> is not a command option for command <{1}>", optionArg, CurrentCommand.Name);
                        System.Console.WriteLine();
                        PrintCommand(CurrentCommand);
                        return false;
                    }

                    // Set Parameter value
                    if (optionParamterArg != null)
                    {
                        option.SetParameterValue(optionParamterArg);
                    }
                }

                return true;
            }

            // Should never end up here
            System.Console.WriteLine("<{0}> is not a valid command", (string)enumerator.Current);
            PrintHelp();
            return false;
        }

        public bool ValidateArguments()
        {
            // TODO check if file path exist
            return true;
        }

        public bool Run()
        {
            try
            {
                CurrentCommand?.Run(Configuration);
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        private void PrintHelp()
        {
            System.Console.WriteLine("Usage: stardump <command> [<command options>]");
            System.Console.WriteLine();
            System.Console.WriteLine("Commands:");
            foreach(Command c in Commands)
            {
                System.Console.WriteLine(c.ToString());
            }
        }

        private void PrintCommand(Command command)
        {
            System.Console.WriteLine("Command:      <{0}>", command.Name);
            System.Console.WriteLine("Description:  {0}", command.Description);
            System.Console.WriteLine("Usage:        {0}", command.Usage);
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            foreach (Option o in command.CommandOptions)
            {
                System.Console.WriteLine(o.ToString());
            }
        }

        public class Command : BaseCommand
        {
            public List<Option> CommandOptions { get; set; } = new List<Option>();
            public Action<Configuration> Run { get; set; }
            public override string ToString()
            {
                return string.Format("{0}{1}{2}", INDENT, Name.PadRight(25), Description);
            }
        }

        public class Option : BaseCommand
        {
            public Action<string> SetParameterValue;
            public override string ToString()
            {
                string name = SetParameterValue == null ? Name : Name + "=<parameter>";
                return string.Format("{0}{1}{2}", INDENT, name.PadRight(25), Description);
            }
        }

        public class BaseCommand
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string Usage { get; set; } = "";
        }
    }
}
