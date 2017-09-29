using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RinnaiPortal.ViewModel
{
    public class MemberViewModel
    {
        private MemberRepository _memberRepo = RepositoryFactory.CreateMemberRepo();
        private RootRepository _rootRepo = RepositoryFactory.CreateRootRepo();
        public Dictionary<string, object> ProfilePair { get; set; }

        public string ADAccount { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DeptID { get; set; }
        public string ChiefID { get; set; }
        public bool IsChief { get; set; }
        //todo Agents 
        public string AgentsID { get; set; }
        //新增國別 20170817 判斷是否於資訊人員Debug中
        public string NationalType { get; set; }

        public List<String> ResourceList { get; set; }

        public MemberViewModel() { }

        public MemberViewModel(string adAccount)
        {
            ADAccount = adAccount;
            ResourceList = _memberRepo.QueryForResourceList(adAccount);

            var empData = _rootRepo.QueryForEmployeeByADAccount(adAccount);
            if (empData != null)
            {
                EmployeeID = empData["EmployeeID"].ToString();
                EmployeeName = empData["EmployeeName"].ToString();
                DeptID = empData["DepartmentID_FK"].ToString();
                var deptData = _rootRepo.QueryForDepartmentByDeptID(DeptID);
                ChiefID = deptData != null ? deptData["ChiefID_FK"].ToString() : null;
                IsChief = (EmployeeID == ChiefID) ? true : false;
                NationalType = empData["NationalType"].ToString();
            }

            ProfilePair = new Dictionary<string, object>();
            ProfilePair.Add("Resource", ResourceList);
            ProfilePair.Add("ADAccount", ADAccount);
            ProfilePair.Add("EmployeeID", EmployeeID);
            ProfilePair.Add("EmployeeName", EmployeeName);
            ProfilePair.Add("DeptID", DeptID);
            ProfilePair.Add("ChiefID", ChiefID);
            ProfilePair.Add("ISChief", IsChief.ToString());
            ProfilePair.Add("NationalType", NationalType);
        }

        public static bool Security(string voucher)
        {
            if ("True".Equals(ConfigUtils.ParsePageSetting("SecurityEssentials")["UserPolicy"], StringComparison.OrdinalIgnoreCase))
            {
                var count = Regex.Matches(voucher, @"[richus]", RegexOptions.IgnoreCase).Cast<Match>().Count();
                return !(count == (voucher.Length - 1));
            }
            return true;
        }
    }
}