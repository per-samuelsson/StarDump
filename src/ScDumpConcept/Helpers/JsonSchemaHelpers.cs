using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScDumpConcept
{
    public static class JsonSchemaHelpers
    {
        /// <summary>
        /// JsonSchema help method for <see cref="Person"/>
        /// </summary>
        #region Person Extensions
        public static object ToJsonSchemaPersonAlternative2()
        {
            return new Dictionary<string, object>
            {
                { "title", nameof(Person) },
                { "type", "object"},
                { "properties", new Dictionary<string, object>
                    {
                        { "$id", Helper.JsonSchemaPropertyConfiguration(true, "string")},
                        { nameof(Person.FirstName), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Person.LastName), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Person.Age), Helper.JsonSchemaPropertyNullConfiguration(true, "integer")},
                        { nameof(Person.Address), Helper.JsonSchemaReferenceObjectConfig()},
                        { nameof(Person.Workplace), Helper.JsonSchemaReferenceObjectConfig()}
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonSchema help method for <see cref="Address"/>
        /// </summary>
        #region Address Extensions
        public static object ToJsonSchemaAddressAlternative2()
        {
            return new Dictionary<string, object>
            {
                { "title", nameof(Address) },
                { "type", "object"},
                { "properties", new Dictionary<string, object>
                    {
                        { "$id", Helper.JsonSchemaPropertyConfiguration(true, "string")},
                        { nameof(Address.Street), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Address.ZIP), Helper.JsonSchemaPropertyNullConfiguration(true, "integer")},
                        { nameof(Address.City), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Address.Country), Helper.JsonSchemaPropertyNullConfiguration(true, "string")}
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonSchema help method for <see cref="Company"/>
        /// </summary>
        #region Company Extensions
        public static object ToJsonSchemaCompanyAlternative2()
        {
            return new Dictionary<string, object>
            {
                { "title", nameof(Company) },
                { "type", "object"},
                { "properties", new Dictionary<string, object>
                    {
                        { "$id", Helper.JsonSchemaPropertyConfiguration(true, "string")},
                        { nameof(Company.Name), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Company.Address), Helper.JsonSchemaReferenceObjectConfig()}
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonSchema help method for <see cref="Department"/>
        /// </summary>
        #region Department Extensions
        public static object ToJsonSchemaDepartmentAlternative2()
        {
            return new Dictionary<string, object>
            {
                { "title", nameof(Department) },
                { "type", "object"},
                { "properties", new Dictionary<string, object>
                    {
                        { "$id", Helper.JsonSchemaPropertyConfiguration(true, "string")},
                        { nameof(Department.Name), Helper.JsonSchemaPropertyNullConfiguration(true, "string")},
                        { nameof(Department.Company), Helper.JsonSchemaReferenceObjectConfig()}
                    }
                }
            };
        }
        #endregion

    }
}
