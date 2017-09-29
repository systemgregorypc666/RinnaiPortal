using DBTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.FactoryMethod
{
    public static class ConnectionFactory
    {
        public static DB GetPortalDC()
        {
            string portalConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString;
            return new DB(portalConnectionString);
        }

        public static DB GetSmartManDC()
        {
            string smartManConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SmartManConnectionStringName"].ConnectionString;
            return new DB(smartManConnectionString);
        }

        public static DB GetTrainingDC()
        {
            string trainingConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TrainingConnectionStringName"].ConnectionString;
            return new DB(trainingConnectionString);
        }
    }
}