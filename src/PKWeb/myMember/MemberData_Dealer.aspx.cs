using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtensionMethods;
using ExtensionUI;
using LogRecord;

public partial class MemberData_Dealer : SecurityCheck
{

    public string ErrMsg;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //** 次標題 **
                this.Page.Title = Resources.resPublic.title_會員資料;

                //填入語系文字
                this.btn_Submit.Text = this.GetLocalResourceObject("txt_傳送").ToString();

                //判斷區域, 顯示欄位
                switch (fn_Area.PKWeb_Area)
                {
                    case "1":
                        //Global
                        this.ph_FieldOfEnglish.Visible = true;

                        break;

                    default:
                        this.ph_FieldOfEnglish.Visible = false;

                        break;
                }


                #region >> 英文版表單 <<

                //[取得參數] - BusinessType
                if (Get_CbxItemList(this.cb_BusinessType, null, "BusinessType", out ErrMsg) == false)
                {
                    this.ph_BusinessType.Visible = false;
                }

                //[取得參數] - SupplierLocation
                if (Get_CbxItemList(this.cb_SupplierLocation, null, "SupplierLocation", out ErrMsg) == false)
                {
                    this.ph_SupplierLocation.Visible = false;
                }

                //[取得參數] - cb_BusinessFieldA
                if (Get_CbxItemList(this.cb_BusinessFieldA, null, "BusinessFieldA", out ErrMsg) == false)
                {
                    this.ph_BusinessField.Visible = false;
                }

                //[取得參數] - cb_BusinessFieldB
                if (Get_CbxItemList(this.cb_BusinessFieldB, null, "BusinessFieldB", out ErrMsg) == false)
                {
                    this.ph_BusinessField.Visible = false;
                }

                #endregion

                #region >> 中文版表單 <<
                //[取得參數] - CompType
                if (Get_CbxItemList(this.cb_CompType, null, "CompanyType", out ErrMsg) == false)
                {
                    this.ph_CompType.Visible = false;
                }
                //[取得參數] - BussinessItem
                if (Get_CbxItemList(this.cb_BusinessItem, null, "BusinessItem", out ErrMsg) == false)
                {
                    this.ph_BusinessItem.Visible = false;
                }
                //[取得參數] - SalesTarget
                if (Get_CbxItemList(this.cb_SalesTarget, null, "SalesTarget", out ErrMsg) == false)
                {
                    this.ph_SalesTarget.Visible = false;
                }

                #endregion

                //讀取資料
                LookupData();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 讀取資料
    /// </summary>
    private void LookupData()
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //判斷是否已新增過
                SBSql.AppendLine(" SELECT Mem_ID ");
                SBSql.AppendLine(" FROM Member_DealerData ");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //已送過申請, 直接跳到會員頁
                        fn_Extensions.JsAlert("Your application has been submitted.", Application["WebUrl"] + "MemberData");
                        return;
                    }
                }

            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    /// <summary>
    /// 新增資料
    /// </summary>
    protected void btn_Submit_Click(object sender, EventArgs e)
    {
        try
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                //宣告
                StringBuilder SBSql = new StringBuilder();

                //判斷是否已新增過
                SBSql.AppendLine(" SELECT Mem_ID ");
                SBSql.AppendLine(" FROM Member_DealerData ");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count > 0)
                    {
                        //已送過申請, 直接跳到成功頁
                        Response.Redirect("{0}Notification/11".FormatThis(Application["WebUrl"].ToString()));
                        return;
                    }
                }

                //清除參數
                cmd.Parameters.Clear();

                //[SQL] - 資料處理, 新增經銷商資料表單
                SBSql.AppendLine(" INSERT INTO Member_DealerData( ");
                SBSql.AppendLine(" Mem_ID, CompName, CompCaptital, CEO, Tel, Fax, WebSite, Address, City, State, ZIP, Country ");
                SBSql.AppendLine(" , EmpNum1, EmpNum2, EmpNum3, EmpNum4 ");
                SBSql.AppendLine(" , CP_Name1, CP_Dept1, CP_Line1, CP_Email1 ");
                SBSql.AppendLine(" , CP_Name2, CP_Dept2, CP_Line2, CP_Email2 ");
                SBSql.AppendLine(" , BusinessType, BusinessType_Other, BranchOffice, Annual, ProductYear ");
                SBSql.AppendLine(" , AnnualProduct, AgentBrands, MainMarket, MajorProduct, SupplierLocation, SupplierLocation_Other ");
                SBSql.AppendLine(" , BusniessField_Per1, BusniessField_Opt1, BusniessField_Per2, BusniessField_Opt2, BusniessField_Per3 ");
                SBSql.AppendLine(" , CompType, BusinessItem, SalesTarget, LangCode");
                SBSql.AppendLine(" )VALUES( ");
                SBSql.AppendLine(" @Mem_ID, @CompName, @CompCaptital, @CEO, @Tel, @Fax, @WebSite, @Address, @City, @State, @ZIP, @Country ");
                SBSql.AppendLine(" , @EmpNum1, @EmpNum2, @EmpNum3, @EmpNum4 ");
                SBSql.AppendLine(" , @CP_Name1, @CP_Dept1, @CP_Line1, @CP_Email1 ");
                SBSql.AppendLine(" , @CP_Name2, @CP_Dept2, @CP_Line2, @CP_Email2 ");
                SBSql.AppendLine(" , @BusinessType, @BusinessType_Other, @BranchOffice, @Annual, @ProductYear ");
                SBSql.AppendLine(" , @AnnualProduct, @AgentBrands, @MainMarket, @MajorProduct, @SupplierLocation, @SupplierLocation_Other ");
                SBSql.AppendLine(" , @BusniessField_Per1, @BusniessField_Opt1, @BusniessField_Per2, @BusniessField_Opt2, @BusniessField_Per3 ");
                SBSql.AppendLine(" , @CompType, @BusinessItem, @SalesTarget, LOWER(@LangCode)");
                SBSql.AppendLine(");");

                //[SQL] - 資料處理, 修改會員狀態
                SBSql.AppendLine(" UPDATE Member_Data SET ");
                SBSql.AppendLine(" DealerCheck = 'S', Update_Time = GETDATE()");
                SBSql.AppendLine(" WHERE (Mem_ID = @Mem_ID)");

                //[SQL] - Command
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.AddWithValue("Mem_ID", fn_Param.MemberID);
                cmd.Parameters.AddWithValue("CompName", this.CompName.Text);
                cmd.Parameters.AddWithValue("CompCaptital", this.CompCaptital.Text);
                cmd.Parameters.AddWithValue("CEO", this.CEO.Text);
                cmd.Parameters.AddWithValue("Tel", this.Tel.Text);
                cmd.Parameters.AddWithValue("Fax", this.Fax.Text);
                cmd.Parameters.AddWithValue("WebSite", this.WebSite.Text);
                cmd.Parameters.AddWithValue("Address", this.Address.Text);
                cmd.Parameters.AddWithValue("City", this.City.Text);
                cmd.Parameters.AddWithValue("State", this.State.Text);
                cmd.Parameters.AddWithValue("ZIP", this.ZIP.Text);
                cmd.Parameters.AddWithValue("Country", this.Country.Text);
                cmd.Parameters.AddWithValue("EmpNum1", this.EmpNum1.Text);
                cmd.Parameters.AddWithValue("EmpNum2", this.EmpNum2.Text);
                cmd.Parameters.AddWithValue("EmpNum3", this.EmpNum3.Text);
                cmd.Parameters.AddWithValue("EmpNum4", this.EmpNum4.Text);
                cmd.Parameters.AddWithValue("CP_Name1", this.CP_Name1.Text);
                cmd.Parameters.AddWithValue("CP_Dept1", this.CP_Dept1.Text);
                cmd.Parameters.AddWithValue("CP_Line1", this.CP_Line1.Text);
                cmd.Parameters.AddWithValue("CP_Email1", this.CP_Email1.Text);
                cmd.Parameters.AddWithValue("CP_Name2", this.CP_Name2.Text);
                cmd.Parameters.AddWithValue("CP_Dept2", this.CP_Dept2.Text);
                cmd.Parameters.AddWithValue("CP_Line2", this.CP_Line2.Text);
                cmd.Parameters.AddWithValue("CP_Email2", this.CP_Email2.Text);
                cmd.Parameters.AddWithValue("BusinessType", Get_CbxItemValues(this.cb_BusinessType));
                cmd.Parameters.AddWithValue("BusinessType_Other", this.BusinessType_Other.Text);
                cmd.Parameters.AddWithValue("BranchOffice", this.BranchOffice.Text);
                cmd.Parameters.AddWithValue("Annual", this.Annual.Text);
                cmd.Parameters.AddWithValue("ProductYear", this.ProductYear.Text);
                cmd.Parameters.AddWithValue("AnnualProduct", this.AnnualProduct.Text);
                cmd.Parameters.AddWithValue("AgentBrands", this.AgentBrands.Text);
                cmd.Parameters.AddWithValue("MainMarket", this.MainMarket.Text);
                cmd.Parameters.AddWithValue("MajorProduct", this.MajorProduct.Text);
                cmd.Parameters.AddWithValue("SupplierLocation", Get_CbxItemValues(this.cb_SupplierLocation));
                cmd.Parameters.AddWithValue("SupplierLocation_Other", this.SupplierLocation_Other.Text);
                cmd.Parameters.AddWithValue("BusniessField_Per1", this.BusniessField_Per1.Text);
                cmd.Parameters.AddWithValue("BusniessField_Opt1", Get_CbxItemValues(this.cb_BusinessFieldA));
                cmd.Parameters.AddWithValue("BusniessField_Per2", this.BusniessField_Per2.Text);
                cmd.Parameters.AddWithValue("BusniessField_Opt2", Get_CbxItemValues(this.cb_BusinessFieldB));
                cmd.Parameters.AddWithValue("BusniessField_Per3", this.BusniessField_Per3.Text);
                cmd.Parameters.AddWithValue("CompType", Get_CbxItemValues(this.cb_CompType));
                cmd.Parameters.AddWithValue("BusinessItem", Get_CbxItemValues(this.cb_BusinessItem));
                cmd.Parameters.AddWithValue("SalesTarget", Get_CbxItemValues(this.cb_SalesTarget));
                cmd.Parameters.AddWithValue("LangCode", fn_Language.PKWeb_Lang);
                if (false == dbConn.ExecuteSql(cmd, out ErrMsg))
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "申請經銷商-送出審核", "1003", "送出審核單失敗,原因:{0}".FormatThis(ErrMsg));
                    #endregion

                    Response.Redirect("{0}Notification/9".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }
                else
                {
                    #region -- Log記錄 --
                    fn_Log.writeLog(fn_Param.MemberID, "申請經銷商-送出審核", "1003", "已送出審核單");
                    #endregion

                    Response.Redirect("{0}Notification/11".FormatThis(Application["WebUrl"].ToString()));
                    return;
                }

            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 取得Checkbox設定值
    /// </summary>
    /// <param name="setMenu"></param>
    /// <param name="inputValues"></param>
    /// <param name="inputType"></param>
    /// <param name="ErrMsg"></param>
    /// <returns></returns>
    public static bool Get_CbxItemList(CheckBoxList setMenu, string[] inputValues, string inputType, out string ErrMsg)
    {
        //清除參數
        setMenu.Items.Clear();

        try
        {
            //[取得資料]
            using (SqlCommand cmd = new SqlCommand())
            {
                StringBuilder SBSql = new StringBuilder();

                SBSql.AppendLine(" SELECT Param_ID AS ID, Param_Text AS Label ");
                SBSql.AppendLine(" FROM Param_DealerData Cls WITH (NOLOCK) ");
                SBSql.AppendLine(" WHERE (Param_Kind = @type) AND (LOWER(LangCode) = LOWER(@lang)) AND (AreaCode = @area)");
                SBSql.AppendLine(" ORDER BY Sort");
                cmd.CommandText = SBSql.ToString();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("type", inputType);
                cmd.Parameters.AddWithValue("lang", fn_Language.PKWeb_Lang);
                cmd.Parameters.AddWithValue("area", fn_Area.PKWeb_Area);
                using (DataTable DT = dbConn.LookupDT(cmd, out ErrMsg))
                {
                    if (DT.Rows.Count == 0)
                    {
                        return false;
                    }

                    //新增選單項目
                    for (int row = 0; row < DT.Rows.Count; row++)
                    {
                        setMenu.Items.Add(new ListItem("&nbsp;" + DT.Rows[row]["Label"].ToString() + "&nbsp;&nbsp;&nbsp;"
                                     , DT.Rows[row]["ID"].ToString()));
                    }
                    //判斷是否有已選取的項目
                    if (inputValues != null)
                    {
                        foreach (string item in inputValues)
                        {
                            for (int col = 0; col < setMenu.Items.Count; col++)
                            {
                                if (setMenu.Items[col].Value.Equals(item.ToString()))
                                {
                                    setMenu.Items[col].Selected = true;
                                }
                            }
                        }
                    }

                }
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message.ToString();
            return false;
        }
    }

    /// <summary>
    /// 回傳Checkbox已勾選值
    /// </summary>
    /// <param name="cbItem">checkbox object</param>
    /// <returns></returns>
    string Get_CbxItemValues(CheckBoxList cbItem)
    {
        var selectedValues = from ListItem item in cbItem.Items where item.Selected select item.Value;
        var delimitedString = "";
        if (selectedValues.Count() > 0)
        {
            delimitedString = selectedValues.Aggregate((x, y) => x + "," + y);
        }
        return delimitedString;
    }

}