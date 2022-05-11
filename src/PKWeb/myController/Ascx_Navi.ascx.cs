using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ExtensionMethods;
using LogRecord;

public partial class myController_Ascx_Navi : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //[取得資料] - 取得資料
                using (SqlCommand cmd = new SqlCommand())
                {
                    //宣告
                    StringBuilder SBSql = new StringBuilder();
                    StringBuilder Html = new StringBuilder();
                    string ErrMsg;

                    //清除參數
                    cmd.Parameters.Clear();

                    //[SQL] - 資料查詢
                    SBSql.AppendLine(" SELECT Base.Menu_Name AS Nav_Current, Base.Menu_Uri ");
                    SBSql.AppendLine(" , (SELECT TOP 1 Menu_Name FROM Navi_Menu WHERE (Menu_ID = Base.Parent_ID) AND (LOWER(LangCode) = LOWER(Base.LangCode))) AS Nav_Up1 ");
                    SBSql.AppendLine(" , (SELECT TOP 1 Menu_Name FROM Navi_Menu WHERE (LOWER(LangCode) = LOWER(Base.LangCode)) AND (Menu_ID = (SELECT TOP 1 Parent_ID FROM Navi_Menu WHERE (Menu_ID = Base.Parent_ID) AND (LOWER(LangCode) = LOWER(Base.LangCode))))) AS Nav_Up2 ");
                    SBSql.AppendLine(" FROM Navi_Menu Base ");
                    SBSql.AppendLine(" WHERE (Base.Menu_ID = @CurrID) AND (LOWER(Base.LangCode) = LOWER(@LangCode)) AND (Base.IsRoot = 'N') ");

                    cmd.CommandText = SBSql.ToString();
                    cmd.Parameters.AddWithValue("CurrID", Param_CurrID);
                    cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                    using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                    {
                        if (DT.Rows.Count > 0)
                        {
                            string Nav_Up2 = DT.Rows[0]["Nav_Up2"].ToString();
                            string Nav_Up1 = DT.Rows[0]["Nav_Up1"].ToString();
                            string Nav_Curr = DT.Rows[0]["Nav_Current"].ToString();
                            string Nav_Uri = DT.Rows[0]["Menu_Uri"].ToString();

                            if (!string.IsNullOrWhiteSpace(Nav_Up1))
                            {
                                //Show:breadcrumb
                                Html.Append("<div><ol class=\"breadcrumb\">");
                                if (!string.IsNullOrEmpty(Nav_Up2)) { Html.Append("<li>{0}</li>".FormatThis(Nav_Up2)); }
                                if (!string.IsNullOrEmpty(Nav_Up1)) { Html.Append("<li>{0}</li>".FormatThis(Nav_Up1)); }
                                if (!string.IsNullOrEmpty(Nav_Curr)) { Html.Append("<li><a href=\"{1}\">{0}</a></li>".FormatThis(Nav_Curr, Application["WebUrl"] + Nav_Uri)); }

                                //Output
                                if (!string.IsNullOrEmpty(Param_CustomName))
                                {
                                    Html.Append("<li>{0}</li>".FormatThis(Param_CustomName));
                                }

                                Html.Append("</ol></div>");

                                this.lt_Navi.Text = Html.ToString();
                            }
                            else
                            {
                                //Show:page-header
                                Html.Append("<div class=\"page-header\"><h3>{0}&nbsp;<small>{1}</small></h3></div>".FormatThis(
                                    Nav_Curr
                                    , (string.IsNullOrEmpty(Param_CustomName)) ? "" : Param_CustomName));

                                this.lt_Navi.Text = Html.ToString();
                            }



                            //** 寫入Log **
                            if (!string.IsNullOrEmpty(fn_Param.MemberID))
                            {
                                fn_Log.writeLog(
                                    fn_Param.MemberID
                                    , "逛街"
                                    , "1001"
                                    , "{0} / {1} / {2}".FormatThis(Nav_Up2, Nav_Up1, Nav_Curr)
                                    );
                            }
                        }

                    }

                }
            }
            catch (Exception)
            {
                throw new Exception("系統發生錯誤 - Navi");
            }
        }
    }

    #region -- 參數設定 --
    /// <summary>
    /// [參數] - 目前編號
    /// </summary>
    private string _Param_CurrID;
    public string Param_CurrID
    {
        get;
        set;
    }

    /// <summary>
    /// [參數] - 自訂路徑名
    /// </summary>
    private string _Param_CustomName;
    public string Param_CustomName
    {
        get;
        set;
    }
    #endregion
}