using System.Web.Script.Serialization;
using System.Web.Services;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using RinnaiPortal.Repository;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Tools;
using System;
using System.IO;
using System.Collections.Generic;
using RinnaiPortal.ViewModel.Sign.Forms;
using RinnaiPortal.ViewModel;

namespace RinnaiPortal.Service.Sign.Forms
{
	/// <summary>
	/// Summary description for Train
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class Train : System.Web.Services.WebService
	{
		private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
		private TrainRepository _trainRepo { get; set; }
		private RootRepository _rootRepo { get; set; }

		public Train()
		{
			_trainRepo = RepositoryFactory.CreateTrainRepo();
			_rootRepo = RepositoryFactory.CreateRootRepo();
			GlobalDiagnosticsContext.Set("User", User.Identity.Name);
		}

		[WebMethod]
		public void ApplyForm()
		{
			//處理 request jsonData
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			var jsonData = String.Empty;
			Context.Request.InputStream.Position = 0;
			using (var inputStream = new StreamReader(Context.Request.InputStream))
			{
				jsonData = inputStream.ReadToEnd();
			}

			var data = serializer.Deserialize<Dictionary<string, object>>(jsonData);
			var model = viewModelMapping(data);

			var responseObj = new Dictionary<string, string>();
			try
			{
				_trainRepo.SubmitData(model);
				responseObj.Add("message", String.Format("簽核已送出!你簽核編號為: {0} ,", model.SignDocID_FK));
				responseObj.Add("SignDocID", model.SignDocID_FK);
			}
			catch (Exception ex)
			{
				responseObj.Add("message", String.Format("簽核送出失敗! 錯誤訊息: {0}", ex.Message));
				//錯誤的請求
				this.Context.Response.StatusCode = 400;
			}
			finally
			{
				//設定輸出格式為json格式
				this.Context.Response.ContentType = "application/json";
				//輸出json格式
				this.Context.Response.Write(serializer.Serialize(responseObj));
			}

		}

		private TrainViewModel viewModelMapping(Dictionary<string, object> data)
		{
			var model = WebUtils.ViewModelMapping<TrainViewModel>(data);

			//根據表單系列決定 SignType Data
			var signTypeData = _rootRepo.QueryForSignTypeDataBySeries(model.FormSeries);
			model.FormID_FK = Int32.Parse(signTypeData["FormID"].ToString());
			model.RuleID_FK = signTypeData["SignID_FK"].ToString();

			return model;
		}

	}
}
