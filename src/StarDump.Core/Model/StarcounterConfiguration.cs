using System;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using Starcounter.Core;
using Starcounter.Core.Options;

namespace StarDump.Core
{
    public class StarcounterConfiguration
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public static StarcounterConfiguration GetConfiguration(string databaseNameOrDirectory)
        {
            StarcounterOptions options = StarcounterOptions.TryOpenExisting(databaseNameOrDirectory);
            StarcounterConfiguration result = new StarcounterConfiguration()
            {
                Name = options.DatabaseOptions.DatabaseName,
                Version = GetStarcounterVersion()
            };

            return result;
        }

        public static string GetStarcounterVersion()
        {
            string line = null;
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "star",
                    Arguments = "-v",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                line = process.StandardOutput.ReadLine();
            }

            if (string.IsNullOrEmpty(line))
            {
                throw new Exception("Unable to retrieve Starcounter version");
            }

            string[] parts = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new Exception("Invalid Starcounter version: " + line);
            }

            return parts[1];
        }
    }
}