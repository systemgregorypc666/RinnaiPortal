namespace RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceWinningNumberModels
{
    public class EinvoiceWinningNumberSendModel
    {
        private string m_version = string.Empty;

        private string m_action = string.Empty;

        private string m_UUID = string.Empty;

        /// <summary>
        /// 版本
        /// </summary>
        public string version { get { return this.m_version; } set { this.m_version = value; } }

        /// <summary>
        /// API行為
        /// </summary>
        public string action { get { return this.m_action; } set { this.m_action = value; } }

        /// <summary>
        /// 行動工具 Unique ID
        /// </summary>
        public string UUID { get { return this.m_UUID; } set { this.m_UUID = value; } }

        /// <summary>
        /// 透過財政資訊中心大平台申請之軟體 ID
        /// </summary>
        public string appID { get { return ConnectionStringModel.APPID; } }

        /// <summary>
        /// 查詢月份
        /// </summary>
        public string invTerm { get; set; }
    }
}