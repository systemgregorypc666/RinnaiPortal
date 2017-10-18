using System;
using System.Collections.Generic;
namespace RinnaiPortal.Models.IsoModels
{
    public class IsoManagePublishModel
    {
        /// <summary>
        /// 檔案ID
        /// </summary>
        public int PublishFileID { get; set; }

        /// <summary>
        /// 文件號碼
        /// </summary>
        public string PublishDocNum { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string PublishProductName { get; set; }

        /// <summary>
        /// 擔當部門ID
        /// </summary>
        public int PublishTakeDepID { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime PublishEffDate { get; set; }

        /// <summary>
        /// 版次
        /// </summary>
        public string PublishVersion { get; set; }

        /// <summary>
        /// 頁次
        /// </summary>
        public string PublishPage { get; set; }
        /// <summary>
        /// 發行單位群組ID
        /// </summary>
        public List<PublishDep> PublishDepGroup { get; set; }
    }

    public class PublishDep
    {
        /// <summary>
        /// 志元單位代碼CODEDTL.CODECD
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 志元單位類別CODEDTL.TYPECD
        /// </summary>
        public string Type { get; set; }
    }
}