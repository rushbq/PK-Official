<%@ WebHandler Language="C#" Class="GetData_ProdPhotos" %>

using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ExtensionMethods;

public class GetData_ProdPhotos : IHttpHandler
{

    /// <summary>
    /// jQuery DataTable 取得資料(Ajax)
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <see cref="http://www.codeproject.com/Tips/1011531/Using-jQuery-DataTables-with-Server-Side-Processin"/>
    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg;

        //[接收參數] 內建參數
        Int16 draw = Convert.ToInt16(context.Request["draw"]);      //為避免XSS攻擊，內建的控制
        Int16 start = Convert.ToInt16(context.Request["start"]);    //該頁的第一筆為所有資料的第n筆(從0開始)
        Int16 length = Convert.ToInt16(context.Request["length"]);  //每頁顯示筆數


        //[接收參數] 查詢字串
        string searchVal = context.Request["search[value]"];

        //分頁變數
        int TotalRow = 0;  //總筆數
        int BgItem = start + 1;  //開始筆數
        int EdItem = BgItem + (length - 1);  //結束筆數 

        //宣告
        StringBuilder SBSql = new StringBuilder();

        //查詢
        using (SqlCommand cmd = new SqlCommand())
        {
            using (SqlCommand cmdTotalCnt = new SqlCommand())
            {
                //----- [主要資料欄位] Start -----
                SBSql.Append(" SELECT Tbl.* ");
                SBSql.Append(" FROM (");
                SBSql.Append("    SELECT RTRIM(Prod.Model_No) AS ID, (Prod.Model_Name_{0}) AS Label, Cls.Class_Name_{0} AS ClassName".FormatThis(fn_Language.Param_Lang));
                SBSql.Append("    , (SELECT COUNT(*) FROM [ProductCenter].dbo.ProdPic_Photo WHERE (Model_No = Prod.Model_No)) AS Cnt_Photo");
                SBSql.Append("    , (SELECT COUNT(*) FROM [ProductCenter].dbo.ProdPic_Group WHERE (Model_No = Prod.Model_No) AND (Pic_Class = 3)) AS Cnt_DM");
                SBSql.Append("    , (SELECT COUNT(*) FROM [ProductCenter].dbo.ProdPic_Figure WHERE (Model_No = Prod.Model_No) AND (");
                SBSql.Append("     (Pic01 <> '' AND Pic01 IS NOT NULL) OR (Pic02 <> '' AND Pic02 IS NOT NULL)");
                SBSql.Append("     OR (Pic03 <> '' AND Pic03 IS NOT NULL) OR (Pic04 <> '' AND Pic04 IS NOT NULL)");
                SBSql.Append("     OR (Pic05 <> '' AND Pic05 IS NOT NULL) OR (Pic06 <> '' AND Pic06 IS NOT NULL)");
                SBSql.Append("     OR (Pic07 <> '' AND Pic07 IS NOT NULL) OR (Pic08 <> '' AND Pic08 IS NOT NULL)");
                SBSql.Append("     )) AS Cnt_Figure");
                SBSql.Append("    , (SELECT COUNT(*) FROM [ProductCenter].dbo.Prod_MallPic WHERE (Model_No = Prod.Model_No) AND (UPPER(LangCode) = UPPER(@LangCode))) AS Cnt_MallPic");
                SBSql.Append("    , ROW_NUMBER() OVER (ORDER BY Prod.Model_No ASC) AS RowRank");
                SBSql.Append("    FROM Prod GP");
                SBSql.Append("     INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH (NOLOCK) ON GP.Model_No = Prod.Model_No");
                SBSql.Append("     INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH (NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append("    WHERE (GP.Display = 'Y') AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

                //-- 查詢條件 --
                if (!string.IsNullOrEmpty(searchVal))
                {
                    SBSql.Append("  AND (");
                    SBSql.Append("   (UPPER(Prod.Model_No) LIKE '%' + UPPER(@Keyword) + '%') ");
                    SBSql.Append("   OR (UPPER(Prod.Model_Name_{0}) LIKE '%' + UPPER(@Keyword) + '%') ".FormatThis(fn_Language.Param_Lang));
                    SBSql.Append("   OR (UPPER(Cls.Class_Name_{0}) LIKE '%' + UPPER(@Keyword) + '%') ".FormatThis(fn_Language.Param_Lang));
                    SBSql.Append("  )");

                    cmd.Parameters.AddWithValue("Keyword", searchVal);
                }

                SBSql.Append(" ) AS Tbl");
                SBSql.Append(" WHERE (RowRank >= @BG_ITEM) AND (RowRank <= @ED_ITEM)");
                SBSql.Append(" ORDER BY RowRank");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("BG_ITEM", BgItem);
                cmd.Parameters.AddWithValue("ED_ITEM", EdItem);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);

                //----- [主要資料欄位] End -----


                //----- [計算資料筆數] Start -----
                SBSql.Clear();
                SBSql.Append(" SELECT COUNT(*)");
                SBSql.Append(" FROM Prod GP");
                SBSql.Append("  INNER JOIN [ProductCenter].dbo.Prod_Item Prod WITH (NOLOCK) ON GP.Model_No = Prod.Model_No");
                SBSql.Append("  INNER JOIN [ProductCenter].dbo.Prod_Class Cls WITH (NOLOCK) ON Prod.Class_ID = Cls.Class_ID");
                SBSql.Append(" WHERE (GP.Display = 'Y') AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");

                //-- 查詢條件 --
                if (!string.IsNullOrEmpty(searchVal))
                {
                    SBSql.Append("  AND (");
                    SBSql.Append("   (UPPER(Prod.Model_No) LIKE '%' + UPPER(@Keyword) + '%') ");
                    SBSql.Append("   OR (UPPER(Prod.Model_Name_{0}) LIKE '%' + UPPER(@Keyword) + '%') ".FormatThis(fn_Language.Param_Lang));
                    SBSql.Append("   OR (UPPER(Cls.Class_Name_{0}) LIKE '%' + UPPER(@Keyword) + '%') ".FormatThis(fn_Language.Param_Lang));
                    SBSql.Append("  )");

                    cmdTotalCnt.Parameters.AddWithValue("Keyword", searchVal);
                }

                //[SQL] - Command
                cmdTotalCnt.CommandText = SBSql.ToString();

                //----- [計算資料筆數] End -----


                //[SQL] - 取得資料
                using (DataTable DT = dbConn.LookupDTwithPage(cmd, cmdTotalCnt, out TotalRow, out ErrMsg))
                {
                    //序列化DT
                    string data = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.Indented);

                    //將Json加到data的屬性下
                    JObject json = new JObject();

                    json.Add(new JProperty("draw", draw));
                    json.Add(new JProperty("recordsTotal", TotalRow));
                    json.Add(new JProperty("recordsFiltered", TotalRow));
                    json.Add(new JProperty("data", JsonConvert.DeserializeObject<JArray>(data)));

                    /*
                     * [回傳格式] - Json
                     * draw：內建函數(查詢次數)
                     * recordsTotal：篩選前的總資料數 (serverside模式)
                     * recordsFiltered：篩選後的總資料數 (serverside模式)
                     * data：該分頁所需要的資料
                     */

                    //輸出Json
                    context.Response.ContentType = "application/json";
                    context.Response.Write(json);
                }
            }
        }

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}