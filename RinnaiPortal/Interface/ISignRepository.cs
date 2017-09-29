using DBTools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Interface
{
    /// <summary>
    /// 分頁界面，繼承此介面必須實做分頁功能
    /// </summary>
    public interface ISignRepository
    {
        Pagination GetPagination(SignListParms slParms, PaggerParms pParms);
    }
}