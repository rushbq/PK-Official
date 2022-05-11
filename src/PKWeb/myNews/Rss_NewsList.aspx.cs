using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using ExtensionMethods;
using Resources;

public partial class myNews_Rss_NewsList : System.Web.UI.Page
{
    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //[XML] - 根目錄
            XElement DataNode = new XElement("rss", new XAttribute("version", "2.0"));

            //[XML] - 節點 (第1層)
            XElement xLv1 = new XElement("channel");

            //[XML] - 節點 (第2層)
            xLv1.Add(new XElement("title", Application["WebName"].ToString()));
            xLv1.Add(new XElement("description", resPublic.txt_Slogan));
            xLv1.Add(new XElement("link", Application["WebUrl"].ToString()));
            xLv1.Add(new XElement("language", fn_Language.PKWeb_Lang));

            //[XML] - 內容Item
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT myData.News_ID, myData.Group_ID, myData.News_Title, myData.News_Desc, myData.News_PubDate, myData.News_Pic, myData.Create_Time ");
                SBSql.AppendLine(" FROM News_Group GP ");
                SBSql.AppendLine("  INNER JOIN News_Area Area ON GP.Group_ID = Area.Group_ID ");
                SBSql.AppendLine("  INNER JOIN News myData ON GP.Group_ID = myData.Group_ID ");
                SBSql.AppendLine(" WHERE (Area.AreaCode = @AreaCode) ");
                SBSql.AppendLine("  AND (GETDATE() >= GP.StartTime) AND (GETDATE() <= GP.EndTime)");
                SBSql.AppendLine("  AND (GP.Display = 'Y') AND (LOWER(myData.LangCode) = LOWER(@LangCode))");
                SBSql.AppendLine(" ORDER BY GP.Sort ASC, myData.News_PubDate DESC ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("AreaCode", fn_Area.PKWeb_Area);
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int idx = 0; idx < DT.Rows.Count; idx++)
                    {
                        //[XML] - 節點 (第2層的尾端)
                        XElement xLv2 = new XElement("item");

                        //[XML] - 節點 (第3層)
                        xLv2.Add(new XElement("title", DT.Rows[idx]["News_Title"].ToString()));


                        string myDesc = DT.Rows[idx]["News_Desc"].ToString();
                        string myPic = DT.Rows[idx]["News_Pic"].ToString();
                        if (!string.IsNullOrEmpty(myPic))
                        {
                            string getPic = "<br/><img src=\"{0}\" width=\"165\" />".FormatThis(
                                fn_stringFormat.show_Pic("{0}News/{1}/{2}".FormatThis(Param_FileWebFolder, DT.Rows[idx]["Group_ID"].ToString(), myPic))
                             );

                            myDesc = myDesc + getPic;
                        }
                        xLv2.Add(new XElement("description", myDesc));
                        xLv2.Add(new XElement("link", "{0}News/View/{1}".FormatThis(
                                Application["WebUrl"].ToString()
                                , Cryptograph.MD5Encrypt(DT.Rows[idx]["News_ID"].ToString(), Application["DesKey"].ToString())
                            )));
                        xLv2.Add(new XElement("pubDate", Convert.ToDateTime(DateString(DT.Rows[idx]["Create_Time"].ToString()))));
                        
                        //新增節點
                        xLv1.Add(xLv2);
                    }
                }
            }

            //[XML] - 新增節點
            DataNode.Add(xLv1);

            //[XML] - 宣告XML
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), DataNode);

            //輸出XML檔
            Response.ContentType = "text/xml";
            xdoc.Save(Response.Output);
            Response.End();

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 轉換日期格式 (RFC822)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private string DateString(string str)
    {
        DateTime pubDate = DateTime.Parse(str);
        var value = pubDate.ToString("ddd',' dd MMM yyyy HH':'mm':'ss") +
            " " +
            pubDate.ToString("zzzz").Replace(":", "");
        return value;
    }

    /// <summary>
    /// [參數] - 檔案Web資料夾
    /// </summary>
    private string _Param_FileWebFolder;
    public string Param_FileWebFolder
    {
        get
        {
            return System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"];
        }
        set
        {
            this._Param_FileWebFolder = value;
        }
    }
}