using Microsoft.EntityFrameworkCore;
using System;

namespace MyProject.Domain.System
{
    public class AuditHistory : AutoHistory
    {
        public string LastModifiedBy { get; set; }
        public new DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
    }
}