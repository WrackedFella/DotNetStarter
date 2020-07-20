using MyProject.Domain.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Domain.People
{
    public class Person : DomainObject
    {
        public Guid PersonId { get; set; }

        [Display(Name = "First Name"), Required, MinLength(3)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name"), Required, MinLength(3)]
        public string LastName { get; set; }

        [Display(Name = "Middle Name"), Required, MinLength(3)]
        public string MiddleName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{LastName}, {FirstName} {MiddleName?.Substring(0, 1)}";
    }
}