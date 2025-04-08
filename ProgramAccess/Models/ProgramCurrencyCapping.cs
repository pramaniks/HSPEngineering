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
    [Table("PROGRAM_CURRENCY_CAPPING")]
    public class ProgramCurrencyCapping : ColumnsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } = 0;

        [Required]
        [Column("PROGRAM_CURRENCY_ID")]
        public long ProgramCurrencyId { get; set; }

        [Column("CATEGORY")]
        public string Category { get; set; } = string.Empty;

        [Column("MEMBER_TIER")]
        public string MemberTier { get; set; } = string.Empty;

        [Column("CAPPING_AMOUNT")]
        public int CapingAmount { get; set; }

        [Column("CAPPING_PERCENTAGE")]
        public int CapingPercentage { get; set; }

        [Column("THRESHOLD")]
        public float Threshold { get; set; }

        [Column("EFFECTIVE_DATE")]
        public DateTime EffectiveDate { get; set; } = DateTime.MinValue.ToUniversalTime();

        [Column("END_DATE")]
        public DateTime EndDate { get; set; } = DateTime.MaxValue.ToUniversalTime();

        [NotMapped]
        [JsonIgnore]
        public ProgramCurrency Currency { get; set; } = new ProgramCurrency();

    }

}
