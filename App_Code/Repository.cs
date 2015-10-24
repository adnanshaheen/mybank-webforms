using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

/// <summary>
/// Summary description for DataLayer
/// </summary>
public class Repository : IRepositoryDataAuthentication, IRepositoryDataAccount
{
    IDataAccess _idataAccess = null;
    CacheAbstraction webCache = null;

    public Repository(IDataAccess ida, CacheAbstraction webc)
    {
        _idataAccess = ida;
        webCache = webc;
    }

    public Repository()
        : this(GenericFactory<DataAccess,IDataAccess>.CreateInstance(),
        new CacheAbstraction())
    {
    }


    #region IDataAuthentication Members

    public string IsValidUser(string uname, string pwd)
    {
        string res = "";
        try
        {
            string sql = "select CheckingAccountNum from Users where " +
                "Username=@uname and Password=@pwd";
            List<SqlParameter> PList = new List<SqlParameter>();
            SqlParameter p1 = new SqlParameter("@uname", SqlDbType.VarChar, 50);
            p1.Value = uname;
            PList.Add(p1);
            SqlParameter p2 = new SqlParameter("@pwd", SqlDbType.VarChar, 50);
            p2.Value = pwd;
            PList.Add(p2);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = obj.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return res;

    }

    #endregion

    #region IDataAccount Members

    public bool TransferChkToSav(string chkAcctNum, string savAcctNum, double amt)
    {
        // to do
        return true;
    }

    public double GetCheckingBalance(string chkAcctNum)
    {
        double res = 0;
        try
        {
            string sql = "select Balance from CheckingAccounts where " +
                "CheckingAccountNumber=@chkAcctNum";
            List<SqlParameter> PList = new List<SqlParameter>();
            SqlParameter p1 = new SqlParameter("@chkAcctNum", SqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql,PList);
            if (obj != null)
                res = double.Parse(obj.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return res;
    }

    public bool TransferChkToSavViaSP(string chkAcctNum, string savAcctNum,
           double amt)
    {
        string CONNSTR = ConfigurationManager.ConnectionStrings["BANKDBCONN"].ConnectionString;
        bool res = false;
        SqlConnection conn = new SqlConnection(CONNSTR);
        try
        {
            conn.Open();
            string sql = "SPXferChkToSav"; // name of SP
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlParameter p1 = new SqlParameter("@ChkAcctNum",
                                System.Data.SqlDbType.VarChar, 50);

            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            p1.Value = chkAcctNum;
            cmd.Parameters.Add(p1);
            SqlParameter p2 = new SqlParameter("@SavAcctNum",
                                System.Data.SqlDbType.VarChar, 50);
            p2.Value = savAcctNum;
            cmd.Parameters.Add(p2);
            SqlParameter p3 = new SqlParameter("@amt",
                                System.Data.SqlDbType.Money);
            p3.Value = amt;
            cmd.Parameters.Add(p3);
            int rows = cmd.ExecuteNonQuery();
            if (rows == 3)
                res = true;
            
            // clear cache for TransferHistory
            string key = String.Format("TransferHistory_{0}",
                chkAcctNum);
            webCache.Remove(key);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return res;
    }

  
    public double GetSavingBalance(string savAcctNum)
    {
        double res = 0;
        try
        {
            string sql = "select Balance from SavingAccounts where " +
                "SavingAccountNumber=@savAcctNum";
            List<SqlParameter> PList = new List<SqlParameter>();
            SqlParameter p1 = new SqlParameter("@savAcctNum", SqlDbType.VarChar, 50);
            p1.Value = savAcctNum;
            PList.Add(p1);
            object obj = _idataAccess.GetSingleAnswer(sql,PList);
            if (obj != null)
                res = double.Parse(obj.ToString());
        }
        catch (Exception)
        {
            throw;
        };
        return res;
    }

    #endregion

    #region IDataAccount Members
    public List<TransferHistory> GetTransferHistory(string chkAcctNum)
    {
        List<TransferHistory> TList = null;
        try
        {
            string key = String.Format("TransferHistory_{0}",
                chkAcctNum);
            TList = webCache.Retrieve<List<TransferHistory>>(key);
            if (TList == null)  
            {
                //TList = new List<TransferHistory>();
                DataTable dt = GetTransferHistoryDB(chkAcctNum);
                TList = RepositoryHelper.ConvertDataTableToList<TransferHistory>(dt);
                //foreach (DataRow dr in dt.Rows)
                //{
                //    TransferHistory the = new TransferHistory();
                //    the.SetFields(dr);
                //    TList.Add(the);
                //}
                webCache.Insert(key, TList);
            }
         }
        catch (Exception ex)
        {
            throw ex;
        };
        return TList;
    }

    

    #endregion

    public System.Data.DataTable GetTransferHistoryDB(string chkAcctNum)
    {
        DataTable dt = null;
        try
        {
            string sql = "select * from TransferHistory where " +
                "CheckingAccountNumber=@chkAcctNum";
            List<SqlParameter> PList = new List<SqlParameter>();
            SqlParameter p1 = new SqlParameter("@chkAcctNum", SqlDbType.VarChar, 50);
            p1.Value = chkAcctNum;
            PList.Add(p1);
            dt = _idataAccess.GetDataTable(sql,PList);
        }
        catch (Exception ex)
        {
            throw ex;
        };
        return dt;
    }


    public bool UpdatePassword(string uname, string oldPW, string newPW)
    {
        string res = "";
        try
        {
            res = IsValidUser(uname, oldPW);
            string sql = "update Users set Password=@newPW where " +
                "Username=@uname and Password=@oldPW";
            List<SqlParameter> PList = new List<SqlParameter>();
            SqlParameter p1 = new SqlParameter("@uname", SqlDbType.VarChar, 50);
            p1.Value = uname;
            PList.Add(p1);
            SqlParameter p2 = new SqlParameter("@oldPW", SqlDbType.VarChar, 50);
            p2.Value = oldPW;
            PList.Add(p2);
            SqlParameter p3 = new SqlParameter("@newPW", SqlDbType.VarChar, 50);
            p3.Value = newPW;
            PList.Add(p3);
            object obj = _idataAccess.GetSingleAnswer(sql, PList);
            if (obj != null)
                res = obj.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return true;
   }
}