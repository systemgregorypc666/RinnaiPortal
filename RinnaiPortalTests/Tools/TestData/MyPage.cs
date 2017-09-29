using RinnaiPortal.Extensions;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortalTests.Tools.TestData
{
    public class MyPage : Page
    {   
        private DateTime _dateTimeNow = DateTime.Now;

        public TextBox DepartmentID { get; set; }
        public HiddenField TimeStamp  {get; set;}
        public DropDownList FilingEmployeeID_FK { get; set; }
        public CheckBox Disabled { get; set;}
        public Label DisabledDate { get; set; }
        public RadioButtonList UpperDepartmentID { get; set; }

        public MyPage() 
        {
            init();
        }

        private void init()
        {
            DepartmentID = new TextBox();
            DepartmentID.ID = "DepartmentID";
            DepartmentID.Text = "9999";

            TimeStamp = new HiddenField();
            TimeStamp.ID = "TimeStamp";
            TimeStamp.Value = _dateTimeNow.FormatDatetime();

            FilingEmployeeID_FK = new DropDownList();
            FilingEmployeeID_FK.ID = "FilingEmployeeID_FK";
            ViewUtils.SetOptions(FilingEmployeeID_FK, new Dictionary<string, string>() { { "01497", "randy" } });
            FilingEmployeeID_FK.SelectedIndex = 0;

            Disabled = new CheckBox();
            Disabled.ID = "Disabled";
            Disabled.Text = "false";

            DisabledDate = new Label();
            DisabledDate.ID = "DisabledDate";
            DisabledDate.Text = _dateTimeNow.FormatDatetime();

            UpperDepartmentID = new RadioButtonList();
            UpperDepartmentID.ID = "UpperDepartmentID";
            UpperDepartmentID.Items.Add(new ListItem("8888", "8888"));
            UpperDepartmentID.SelectedValue = "8888";

            this.Controls.Add(DepartmentID);
            this.Controls.Add(TimeStamp);
            this.Controls.Add(FilingEmployeeID_FK);
            this.Controls.Add(Disabled);
            this.Controls.Add(DisabledDate);
            this.Controls.Add(UpperDepartmentID);
        }

        public void Add(Control control, string id)
        {
            control.ID = id;
            if (!this.Controls.Contains(control)) { this.Controls.Add(control); }
        }
    }
}
