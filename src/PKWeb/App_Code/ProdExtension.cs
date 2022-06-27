using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

/// <summary>
/// ProdExtension 的摘要描述
/// </summary>
public class ProdExtension
{
    public enum lst功能區塊
    {
        [Description("A")]
        熱銷,
        [Description("B")]
        新品,
        [Description("C")]
        產品類別
    };

    public enum lst產品類型
    {
        [Description("A")]
        工具,
        [Description("B")]
        玩具
    };


    public static string Get_宣傳Html(string val_功能區塊, string val_產品類型, string val_關聯編號)
    {
        string ErrMsg;
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string sql;

                //READ
                sql = @"SELECT TOP 1 Content1
                    FROM [產品宣傳]
                    WHERE (UPPER(LangCode) = UPPER(@lang))
                     AND (功能區塊 = @param_功能區塊)
                     AND (產品類型 = @param_產品類型)
                     AND (關聯編號 = @param_關聯編號)";

                //----- SQL 執行 -----
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("lang", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("param_功能區塊", val_功能區塊);
                cmd.Parameters.AddWithValue("param_產品類型", val_產品類型);
                cmd.Parameters.AddWithValue("param_關聯編號", val_關聯編號);

                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return "";
                    }

                    string GetContent = DT.Rows[0]["Content1"].ToString();
                    return GetContent;
                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return "載入失敗!";
        }

    }

}

/// <summary>
/// 擴充方法 Enum ToDescription
/// </summary>
public static class EnumExtenstions
{
    public static string ToDescription(this Enum value)
    {
        return value.GetType()
            .GetRuntimeField(value.ToString())
            .GetCustomAttributes<DescriptionAttribute>()
            .Select(p => p.Description)
            .FirstOrDefault() ?? string.Empty;
    }
}