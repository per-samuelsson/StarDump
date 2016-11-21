using Newtonsoft.Json;
using Starcounter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScDumpConcept
{
    public static class Extensions
    {
        /// <summary>
        /// Extensions for <see cref="Person"/>
        /// </summary>
        #region Person Extensions
        // Json helpers
        public static object ToJsonAlternative1(this Person p)
        {
            return new Dictionary<string, object>
            {
                { "$id", p.GetObjectNo().ToString() },
                { nameof(p.FirstName), p.FirstName },
                { nameof(p.LastName), p.LastName },
                { nameof(p.Age), p.Age },
                { nameof(p.Address), new Dictionary<string, string> { { "$ref", p.Address?.GetObjectNo().ToString() } } },
                { nameof(p.Workplace), new Dictionary<string, string> { { "$ref", p.Workplace?.GetObjectNo().ToString() } } }
            };
        }

        // JsonDictionary helpers
        public static KeyValuePair<string, object> ToJsonAlternative2(this Person p)
        {
            return new KeyValuePair<string, object>(
                p?.ObjectPropertyName(),
                new Dictionary<string, object>
                    {
                        { nameof(p.FirstName), p.FirstName },
                        { nameof(p.LastName), p.LastName },
                        { nameof(p.Age), p.Age },
                        { nameof(p.Address), new Dictionary<string, string> { { "$ref", p.Address?.ReferenceJsonPath() } } },
                        { nameof(p.Workplace), new Dictionary<string, string> { { "$ref", p.Workplace?.ReferenceJsonPath() } } }
                    });
        }

        public static void ToJsonAlternative2Stream(this Person p, JsonTextWriter writer)
        {
            writer.WritePropertyName(p?.ObjectPropertyName());
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(p.FirstName));
            writer.WriteValue(p.FirstName);

            writer.WritePropertyName(nameof(p.LastName));
            writer.WriteValue(p.LastName);

            writer.WritePropertyName(nameof(p.Age));
            writer.WriteValue(p.Age);

            writer.WritePropertyName(nameof(p.Address));
            writer.WriteStartObject();
            writer.WritePropertyName("$ref");
            writer.WriteValue(p.Address?.ReferenceJsonPath());
            writer.WriteEndObject();

            writer.WritePropertyName(nameof(p.Workplace));
            writer.WriteStartObject();
            writer.WritePropertyName("$ref");
            writer.WriteValue(p.Workplace?.ReferenceJsonPath());
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public static string ObjectPropertyName(this Person p)
        {
            return Helper.ObjectPropertyName(nameof(Person), p.GetObjectNo().ToString());
        }

        public static string ReferenceJsonPath(this Person p)
        {
            return Helper.ReferenceJsonPath(nameof(Person), p.GetObjectNo().ToString());
        }
        #endregion

        /// <summary>
        /// Extensions for <see cref="Address"/>
        /// </summary>
        #region Address Extensions
        // Json helpers
        public static object ToJsonAlternative1(this Address a)
        {
            return new Dictionary<string, object>
            {
                { "$id", a.GetObjectNo().ToString() },
                { nameof(a.Street), a.Street },
                { nameof(a.ZIP), a.ZIP },
                { nameof(a.City), a.City },
                { nameof(a.Country), a.Country }
            };
        }

        // JsonDictionary helpers
        public static KeyValuePair<string, object> ToJsonAlternative2(this Address a)
        {
            return new KeyValuePair<string, object>(
                a?.ObjectPropertyName(),
                new Dictionary<string, object>
                    {
                        { nameof(a.Street), a.Street },
                        { nameof(a.ZIP), a.ZIP },
                        { nameof(a.City), a.City },
                        { nameof(a.Country), a.Country }
                    });
        }

        public static void ToJsonAlternative2Stream(this Address a, JsonTextWriter writer)
        {
            writer.WritePropertyName(a?.ObjectPropertyName());
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(a.Street));
            writer.WriteValue(a.Street);

            writer.WritePropertyName(nameof(a.ZIP));
            writer.WriteValue(a.ZIP);

            writer.WritePropertyName(nameof(a.City));
            writer.WriteValue(a.City);

            writer.WritePropertyName(nameof(a.Country));
            writer.WriteValue(a.Country);

            writer.WriteEndObject();
        }

        public static string ObjectPropertyName(this Address a)
        {
            return Helper.ObjectPropertyName(nameof(Address), a.GetObjectNo().ToString());
        }

        public static string ReferenceJsonPath(this Address a)
        {
            return Helper.ReferenceJsonPath(nameof(Address), a.GetObjectNo().ToString());
        }
        #endregion

        /// <summary>
        /// Extensions for <see cref="Company"/>
        /// </summary>
        #region Company Extensions
        // Json helpers
        public static object ToJsonAlternative1(this Company c)
        {
            return new Dictionary<string, object>
            {
                { "$id", c.GetObjectNo().ToString() },
                { nameof(c.Name), c.Name },
                { nameof(c.Address), new Dictionary<string, string> { { "$ref", c.Address?.GetObjectNo().ToString() } } }
            };
        }

        // JsonDictionary helpers
        public static KeyValuePair<string, object> ToJsonAlternative2(this Company c)
        {
            return new KeyValuePair<string, object>(
                c?.ObjectPropertyName(),
                new Dictionary<string, object>
                    {
                        { nameof(c.Name), c.Name },
                        { nameof(c.Address), new Dictionary<string, string> { { "$ref", c.Address?.ReferenceJsonPath() } } }
                    });
        }

        public static void ToJsonAlternative2Stream(this Company c, JsonTextWriter writer)
        {
            writer.WritePropertyName(c?.ObjectPropertyName());
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(c.Name));
            writer.WriteValue(c.Name);
            
            writer.WritePropertyName(nameof(c.Address));
            writer.WriteStartObject();
            writer.WritePropertyName("$ref");
            writer.WriteValue(c.Address?.ReferenceJsonPath());
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public static string ObjectPropertyName(this Company c)
        {
            return Helper.ObjectPropertyName(nameof(Company), c.GetObjectNo().ToString());
        }

        public static string ReferenceJsonPath(this Company c)
        {
            return Helper.ReferenceJsonPath(nameof(Company), c.GetObjectNo().ToString());
        }
        #endregion

        /// <summary>
        /// Extensions for <see cref="Department"/>
        /// </summary>
        #region Department Extensions
        // Json helpers
        public static object ToJsonAlternative1(this Department d)
        {
            return new Dictionary<string, object>
            {
                { "$id", d.GetObjectNo().ToString() },
                { nameof(d.Name), d.Name },
                { nameof(d.Company), new Dictionary<string, string> { { "$ref", d.Company?.GetObjectNo().ToString() } } }
            };
        }

        // JsonDictionary helpers
        public static KeyValuePair<string, object> ToJsonAlternative2(this Department d)
        {
            return new KeyValuePair<string, object>(
                d?.ObjectPropertyName(),
                new Dictionary<string, object>
                    {
                        { nameof(d.Name), d.Name },
                        { nameof(d.Company), new Dictionary<string, string> { { "$ref", d.Company?.ReferenceJsonPath() } } }
                    });
        }

        public static void ToJsonAlternative2Stream(this Department d, JsonTextWriter writer)
        {
            writer.WritePropertyName(d?.ObjectPropertyName());
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(d.Name));
            writer.WriteValue(d.Name);

            writer.WritePropertyName(nameof(d.Company));
            writer.WriteStartObject();
            writer.WritePropertyName("$ref");
            writer.WriteValue(d.Company?.ReferenceJsonPath());
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public static string ObjectPropertyName(this Department d)
        {
            return Helper.ObjectPropertyName(nameof(Department), d.GetObjectNo().ToString());
        }

        public static string ReferenceJsonPath(this Department d)
        {
            return Helper.ReferenceJsonPath(nameof(Department), d.GetObjectNo().ToString());
        }
        #endregion
    }
}
