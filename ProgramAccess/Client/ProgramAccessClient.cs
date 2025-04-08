using ProgramAccess.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramAccess.Client
{
    public class ProgramAccessClient
    {
        #region Declarations

        private IProgramDao _ProgramDao;

        #endregion Declarations
        public ProgramAccessClient(IProgramDao programDao)
        {
            _ProgramDao = programDao;
        }

    }
}
