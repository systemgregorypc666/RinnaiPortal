using DBTools;
using RinnaiPortal.Area.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Interface
{
    public interface IManageRepository
    {
        Pagination GetPagination(ManageListParms mlParms, PaggerParms pParms);
    }
}