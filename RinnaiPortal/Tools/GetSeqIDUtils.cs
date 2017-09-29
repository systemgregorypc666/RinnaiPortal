using DBTools;
using System;

namespace RinnaiPortal.Tools
{
    public static class GetSeqIDUtils
    {
        private static DB _dc { get; set; }

        static GetSeqIDUtils()
        {
            _dc = new DB(System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString);
        }

        /// <summary>
        /// 建立並取得最新的簽核ID
        /// </summary>
        /// <param name="adAccount"></param>
        /// <param name="prefixCode">簽核的類型 ex：FP 忘刷單(可改為列舉較理想)</param>
        /// <returns></returns>
        public static string GetSignDocID(string adAccount, string prefixCode)
        {
            var orgTicket = getTicket();
            //取郵戳做大值 用意? by 俊晨
            var data = _dc.QueryForDataRow(@"Select Max(SN) as SN From UserPriority", null);
            var maxTicket = data != null ? data["SN"].ToString() : "-1";
            //比對insert和取出是否相同，為啥不用transaction??by 俊晨
            if (!maxTicket.Equals(orgTicket))
                return string.Empty;
            //建立一筆新簽核序號
            string newSignID = generateAndCreateID(adAccount, prefixCode);
            return newSignID;
        }

        /// <summary>
        /// 產生簽核序號
        /// </summary>
        /// <param name="adAccount">學員的ADAccount</param>
        /// <param name="prefixCode">簽核種類</param>
        /// <returns></returns>
        private static string generateAndCreateID(string adAccount, string prefixCode)
        {
            var id = String.Empty;
            var date = DateTime.Now.ToString("yyyyMMdd");
            var strSQL = String.Format(@"Select top 1 * from SignDocCode Where SignDocID like '{0}{1}%' Order by SignDocID Desc", prefixCode, date);
            //根據簽核代碼的類型取得該最大ID值，ex：FP0000001取出後+1 為FP0000002 by 俊晨
            var lastindentityID = _dc.QueryForDataRow(strSQL, null);
            var seq = (lastindentityID != null) ? lastindentityID["SequenceNum"].ToString() : "0";
            seq = (Int32.Parse(seq) + 1).ToString().PadLeft(4, '0');
            //單後生產邏輯 簽核類別+日期+序號 ex：FP2017082301
            id = String.Concat(prefixCode, date, seq);

            //建立新簽核號碼
            strSQL =
                    @"INSERT INTO signdoccode
                        (signdocid,
                        code,
                        date,
                        sequencenum,
                        creator,
                        createdate)
                    VALUES     (@SignDocID,
                        @Code,
                        @Date,
                        @SequenceNum,
                        @Creator,
                        @CreateDate) ";
            var mainipulationConditions = new Conditions()
            {
                {"@SignDocID", id},
                {"@Code", prefixCode},
                {"@Date", date},
                {"@SequenceNum", seq},
                {"@Creator", adAccount},
                {"@CreateDate", DateTime.Now}
            };

            if (_dc.ExecuteAndCheck(strSQL, mainipulationConditions))
                return id;
            else
                return String.Empty;
        }

        /// <summary>
        /// 取得郵戳
        /// </summary>
        /// <returns></returns>
        private static string getTicket()
        {
            var dic = new Conditions();
            dic.Add("@motion", "getSN");
            return _dc.ExecuteAndGetTicket(@"insert into UserPriority values(@motion)", dic).ToString();
        }
    }
}