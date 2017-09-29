using DBTools;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Interface
{
    public interface IRepository
    {
        Pagination GetPagination(ListParms parms);
    }
}