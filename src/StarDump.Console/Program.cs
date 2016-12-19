using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using StarDump;
using Microsoft.Extensions.CommandLineUtils;

namespace StarDump.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CommandLine commandLine = new CommandLine();
            //
            //if (commandLine.Parse(args) == false)
            //{
            //    commandLine.Out.WriteErrorLine();
            //    commandLine.Out.WriteErrorLine("Argument parsing failed.");
            //    return;
            //}
            //
            //if (commandLine.ValidateArguments() == false)
            //{
            //    commandLine.Out.WriteErrorLine();
            //    commandLine.Out.WriteErrorLine("Argument validation failed.");
            //    return;
            //}
            //
            //if (commandLine.SetOptions() == false)
            //{
            //    commandLine.Out.WriteErrorLine();
            //    commandLine.Out.WriteErrorLine("Setting command line options failed.");
            //    return;
            //}
            //
            //if (commandLine.Run() == false)
            //{
            //    commandLine.Out.WriteErrorLine();
            //    commandLine.Out.WriteErrorLine("Execution failed.");
            //    return;
            //}

            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            CommandOption optionDatabase = null;
            CommandOption optionDump = null;
            commandLineApplication.Command("unload",
              (target) =>
              {
                  target.Description = "Unload database";
                  optionDatabase = target.Option("-d | --database <database>", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("--dump <dump>", "Output dump filename", CommandOptionType.SingleValue);

                  target.HelpOption("-? | -h | --help");

                  target.OnExecute(() =>
                  {
                      if (!optionDatabase.HasValue())
                      {
                          return -1;
                      }
                      if (!optionDump.HasValue())
                      {
                          return -1;
                      }

                      StarDump.Configuration configuration = new StarDump.Configuration();
                      configuration.DatabaseName = optionDatabase.Value();

                      string dump = optionDump.Value();
                      if (dump.EndsWith(".sqlite3") == false)
                      {
                          dump += ".sqlite3";
                      }

                      string path = Path.GetTempPath(); // TODO temporary path?
                      configuration.FileName = Path.IsPathRooted(dump) ? dump : Path.Combine(path, dump);

                      StarDump.CommandInterface.Unload(configuration);

                      return 0;
                  });


              });

            commandLineApplication.Command("reload",
              (target) =>
              {
                  target.Description = "Reload database";
                  optionDatabase = target.Option("--database", "Name of starcounter database to dump", CommandOptionType.SingleValue);
                  optionDump = target.Option("--dump", "Output dump filename", CommandOptionType.SingleValue);
                  target.HelpOption("-? | -h | --help");

                  target.OnExecute(() =>
                  {


                      return 0;
                  });
              });

            CommandOption tempOption = commandLineApplication.Option("-d | --database <database>", "Name of starcounter database to dump", CommandOptionType.SingleValue);

            commandLineApplication.OnExecute(() =>
            {
                if (!tempOption.HasValue())
                {
                    return -1;
                }

                System.Console.WriteLine(tempOption.Value());
                return 0;
            });

            commandLineApplication.Execute(args);
        }
    }
}
