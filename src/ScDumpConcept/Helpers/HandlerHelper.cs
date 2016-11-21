using System;
using System.Collections.Generic;
using System.Linq;
using Starcounter;

namespace ScDumpConcept
{
    internal class HandlerHelper
    {
        /// <summary>
        /// Delete all data
        /// </summary>
        public void DeleteData()
        {
            var addresses = QueryDatabaseHelper.QueryAllAddresses();
            var companies = QueryDatabaseHelper.QueryAllCompanies();
            var departments = QueryDatabaseHelper.QueryAllDepartments();
            var persons = QueryDatabaseHelper.QueryAllPersons();

            Db.Transact(() =>
            {
                // Address
                foreach (var address in addresses)
                {
                    address.Delete();
                }

                // Company
                foreach (var company in companies)
                {
                    company.Delete();
                }

                // Department
                foreach (var department in departments)
                {
                    department.Delete();
                }

                // Person
                foreach (var person in persons)
                {
                    person.Delete();
                }
            });
        }

        /// <summary>
        /// Creating data, some properties are null by choice
        /// </summary>
        public void CreateData()
        {
            Db.Transact(() =>
            {
                // Address
                Address address1 = new Address
                {
                    Street = "FirstStreet",
                    ZIP = 11111,
                    City = "FirstCity",
                    Country = "FirstCountry"
                };
                Address address2 = new Address
                {
                    Street = "SecondStreet",
                    ZIP = 22222,
                    City = "SecondCity",
                    Country = "SecondCountry"
                };
                Address address3 = new Address
                {
                    Street = "ThirdStreet",
                    ZIP = 33333,
                    City = "ThirdCity",
                    Country = "ThirdCountry"
                };
                Address address4 = new Address
                {
                    Street = "FourthStreet",
                    ZIP = 44444,
                    City = "FourthCity",
                    Country = "FourthCountry"
                };
                Address address5 = new Address
                {
                    Street = "FifthStreet",
                    ZIP = 55555,
                    City = "FifthCity",
                    Country = "FifthCountry"
                };

                // Company
                Company company1 = new Company
                {
                    Name = "FirstCompany",
                    Address = address1
                };
                Company company2 = new Company
                {
                    Name = "SecondCompany",
                    Address = address2
                };

                // Department
                Department department1 = new Department
                {
                    Name = "HQ",
                    Company = company1
                };
                Department department2 = new Department
                {
                    Name = "R&D",
                    Company = company1
                };
                Department department3 = new Department
                {
                    Name = "Sales",
                    Company = company1
                };
                Department department4 = new Department
                {
                    Name = "HQ",
                    Company = company2
                };
                Department department5 = new Department
                {
                    Name = "Sales",
                    Company = company2
                };

                // Person
                Person person1 = new Person
                {
                    FirstName = "F1",
                    LastName = "L1",
                    Age = 31,
                    Address = address1,
                    Workplace = department1
                };
                Person person2 = new Person
                {
                    FirstName = "F2",
                    LastName = "L2",
                    Age = 32,
                    Address = null,
                    Workplace = department2
                };
                Person person3 = new Person
                {
                    FirstName = "F3",
                    LastName = "L3",
                    Age = 33,
                    Address = address3,
                    Workplace = department3
                };
                Person person4 = new Person
                {
                    FirstName = "F4",
                    LastName = "N4",
                    Age = 34,
                    Address = address4,
                    Workplace = department4
                };
                Person person5 = new Person
                {
                    FirstName = "F5",
                    LastName = "N56",
                    Age = 35,
                    Address = address5,
                    Workplace = department5
                };
                Person person6 = new Person
                {
                    FirstName = "F6",
                    LastName = "F56",
                    Age = 36,
                    Address = address5,
                    Workplace = department1
                };
                Person person7 = new Person
                {
                    FirstName = "F7",
                    LastName = "L7",
                    Age = 37,
                    Address = null,
                    Workplace = null
                };
            });
        }

        /// <summary>
        /// Creating loops number of Address, Company, Department and Person database entries
        /// </summary>
        /// <param name="loops"></param>
        public void CreateBigData(int loops)
        {
            Db.Transact(() =>
            {
                for (int i = 0; i < loops; i++)
                {
                    string loopNr = i.ToString();
                    Address address = new Address
                    {
                        Street = loopNr,
                        ZIP = i,
                        City = loopNr,
                        Country = loopNr
                    };

                    Company company = new Company
                    {
                        Name = loopNr,
                        Address = address
                    };

                    Department department = new Department
                    {
                        Name = loopNr,
                        Company = company
                    };

                    Person person = new Person
                    {
                        FirstName = loopNr,
                        LastName = loopNr,
                        Age = i,
                        Address = address,
                        Workplace = department
                    };
                }
            });
        }
    }
}
