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
    [Table("OTP_CONFIGURATIONS")]
    public class OTPConfigurations : ColumnsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } = 0;

        [Required]
        [Column("PROGRAM_ID")]
        public int ProgramId { get; set; } = 0;

        [Column("LENGTH")]
        public int Length { get; set; }

        [Column("IS_ALPHANUMERIC")]
        public bool IsAlphanumeric { get; set; }

        [Column("EXPIRATION_TIME_IN_SECONDS")]
        public int ExpirationTimeInSeconds { get; set; }

        [Column("ALLOWED_FAILED_ATTEMPTS")]
        public bool AllowedFailedAttempts { get; set; }

        [Column("NEW_OTP_ON_EVERY_REQUEST")]
        public bool NewOTPOnEveryRequest { get; set; }

        [NotMapped]
        [JsonIgnore]
        public Program Program { get; set; } = null;

    }

}
