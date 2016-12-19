using System;
using System.Net.Http;
using Newtonsoft.Json;
using Starcounter;
using Starcounter.Configuration;

namespace StarDump.Core
{
    public class StarcounterConfiguration
    {
        public string Name { get; set; }
        public int SystemHttpPort { get; set; }
        public int AggregationPort { get; set; }
        public string Version { get; set; }
        public string VersionDate { get; set; }
        public string Channel { get; set; }
        public string Edition { get; set; }

        public static StarcounterConfiguration GetCurrentConfiguration()
        {
            string path = Installation.GetInstallationDirectory();
            Installation installation = new Installation(path);
            ServerConfiguration config = ServerConfiguration.Load(installation.RepositoryPath);
            string url = string.Format("http://localhost:{0}/api/admin/servers/personal/settings", config.SystemHttpPort);
            HttpClient client = new HttpClient();

            string json = client.GetStringAsync(url).Result;
            StarcounterConfiguration result = JsonConvert.DeserializeObject<StarcounterConfiguration>(json);

            return result;
        }
    }
}