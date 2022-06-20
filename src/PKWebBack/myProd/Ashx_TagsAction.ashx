<%@ WebHandler Language="C#" Class="Ashx_TagsAction" %>

using System;
using System.Web;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using ExtensionMethods;

/// <summary>
/// 取得Tags
/// </summary>
public class Ashx_TagsAction : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string ErrMsg;

        //System.Threading.Thread.Sleep(2000);

        try
        {
            context.Response.ContentType = "text/html";

            //[接收參數]
            string _model = context.Request["model"];
            string _id = context.Request["id"];
            string _name = context.Request["name"];
            string _lang = context.Request["lang"];
            string _type = context.Request["type"];

            /* Check */
            if (CheckReqNull(_type))
            {
                context.Response.Write("<h4>參數錯誤...請重新執行!</h4>");
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                string sql;

                //Check Type:CREATE / UPDATE / DELETE / READ
                switch (_type.ToUpper())
                {
                    case "CREATE":
                        sql = @"
                         IF NOT EXISTS (SELECT * FROM Prod_Tags WHERE (Tag_Name = @Tag_Name))
                            BEGIN
                             DECLARE @New_ID AS INT;
                             SET @New_ID = (SELECT ISNULL(MAX(Tag_ID), 0) + 1 FROM Prod_Tags);
                             INSERT INTO Prod_Tags(Tag_ID, Tag_Name) VALUES (@New_ID, @Tag_Name);
                            END";

                        cmd.CommandText = sql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Tag_Name", _name);
                        if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                        {
                            context.Response.Write("fail..." + ErrMsg);
                        }
                        else
                        {
                            context.Response.Write("success");
                        }
                        break;


                    case "UPDATE":
                        sql = @"
                          IF (SELECT COUNT(*) FROM Prod_Rel_Tags WHERE (Model_No = @Model_No) AND (Tag_ID = @Tag_ID) AND (LOWER(LangCode) = LOWER(@LangCode))) = 0
                            BEGIN
                             INSERT INTO Prod_Rel_Tags(Model_No, Tag_ID, LangCode)
                             VALUES (@Model_No, @Tag_ID, @LangCode)
                            END
                          ELSE
                            SELECT 'ok'";

                        cmd.CommandText = sql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Model_No", _model);
                        cmd.Parameters.AddWithValue("Tag_ID", _id);
                        cmd.Parameters.AddWithValue("LangCode", _lang.ToLower());
                        if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                        {
                            context.Response.Write("fail..." + ErrMsg);
                        }
                        else
                        {
                            context.Response.Write("success");
                        }
                        break;


                    case "DELETE":
                        sql = @"DELETE FROM Prod_Rel_Tags WHERE (Model_No = @Model_No) AND (Tag_ID = @Tag_ID) AND (LOWER(LangCode) = LOWER(@LangCode))";

                        cmd.CommandText = sql;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Model_No", _model);
                        cmd.Parameters.AddWithValue("Tag_ID", _id);
                        cmd.Parameters.AddWithValue("LangCode", _lang.ToLower());
                        if (!dbConn.ExecuteSql(cmd, out ErrMsg))
                        {
                            context.Response.Write("fail..." + ErrMsg);
                        }
                        else
                        {
                            context.Response.Write("success");
                        }
                        break;


                    default:
                        //READ
                        sql = @"SELECT Tags.Tag_ID, Tags.Tag_Name, Rel.LangCode
                                FROM Prod_Tags Tags
                                INNER JOIN Prod_Rel_Tags Rel ON Tags.Tag_ID = Rel.Tag_ID
                                WHERE (LOWER(Rel.LangCode) = LOWER(@LangCode)) AND (Rel.Model_No = @Model_No)";

                        //----- SQL 執行 -----
                        cmd.CommandText = sql;
                        cmd.Parameters.AddWithValue("Model_No", _model);
                        cmd.Parameters.AddWithValue("LangCode", _lang); //e.g:zh-tw

                        using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                        {
                            if (DT.Rows.Count == 0)
                            {
                                context.Response.Write("" + ErrMsg);
                            }
                            else
                            {
                                StringBuilder html = new StringBuilder();

                                for (int row = 0; row < DT.Rows.Count; row++)
                                {
                                    string tagID = DT.Rows[row]["Tag_ID"].ToString();
                                    string tag_Name = DT.Rows[row]["Tag_Name"].ToString();
                                    string langCode = DT.Rows[row]["LangCode"].ToString();
                                    string eleTag;
                                    switch (langCode.ToLower())
                                    {
                                        case "zh-cn":
                                            eleTag = "cnTags";
                                            break;
                                        case "en-us":
                                            eleTag = "enTags";
                                            break;
                                        default:
                                            eleTag = "twTags";
                                            break;
                                    }

                                    //填入DelTag script呼叫
                                    html.Append(string.Format(
                                        "<li><a href=\"javascript:DelTag('{0}', '{2}', '{3}');\" class=\"btn btn-default\">{1}&nbsp;<span class=\"glyphicon glyphicon-remove\"></span></a></li>"
                                            , tagID, tag_Name, langCode, eleTag
                                        ));
                                }

                                //output
                                context.Response.Write(html.ToString());
                            }

                        }


                        break;
                }

            }
        }
        catch (Exception ex)
        {
            context.Response.Write("<h4>發生錯誤!!若持續看到此訊息,請聯絡系統管理人員.</h4>" + ex.Message.ToString());
        }

    }


    private bool CheckReqNull(string reqVal)
    {
        return string.IsNullOrEmpty(reqVal);
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}