using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarDump;
using System.IO;

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
        private Dictionary<Option, string> OptionsUsed { get; set; } = new Dictionary<Option, string>();

        public StarDump.Configuration Configuration { get; set; }

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
                        Description = "Name of starcounter database to dump",
                        SetParameterValue = (value) => Configuration.DatabaseName = value,
                        Required = false
                    },
                    new Option
                    {
                        Name = "--dump",
                        Description = "Output dump filename",
                        SetParameterValue = (value) => 
                        {
                            if (value.EndsWith(".sqlite3") == false)
                            {
                                value += ".sqlite3";
                            }

                            string path = Path.GetTempPath(); // TODO temporary path?
                            Configuration.FileName = Path.IsPathRooted(value) ? value : Path.Combine(path, value);
                        },
                        Required = false
                    },
                    new Option
                    {
                        Name = "--buffersize",
                        Description = "Set insert rows buffer size to dump database.",
                        SetParameterValue = (value) =>
                        {
                            int v;

                            if (Int32.TryParse(value, out v) == true)
                            {
                                Configuration.InsertRowsBufferSize = v;
                            }
                            else
                            {
                                Out.WriteWarningLine(string.Format("Could not parse \"{0}\" to int, using default value for <--buffersize>, {1}", value, Configuration.InsertRowsBufferSize));
                            }
                        },
                        Required = false
                    }
                },
                Run = (x) => { return StarDump.CommandInterface.Unload(x); }
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
                Run = (x) => { return StarDump.CommandInterface.Reload(x); }
            });

            Commands.Add(new Command
            {
                Name = COMMAND_HELP,
                Description = "Show help of all commands",
                Usage = "stardump help [<command options>]",
                Run = (x) => { return PrintHelp(); }
            });

            ConfigurationInit();
        }

        private void ConfigurationInit()
        {
            Configuration = new StarDump.Configuration();
            Configuration.Verbose = 1;
            Configuration.DatabaseName = "default";
            Configuration.SkipColumnPrefixes = new string[] { "__" };
            Configuration.SkipTablePrefixes = new string[] { "Starcounter.", "Concepts." };
            Configuration.InsertRowsBufferSize = 25;

            string name = string.Format("stardump-{0}-{1}.sqlite3", Configuration.DatabaseName, DateTime.Now.ToString("yyyy.MM.dd-HH.mm"));
            string path = Path.GetTempPath();

            Configuration.FileName = Path.Combine(path, name);
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
                        Out.WriteErrorLine(string.Format("<{0}> may not contain more than 1 equal sign (=)", (string)enumerator.Current));
                        Out.WriteErrorLine();
                        PrintCommand(CurrentCommand);
                        return false;
                    }

                    option = CurrentCommand.CommandOptions.Where(x => x.Name == optionArg).FirstOrDefault();

                    if (option == null)
                    {
                        Out.WriteErrorLine(string.Format("<{0}> is not a command option for command <{1}>", optionArg, CurrentCommand.Name));
                        Out.WriteErrorLine();
                        PrintCommand(CurrentCommand);
                        return false;
                    }
                    else
                    {
                        OptionsUsed.Add(option, optionParamterArg);
                    }
                }

                return true;
            }

            // Should never end up here
            Out.WriteErrorLine(string.Format("<{0}> is not a valid command", (string)enumerator.Current));
            Out.WriteErrorLine();
            PrintHelp();
            return false;
        }

        public bool ValidateArguments()
        {
            // Validate if all Required command options has been set
            foreach (Option option in CurrentCommand.CommandOptions.Where(o => o.Required == true))
            {
                if (OptionsUsed.ContainsKey(option) == false)
                {
                    Out.WriteErrorLine(string.Format("Command option <{0}> is required but was not set", option.Name));
                    Out.WriteErrorLine("");
                    PrintCommand(CurrentCommand);
                    return false;
                }
            }

            // Validate all command options which needs a parameter
            foreach (var x in OptionsUsed)
            {
                if (x.Key.SetParameterValue != null && String.IsNullOrEmpty(x.Value))
                {
                    Out.WriteErrorLine(string.Format("Command option <{0}> needs a parameter value", x.Key.Name));
                    Out.WriteErrorLine("");
                    PrintCommand(CurrentCommand);
                    return false;
                }
            }

            return true;
        }

        public bool SetOptions()
        {
            foreach (var x in OptionsUsed)
            {
                if (x.Value != null)
                {
                    x.Key.SetParameterValue(x.Value);
                }
                else
                {
                    // Use default value
                }
            }

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

        private bool PrintHelp()
        {
            Out.WriteLine("Usage: stardump <command> [<command options>]");
            Out.WriteLine();
            Out.WriteLine("Commands:");
            foreach(Command c in Commands)
            {
                Out.WriteLine(c.ToString());
            }

            return true;
        }

        private void PrintCommand(Command command)
        {
            Out.WriteLine(string.Format("Command:      <{0}>", command.Name));
            Out.WriteLine(string.Format("Description:  {0}", command.Description));
            Out.WriteLine(string.Format("Usage:        {0}", command.Usage));
            Out.WriteLine();

            List<Option> options = command.CommandOptions.Where(x => x.Required == true).ToList();
            if (options.Count() > 0)
            {
                Out.WriteLine("Options (Required):");
                foreach (Option o in options)
                {
                    Out.WriteLine(o.ToString());
                }
            }

            Out.WriteLine();

            options = command.CommandOptions.Where(x => x.Required == false).ToList();
            if (options.Count() > 0)
            {
                Out.WriteLine("Options (Optional):");
                foreach (Option o in options)
                {
                    Out.WriteLine(o.ToString());
                }
            }
        }

        public class Command : BaseCommand
        {
            public List<Option> CommandOptions { get; set; } = new List<Option>();
            public Func<Configuration, bool> Run { get; set; }
            public override string ToString()
            {
                return string.Format("{0}{1}{2}", INDENT, Name.PadRight(25), Description);
            }
        }

        public class Option : BaseCommand
        {
            public Action<string> SetParameterValue;
            public bool Required { get; set; } = false;
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
