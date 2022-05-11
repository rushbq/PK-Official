using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using AdvData.Models;
using PKLib_Method.Methods;

namespace AdvData.Controllers
{
    /// <summary>
    /// 查詢類別
    /// </summary>
    public enum searchType : int
    {
        橫幅廣告 = 1,
        其他 = 2
    }


    public class AdvRepository
    {
        public string ErrMsg;


        /// <summary>
        /// 取得所有資料
        /// </summary>
        /// <param name="type">查詢類別</param>
        /// <param name="lang">語系</param>
        /// <returns></returns>
        public IQueryable<Adv> GetAdvs(searchType type, string lang)
        {
            //----- 宣告 -----
            List<Adv> advs = new List<Adv>();

            //----- 資料取得 -----
            using (DataTable DT = LookupRawData(type, lang))
            {
                //LinQ 查詢
                var query = DT.AsEnumerable();

                //資料迴圈
                foreach (var item in query)
                {
                    //加入項目
                    var adv = new Adv
                    {
                        ID = item.Field<int>("Adv_ID"),
                        Title = item.Field<string>("Adv_Title"),
                        Target = item.Field<string>("Adv_Target"),
                        Url = string.IsNullOrEmpty(item.Field<string>("Adv_Url")) ? "#!" : item.Field<string>("Adv_Url"),
                        ImgUrl = "{0}{1}Adv/{2}/{3}".FormatThis(
                            fn_Param.FileUrl,
                            fn_Param.FileFolder,
                            item.Field<int>("Group_ID"),
                            item.Field<string>("Adv_Pic"))
                    };

                    //將項目加入至集合
                    advs.Add(adv);

                }

            }

            //回傳集合
            return advs.AsQueryable();

        }


        /// <summary>
        /// 取得原始資料
        /// </summary>
        /// <param name="type">查詢類別</param>
        /// <returns></returns>
        private DataTable LookupRawData(searchType type, string lang)
        {
            //----- 宣告 -----
            StringBuilder sql = new StringBuilder();

            //----- 資料查詢 -----
            using (SqlCommand cmd = new SqlCommand())
            {
                //----- SQL 查詢語法 -----
                sql.AppendLine(" SELECT GP.Adv_Target, GP.Group_ID, myData.Adv_ID, myData.Adv_Title, myData.Adv_Pic, myData.Adv_Url");
                sql.AppendLine(" FROM Adv_Group GP WITH(NOLOCK)");
                sql.AppendLine("    INNER JOIN Adv myData WITH(NOLOCK) ON GP.Group_ID = myData.Group_ID");
                sql.AppendLine(" WHERE (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                sql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                sql.AppendLine("  AND (GP.Adv_Position = @type)");
                sql.AppendLine(" ORDER BY GP.Sort ASC, GP.EndTime DESC");


                //----- SQL 執行 -----
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("type", type);
                cmd.Parameters.AddWithValue("LangCode", lang);


                //----- 回傳資料 -----
                return dbConn.LookupDT(cmd, out ErrMsg);
            }
        }
    }
}
