using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Models.IsoModels
{
    public class IsoManageListViewModel
    {
        private List<IsoManageDataModel> m_data = new List<IsoManageDataModel>();
        public List<IsoManageDataModel> Data { get { return m_data; } set { m_data = value; } }
    }
}