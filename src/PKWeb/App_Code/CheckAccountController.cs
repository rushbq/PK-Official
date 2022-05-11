using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;

public class CheckAccountController : ApiController
{
    public IEnumerable<string> Get()
    {
        throw new HttpResponseException(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// 判斷帳號是否存在
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    /// <remarks>
    /// </remarks>
    public string Get(string account)
    {
        try
        {
            string ErrMsg;

            //判斷EMail格式
            if (!Regex.IsMatch(account, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                return "ERROR";
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();

                //[SQL] - SQL Statement
                sbSQL.AppendLine(" SELECT Mem_ID, Mem_Account ");
                sbSQL.AppendLine(" FROM Member_Data WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Mem_Account = @account) ");

                //[SQL] - SQL Source
                cmd.CommandText = sbSQL.ToString();

                //[SQL] - Parameters
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("account", account);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //可使用
                        return "OK";
                    }
                    else
                    {
                        //不可使用
                        return "NO";
                    }
                }
            }

        }
        catch (Exception)
        {

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }

}
