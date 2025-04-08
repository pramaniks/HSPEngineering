using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ProgramAccess.Models
{
    [Table("PROGRAM_MASTER")]
    public class Program : ColumnsBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("PROGRAM_ID")]
        public int ProgramId { get; set; }

        [Required]
        [Column("PROGRAM_NAME")]
        public string ProgramName { get; set; } = string.Empty;

        [Required]
        [Column("PROGRAM_DISPLAY_NAME")]
        public string ProgramDisplayName { get; set; } = string.Empty;

        [Required]
        [Column("PROGRAM_CODE")]
        public string ProgramCode { get; set; } = string.Empty;

        [Column("COUNTRY")]
        public string? Country { get; set; } = string.Empty;

        [Column("CATEGORY")]
        public string? Category { get; set; } = string.Empty;

        [Column("IMAGE_URL")]
        public string? LogoUrl { get; set; } = string.Empty;

        [NotMapped]
        public ProgramSecurityParameters SecurityParameter { get; set; } = new ProgramSecurityParameters();

        [NotMapped]
        public List<ProgramCurrency> Currencies { get; set; } = new List<ProgramCurrency>();

        [NotMapped]
        public OTPConfigurations OtpConfigurations { get; set; } = new OTPConfigurations();

        public static implicit operator Program(EntityEntry<Program> v)
        {
            throw new NotImplementedException();
        }
    }

}
