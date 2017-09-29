using RinnaiPortal.Interface;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Extensions;
using System.Collections.Generic;


namespace RinnaiPortalTests.Tools.Compare
{
    public class ParmsCompare<T> : IEqualityComparer<T>
    {
        public bool Equals(T actual, T expect)
        {
            bool result = true;
            if (actual is PaggerParms && expect is PaggerParms)
            {
                var a = actual as PaggerParms;
                var b = expect as PaggerParms;
                if (a.PageSize != b.PageSize) { result &= false; }
                if (a.PageIndex != b.PageIndex) { result &= false; }
                if (a.OrderField != b.OrderField) { result &= false; }
                if (a.Descending != b.Descending) { result &= false; }
            }
            else if (actual is SignListParms && expect is SignListParms)
            {
                var a = actual as SignListParms;
                var b = expect as SignListParms;
                if (a.QueryText != b.QueryText) { result &= false; }
                if (a.SignDocID != b.SignDocID) { result &= false; }
            }
            else if (actual is DepartmentViewModel && expect is DepartmentViewModel)
            {
                var a = actual as DepartmentViewModel;
                var b = expect as DepartmentViewModel;
                if (a.ChiefID_FK != b.ChiefID_FK) { result &= false; }
                if (a.ChiefID_FKSelectedIndex != b.ChiefID_FKSelectedIndex) { result &= false; }
                if (a.Creator != b.Creator) { result &= false; }
                if (a.DepartmentID != b.DepartmentID) { result &= false; }
                if (a.DepartmentLevel != b.DepartmentLevel) { result &= false; }
                if (a.DepartmentName != b.DepartmentName) { result &= false; }
                if (a.Disabled != b.Disabled) { result &= false; }
                if (a.DisabledDate.FormatDatetimeNullable() != b.DisabledDate.FormatDatetimeNullable()) { result &= false; }
                if (a.FilingEmployeeID_FK != b.FilingEmployeeID_FK) { result &= false; }
                if (a.FilingEmployeeID_FKSelectedIndex != b.FilingEmployeeID_FKSelectedIndex) { result &= false; }
                if (a.Modifier != b.Modifier) { result &= false; }
                if (a.TimeStamp != b.TimeStamp) { result &= false; }
                if (a.UpperDepartmentID != b.UpperDepartmentID) { result &= false; }
                if (a.UpperDepartmentIDSelectedIndex != b.UpperDepartmentIDSelectedIndex) { result &= false; }
            }
            else
            {
                result &= false;
            }
            return result;
        }

        public int GetHashCode(T a)
        {
            return 0;
        }
    }
}
