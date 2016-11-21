using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace ScDumpConcept
{
    internal class Handlers
    {
        private HandlerHelper helper = new HandlerHelper();

        public void Register()
        {
            /// <summary>
            /// Deletes all data <see cref="HandlerHelper.DeleteData"/>
            /// </summary>
            Handle.GET("/deletedata", () =>
            {
                //duration 1M ~ 7s
                var sw = new Stopwatch();
                sw.Start();
                helper.DeleteData();
                sw.Stop();
                return string.Format("Data deleted \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Creates data <see cref="HandlerHelper.CreateData"/>
            /// </summary>
            Handle.GET("/createdata", () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                helper.CreateData();
                sw.Stop();
                return string.Format("Data created \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Creates one entry of each class <see cref="HandlerHelper.CreateBigData(int)"/>
            /// </summary>
            Handle.GET("/createdata/1", () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                helper.CreateBigData(100000);
                sw.Stop();
                return string.Format("100k data created \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Creates 100k entries of all classes <see cref="HandlerHelper.CreateBigData(int)"/>
            /// </summary>
            Handle.GET("/createdata/100k", () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                helper.CreateBigData(100000);
                sw.Stop();
                return string.Format("100k data created \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Creates 1M entries of all classes <see cref="HandlerHelper.CreateBigData(int)"/>
            /// </summary>
            Handle.GET("/createdata/1M", () =>
            {
                //Duration ~ 30s
                var sw = new Stopwatch();
                sw.Start();
                helper.CreateBigData(1000000);
                sw.Stop();
                return string.Format("1M data created \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Recreates database
            /// Deletes all <see cref="HandlerHelper.DeleteData"/>
            /// Create "smart" data <see cref="HandlerHelper.CreateData"/>
            /// </summary>
            Handle.GET("/recreatedata", () =>
            {
                var sw = new Stopwatch();
                sw.Start();
                helper.DeleteData();
                helper.CreateData();
                sw.Stop();
                return string.Format("Data recreated \r\nExecusion time: {0}ms", sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Dump database to json file
            /// References property id [List structure]
            /// exmple: localhost:8080/dumpdata/alternative1/C:/ScDumpConcept
            /// </summary>
            Handle.GET("/dumpdata/alternative1/{?}", (string dumpPath) =>
            {
                // duration 1M ~ 33s, size ~610Mb

                var sw = new Stopwatch();
                sw.Start();

                // Init, create names and paths
                DirectoryInfo outputDir = new DirectoryInfo(dumpPath);
                if (outputDir.Exists == false)
                {
                    Directory.CreateDirectory(outputDir.FullName);
                }

                string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string fullPathJson = Path.Combine(outputDir.FullName, "dump_alt1_" + dateTime + ".json");
                string fullPathJsonSchema = Path.Combine(outputDir.FullName, "dump_alt1_" + dateTime + "_schema.json");
                string fullPathJsonStructure = Path.Combine(outputDir.FullName, "dump_alt1_" + dateTime + "_structure.json");

                if (File.Exists(fullPathJson) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJson, sw.ElapsedMilliseconds);
                }
                else if (File.Exists(fullPathJsonSchema) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonSchema, sw.ElapsedMilliseconds);
                }
                else if (File.Exists(fullPathJsonStructure) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonStructure, sw.ElapsedMilliseconds);
                }

                // Dump Json
                Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();
                jsonDictionary.Add(nameof(Person), QueryDatabaseHelper.QueryAllPersons().Select(x => x.ToJsonAlternative1()));
                jsonDictionary.Add(nameof(Address), QueryDatabaseHelper.QueryAllAddresses().Select(x => x.ToJsonAlternative1()));
                jsonDictionary.Add(nameof(Company), QueryDatabaseHelper.QueryAllCompanies().Select(x => x.ToJsonAlternative1()));
                jsonDictionary.Add(nameof(Department), QueryDatabaseHelper.QueryAllDepartments().Select(x => x.ToJsonAlternative1()));

                // Write to file
                string json = JsonConvert.SerializeObject(jsonDictionary, Formatting.Indented); //Self referencing loop error when using "new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }" argument
                File.WriteAllText(fullPathJson, json);

                // Dump JsonSchema
                Dictionary<string, object> jsonSchemaDictionary = new Dictionary<string, object>();
                jsonSchemaDictionary.Add(nameof(Person), JsonSchemaHelpers.ToJsonSchemaPersonAlternative2());
                jsonSchemaDictionary.Add(nameof(Address), JsonSchemaHelpers.ToJsonSchemaAddressAlternative2());
                jsonSchemaDictionary.Add(nameof(Company), JsonSchemaHelpers.ToJsonSchemaCompanyAlternative2());
                jsonSchemaDictionary.Add(nameof(Department), JsonSchemaHelpers.ToJsonSchemaDepartmentAlternative2());

                // Write to file
                string jsonSchema = JsonConvert.SerializeObject(jsonSchemaDictionary, Formatting.Indented);
                File.WriteAllText(fullPathJsonSchema, jsonSchema);

                // Dump JsonStructure
                List<object> jsonStructureList = new List<object>();
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructurePersonAlternative1());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureAddressAlternative1());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureCompanyAlternative1());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureDepartmentAlternative1());

                // Write to file
                string jsonStructure = JsonConvert.SerializeObject(jsonStructureList, Formatting.Indented);
                File.WriteAllText(fullPathJsonStructure, jsonStructure);

                sw.Stop();
                return string.Format("Data dumped \r\nFile \"{0}\" was successfully exported. \r\nFile \"{1}\" was successfully exported. \r\nExecution time: {2}ms", fullPathJson, fullPathJsonSchema, sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Dump database to json file
            /// References Property names [Dictionary structure]
            /// exmple: localhost:8080/dumpdata/alternative2/C:/ScDumpConcept
            /// </summary>
            Handle.GET("/dumpdata/alternative2/{?}", (string dumpPath) =>
            {
                // duration 1M ~ 37s, size ~650Mb
                var sw = new Stopwatch();
                sw.Start();

                // Init, create names and paths
                DirectoryInfo outputDir = new DirectoryInfo(dumpPath);
                if (outputDir.Exists == false)
                {
                    Directory.CreateDirectory(outputDir.FullName);
                }

                string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string fullPathJson = Path.Combine(outputDir.FullName, "dump_alt2_" + dateTime + ".json");
                //string fullPathJsonSchema = Path.Combine(outputDir.FullName, "dump_alt2_" + dateTime + "_schema.json");
                string fullPathJsonStructure = Path.Combine(outputDir.FullName, "dump_alt2_" + dateTime + "_structure.json");

                if (File.Exists(fullPathJson) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJson, sw.ElapsedMilliseconds);
                }
                //else if (File.Exists(fullPathJsonSchema) == true)
                //{
                //    sw.Stop();
                //    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonSchema, sw.ElapsedMilliseconds);
                //}
                else if (File.Exists(fullPathJsonStructure) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonStructure, sw.ElapsedMilliseconds);
                }

                // Dump Json
                Dictionary<string, object> jsonDictionary = new Dictionary<string, object>();
                jsonDictionary.Add(nameof(Person), QueryDatabaseHelper.QueryAllPersons().Select(x => x.ToJsonAlternative2()).ToDictionary((k) => k.Key, (v) => v.Value));
                jsonDictionary.Add(nameof(Address), QueryDatabaseHelper.QueryAllAddresses().Select(x => x.ToJsonAlternative2()).ToDictionary((k) => k.Key, (v) => v.Value));
                jsonDictionary.Add(nameof(Company), QueryDatabaseHelper.QueryAllCompanies().Select(x => x.ToJsonAlternative2()).ToDictionary((k) => k.Key, (v) => v.Value));
                jsonDictionary.Add(nameof(Department), QueryDatabaseHelper.QueryAllDepartments().Select(x => x.ToJsonAlternative2()).ToDictionary((k) => k.Key, (v) => v.Value));

                // Write to file
                string json = JsonConvert.SerializeObject(jsonDictionary, Formatting.Indented); //Self referencing loop error when using "new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects }" argument
                File.WriteAllText(fullPathJson, json);

                //
                // Validation Json schema will be about the same length as the Json file itself
                //

                // Dump JsonStructure
                List<object> jsonStructureList = new List<object>();
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructurePersonAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureAddressAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureCompanyAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureDepartmentAlternative2());

                // Write to file
                string jsonStructure = JsonConvert.SerializeObject(jsonStructureList, Formatting.Indented);
                File.WriteAllText(fullPathJsonStructure, jsonStructure);

                sw.Stop();
                //return string.Format("Data dumped \r\nFile \"{0}\" was successfully exported. \r\nFile \"{1}\" was successfully exported. \r\nExecution time: {2}ms", fullPathJson, fullPathJsonSchema, sw.ElapsedMilliseconds);
                return string.Format("Data dumped \r\nFile \"{0}\" was successfully exported. \r\nExecution time: {1}ms", fullPathJson, sw.ElapsedMilliseconds);
            });

            /// <summary>
            /// Dump database to json file 
            /// Same structure as alernative2 but continuosly writes to file
            /// References Property names [Dictionary structure]
            /// exmple: localhost:8080/dumpdata/alternative2stream/C:/ScDumpConcept
            /// </summary>
            Handle.GET("/dumpdata/alternative2stream/{?}", (string dumpPath) =>
            {
                // duration 1M ~ 14s, size ~650Mb
                var sw = new Stopwatch();
                sw.Start();

                // Init, create names and paths
                DirectoryInfo outputDir = new DirectoryInfo(dumpPath);
                if (outputDir.Exists == false)
                {
                    Directory.CreateDirectory(outputDir.FullName);
                }

                string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

                string fullPathJson = Path.Combine(outputDir.FullName, "dump_alt2stream_" + dateTime + ".json");
                //string fullPathJsonSchema = Path.Combine(outputDir.FullName, "dump_alt2stream_" + dateTime + "_schema.json");
                string fullPathJsonStructure = Path.Combine(outputDir.FullName, "dump_alt2stream_" + dateTime + "_structure.json");

                if (File.Exists(fullPathJson) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJson, sw.ElapsedMilliseconds);
                }
                //else if (File.Exists(fullPathJsonSchema) == true)
                //{
                //    sw.Stop();
                //    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonSchema, sw.ElapsedMilliseconds);
                //}
                else if (File.Exists(fullPathJsonStructure) == true)
                {
                    sw.Stop();
                    return string.Format("ABORTED!\r\nThere already exists a file called \"{0}\" \r\nExecusion time: {1}ms", fullPathJsonStructure, sw.ElapsedMilliseconds);
                }

                // Dump Json
                using (StreamWriter file = File.CreateText(fullPathJson))
                using(JsonTextWriter writer = new JsonTextWriter(file))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartObject();

                    // Person
                    writer.WritePropertyName(nameof(Person));
                    writer.WriteStartObject();
                    foreach (Person x in QueryDatabaseHelper.QueryAllPersons())
                    {
                        x.ToJsonAlternative2Stream(writer);
                    }
                    writer.WriteEndObject();

                    // Address
                    writer.WritePropertyName(nameof(Address));
                    writer.WriteStartObject();
                    foreach (Address x in QueryDatabaseHelper.QueryAllAddresses())
                    {
                        x.ToJsonAlternative2Stream(writer);
                    }
                    writer.WriteEndObject();

                    // Company
                    writer.WritePropertyName(nameof(Company));
                    writer.WriteStartObject();
                    foreach (Company x in QueryDatabaseHelper.QueryAllCompanies())
                    {
                        x.ToJsonAlternative2Stream(writer);
                    }
                    writer.WriteEndObject();

                    // Department
                    writer.WritePropertyName(nameof(Department));
                    writer.WriteStartObject();
                    foreach (Department x in QueryDatabaseHelper.QueryAllDepartments())
                    {
                        x.ToJsonAlternative2Stream(writer);
                    }
                    writer.WriteEndObject();

                    // End
                    writer.WriteEndObject();
                }

                //
                // Validation Json schema will be about the same length as the Json file itself
                //

                // Dump JsonStructure
                List<object> jsonStructureList = new List<object>();
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructurePersonAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureAddressAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureCompanyAlternative2());
                jsonStructureList.Add(JsonStructureHelpers.ToJsonStructureDepartmentAlternative2());

                // Write to file
                string jsonStructure = JsonConvert.SerializeObject(jsonStructureList, Formatting.Indented);
                File.WriteAllText(fullPathJsonStructure, jsonStructure);

                sw.Stop();
                //return string.Format("Data dumped \r\nFile \"{0}\" was successfully exported. \r\nFile \"{1}\" was successfully exported. \r\nExecution time: {2}ms", fullPathJson, fullPathJsonSchema, sw.ElapsedMilliseconds);
                return string.Format("Data dumped \r\nFile \"{0}\" was successfully exported. \r\nExecution time: {1}ms", fullPathJson, sw.ElapsedMilliseconds);
            });
        }
    }
}
