using RinnaiPortal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBTools;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.Extensions;


namespace RinnaiPortal.Repository.Sign
{
	public class SalaryLimitRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public SalaryLimitRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		public SalaryLimitViewModel GetLimitData(string sn)
		{
			SalaryLimitViewModel model = null;
			string strSQL = @"Select * from SalaryLimit where SN = @SN";
			var strCondition = new Conditions() { { "@SN", sn } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new SalaryLimitViewModel()
			{
				SN = Int32.Parse(result["SN"].ToString()),
				LimitDate = result["LimitDate"].ToString().ToDateTime(),
				Creator = result["Creator"].ToString(),
				CreateDate = DateTime.Parse(result["CreateDate"].ToString()),
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
			};

			return model;
		}

		public void SaveData(SalaryLimitViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			if (model.SN <= 0)
			{
				//create data
				var mainpulationConditions = new Conditions()
				{
					{"@LimitDate", model.LimitDate},
					{"@Creator", model.Creator},
					{"@CreateDate", model.CreateDate},
					{"@Modifier", model.Creator }
				};
				try
				{

					_dc.ExecuteAndCheck(
@"Insert into SalaryLimit ( LimitDate, Creator, CreateDate, Modifier)
					Values (@LimitDate, @Creator, @CreateDate, @Modifier)", mainpulationConditions);

				}
				catch (Exception ex)
				{
					throw ex;
				}


			}
			else
			{
				//edit data
				var orgModel = GetLimitData(model.SN.ToString());
				//model 不得為 null
				if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

				if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetime()))
				{
					throw new Exception("此筆資料已被他人更新，請重新整理後，再次檢查已更動的資料!");
				}

				var mainpulationConditions = new Conditions()
				{
					{"@SN", model.SN },
					{"@LimitDate", model.LimitDate},
					{"@Modifier", model.Modifier},
				};

				try
				{
					_dc.ExecuteAndCheck(
@"Update SalaryLimit Set 
		LimitDate = @LimitDate, 
		Modifier = @Modifier,
		ModifyDate = getDate()
Where SN = @SN", mainpulationConditions);
				}
				catch (Exception ex)
				{
					throw ex;
				}

			}
		}
	}

}
