using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Extensions
{
    public static class ValueCheck
    {
        public static bool IsDBNullOrWhiteSpace(this object value, TypeCode type = TypeCode.String)
        {
            try
            {
                if (DBNull.Value.Equals(value)) { return true; }
                if (value == null) { return true; }

                var convertValue = Convert.ChangeType(value, type);
                if (String.Empty.Equals(convertValue)) { return true; }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}