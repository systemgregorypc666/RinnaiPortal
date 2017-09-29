using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Web.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;


namespace RinnaiPortal
{
    public class DBClass
    {
        public DBClass()
        {
            strConn = WebConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString;
        }
        private string strConn;

        #region 執行SQL
        public DataTable executeSQLQuery(SqlCommand cmd)
        {
            SqlConnection conn = new SqlConnection(strConn);
            DataTable dt = new DataTable();

            try
            {
                cmd.Connection = conn;
                conn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                dt.Load(dr);

                return dt;
            }
            catch { return dt; }
            finally { conn.Close(); }
        }
        #endregion

        #region 直接使用
        public static DataTable Create_Table(string SQLcmd, string TabName)
        {
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString);
            conn.Open();
            DataSet myDataSet = new DataSet();
            SqlDataAdapter myAdapter;
            myAdapter = new SqlDataAdapter(SQLcmd, conn);
            myAdapter.Fill(myDataSet, TabName);
            conn.Close();
            return myDataSet.Tables[TabName];

        }

        public static DataTable Create_Table_T(string SQLcmd, string TabName)
        {
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["TrainingConnectionStringName"].ConnectionString);
            conn.Open();
            DataSet myDataSet = new DataSet();
            SqlDataAdapter myAdapter;
            myAdapter = new SqlDataAdapter(SQLcmd, conn);
            myAdapter.Fill(myDataSet, TabName);
            conn.Close();
            return myDataSet.Tables[TabName];

        }

        public static DataTable Create_Table_S(string SQLcmd, string TabName)
        {
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["SmartManConnectionStringName"].ConnectionString);
            conn.Open();
            DataSet myDataSet = new DataSet();
            SqlDataAdapter myAdapter;
            myAdapter = new SqlDataAdapter(SQLcmd, conn);
            myAdapter.Fill(myDataSet, TabName);
            conn.Close();
            return myDataSet.Tables[TabName];

        }
        public static string ExcuteSqlReturn(string strSQL)
        {
            DataTable temp = Create_Table(strSQL, "GET");
            if (temp.Rows.Count == 1)
            {
                return temp.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        #endregion

        public static DataTable DealCommandTable(string sql, List<SqlParameter> SqlPrams)
        {
            DataTable dt = new DataTable();
            string strConn = WebConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConn);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                if (SqlPrams != null && SqlPrams.Count() > 0)
                {
                    foreach (SqlParameter para in SqlPrams)
                    {

                        cmd.Parameters.Add(para);
                    }
                }

                dt.Load(cmd.ExecuteReader());

                return dt;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Close();
            }
        }
        static SqlTransaction Btransation;

        public static bool DealCommand(string sql, SqlParameter[] SqlPrams)
        {
            string strConn = WebConfigurationManager.ConnectionStrings["LocalConnectionStringName"].ConnectionString;
            SqlConnection conn = new SqlConnection(strConn);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Btransation = conn.BeginTransaction();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Transaction = Btransation;
            try
            {
                if (SqlPrams != null && SqlPrams.Count() > 0)
                {
                    foreach (SqlParameter para in SqlPrams)
                    {
                        cmd.Parameters.Add(para);
                    }
                }

                if (cmd.ExecuteNonQuery() != -1)
                {
                    Btransation.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                Btransation.Rollback();
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Close();
            }
        }

    }
    public partial class Show
    {
        //預設訊息
        public static void MessageAlert(Button obj, string type)
        {
            switch (type)
            {
                case "INSY":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "新增成功！"), true);
                    break;
                case "INSN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "新增失敗，請檢查資料或聯絡資訊人員！"), true);
                    break;
                case "UPDY":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "修改成功！"), true);
                    break;
                case "UPDN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "修改失敗，請檢查資料或聯絡資訊人員！"), true);
                    break;
                case "DELY":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "刪除成功！"), true);
                    break;
                case "DELN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "刪除失敗，請檢查資料或聯絡資訊人員！"), true);
                    break;
                case "BAKY":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "撤標成功！"), true);
                    break;
                case "BAKN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "撤標失敗，請檢查資料或聯絡資訊人員！"), true);
                    break;
                case "GOBAKY":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "還原發出成功！"), true);
                    break;
                case "GOBAKN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "還原失敗，請檢查資料或聯絡資訊人員！"), true);
                    break;
                case "FN":
                    ScriptManager.RegisterClientScriptBlock(obj, typeof(string), "click", string.Format("alert('{0}');", "沒有檔案，請檢查檔案路徑或聯絡資訊人員！"), true);
                    break;
            }
        }
    }
    public partial class CClass
    {
        public static void GridViewSHOW(ref System.Web.UI.WebControls.GridView gv, DataTable dt)
        {
            gv.DataSource = dt;
            gv.DataBind();
        }

        public static void DropDownListSHOW(ref System.Web.UI.WebControls.DropDownList ddl, DataTable dt)
        {
            ddl.DataSource = dt;
            ddl.DataBind();
        }
    }
}