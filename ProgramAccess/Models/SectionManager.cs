using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramAccess.Models
{
    internal delegate Task DbAction();
    internal class SectionManager
    {
        public async static Task Execute(DbAction dbAction, ValidationResults ValidationResults)
        {
            try
            {
                await dbAction();
            }
            catch (Exception ex)
            {
                ValidationResults.AddResult(new ValidationResult
                {
                    ValidationMessage = ex.Message,
                    ExceptionInformation = ex?.StackTrace
                });
            }
        }
    }
}
