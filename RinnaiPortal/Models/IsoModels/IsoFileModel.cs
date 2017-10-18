using System;
namespace RinnaiPortal.Models.IsoModels
{
    public class IsoFileModel
    {
        /// <summary>
        /// 索引值
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 檔案Server磁碟路徑
        /// </summary>
        public string FileServerPath { get; set; }

        /// <summary>
        /// 檔案Server網頁路徑路徑(下載)
        /// </summary>
        public string FileUrlPath { get; set; }
        /// <summary>
        /// 檔案尺寸大小
        /// </summary>
        public double FileSize { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime BuildDate { get; set; }
    }
}