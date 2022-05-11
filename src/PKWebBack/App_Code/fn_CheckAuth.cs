using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using ExtensionMethods;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data;
using ExtensionIO;

/// <summary>
/// 權限處理
/// </summary>
/// <remarks>
/// 判斷個人權限XML是否存在
///   -否:前往群組權限
///   -是:
///     1.判斷帳戶是否停用
///     2.判斷權限編號是否正常設定
/// </remarks>
public class fn_CheckAuth
{
    #region -- 權限檢查 --
    /// <summary>
    /// 權限檢查
    /// </summary>
    /// <param name="authProgID">欲判斷的權限編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    /// <remarks>
    /// 先判斷是否有個人權限, 若沒有才檢查群組權限
    /// </remarks>
    public static bool CheckAuth(string authProgID, out string ErrMsg)
    {
        try
        {
            //取得個人Guid
            string tmpGuid = HttpContext.Current.Session["Login_GUID"].ToString();
            if (string.IsNullOrEmpty(tmpGuid))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }
            //取得個人帳號
            string tmpAccount = HttpContext.Current.Session["Login_UserID"].ToString();
            if (string.IsNullOrEmpty(tmpAccount))
            {
                ErrMsg = "無法取得個人參數，請聯絡系統管理員!";
                return false;
            }

            //判斷是否有個人權限
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Guid, Prog_ID ");
                sbSQL.AppendLine(" FROM User_Profile_Rel_Program WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Prog_ID = @Prog_ID) AND (Guid = @Guid) ");

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("Prog_ID", authProgID);
                cmd.Parameters.AddWithValue("Guid", tmpGuid);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        //未建立個人權限，前往取得部門權限
                        return CheckAuth_Group(authProgID, out ErrMsg);

                    }
                    else
                    {
                        ErrMsg = "";
                        return true;

                    }
                }
            }

            /*
            //取得Xml
            string XmlResult = fn_Extensions.IORequest_GET(
                System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\User_Profile_" + tmpAccount + ".xml");
            if (string.IsNullOrEmpty(XmlResult))
            {
                //未建立個人權限，前往取得部門權限
                return CheckAuth_Group(authProgID, out ErrMsg);
            }

            //將Xml字串轉成byte
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
            //讀取Xml
            using (XmlReader reader = XmlTextReader.Create(stream))
            {
                //使用XElement載入Xml
                XElement XmlDoc = XElement.Load(reader);
                //[XML處理] - 取得User的ProgID
                var Results = from result in XmlDoc.Elements("User")
                              where tmpGuid.Contains(result.Attribute("Guid").Value)
                              select new
                              {
                                  GUID = result.Attribute("Guid").Value,
                                  Display = result.Attribute("Display").Value,
                                  ProgIDs = result.Elements("ProgID")
                              };
                if (Results.Count() == 0)
                {
                    //未建立個人權限，前往取得部門權限
                    return CheckAuth_Group(authProgID, out ErrMsg);
                }
                else
                {
                    //[XML處理] - 判斷是否有設定個人權限
                    if (Results.ElementAt(0).ProgIDs.Count() == 0)
                    {
                        //未建立個人權限，前往取得部門權限
                        return CheckAuth_Group(authProgID, out ErrMsg);
                    }

                    //[XML處理] - 判斷帳號是否已停用
                    if (Results.ElementAt(0).Display.Equals("N"))
                    {
                        //前往失敗頁
                        ErrMsg = "帳號已停用!";
                        return false;
                    }

                    //[XML處理] - 判斷是否有使用權限
                    var CheckAuth = from auth in Results.ElementAt(0).ProgIDs
                                    where auth.Value.Contains(authProgID)
                                    select new
                                    {
                                        ProgID = auth.Value
                                    };

                    if (CheckAuth.Count() == 0)
                    {
                        ErrMsg = "帳號權限不足!";
                        return false;
                    }
                    else
                    {
                        ErrMsg = "";
                        return true;
                    }
                }
            }
             */

        }
        catch (Exception)
        {
            ErrMsg = "權限判斷發生錯誤，請聯絡系統管理員!";
            return false;
        }
    }

    /// <summary>
    /// [權限判斷] - 群組
    /// </summary>
    /// <param name="tmpGuid">所屬群組的GUID</param>
    /// <param name="authProgID">欲判斷的權限編號</param>
    /// <param name="ErrMsg">錯誤訊息</param>
    /// <returns>bool</returns>
    private static bool CheckAuth_Group(string authProgID, out string ErrMsg)
    {
        try
        {
            //取得所屬群組Guid
            ArrayList tmpGuid = (ArrayList)HttpContext.Current.Session["Login_UserGroups"];
            if (tmpGuid == null)
            {
                ErrMsg = "無法取得所屬群組，請聯絡系統管理員!";
                return false;
            }


            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder sbSQL = new StringBuilder();
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                sbSQL.AppendLine(" SELECT Guid, Prog_ID ");
                sbSQL.AppendLine(" FROM User_Group_Rel_Program WITH (NOLOCK) ");
                sbSQL.AppendLine(" WHERE (Prog_ID = @Prog_ID) ");

                #region >>群組參數組合<<
                //[SQL] - 暫存參數
                string tempParam = "";
                for (int row = 0; row < tmpGuid.Count; row++)
                {
                    if (string.IsNullOrEmpty(tempParam) == false) { tempParam += ","; }
                    tempParam += "@ParamTmp" + row;
                }
                //[SQL] - 代入暫存參數
                sbSQL.AppendLine(" AND (Guid IN (" + tempParam + "))");
                for (int row = 0; row < tmpGuid.Count; row++)
                {
                    cmd.Parameters.AddWithValue("ParamTmp" + row, tmpGuid[row]);
                }
                #endregion

                //[SQL] - Command
                cmd.CommandText = sbSQL.ToString();
                cmd.Parameters.AddWithValue("Prog_ID", authProgID);

                //取得資料
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        ErrMsg = "所屬群組權限不足!";
                        return false;
                    }
                    else
                    {
                        ErrMsg = "";
                        return true;
                    }
                }
            }

            /*

            //取得Xml
            string XmlResult = fn_Extensions.IORequest_GET(
                System.Web.Configuration.WebConfigurationManager.AppSettings["DiskUrl"] + @"Data_File\Authorization\User_Group.xml");
            if (string.IsNullOrEmpty(XmlResult))
            {
                ErrMsg = "無法取得群組權限，請聯絡系統管理員!";
                return false;
            }
            //將Xml字串轉成byte
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(XmlResult));
            //讀取Xml
            using (XmlReader reader = XmlTextReader.Create(stream))
            {
                //使用XElement載入Xml
                XElement XmlDoc = XElement.Load(reader);
                //[XML處理] - 取得各群組的ProgID
                var Results = from result in XmlDoc.Elements("Group")
                              where tmpGuid.Contains(result.Attribute("Guid").Value) && result.Attribute("Display").Value.Contains("Y")
                              select new
                              {
                                  GUID = result.Attribute("Guid").Value,
                                  ProgIDs = result.Elements("ProgID")
                              };

                //[XML處理] - 暫存各群組的ProgID
                List<string> tmpProgID = new List<string>();
                foreach (var result in Results)
                {
                    foreach (var prog in result.ProgIDs)
                    {
                        tmpProgID.Add(prog.Value);
                    }
                }

                //[XML處理] - 判斷是否有使用權限
                var CheckAuth = from auth in tmpProgID
                                where auth.Contains(authProgID)
                                group auth by auth.ToString() into gp
                                select new
                                {
                                    ProgID = gp.Key
                                };

                if (CheckAuth.Count() == 0)
                {
                    ErrMsg = "所屬群組權限不足!";
                    return false;
                }
                else
                {
                    ErrMsg = "";
                    return true;
                }
            }
             */
        }
        catch (Exception)
        {
            ErrMsg = "權限判斷發生錯誤，請聯絡系統管理員!";
            return false;
        }
    }
    #endregion

}