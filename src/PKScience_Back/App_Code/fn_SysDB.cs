using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 判斷是否AD_IsUse, 變更 SYS的資料庫名
/// </summary>
public class fn_SysDB
{

    /// <summary>
    /// 回傳資料庫名稱
    /// </summary>
    private static string _Param_DB;
    public static string Param_DB
    {
        get
        {
            //判斷是否使用內部AD驗證
            string IsAD = System.Web.Configuration.WebConfigurationManager.AppSettings["AD_IsUse"];
            if (IsAD.ToUpper().Equals("Y"))
            {
                return "PKSYS";
            }
            else
            {
                return "MySYS";
            }

        }
        private set
        {
            _Param_DB = value;
        }
    }
}