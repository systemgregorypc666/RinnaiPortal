namespace RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels
{
    /// <summary>
    /// 財政部電子票API 中獎號碼查詢回船模型
    /// </summary>
    public class EinvoiceWinningNumberResultModel
    {
            /*
            "v":"<版本號碼>"，
            "code":"<訊息回應碼>"，
            "msg":"<系統回應訊息>"，
            "invoYm":"<查詢開獎期別>"，
            "superPrizeNo":"<千萬特獎號碼>"，
            "spcPrizeNo":"<特獎號碼>"，
            "spcPrizeNo2":"<特獎號碼 2>"，
            "spcPrizeNo3":"<特獎號碼3>"，
            "firstPrizeNo1":"<頭獎號碼 1>"，
            "firstPrizeNo2":"<頭獎號碼2>"，
            "firstPrizeNo3":"<頭獎號碼3>"，
            "firstPrizeNo4":"<頭獎號碼4>"，
            "firstPrizeNo5":"<頭獎號碼5>"，
            "firstPrizeNo6":"<頭獎號碼6>"，
            "firstPrizeNo7":"<頭獎號碼7>"，
            "firstPrizeNo8":"<頭獎號碼8>"，
            "firstPrizeNo9":"<頭獎號碼9>"，
            "firstPrizeNo10":"<頭獎號碼10>"，
            "sixthPrizeNo1":"<六獎號碼 1>"，
            "sixthPrizeNo2":"<六獎號碼2>"，
            "sixthPrizeNo3":"<六獎號碼3>"，
            "superPrizeAmt":"<千萬特獎金額>"，
            "spcPrizeAmt":"<特獎金額>"，
            "firstPrizeAmt":"<頭獎金額>"，
            "secondPrizeAmt":"<二獎金額>"，
            "thirdPrizeAmt":"<三獎金額>"，
            "fourthPrizeAmt":"<四獎金額>"，
            "fifthPrizeAmt":"<五獎金額>"，
            "sixthPrizeAmt":"<六獎金額>"，
            "sixthPrizeNo4":"<六獎號碼 4>"，
            "sixthPrizeNo5":"<六獎號碼5>"，
            "sixthPrizeNo6":"<六獎號碼6>"
            */
        public string fifthPrizeAmt { get; set; }
        public string firstPrizeAmt { get; set; }
        public string firstPrizeNo1 { get; set; }
        public string firstPrizeNo10 { get; set; }
        public string firstPrizeNo2 { get; set; }
        public string firstPrizeNo3 { get; set; }
        public string firstPrizeNo4 { get; set; }
        public string firstPrizeNo5 { get; set; }
        public string firstPrizeNo6 { get; set; }
        public string firstPrizeNo7 { get; set; }
        public string firstPrizeNo8 { get; set; }
        public string firstPrizeNo9 { get; set; }
        public string fourthPrizeAmt { get; set; }
        public string invoYm { get; set; }
        public string secondPrizeAmt { get; set; }
        public string sixthPrizeAmt { get; set; }
        public string sixthPrizeNo1 { get; set; }
        public string sixthPrizeNo2 { get; set; }
        public string sixthPrizeNo3 { get; set; }
        public string sixthPrizeNo4 { get; set; }
        public string sixthPrizeNo5 { get; set; }
        public string sixthPrizeNo6 { get; set; }
        public string spcPrizeAmt { get; set; }
        public string spcPrizeNo { get; set; }
        public string spcPrizeNo2 { get; set; }
        public string spcPrizeNo3 { get; set; }
        public string superPrizeAmt { get; set; }
        public string superPrizeNo { get; set; }
        public string thirdPrizeAmt { get; set; }
        public Timestamp timeStamp { get; set; }
        public string updateDate { get; set; }
        public string v { get; set; }
        public string code { get; set; }
        public string msg { get; set; }
    }

    public class Timestamp
    {
        public int date { get; set; }
        public int day { get; set; }
        public int hours { get; set; }
        public int minutes { get; set; }
        public int month { get; set; }
        public int seconds { get; set; }
        public long time { get; set; }
        public int timezoneOffset { get; set; }
        public int year { get; set; }
    }
}