using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionIO;
using ExtensionMethods;
using ExtensionUI;

public partial class Member_Edit : SecurityCheck
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth("510", out ErrMsg) == false)
                {
                    Response.Redirect("{0}401.aspx".FormatThis(Application["WebUrl"]));
                    return;
                }

                //** 設定程式編號(重要) **
                if (false == setProgIDs.setID(this.Master, "500", "510"))
                {
                    throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
                }

                //[參數判斷] - 判斷是否有資料編號
                if (!string.IsNullOrEmpty(Param_thisID))
                {
                    LookupData();
                }

            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --
    /// <summary>
    /// 資料顯示 - 基本資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            //[取得/檢查參數] - 系統編號
            if (string.IsNullOrEmpty(Param_thisID))
            {
                fn_Extensions.JsAlert("參數傳遞錯誤！", Page_SearchUrl);
                return;
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢

                SBSql.AppendLine(" SELECT Base.*, GC.Country_Name, RTRIM(Cust.MA002) CustName ");
                SBSql.AppendLine("  ,(SELECT COUNT(*) FROM Member_DealerData WHERE (Mem_ID = @DataID)) AS ApplyCnt");
                SBSql.AppendLine(" FROM Member_Data Base ");
                SBSql.AppendLine("   LEFT JOIN Geocode_CountryName GC ON Base.Country_Code = GC.Country_Code AND GC.LangCode = 'zh-tw'");
                SBSql.AppendLine("   LEFT JOIN PKSYS.dbo.Customer Cust ON Base.DealerID = Cust.MA001 AND DBS = DBC");
                SBSql.AppendLine(" WHERE (Base.Mem_ID = @DataID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        fn_Extensions.JsAlert("查無資料！", Page_SearchUrl);
                        return;
                    }
                    else
                    {
                        //[填入資料]
                        this.lt_Email.Text = DT.Rows[0]["Mem_Account"].ToString();
                        this.lb_MemType.Text = fn_Desc.MemberInfo.MemberType(DT.Rows[0]["Mem_Type"].ToString());
                        this.lt_Company.Text = DT.Rows[0]["Company"].ToString();
                        this.lt_LastName.Text = DT.Rows[0]["LastName"].ToString();
                        this.lt_FirstName.Text = DT.Rows[0]["FirstName"].ToString();
                        this.lt_Sex.Text = fn_Desc.PubAll.Sex(DT.Rows[0]["Sex"].ToString());
                        this.lt_Birthday.Text = DT.Rows[0]["Birthday"].ToString().ToDateString("yyyy-MM-dd");
                        this.lt_Country.Text = DT.Rows[0]["Country_Name"].ToString();
                        this.lt_Address.Text = DT.Rows[0]["Address"].ToString();
                        this.lt_Tel.Text = DT.Rows[0]["Tel"].ToString();
                        this.lt_Mobile.Text = DT.Rows[0]["Mobile"].ToString();
                        this.lt_RegDate.Text = DT.Rows[0]["Create_Time"].ToString().ToDateString("yyyy-MM-dd");
                        this.lt_IsWrite.Text = fn_Desc.PubAll.YesNo(DT.Rows[0]["IsWrite"].ToString());
                        this.lt_Social.Text = Show_SocialAcct();

                        #region >> 帳號狀態 <<

                        //判斷帳號狀態
                        string Display = DT.Rows[0]["Display"].ToString();
                        var queryVal = fn_CustomUI.Get_ActiveDisp(true)
                          .Where(el => el.ID.Equals(Display.ToUpper()))
                          .First();

                        //取得控制項, 顯示狀態
                        this.lb_Status.Text = queryVal.Name;
                        //判斷狀態, 改變顏色
                        switch (queryVal.ID.ToUpper())
                        {
                            case "Y":
                                this.lb_Status.CssClass = "label label-success";
                                this.ph_disable.Visible = true;
                                break;

                            case "N":
                                this.lb_Status.CssClass = "label label-default";
                                this.pl_Sign.Visible = true;
                                this.ph_enable.Visible = true;
                                break;

                            default:
                                this.lb_Status.CssClass = "label label-info";

                                break;
                        }

                        #endregion

                        #region >> 經銷商 <<

                        //-- 判斷是否申請經銷商 --
                        string DealerCheck = DT.Rows[0]["DealerCheck"].ToString();
                        //顯示設定區塊
                        this.pl_DealerApply.Visible = DealerCheck.ToUpper().Equals("N") || DealerCheck.ToUpper().Equals("R") ? false : true;
                        //顯示狀態
                        this.lb_DealerCheck.Text = "<i class=\"fa fa-file-text-o fa-fw\"></i>&nbsp;{0}".FormatThis(fn_Desc.MemberInfo.DealerStatus(DealerCheck));
                        this.lb_DealerStatus.Text = "我是經銷商,請關聯ERP代號";

                        //-- 判斷是否有填寫申請單 --
                        if (Convert.ToInt16(DT.Rows[0]["ApplyCnt"]) > 0)
                        {
                            this.lb_DealerStatus.Text = "我是新的經銷商";

                            //顯示查看申請單按鈕
                            this.pl_ViewApply.Visible = true;

                            //帶出表單資料(modal)
                            LookupData_ApplyForm();
                        }


                        //-- 判斷經銷商編號是否存在 --
                        string DealerID = DT.Rows[0]["DealerID"].ToString();
                        string DealerName = DT.Rows[0]["CustName"].ToString();
                        if (!string.IsNullOrEmpty(DealerID))
                        {
                            //代入客戶編號
                            this.hf_CustID.Value = DealerID;
                            this.hf_CustName.Value = DealerName;
                            //代入客戶名稱
                            this.tb_CustID.Text = "({0}) {1}".FormatThis(DealerID, DealerName);

                            //打開經銷商資料區塊
                            this.ph_DealerInfo.Visible = true;
                            //帶出經銷商資料
                            LookupData_Detail(DealerID);
                        }

                        #endregion

                    }
                }
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 資料查詢");
        }
    }

    /// <summary>
    /// 顯示經銷商資料 - ERP
    /// </summary>
    private void LookupData_Detail(string dealerID)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                /*
                 * MA001 = 客戶代號
                 * MA002 = 客戶簡稱
                 * MA003 = 客戶全稱
                 * MA009 = 客戶EMail
                 * MA014 = 交易幣別
                 * MA027 = 出貨地址
                 */

                //[SQL] - 清除參數設定
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Cust.MA001, Cust.MA002, Cust.MA003, Cust.MA009, Cust.MA014, Cust.MA027 ");
                SBSql.AppendLine(" FROM PKSYS.dbo.Customer Cust ");
                SBSql.AppendLine(" WHERE (Cust.DBS = Cust.DBC) AND (RTRIM(Cust.MA001) = @DealerID) ");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DealerID", dealerID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        this.lt_MA001.Text = DT.Rows[0]["MA001"].ToString();
                        this.lt_MA002.Text = DT.Rows[0]["MA002"].ToString();
                        this.lt_MA003.Text = DT.Rows[0]["MA003"].ToString();
                        this.lt_MA009.Text = DT.Rows[0]["MA009"].ToString();
                        this.lt_MA014.Text = DT.Rows[0]["MA014"].ToString();
                        this.lt_MA027.Text = DT.Rows[0]["MA027"].ToString();
                    }
                }
            }
        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 經銷商！", "");
        }
    }


    /// <summary>
    /// 資料顯示 - 經銷商申請單
    /// </summary>
    private void LookupData_ApplyForm()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Base.* ");
                SBSql.AppendLine(" FROM Member_DealerData Base ");
                SBSql.AppendLine(" WHERE (Base.Mem_ID = @DataID) ");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    this.myForm.DataSource = DT.DefaultView;
                    this.myForm.DataBind();
                }

            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 經銷商資料");
        }
    }

    protected void myForm_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            /** 圖片顯示 **/
            //取得資料
            string GetLang = DataBinder.Eval(e.Item.DataItem, "LangCode").ToString();

            //取得控制項
            PlaceHolder ph_Eng = (PlaceHolder)e.Item.FindControl("ph_FieldOfEnglish");
            PlaceHolder ph_Chi = (PlaceHolder)e.Item.FindControl("ph_FieldOfChinese");

            switch (GetLang.ToLower())
            {
                case "en-us":
                    ph_Eng.Visible = true;
                    ph_Chi.Visible = false;

                    break;

                default:
                    ph_Eng.Visible = false;
                    ph_Chi.Visible = true;

                    break;
            }

        }
    }


    /// <summary>
    /// 回傳已勾選項目
    /// </summary>
    /// <param name="inputValues"></param>
    /// <param name="inputType"></param>
    /// <returns></returns>
    public string Show_cbItems(string inputValues, string inputType, string inputLang)
    {
        try
        {
            if (string.IsNullOrEmpty(inputValues))
            {
                return "";
            }

            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder Html = new StringBuilder();

                //取得參數值並轉換為List
                List<string> myValues = new List<string>();
                string[] strAry = Regex.Split(inputValues, @"\,{1}");
                foreach (string item in strAry)
                {
                    myValues.Add(item.ToString());
                }

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Param_Text ");
                SBSql.AppendLine(" FROM Param_DealerData ");
                SBSql.AppendLine(" WHERE (LangCode = @inputLang) AND (Param_Kind = @inputType)");
                //SQL參數串
                if (myValues.Count > 0)
                {
                    SBSql.Append(" AND (Param_ID IN ({0})) ".FormatThis(
                            fn_Extensions.GetSQLParam(myValues, "myVals")
                        ));
                }
                SBSql.AppendLine(" ORDER BY Sort");

                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("inputType", inputType);
                cmd.Parameters.AddWithValue("inputLang", inputLang);
                //SQL參數串
                for (int row = 0; row < myValues.Count; row++)
                {
                    cmd.Parameters.AddWithValue("myVals{0}".FormatThis(row), myValues[row].ToString());
                }

                //取得資料集
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        Html.AppendLine("<ul class=\"list-unstyled\">");

                        for (int row = 0; row < DT.Rows.Count; row++)
                        {
                            Html.AppendLine("<li><i class=\"fa fa-check fa-fw\"></i>&nbsp;{0}</li>".FormatThis(
                                    DT.Rows[row]["Param_Text"].ToString()
                                ));

                        }

                        Html.AppendLine("</ul>");
                    }
                }

                return Html.ToString();
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 經銷商資料勾選項目");
        }
    }

    /// <summary>
    /// 判斷是否有社群帳號登入, 回傳對應的圖示
    /// </summary>
    /// <returns></returns>
    public string Show_SocialAcct()
    {
        try
        {
            //[取得資料] - 取得資料
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();
                StringBuilder Html = new StringBuilder();

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料查詢
                SBSql.AppendLine(" SELECT Platform ");
                SBSql.AppendLine(" FROM Member_SocialToken ");
                SBSql.AppendLine(" WHERE (Mem_ID = @DataID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);

                //取得資料集
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        Html.AppendLine("<img src=\"{0}images/icon/{1}.png\" style=\"width:25px;\" width=\"25\" />".FormatThis(
                                Application["WebUrl"]
                                , DT.Rows[row]["Platform"].ToString().ToLower()
                            ));
                    }
                }

                return Html.ToString();
            }
        }
        catch (Exception)
        {
            throw new Exception("系統發生錯誤 - 經銷商資料勾選項目");
        }
    }
    #endregion -- 資料顯示 End --


    #region -- 資料編輯 Start --
    /// <summary>
    /// 啟用帳號
    /// </summary>
    protected void lbtn_Enable_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Member_Data SET Display = 'Y' WHERE (Mem_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("啟用帳號失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 啟用帳號", "");
            return;
        }
    }

    /// <summary>
    /// 停用帳號
    /// </summary>
    protected void lbtn_Disable_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Member_Data SET Display = 'N' WHERE (Mem_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("停用帳號失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_CurrentUrl);
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 停用帳號", "");
            return;
        }
    }

    /// <summary>
    /// 設定經銷商關聯
    /// </summary>
    protected void btn_SetDealer_Click(object sender, EventArgs e)
    {
        try
        {
            string _custID = this.hf_CustID.Value;
            if (string.IsNullOrWhiteSpace(_custID))
            {
                fn_Extensions.JsAlert("客戶編號空白！", Page_CurrentUrl);
                return;
            }

            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Member_Data SET DealerCheck = 'Y', Mem_Type = 1, DealerID = @DealerID, Company = @Company");
                SBSql.AppendLine(" WHERE (Mem_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                cmd.Parameters.AddWithValue("DealerID", _custID);
                cmd.Parameters.AddWithValue("Company", this.hf_CustName.Value);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("設定失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //導向本頁
                    fn_Extensions.JsAlert("設定完成，請通知經銷商重新登入", Page_SearchUrl);
                    return;
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 停用帳號", "");
            return;
        }
    }

    /// <summary>
    /// 駁回經銷商申請
    /// </summary>
    protected void lbtn_RejectDealer_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                //[SQL] - 資料更新
                SBSql.AppendLine(" UPDATE Member_Data SET DealerCheck = 'R', Mem_Type = 0, DealerID = '' WHERE (Mem_ID = @DataID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("DataID", Param_thisID);
                if (dbConn.ExecuteSql(cmd, out ErrMsg) == false)
                {
                    fn_Extensions.JsAlert("駁回經銷商申請失敗！", Page_CurrentUrl);
                    return;
                }
                else
                {
                    //導向本頁
                    Response.Redirect(Page_SearchUrl);
                }
            }

        }
        catch (Exception)
        {
            fn_Extensions.JsAlert("系統發生錯誤 - 駁回經銷商申請", "");
            return;
        }
    }
    #endregion -- 資料編輯 End --

    #region -- 參數設定 --
    /// <summary>
    /// 取得傳遞參數 - 資料編號
    /// </summary>
    private string _Param_thisID;
    public string Param_thisID
    {
        get
        {
            String DataID = Page.RouteData.Values["DataID"].ToString();

            return DataID.Equals("New") ? "" : Cryptograph.MD5Decrypt(DataID, Application["DesKey"].ToString());
        }
        set
        {
            this._Param_thisID = value;
        }
    }

    /// <summary>
    /// 設定參數 - 本頁Url
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}Member/Edit/{1}/".FormatThis(
                Application["WebUrl"]
                , HttpUtility.UrlEncode(Cryptograph.MD5Encrypt(Param_thisID, Application["DesKey"].ToString()))
            );
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }

    /// <summary>
    /// 設定參數 - 列表頁Url
    /// </summary>
    private string _Page_SearchUrl;
    public string Page_SearchUrl
    {
        get
        {
            String Url;
            if (Session["BackListUrl"] == null)
            {
                Url = "{0}Member/Search/".FormatThis(Application["WebUrl"]);
            }
            else
            {
                Url = Session["BackListUrl"].ToString();
            }

            return Url;
        }
        set
        {
            this._Page_SearchUrl = value;
        }
    }

    #endregion

}