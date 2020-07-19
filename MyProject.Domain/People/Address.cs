using System;
using System.ComponentModel.DataAnnotations;
using MyProject.Domain.Core;

namespace MyProject.Domain.People
{
	public class Address : DomainObject
	{
		public Guid AddressId { get; set; }

		[Display(Name = "Address Line 1"), Required, MinLength(3)]
		public string Line1 { get; set; }

		[Display(Name = "Address Line 2"), MinLength(3)]
		public string Line2 { get; set; }

		[Display(Name = "City"), Required, MinLength(3)]
		public string City { get; set; }

		[Display(Name = "State"), Required, MinLength(2), MaxLength(2)]
		public string State { get; set; }

		[Display(Name = "Zip"), Required, Range(11111, 99999)]
		public int Zip { get; set; }
	}
}