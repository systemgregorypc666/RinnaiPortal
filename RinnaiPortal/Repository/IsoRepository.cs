using RinnaiPortal.Entities;
using RinnaiPortal.Enums;
using RinnaiPortal.Models.IsoModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        public void SaveFile(int docID, string isoNum, FileUpload files, string saveMode)
        {
            string savePath = HttpContext.Current.Server.MapPath("~/FileUploads/Document/" + isoNum + "/");
            CheckFolder(savePath);
            foreach (var file in files.PostedFiles)
            {
                string fileName = file.FileName;
                string savePathFile = savePath + fileName;
                file.SaveAs(savePathFile);
                string urlPath = @"/FileUploads/Document/" + isoNum + "/" + file.FileName;
                SaveDB(docID, isoNum, savePathFile, urlPath, file.FileName, file.ContentLength, saveMode);
            }
        }

        /// <summary>
        /// 儲存至DB
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="isoNum"></param>
        /// <param name="savePath"></param>
        /// <param name="file"></param>
        public void SaveDB(int docID, string isoNum, string savePath, string urlPath, string fileName, double fileSize, string saveMode)
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
                REF_ISO_NUM = isoNum,
                UP_MODE = saveMode
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
        public List<IsoFiles> GetFileByDocID(int docID, string mode)
        {
            var files = this.DB.IsoFiles.Where(o => o.MAP_MAIN_ID == docID && o.UP_MODE == mode).ToList();
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


        /// <summary>
        /// 取得所有ISO表單列表
        /// </summary>
        /// <returns></returns>
        public List<IsoMain> GetAllIsoAppList()
        {
            var resultList = this.DB.IsoMain.ToList();
            return resultList;
        }


        /// <summary>
        /// 取得所有ISO表單列表
        /// </summary>
        /// <returns></returns>
        public List<IsoManageDataModel> GetAllIsoAppListForManage()
        {
            List<IsoManageDataModel> data = new List<IsoManageDataModel>();
            var resultList = this.DB.IsoMain.ToList();
            foreach (var r in resultList)
            {
                IsoManageDataModel d = new IsoManageDataModel()
                {
                    ID = r.ID,
                    Applicant = r.APP_USR,
                    ApplicationDate = r.APP_DT,
                    ApplicationStatus = r.APP_ST,
                    IsoNumber = r.ISO_NUM,
                    IsoDocmentLevel = r.DOC_LEVEL,
                    AppUser = r.APP_USR,
                    Remark = r.REMARK
                };
                var userFileProto = GetFileByDocID(r.ID, "User");
                foreach (var f in userFileProto)
                {
                    IsoFiles file = new IsoFiles()
                    {
                        ID = f.ID,
                        FILE_NM = f.FILE_NM,
                        FILE_PATH = f.FILE_PATH,
                        URL_PATH = f.URL_PATH,
                        FILE_SZ = f.FILE_SZ,
                        BUD_DT = f.BUD_DT
                    };
                    d.UserFiles.Add(file);
                }

                var manageFileProto = GetFileByDocID(r.ID, "Manage");
                foreach (var f in manageFileProto)
                {
                    IsoFiles file = new IsoFiles()
                    {
                        ID = f.ID,
                        FILE_NM = f.FILE_NM,
                        FILE_PATH = f.FILE_PATH,
                        URL_PATH = f.URL_PATH,
                        FILE_SZ = f.FILE_SZ,
                        BUD_DT = f.BUD_DT
                    };
                    d.ManageFiles.Add(file);
                }

                data.Add(d);
            }

            foreach (var d in data)
                d.EmployeeData = GetEmployeeDataWithIso(d.AppUser);
            return data;
        }


        /// <summary>
        /// 取得該ISO文件的申請人
        /// </summary>
        /// <param name="userAdAccount"></param>
        /// <returns></returns>
        public IsoEmployeeModel GetEmployeeDataWithIso(string userAdAccount)
        {
            var empData = this.DB.Employee.Where(o => o.ADAccount == userAdAccount).FirstOrDefault();
            var depData = this.DB.Department.Where(o => o.DepartmentID == empData.DepartmentID_FK).FirstOrDefault();

            IsoEmployeeModel emp = new IsoEmployeeModel()
            {
                ID = empData.EmployeeID,
                DepartmentName = depData.DepartmentName,
                Name = empData.EmployeeName
            };
            return emp;
        }

        /// <summary>
        /// 取得單筆ISO申請明細(ISO管理)
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public IsoManageDataModel GetIsoAppDocumentByIDForManage(int docID)
        {
            var data = GetAllIsoAppListForManage().Where(o => o.ID == docID).FirstOrDefault();
            return data;
        }

        /// <summary>
        /// 儲存一筆ISO號碼
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public IsoManageDataModel SetIsoData(int docID, string isoNum, string status, int docLevel, string remark)
        {
            var data = this.DB.IsoMain.Where(o => o.ID == docID).FirstOrDefault();
            data.ISO_NUM = isoNum;
            data.DOC_LEVEL = docLevel;
            data.REMARK = remark;
            this.DB.Entry(data).State = EntityState.Modified;
            this.DB.SaveChanges();
            UpdateIsoStatusByDocID(docID, status);
            return GetIsoAppDocumentByIDForManage(docID);
        }

        /// <summary>
        /// 發行設定
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="publish"></param>
        public void SaveIsoFIleSettingData(int docID, IsoManagePublishModel publish)
        {
            if (publish.PublishFileID > 0)
            {
                IsoFileSetting setting = new IsoFileSetting();
                setting.MAP_FILE_ID = publish.PublishFileID;
                setting.NM_OF_PRODT = publish.PublishProductName;
                setting.REF_DOC_ID = docID;
                setting.REF_ISO_NUM = publish.PublishDocNum;
                setting.VERSION = publish.PublishVersion;
                setting.PAGE = publish.PublishPage;
                setting.TAKE_ON_DEP_ID = publish.PublishTakeDepID;
                setting.EFF_DT = publish.PublishEffDate;
                this.DB.IsoFileSetting.Add(setting);

                if (publish.PublishDepGroup.Count > 0)
                {
                    List<IsoPublishGroup> groupList = new List<IsoPublishGroup>();
                    foreach (var g in publish.PublishDepGroup)
                    {
                        IsoPublishGroup group = new IsoPublishGroup();
                        group.ID = g.ID;
                        group.MAP_FILE_ID = publish.PublishFileID;
                        group.GROUP_TYPE_NAME = g.Type;
                        groupList.Add(group);
                    }
                    this.DB.IsoPublishGroup.AddRange(groupList);
                }
                try
                {
                    this.DB.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 更改審核狀態
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="status"></param>
        public void UpdateIsoStatusByDocID(int docID, string status)
        {
            var data = this.DB.IsoMain.Where(o => o.ID == docID).FirstOrDefault();
            data.APP_ST = status;
            this.DB.Entry(data).State = EntityState.Modified;
            this.DB.SaveChanges();
        }

        /// <summary>
        /// 文管列表過濾條件
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<IsoManageDataModel> FilterManageList(string qty, string status, int docLevel)
        {
            List<IsoManageDataModel> data = GetAllIsoAppListForManage();
            if (!string.IsNullOrEmpty(qty))
            {
                data = data.Where(o => string.IsNullOrEmpty(o.IsoNumber) ? true : o.IsoNumber == qty || string.IsNullOrEmpty(o.IsoNumber) ? true : o.IsoNumber.Contains(qty)).ToList();
                if (data.Count == 0)
                    data = data.Where(o => o.AppUser == qty || o.AppUser.Contains(qty)).ToList();
            }

            data = data.Where(o => string.IsNullOrEmpty(status) ? true : o.ApplicationStatus == status).ToList();

            if (docLevel > 0)
                data = data.Where(o => o.IsoDocmentLevel == docLevel).ToList();
            return data;
        }
    }
}