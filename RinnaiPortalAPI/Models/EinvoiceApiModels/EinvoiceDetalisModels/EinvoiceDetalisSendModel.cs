using OpenUDIDCSharp;
namespace RinnaiPortalOpenApi.Models.EinvoiceApiModels.EinvoiceDetalisModels
{
    public class EinvoiceDetalisSendModel
    {
        /*
        version
        type
        invNum
        action
        generation
        invTerm
        invDate
        encrypt
        sellerID
        UUID
        randomNumber
        appID
        */
        private double m_version = 0.0d;
        private string m_type = string.Empty;
        private string m_invNum = string.Empty;
        private string m_action = string.Empty;
        private string m_generation = string.Empty;
        private string m_invTerm = string.Empty;
        private string m_invDate = string.Empty;
        private string m_encrypt = string.Empty;
        private string m_sellerID = string.Empty;
        private string m_UUID = string.Empty;
        private string m_randomNumber = string.Empty;
        private string m_appID = string.Empty;
        public double version { get { return m_version; } set { m_version = value; } }
        public string type { get { return m_type; } set { m_type = value; } }
        public string invNum { get { return m_invNum; } set { m_invNum = value; } }
        public string action { get { return m_action; } set { m_action = value; } }
        public string generation { get { return m_generation; } set { m_generation = value; } }
        public string invTerm { get { return m_invTerm; } set { m_invTerm = value; } }
        public string invDate { get { return m_invDate; } set { m_invDate = value; } }
        public string encrypt { get { return m_encrypt; } set { m_encrypt = value; } }
        public string sellerID { get { return m_sellerID; } set { m_sellerID = value; } }
        public string UUID
        {
            get
            {
                return ConnectionStringModel.UUID;
            }
        }
        public string randomNumber { get { return m_randomNumber; } set { m_randomNumber = value; } }
        public string appID
        {
            get
            {
                return ConnectionStringModel.APPID;
            }
        }
    }
}