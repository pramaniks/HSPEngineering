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
    [Table("PROGRAM_CURRENCY")]
    public class ProgramCurrency : ColumnsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; } = 0;

        [Required]
        [Column("NAME")]
        public string Name { get; set; } = string.Empty;

        [Column("DESCRIPTION")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("REDEMPTION_RATE")]
        public decimal RedemptionRate { get; set; } = 0M;

        [Column("PURCHASE_RATE")]
        public decimal PurchaseRate { get; set; } = 0M;

        [Column("TRANSFER_FEE")]
        public decimal TransferFee { get; set; } = 0M;

        [Column("IS_DEFAULT")]
        public bool IsDefault { get; set; } = false;

        [Column("BASE_CURRENCY")]
        public string BaseCurrency { get; set; } = string.Empty;

        [Required]
        [Column("PROGRAM_ID")]
        public int ProgramId { get; set; } = 0;

        [Required]
        [Column("EXPIRY_PERIOD")]
        public string ExpiryPeriod { get; set; } = string.Empty;

        [Column("EXPIRY_SCHEDULE")]
        public int ExpirySchedule { get; set; } = 0;

        [NotMapped]
        public List<ProgramCurrencyCapping> CurrencyCapping { get; set; } = new List<ProgramCurrencyCapping>();

        [NotMapped]
        [JsonIgnore]
        public Program Program { get; set; } = null;

    }

}
