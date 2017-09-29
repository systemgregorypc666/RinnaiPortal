using OpenUDIDCSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortalOpenApi.Models
{
    public class ConnectionStringModel
    {
        public static string APPID { get { return "EINV5201709293943"; } }
        public static string APPKEY { get { return "OGdZVzNlaDRvVmFURVBXaQ=="; } }
        public static string UUID { get { return OpenUDID.value; } }



        public static string EinvoiceSearchApiConnectionStr { get { return @"https://einvoice.nat.gov.tw/PB2CAPIVAN/invapp/InvApp?"; } }
    }
}