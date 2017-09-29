using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RinnaiPortal.ViewModel
{
    public class ListParms
    {
        public GridView GridView{ get; set; }
        public Label TotalRowsCount { get; set; }
        public HtmlGenericControl PaginationBar { get; set; }
        public HtmlGenericControl PaginationBar_Daily { get; set; }
        public string QueryText { get; set; }
        public Label NoDataTip { get; set; }

        public ListParms()
        {            
            QueryText = String.Empty;
            GridView = new GridView();
            TotalRowsCount = new Label();
            PaginationBar = new HtmlGenericControl();
            PaginationBar_Daily = new HtmlGenericControl();
            NoDataTip = new Label();
            NoDataTip.Visible = false;
        }


    }
}