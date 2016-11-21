using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace ScDumpConcept
{
    internal static class Helper
    {
        /// <summary>
        /// Generates Json schema for a specific Propery configuration, null is NOT accepted
        /// </summary>
        /// <param name="Required"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonSchemaPropertyConfiguration(bool Required, string Type)
        {
            return new Dictionary<string, object>
            {
                { "required", Required },
                { "type", Type }
            };
        }

        /// <summary>
        /// Generates Json schema for a specific Propery configuration, null IS accepted
        /// </summary>
        /// <param name="Required"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonSchemaPropertyNullConfiguration(bool Required, string Type)
        {
            return new Dictionary<string, object>
            {
                { "required", Required },
                { "type", new string[] { Type, "null" } }
            };
        }

        /// <summary>
        /// Generates Json schema for a database property which contains a reference property
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> JsonSchemaReferenceObjectConfig()
        {
            Dictionary<string, object> dict = JsonSchemaPropertyConfiguration(true, "object");
            dict.Add("properties", JsonShemaReferenceConfig());

            return dict;
        }

        /// <summary>
        /// Generates Json schema for a reference property
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> JsonShemaReferenceConfig()
        {
            return new Dictionary<string, object> { { "$ref", JsonSchemaPropertyNullConfiguration(true, "string") } };
        }

        #region helpers for JsonDictionary property names
        public static string ObjectPropertyName(string ClassName, string ObjectNo)
        {
            if (ClassName == null || ObjectNo == null)
            {
                return null;
            }
            return ClassName + "_" + ObjectNo;
        }

        public static string ReferenceJsonPath(string ClassName, string ObjectNo)
        {
            if (ClassName == null || ObjectNo == null)
            {
                return null;
            }
            return "$." + ClassName + "." + ObjectPropertyName(ClassName, ObjectNo);
        }
        #endregion
    }
}
