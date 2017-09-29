using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace RinnaiPortal.Repository
{
    public class MemberRepository 
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        public MemberRepository(DB dc, RootRepository rootRepo)
        {
            _dc = dc;
            _rootRepo = rootRepo;
        }


        public List<string> QueryForResourceList(string userName)
        {
            var accessData = _rootRepo.QueryForAccessTypeByADAccount(userName);
            if (accessData == null) { return null; }
            
            var resourceResult = new List<string>();
            var accessArray = accessData["AccessType"].ToString().Split(',');
            foreach (var access in accessArray)
            {
                var groupData = _rootRepo.QueryForGroupDataByGroupType(access);
                if (groupData == null) { continue; }
                resourceResult.AddRange(groupData["Resource"].ToString().Split(','));
            }
            return resourceResult;
            
        }
    }
}