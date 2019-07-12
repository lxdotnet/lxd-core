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

        public Person(string name, string lastname, DateTime bd)
        {
            this.Name = name;
            this.Birthday = bd;
            this.Lastname = lastname;

            this.Relatives = new List<Person>();
            this.Money = new Money(100.0M, "EUR");
        }

        public Person(string name, DateTime bd) : this(name, null, bd)
        {
        }

        public int? AuctionId { get; set; }

        [Required]
        public List<Person> Relatives { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        public Salutation? Salutation { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        [Required]
        public string NullArg { get; set; }

        //[JsonConverter(typeof(TicksConverter))]
        public DateTime Birthday { get; private set; }

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
