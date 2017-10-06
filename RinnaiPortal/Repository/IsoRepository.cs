using RinnaiPortal.Entities;
using RinnaiPortal.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Repository
{
    public class IsoRepository
    {
        private DBISO DB = new DBISO();

        /// <summary>
        /// 申請ISO號碼
        /// </summary>
        /// <returns></returns>
        public int APPIsoNumber()
        {
            IsoMain iso = new IsoMain()
            {
                APP_ST = IsoStatus.W.ToString(),
                APP_UP_DT = DateTime.UtcNow.AddHours(8),
                APP_DT = DateTime.UtcNow.AddHours(8),
                APP_USR = HttpContext.Current.User.Identity.Name,
            };
            try
            {
                this.DB.IsoMain.Add(iso);
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return iso.ID;
        }

        /// <summary>
        /// 取得單筆ISO申請明細
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IsoMain GetIsoAppDocumentByID(int id)
        {
            List<IsoMain> isoList = this.GetIsoAppListByUserID(HttpContext.Current.User.Identity.Name);
            IsoMain result = isoList.Where(o => o.ID == id).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// ISO申請列表
        /// </summary>
        /// <returns></returns>
        public List<IsoMain> GetIsoAppListByUserID(string userID)
        {
            string currentUser = HttpContext.Current.User.Identity.Name;
            List<IsoMain> isoList = this.DB.IsoMain.Where(o => o.APP_USR == currentUser)
                .Where(s => s.APP_USR == userID)
                .ToList();
            return isoList;
        }

        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="isoNum"></param>
        /// <param name="files"></param>
        public void SaveFile(int docID, string isoNum, FileUpload files)
        {
            string savePath = HttpContext.Current.Server.MapPath("~/FileUploads/Document/" + isoNum + "/");
            CheckFolder(savePath);
            foreach (var file in files.PostedFiles)
            {
                string fileName = file.FileName;
                string savePathFile = savePath + fileName;
                file.SaveAs(savePathFile);
                string urlPath = @"/FileUploads/Document/" + isoNum + "/" + file.FileName;
                SaveDB(docID, isoNum, savePathFile, urlPath, file.FileName, file.ContentLength);
            }
        }

        /// <summary>
        /// 儲存至DB
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="isoNum"></param>
        /// <param name="savePath"></param>
        /// <param name="file"></param>
        public void SaveDB(int docID, string isoNum, string savePath, string urlPath, string fileName, double fileSize)
        {
            double b = (double)fileSize;
            double kb = b / 1024;
            double mb = kb / 1024;
            double gb = mb / 1024;

            IsoFiles f = new IsoFiles()
            {
                MAP_MAIN_ID = docID,
                BUD_DT = DateTime.UtcNow.AddHours(8),
                FILE_NM = fileName,
                FILE_SZ = mb,
                FILE_PATH = savePath,
                URL_PATH = urlPath,
                REF_ISO_NUM = isoNum
            };
            try
            {
                this.DB.IsoFiles.Add(f);
                this.DB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 取得檔案列表
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public List<IsoFiles> GetFileByDocID(int docID)
        {
            var files = this.DB.IsoFiles.Where(o => o.MAP_MAIN_ID == docID).ToList();
            return files;
        }

        /// <summary>
        /// 判斷是否有目錄
        /// </summary>
        /// <param name="fd"></param>
        private void CheckFolder(string fd)
        {
            if (!Directory.Exists(fd))
                Directory.CreateDirectory(fd);
        }
    }
}