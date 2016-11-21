using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScDumpConcept
{
    public static class JsonStructureHelpers
    {
        /// <summary>
        /// JsonStructure help method for <see cref="Person"/>
        /// </summary>
        #region Person Extensions
        public static object ToJsonStructurePersonAlternative1()
        {
            Func<string, string> path = (type) => { return "$." + type + ".<index>"; };
            Func<string, string> refPath = (type) => { return "SELECT x FROM $." + type + " x WHERE x.$id = this.$ref"; };
            return new
            {
                ClassName = nameof(Person),
                JsonPath = path(nameof(Person)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Person.FirstName),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.FirstName)
                    },
                    new
                    {
                        Name = nameof(Person.LastName),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.LastName)
                    },
                    new
                    {
                        Name = nameof(Person.Age),
                        Type = Constants.TYPE_INT,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Age)
                    },
                    new
                    {
                        Name = nameof(Person.Address),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Address)),
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Address)
                    },
                    new
                    {
                        Name = nameof(Person.Workplace),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Department)),
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Workplace)
                    }
                }
            };
        }

        public static object ToJsonStructurePersonAlternative2()
        {
            Func<string, string> path = (type) => { return "$." + type + "." + type + "_<ObjectNo>"; };
            Func<string, string> refPath = (type) => { return "$." + type + "." + type + "_<$ref>"; };
            return new
            {
                ClassName = nameof(Person),
                JsonPath = path(nameof(Person)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Person.FirstName),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.FirstName)
                    },
                    new
                    {
                        Name = nameof(Person.LastName),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.LastName)
                    },
                    new
                    {
                        Name = nameof(Person.Age),
                        Type = Constants.TYPE_INT,
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Age)
                    },
                    new
                    {
                        Name = nameof(Person.Address),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Address)),
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Address)
                    },
                    new
                    {
                        Name = nameof(Person.Workplace),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Department)),
                        JsonPath = path(nameof(Person)) + "." + nameof(Person.Workplace)
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonStructure help method for <see cref="Address"/>
        /// </summary>
        #region Address Extensions
        public static object ToJsonStructureAddressAlternative1()
        {
            Func<string, string> path = (type) => { return "$." + type + ".<index>"; };
            Func<string, string> refPath = (type) => { return "SELECT x FROM $." + type + " x WHERE x.$id = this.$ref"; };
            return new
            {
                ClassName = nameof(Address),
                JsonPath = path(nameof(Address)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Address.Street),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.Street)
                    },
                    new
                    {
                        Name = nameof(Address.ZIP),
                        Type = Constants.TYPE_INT,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.ZIP)
                    },
                    new
                    {
                        Name = nameof(Address.City),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.City)
                    },
                    new
                    {
                        Name = nameof(Address.Country),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.Country)
                    },
                }
            };
        }

        public static object ToJsonStructureAddressAlternative2()
        {
            Func<string, string> path = (type) => { return "$." + type + "." + type + "_<ObjectNo>"; };
            return new
            {
                ClassName = nameof(Address),
                JsonPath = path(nameof(Address)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Address.Street),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.Street)
                    },
                    new
                    {
                        Name = nameof(Address.ZIP),
                        Type = Constants.TYPE_INT,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.ZIP)
                    },
                    new
                    {
                        Name = nameof(Address.City),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.City)
                    },
                    new
                    {
                        Name = nameof(Address.Country),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Address)) + "." + nameof(Address.Country)
                    },
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonStructure help method for <see cref="Company"/>
        /// </summary>
        #region Company Extensions
        public static object ToJsonStructureCompanyAlternative1()
        {
            Func<string, string> path = (type) => { return "$." + type + ".<index>"; };
            Func<string, string> refPath = (type) => { return "SELECT x FROM $." + type + " x WHERE x.$id = this.$ref"; };
            return new
            {
                ClassName = nameof(Company),
                JsonPath = path(nameof(Company)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Company.Name),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Company)) + "." + nameof(Company.Name)
                    },
                    new
                    {
                        Name = nameof(Company.Address),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Address)),
                        JsonPath = path(nameof(Company)) + "." + nameof(Company.Address)
                    }
                }
            };
        }

        public static object ToJsonStructureCompanyAlternative2()
        {
            Func<string, string> path = (type) => { return "$." + type + "." + type + "_<ObjectNo>"; };
            return new
            {
                ClassName = nameof(Company),
                JsonPath = path(nameof(Company)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Company.Name),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Department)) + "." + nameof(Department.Name)
                    },
                    new
                    {
                        Name = nameof(Company.Address),
                        Type = path(nameof(Address)),
                        JsonPath = path(nameof(Company)) + "." + nameof(Company.Address)
                    }
                }
            };
        }
        #endregion

        /// <summary>
        /// JsonStructure help method for <see cref="Department"/>
        /// </summary>
        #region Department Extensions
        public static object ToJsonStructureDepartmentAlternative1()
        {
            Func<string, string> path = (type) => { return "$." + type + ".<index>"; };
            Func<string, string> refPath = (type) => { return "SELECT x FROM $." + type + " x WHERE x.$id = this.$ref"; };
            return new
            {
                ClassName = nameof(Department),
                JsonPath = path(nameof(Department)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Department.Name),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Department)) + "." + nameof(Department.Name)
                    },
                    new
                    {
                        Name = nameof(Department.Company),
                        Type = Constants.TYPE_CLASS_REFERENCE,
                        ReferenceJsonPath = refPath(nameof(Company)),
                        JsonPath = path(nameof(Department)) + "." + nameof(Department.Company)
                    }
                }
            };
        }

        public static object ToJsonStructureDepartmentAlternative2()
        {
            Func<string, string> path = (type) => { return "$." + type + "." + type + "_<ObjectNo>"; };
            return new
            {
                ClassName = nameof(Department),
                JsonPath = path(nameof(Department)),
                LanguageTypes = "CSharp",
                Properties = new List<object>
                {
                    new
                    {
                        Name = nameof(Department.Name),
                        Type = Constants.TYPE_STRING,
                        JsonPath = path(nameof(Department)) + "." + nameof(Department.Name)
                    },
                    new
                    {
                        Name = nameof(Department.Company),
                        Type = path(nameof(Company)),
                        JsonPath = path(nameof(Department)) + "." + nameof(Department.Company)
                    }
                }
            };
        }
        #endregion
    }
}
