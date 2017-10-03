using RinnaiPortal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortalOpenApi.Modules
{
    public class EinvoiceModule
    {
        DbINV m_db = new DbINV();
        DbINV DB { get { return this.m_db; } set { this.m_db = value; } }

        public C0401H GetEinvoiceDetalisByNo(string invNo)
        {
            C0401H einvoice = this.DB.C0401H.Where(o => o.MInvoiceNumber == invNo).FirstOrDefault();
            return einvoice;
        }
    }
}