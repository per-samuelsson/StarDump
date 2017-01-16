using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using System.IO;

namespace StarDump
{
    public class CommandLineInterface
    {
        private CommandLineApplication commandLineApplication { get; set; }
        private StarDump.Common.Output Output = new StarDump.Common.Output();

        private const string HELP_TEMPLATE = "-? | -h | --help";

        public CommandLineInterface()
        {
            commandLineApplication = new CommandLineApplication();
            AddUnloadCommand();
            AddReloadCommand();
            AddBaseOptionsAndArguments();
        }

        public int Execute(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    commandLineApplication.ShowHelp();
                    return 0;
                }
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
            CommandOption optionSkipColumnPrefixes = null;
            CommandOption optionSkipTablePrefixes = null;
            commandLineApplication.Command("unload",
              (target) =>
              {
                  target.Description = "Unload database";
                  optionDatabase = target.Option("-db | --database <DatabaseName>", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("-f | --file <FileName>", "Output dump filename", CommandOptionType.SingleValue);
                  optionBufferSize = target.Option("-b | --buffersize <BufferSize>", "Set insert rows buffer size to dump database.", CommandOptionType.SingleValue);
                  optionSkipColumnPrefixes = target.Option("-scp | --skipcolumnprefixes <ColumnPrefixes>", "Column prefixes to skip, space and/or comma separated. Example: -scp=\"a b,c\"", CommandOptionType.SingleValue);
                  optionSkipTablePrefixes = target.Option("-stp | --skiptableprefixes <TablePrefixes>", "Table prefixes to skip, space and/or comma separated. Example: -stp=\"a b,c\"", CommandOptionType.SingleValue);

                  target.HelpOption(HELP_TEMPLATE);

                  target.OnExecute(() =>
                  {
                      StarDump.Common.Configuration starDumpConfiguration = new StarDump.Common.Configuration();

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

                      // SkipColumnPrefixes Option
                      if (optionSkipColumnPrefixes.HasValue())
                      {
                          starDumpConfiguration.SkipColumnPrefixes = optionSkipColumnPrefixes.Value().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                      }

                      // SkipTablePrefixes Option
                      if (optionSkipTablePrefixes.HasValue())
                      {
                          starDumpConfiguration.SkipTablePrefixes = optionSkipTablePrefixes.Value().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                      }

                      // Execute Unload
                      try
                      {
                          bool execution = StarDump.Core.CommandInterface.Unload(starDumpConfiguration, this.Output);
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
            CommandOption optionForceReload = null;
            CommandOption optionSkipTablePrefixes = null;
            commandLineApplication.Command("reload",
              (target) =>
              {
                  target.Description = "Reload database";
                  optionDatabase = target.Option("-db | --database <DatabaseName>", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("-f | --file <FullFileName>", "Full name to dump file", CommandOptionType.SingleValue);
                  optionForceReload = target.Option("-fr | --forcereload", "Force reload even if the database already contains data. User has to take care of object ID uniqueness.", CommandOptionType.NoValue);
                  optionSkipTablePrefixes = target.Option("-stp | --skiptableprefixes <TablePrefixes>", "Table prefixes to skip, space and/or comma separated. Example: -stp=\"a b,c\"", CommandOptionType.SingleValue);
                  target.HelpOption(HELP_TEMPLATE);

                  target.OnExecute(() =>
                  {
                      StarDump.Common.Configuration starDumpConfiguration = new StarDump.Common.Configuration();

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
                      starDumpConfiguration.ForceReload = optionForceReload.HasValue();

                      // SkipTablePrefixes Option
                      if (optionSkipTablePrefixes.HasValue())
                      {
                          starDumpConfiguration.SkipTablePrefixes = optionSkipTablePrefixes.Value().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                      }

                      // Execute Reload
                      try
                      {
                          bool execution = StarDump.Core.CommandInterface.Reload(starDumpConfiguration, this.Output);
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
