using Core.Common;
using Microsoft.EntityFrameworkCore;
using ProgramAccess.Model;
using ProgramAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramAccess.Dao
{
    
    public class GetProgramListRequest
    {
        public int Skip { get; set; }
        public int Take { get; set; } = 1000;
        public ICollection<int>? ProgramIdList { get; set; }
        public string? Country { get; set; }
        public string? Category { get; set; }
    }

    public class GetProgramRequest
    {
        public bool IsGetById { get; set; }
        public bool IsGetByName { get; set; }
        public int? Id { get; set; }
        public string? Name { get; set; }
        public bool IsLoadCurrencyConfiguration { get; set; }
        public bool IsLoadSecurityParameter { get; set; }
        public bool IsLoadOtpConfiguration{ get; set; }
    }

    public class PersistProgramRequest 
    {
        public Program Program { get; set; }
    }

    public interface IProgramDao
    {
        Task<Program> GetProgram(GetProgramRequest Request, ValidationResults ValidationResults);
        Task<ICollection<Program>> GetProgramList(GetProgramListRequest Request, ValidationResults ValidationResults);
        Task<Program> PersistProgram(PersistProgramRequest Request, ValidationResults ValidationResults);
    }

    public class ProgramDao : IProgramDao
    {
        private readonly LoyaltyDBContext _LoyaltyDBContext;

        public ProgramDao(LoyaltyDBContext LoyaltyDBContext)
        {
            _LoyaltyDBContext = LoyaltyDBContext;
        }

        public async Task<ICollection<Program>> GetProgramList(GetProgramListRequest Request, ValidationResults ValidationResults)
        {
            ICollection<Program> resultList = null;
            ValidationResults ??= new ValidationResults();

            await SectionManager.Execute(async () =>
            {
                var query = _LoyaltyDBContext.Programs.AsNoTracking().AsQueryable();
                query = !string.IsNullOrEmpty(Request.Country) ? query.Where(x => x.Country == Request.Country) : query;
                query = !string.IsNullOrEmpty(Request.Category) ? query.Where(x => x.Category == Request.Category) : query;
                query = Request.ProgramIdList != null && Request.ProgramIdList.Any() ? query.Where(x => Request.ProgramIdList.Contains(x.ProgramId)) : query;
                query = query.OrderBy(x => x.ProgramId).Skip(Request.Skip).Take(Request.Take);
                resultList = await query.ToListAsync();
            }, ValidationResults);

            return resultList;
        }

        public async Task<Program> GetProgram(GetProgramRequest Request, ValidationResults ValidationResults)
        {
            Program result = null;
            ValidationResults ??= new ValidationResults();
            await SectionManager.Execute(async () =>
            {
                var query = _LoyaltyDBContext.Programs.AsNoTracking().AsQueryable();
                query = Request.IsGetById ? query.Where(x => x.ProgramId == Request.Id) : query;
                query = Request.IsGetByName ? query.Where(x => x.ProgramName == Request.Name) : query;
                query = Request.IsLoadCurrencyConfiguration ? query.Include(x => x.Currencies) : query;
                query = Request.IsLoadSecurityParameter ? query.Include(x => x.SecurityParameter) : query;
                query = Request.IsLoadOtpConfiguration ? query.Include(x => x.OtpConfigurations) : query;
                result = await query.FirstOrDefaultAsync();
            }, ValidationResults);

            return result;
        }

        public async Task<Program> PersistProgram(PersistProgramRequest Request, ValidationResults ValidationResults)
        {
            ValidationResults ??= new ValidationResults();
            Program result = null;

            await SectionManager.Execute(async () =>
            {
                if (Request.Program.ProgramId == 0)
                {
                    var entry = await _LoyaltyDBContext.Programs.AddAsync(Request.Program);
                    result = entry.Entity;
                }
                else
                {
                    var program = await _LoyaltyDBContext.Programs.FindAsync(Request.Program.ProgramId);
                    if (program != null)
                    {
                        program.ProgramName = Request.Program.ProgramName;
                        program.Country = Request.Program.Country;
                        program.Category = Request.Program.Category;
                        program.LogoUrl = Request.Program.LogoUrl;
                        program.ProgramCode = Request.Program.ProgramCode;
                        program.IsActive = Request.Program.IsActive;
                        program.UpdatedBy = Request.Program.UpdatedBy;
                        program.UpdatedOn = Request.Program.UpdatedOn;
                    }
                }
                await _LoyaltyDBContext.SaveChangesAsync();

            }, ValidationResults);

            return result;
        }
    }
}
