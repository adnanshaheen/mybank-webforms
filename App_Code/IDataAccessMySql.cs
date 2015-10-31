using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for IDataAccessMySql
/// </summary>
public interface IDataAccessMySql
{
    object GetSingleAnswer(string sql, List<MySqlParameter> PList);
    DataTable GetDataTable(string sql, List<MySqlParameter> PList);
    int InsOrUpdOrDel(string sql, List<MySqlParameter> PList);
}
