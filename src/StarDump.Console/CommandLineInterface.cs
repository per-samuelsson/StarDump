using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace StarDump.Console
{
    public class CommandLineInterface
    {
        private CommandLineApplication commandLineApplication { get; set; }
        private ConsoleOutput Output = new ConsoleOutput();

        private const string HELP_TEMPLATE = "-? | -h | --help";

        public CommandLineInterface()
        {
            commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            AddUnloadCommand();
            AddReloadCommand();
            AddBaseOptionsAndArguments();
        }

        public int Execute(string[] args)
        {
            try
            {
                return commandLineApplication.Execute(args);
            }
            catch(Exception e)
            {
                commandLineApplication.ShowHelp();
                Output.WriteErrorLine(e);
                return -1;
            }
            
        }

        private void AddUnloadCommand()
        {
            CommandOption optionDatabase = null;
            CommandOption optionDump = null;
            CommandOption optionBufferSize = null;
            commandLineApplication.Command("unload",
              (target) =>
              {
                  target.Description = "Unload database";
                  optionDatabase = target.Option("-d | --database <databasename>", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("--dump <filename>", "Output dump filename", CommandOptionType.SingleValue);
                  optionBufferSize = target.Option("--buffersize <buffersize>", "Set insert rows buffer size to dump database.", CommandOptionType.SingleValue);

                  target.HelpOption(HELP_TEMPLATE);

                  target.OnExecute(() =>
                  {
                      StarDump.Configuration starDumpConfiguration = new StarDump.Configuration();

                      // Database Option
                      if (optionDatabase.HasValue())
                      {
                          starDumpConfiguration.DatabaseName = optionDatabase.Value();
                      }

                      // Dump Option
                      if (optionDump.HasValue())
                      {
                          string dump = optionDump.Value();
                          if (dump.EndsWith(".sqlite3") == false)
                          {
                              dump += ".sqlite3";
                          }

                          string path = Path.GetTempPath(); // TODO temporary path?
                          starDumpConfiguration.FileName = Path.IsPathRooted(dump) ? dump : Path.Combine(path, dump);
                      }

                      // BufferSize Option
                      if (optionBufferSize.HasValue())
                      {
                          int v;

                          if (Int32.TryParse(optionBufferSize.Value(), out v) == true)
                          {
                              starDumpConfiguration.InsertRowsBufferSize = v;
                          }
                          else
                          {
                              Output.WriteWarningLine(string.Format("Could not parse \"{0}\" to int, using default value, {1}={2}", 
                                  optionBufferSize.Value(),
                                  "--" + optionBufferSize.LongName, 
                                  starDumpConfiguration.InsertRowsBufferSize));
                          }
                      }

                      // Execute Unload
                      try
                      {
                          bool execution = StarDump.CommandInterface.Unload(starDumpConfiguration);
                          return execution ? 0 : -1;
                      }
                      catch (Exception e)
                      {
                          target.ShowHelp(target.Name);
                          Output.WriteErrorLine(e);
                          return -1;
                      }
                  });

              });
        }

        private void AddReloadCommand()
        {
            CommandOption optionDatabase = null;
            CommandOption optionDump = null;
            commandLineApplication.Command("reload",
              (target) =>
              {
                  target.Description = "Reload database";
                  optionDatabase = target.Option("--database <databasename>", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("--dump <fullfilename>", "Output dump filename", CommandOptionType.SingleValue);
                  target.HelpOption(HELP_TEMPLATE);

                  target.OnExecute(() =>
                  {
                      StarDump.Configuration starDumpConfiguration = new StarDump.Configuration();

                      // Database Option
                      if (!optionDatabase.HasValue())
                      {
                          Output.WriteErrorLine(string.Format("Option: {0} has not been set.", optionDatabase.Template));
                          target.ShowHelp(target.Name);
                          return -1;
                      }

                      // Dump Option
                      if (!optionDump.HasValue())
                      {
                          Output.WriteErrorLine(string.Format("Option: {0} has not been set.", optionDump.Template));
                          target.ShowHelp(target.Name);
                          return -1;
                      }

                      starDumpConfiguration.DatabaseName = optionDatabase.Value();
                      starDumpConfiguration.FileName = optionDump.Value();

                      // Execute Reload
                      try
                      {
                          bool execution = StarDump.CommandInterface.Reload(starDumpConfiguration);
                          return execution ? 0 : -1;
                      }
                      catch (Exception e)
                      {
                          target.ShowHelp(target.Name);
                          Output.WriteErrorLine(e);
                          return -1;
                      }
                      
                  });
              });
        }

        private void AddBaseOptionsAndArguments()
        {
            commandLineApplication.HelpOption(HELP_TEMPLATE);
        }
    }
}
