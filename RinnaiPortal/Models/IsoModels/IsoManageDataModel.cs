using RinnaiPortal.Entities;
using System;
using System.Collections.Generic;

namespace RinnaiPortal.Models.IsoModels
{
    public class IsoManageDataModel
    {
        public IsoEmployeeModel EmployeeData { get; set; }
        public IsoManagePublishModel PublishSetting { get; set; }
        /// <summary>
        /// 索引值
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 申請認AD帳號
        /// </summary>
        public string AppUser { get; set; }

        /// <summary>
        /// ISO文件號
        /// </summary>
        public string IsoNumber { get; set; }

        /// <summary>
        /// 申請人
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 申請狀態
        /// </summary>
        public string ApplicationStatus { get; set; }

        /// <summary>
        /// 文件階級
        /// </summary>
        public int? IsoDocmentLevel { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 申請日期
        /// </summary>
        public DateTime ApplicationDate { get; set; }

        private List<IsoFiles> m_userFiles = new List<IsoFiles>();
        private List<IsoFiles> m_manageFiles = new List<IsoFiles>();

        /// <summary>
        /// 檔案模型
        /// </summary>
        public List<IsoFiles> UserFiles { get { return m_userFiles; } set { m_userFiles = value; } }

        public List<IsoFiles> ManageFiles { get { return m_manageFiles; } set { m_manageFiles = value; } }
    }
}