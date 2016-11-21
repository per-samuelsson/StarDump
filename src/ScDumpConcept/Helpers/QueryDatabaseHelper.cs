using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starcounter;

namespace ScDumpConcept
{
    internal static class QueryDatabaseHelper
    {
        public static QueryResultRows<Address> QueryAllAddresses()
        {
            return Db.SQL<Address>("SELECT a FROM ScDumpConcept.Address a");
        }

        public static QueryResultRows<Company> QueryAllCompanies()
        {
            return Db.SQL<Company>("SELECT c FROM ScDumpConcept.Company c");
        }

        public static QueryResultRows<Department> QueryAllDepartments()
        {
            return Db.SQL<Department>("SELECT d FROM ScDumpConcept.Department d");
        }

        public static QueryResultRows<Person> QueryAllPersons()
        {
            return Db.SQL<Person>("SELECT p FROM ScDumpConcept.Person p");
        }
    }
}
