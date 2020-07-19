using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyProject.Domain.Core
{
    public abstract class DomainObject
    {
        /// <summary>
        /// Uses reflection to return the first field with a KeyAttribute
        /// </summary>
        /// <returns>The DomainObject PK (hopefully)</returns>
        public Guid GetId()
        {
            var prop = this.GetType().GetProperties().First(
                p => !p.CustomAttributes.Any(
                    a => a.AttributeType == typeof(KeyAttribute)));
            return (Guid)prop.GetValue(this);
        }

        [Display(Name = "CreatedBy")]
        public string CreatedBy { get; set; }

        [Display(Name = "CreatedDate"), DisplayFormat(DataFormatString = "{0:g}")]
        public DateTimeOffset CreatedDate { get; set; }

        [Display(Name = "LastModifiedBy")]
        public string LastModifiedBy { get; set; }

        [Display(Name = "LastModifiedDate"), DisplayFormat(DataFormatString = "{0:g}")]
        public DateTimeOffset LastModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual void Deactivate()
        {
            this.IsActive = false;
        }

        internal void SetAuditDetails(string username)
        {
            this.LastModifiedBy = username;
            this.LastModifiedDate = DateTimeOffset.Now;

            if (!string.IsNullOrEmpty(this.CreatedBy))
            {
                return;
            }
            this.CreatedBy = username;
            this.CreatedDate = DateTimeOffset.Now;
        }
    }
}