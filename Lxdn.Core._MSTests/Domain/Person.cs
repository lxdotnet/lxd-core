using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lxdn.Core._MSTests.Domain
{
    [DebuggerDisplay("{Name,nq} {Lastname,nq}, {Age}")]
    [CustomValidation(typeof(PersonValidator), "Validate")]
    public class Person : IComparable<Person>
    {
        public Person() {}

        public static Person BuildFamily()
        {
            var person = new Person("Alexander", "Dolnik", new DateTime(1976, 9, 17));
            person.Relatives.Add(new Person("Katya", new DateTime(1978, 6, 26)));
            person.Relatives.Add(new Person("Sofia", new DateTime(2013, 10, 26)));

            return person;
        }

        public Person(string name, string lastname, DateTime bd)
        {
            this.Name = name;
            this.Birthday = bd;
            this.Lastname = lastname;

            this.Relatives = new List<Person>();
            this.Money = new Money { Amount = 100, Currency = "EUR" };
        }

        public Person(string name, DateTime bd) : this(name, null, bd)
        {
        }

        public int? AuctionId { get; set; }

        [Required]
        public List<Person> Relatives { get; set; }

        public Person[] RelativesArray => Relatives?.ToArray();

        public MaritalStatus MaritalStatus { get; set; }

        public Salutation? Salutation { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        [Required]
        public string NullArg { get; set; }

        //[JsonConverter(typeof(TicksConverter))]
        public DateTime Birthday { get; set; }

        public DateTime Today
        {
            get { return DateTime.Now; }
        }

        public int Age
        {
            get
            {
                TimeSpan span = DateTime.Now - this.Birthday;
                double years = span.TotalDays / 365.25D;
                return (int)Math.Floor(years);
            }
        }

        //public Person Self
        //{
        //    get { return this; }
        //}

        public bool? Religious { get; private set; }

        public string Meet(Person person)
        {
            return this.Name + " + " + person.Name;
        }

        public bool IsFullAge
        {
            get { return this.Age >= 18; }
        }

        public Money Money { get; set; }

        public double FiveMinutes
        {
            get { return TimeSpan.FromMinutes(5).TotalMilliseconds; }
        }

        public int CompareTo(Person other)
        {
            if (other == null)
                return 1;
            return String.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }
    }

    //public class Persons : List<Person>
    //{
    //}
}
