using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramAccess.Models
{
    public class ColumnsBase
    {
        [Required]
        [Column("CREATED_DATE")]
        public DateTime CreatedOn { get; set; } = DateTime.MinValue.ToUniversalTime();

        [Column("UPDATED_DATE")]
        public DateTime UpdatedOn { get; set; } = DateTime.MinValue.ToUniversalTime();

        [Required]
        [Column("CREATED_BY")]
        public string CreatedBy { get; set; }

        [Column("UPDATED_BY")]
        public string? UpdatedBy { get; set; } = string.Empty;

        [Required]
        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Column("IS_DELETED")]
        public bool IsDeleted { get; set; } = false;
    }

}
