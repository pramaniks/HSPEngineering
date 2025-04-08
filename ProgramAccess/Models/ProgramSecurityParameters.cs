using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgramAccess.Models
{
    [Table("PROGRAM_SECURITY_PARAMETERS")]
    public class ProgramSecurityParameters : ColumnsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } = 0;

        [Column("PASSWORD_POLICY")]
        public string PasswordPolicy { get; set; } = string.Empty;

        [Column("FAILED_ATTEMPTS_TO_LOCK")]
        public int FailedAttemptsToLock { get; set; }

        [Column("PASSWORD_EXPIRY")]
        public int PasswordExpiry { get; set; }

        [Required]
        [Column("PROGRAM_ID")]
        public int ProgramId { get; set; } = 0;

        [NotMapped]
        [JsonIgnore]
        public Program Program { get; set; } = null;
    }

}
