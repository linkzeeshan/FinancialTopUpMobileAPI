using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common
{
    public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>
    {
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

        public string? CreatedBy { get; set; }

        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

        public string? LastModifiedBy { get; set; }
    }
}
