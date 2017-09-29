using DBTools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinnaiPortal.Interface
{
    public interface IForms
    {
        void CreateData(List<RinnaiForms> formViewModel);
        void EditData(List<RinnaiForms> formViewModel);
        MultiConditions DeleteData(string signDocID);
    }
}
