using System;
using Starcounter;
using System.Collections.Generic;

namespace ScDumpConcept
{
    [Database]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return this.FirstName + " " + this.LastName; } }
        public int Age { get; set; }
        public Address Address { get; set; }
        public Department Workplace { get; set; }
    }

    [Database]
    public class Address
    {
        public string Street { get; set; }
        public int ZIP { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    [Database]
    public class Company
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public IEnumerable<Department> Departments
        {
            get
            {
                return Db.SQL<Department>("SELECT d FROM ScDumpConcept.Department d WHERE d.Company = ?", this);
            }
        }
    }

    [Database]
    public class Department
    {
        public string Name { get; set; }
        public Company Company { get; set; }
        public IEnumerable<Person> Employees
        {
            get
            {
                return Db.SQL<Person>("SELECT p FROM ScDumpConcept.Person p WHERE p.Department = ?", this);
            }
        }
    }
}
