using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Models.SmsModels
{
    public class SmsResultModel
    {
        private string m_msgid = string.Empty;
        private string m_dstaddr = string.Empty;
        private string m_dlvtime = string.Empty;
        private string m_donetime = string.Empty;
        private string m_statusstr = string.Empty;
        private string m_statuscode = string.Empty;
        private string m_statusFlag = string.Empty;


        public string msgid { get { return m_msgid; } set { m_msgid = value; } }
        public string dstaddr { get { return m_dstaddr; } set { m_dstaddr = value; } }
        public string dlvtime { get { return m_dlvtime; } set { m_dlvtime = value; } }
        public string donetime { get { return m_donetime; } set { m_donetime = value; } }
        public string statusstr { get { return m_statusstr; } set { m_statusstr = value; } }
        public string statuscode { get { return m_statuscode; } set { m_statuscode = value; } }
        public string StatusFlag { get { return m_statusFlag; } set { m_statusFlag = value; } }
    }
}