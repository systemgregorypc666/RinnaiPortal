using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RinnaiPortal.Interface
{
    public class PaggerParms
    {
        public int PageIndex { get; set; }
        public int PageIndex1 { get; set; }
        public int PageSize { get; set; }
        public int PageSize1 { get; set; }
        public string OrderField { get; set; }
        public string OrderField1 { get; set; }
        public bool Descending { get; set; }
        public bool Descending1 { get; set; }

        public PaggerParms()
        {
            PageIndex = 1;
            PageIndex1 = 1;
            PageSize = 10;
            PageSize1 = 10;
            OrderField = "CreateDate";
            OrderField1 = "CreateDate";
            Descending = false;
            Descending1 = false;
        }
    }
}
