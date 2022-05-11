using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using eOrder.Models;
using LinqToExcel;
using PKLib_Method.Methods;

public partial class myOrder_Step1_1 : SecurityCheckDealer
{
    public string ErrMsg;

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //檢查ID是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    Response.Redirect(Application["WebUrl"] + "EO/List");
                    return;
                }

                //取得資料
                LookupData();


                //set resources
                this.lbtn_Prev.OnClientClick = "return confirm('{0}')".FormatThis(this.GetLocalResourceObject("btn_Confirm").ToString());
            }


        }
        catch (Exception ex)
        {
            this.ph_errMessage.Visible = true;
            this.lt_ErrMsg.Text = "<p>{0}</p>".FormatThis(ex.Message.ToString());
            return;
            //throw;
        }

    }

    #region -- 資料讀取 --

    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);
        search.Add((int)mySearch.CustID, fn_Param.Get_CustID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1)
            .Select(fld => new
            {
                TraceID = fld.TraceID,
                CustID = fld.CustID,
                CustName = fld.CustName,
                FileName = fld.Upload_File,
                DataType = fld.Data_Type,
                DataTypeName = fld.Data_TypeName,
                DBName = fld.DB_Name,
                Staus = fld.Status

            }).FirstOrDefault();


        //檢查訂單是否已完成
        if (query.Staus.Equals(4))
        {
            Response.Redirect(Application["WebUrl"] + "EO/Step1");
            return;
        }


        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;
        string CustID = query.CustID;
        string FileName = query.FileName;
        string dataType = query.DataType.ToString();

        //取得Excel完整路徑
        string filePath = @"{0}{1}{2}\{3}".FormatThis(
            System.Web.Configuration.WebConfigurationManager.AppSettings["FTP_DiskUrl"]
            , UploadFolder
            , TraceID
            , FileName);

        this.lt_TraceID.Text = TraceID;
        this.hf_FullFileName.Value = filePath;
        this.hf_Type.Value = dataType;
        this.hf_CustID.Value = CustID;
        this.hf_TraceID.Value = TraceID;
        this.hf_DBName.Value = query.DBName;

        query = null;

        //----- [元件][LinqToExcel] - 取得工作表 -----
        Set_SheetMenu(filePath);

    }


    /// <summary>
    /// 產生工作表選單
    /// </summary>
    /// <param name="filePath"></param>
    private void Set_SheetMenu(string filePath)
    {
        //查詢Excel
        var excelFile = new ExcelQueryFactory(filePath);

        //取得Excel 頁籤
        var data = excelFile.GetWorksheetNames();

        this.ddl_Sheet.Items.Clear();
        this.ddl_Sheet.Items.Add(new ListItem("選擇要匯入的工作表", ""));

        foreach (var item in data)
        {
            this.ddl_Sheet.Items.Add(new ListItem(item.ToString(), item.ToString()));
        }


    }
    #endregion


    #region -- 按鈕事件 --

    /// <summary>
    /// 重新上傳:刪除資料及檔案
    /// </summary>
    protected void lbtn_Prev_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();

        //----- 方法:刪除資料 -----
        if (false == _data.Delete(Req_DataID))
        {
            this.ph_errMessage.Visible = true;
            return;
        }
        else
        {
            //刪除整個Folder檔案
            _ftp.FTP_DelFolder(UploadFolder + this.lt_TraceID.Text);

            //導向至首頁
            Response.Redirect("{0}EO/Step1".FormatThis(Application["WebUrl"]));
        }

    }

    /// <summary>
    /// 下一步
    /// </summary>
    protected void btn_Next_Click(object sender, EventArgs e)
    {
        try
        {
            //判斷是否有必填欄位
            if (this.ddl_Sheet.SelectedIndex == 0)
            {
                string chkField = "{0}:{1}".FormatThis(Resources.resPublic.tip_Require, this.GetLocalResourceObject("txt_選擇工作表").ToString());
                CustomExtension.AlertMsg(chkField, "");
                return;
            }


            #region -- 存入暫存Table --
            //----- 宣告:資料參數 -----
            eOrderingRepository _data = new eOrderingRepository();


            //[Excel] - 取得參數
            var filePath = this.hf_FullFileName.Value;
            string sheetName = this.ddl_Sheet.SelectedValue;
            string dataType = this.hf_Type.Value;
            string traceID = this.hf_TraceID.Value;
            string custID = this.hf_CustID.Value;
            string dbName = this.hf_DBName.Value;

            //[Excel] - 取得Excel資料欄位
            var query_Xls = _data.GetExcel_DT(filePath, sheetName, traceID);


            //建立基本資料參數, 用途:資料篩選條件
            var baseData = new ImportData
            {
                Data_ID = new Guid(Req_DataID),
                TraceID = traceID,
                CustID = custID,
                Sheet_Name = sheetName,
                DB_Name = dbName,
                Data_Type = Convert.ToDecimal(dataType)
            };


            //建立暫存Table
            if (!_data.Create_Temp(baseData, query_Xls, out ErrMsg))
            {
                //[Log]
                string Msg = "暫存Table建立失敗(Step1-1)...\n" + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                //Show Error
                //Response.Write(ErrMsg);
                this.ph_errMessage.Visible = true;
                return;
            }


            //檢查客戶品號及取得對應PK品號
            if (!_data.Check_Temp1(baseData, out ErrMsg))
            {
                string Msg = "暫存Table更新失敗, 品號(Step1-1)...\n" + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                this.lt_ErrMsg.Text = ErrMsg;
                this.ph_errMessage.Visible = true;
                return;
            }

            //更新MOD/MinQty/UnitPrice
            if (!_data.Check_Temp2(baseData, out ErrMsg))
            {
                string Msg = "暫存Table更新失敗, MOQ,minQty(Step1-1)...\n" + ErrMsg;
                _data.Create_Log(baseData, Msg, out ErrMsg);

                this.ph_errMessage.Visible = true;
                return;
            }

            //OK
            Response.Redirect(Application["WebUrl"] + "EO/Step2/" + baseData.Data_ID);

            #endregion
        }
        catch (Exception ex)
        {
            eOrderingRepository _data = new eOrderingRepository();
            var baseData = new ImportData
            {
                Data_ID = new Guid(Req_DataID),
                TraceID = this.hf_TraceID.Value
            };

            string Msg = "請檢查Excel格式...\n" + ex.Message.ToString();
            _data.Create_Log(baseData, Msg, out ErrMsg);

            this.ph_errMessage.Visible = true;
        }


    }

    ///// <summary>
    ///// 未出貨訂單(1), 退貨單(2) 流程
    ///// </summary>
    ///// <param name="baseData">基本資料參數</param>
    //private void myFlow1(ImportData baseData)
    //{
    //    //----- 宣告:資料參數 -----
    //    eOrderingRepository _data = new eOrderingRepository();


    //    #region -- 資料Check.1:品號 --

    //    if (!_data.Check_Step1(baseData, out ErrMsg))
    //    {
    //        //[Log]
    //        string Msg = "Check.1:品號。\nERP品號檢查失敗, DT未建立(Step2)...\n" + ErrMsg;
    //        _data.Create_Log(baseData, Msg, out ErrMsg);

    //        //Show Error
    //        this.ph_Message.Visible = true;
    //        return;
    //    }
    //    else
    //    {
    //        this.ph_Message.Visible = false;
    //    }

    //    #endregion


    //    #region -- 資料Check.2:訂單編號 --

    //    if (!_data.Check_Step2(Req_DataID, out ErrMsg))
    //    {
    //        //[Log]
    //        string Msg = "Check.2:訂單編號。\n訂單編號檢查失敗, 狀態碼未更新(Step2)...\n" + ErrMsg;
    //        _data.Create_Log(baseData, Msg, out ErrMsg);

    //        //Show Error
    //        this.ph_Message.Visible = true;
    //        return;
    //    }
    //    else
    //    {
    //        this.ph_Message.Visible = false;
    //    }

    //    #endregion


    //    #region -- 資料Check.3:價格 --

    //    if (!_data.Check_Step3(Req_DataID, out ErrMsg))
    //    {
    //        //[Log]
    //        string Msg = "Check.3:價格。\nERP價格檢查失敗, 狀態碼及價格未更新(Step2)...\n" + ErrMsg;
    //        _data.Create_Log(baseData, Msg, out ErrMsg);

    //        //Show Error
    //        //Response.Write(ErrMsg);
    //        this.ph_Message.Visible = true;
    //        return;
    //    }
    //    else
    //    {
    //        this.ph_Message.Visible = false;
    //    }

    //    #endregion


    //    //導至下一步
    //    Response.Redirect("{0}mySZBBC/ImportStep3.aspx?dataID={1}".FormatThis(
    //        Application["WebUrl"]
    //        , Req_DataID));
    //}



    /// <summary>
    /// 選擇工作表, 產生預覽資料 - onChange
    /// </summary>
    protected void ddl_Sheet_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (this.ddl_Sheet.SelectedIndex > 0 && !string.IsNullOrEmpty(this.hf_FullFileName.Value))
            {
                //宣告
                StringBuilder html = new StringBuilder();
                var filePath = this.hf_FullFileName.Value;
                string sheetName = this.ddl_Sheet.SelectedValue;

                //取得資料
                eOrderingRepository _data = new eOrderingRepository();

                html = _data.GetExcel_Html(filePath, sheetName);


                /*
                 * 檢查重複品號, 顯示警示, Lock下一步
                 */

                //Output Html
                this.lt_tbBody.Text = html.ToString();
            }
        }
        catch (Exception)
        {

            throw;
        }

    }
    #endregion


    #region -- 參數設定 --

    /// <summary>
    /// 設定參數 - 取得DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Convert.ToString(Page.RouteData.Values["DataID"]);

            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;


    /// <summary>
    /// 上傳目錄
    /// </summary>
    private string _UploadFolder;
    public string UploadFolder
    {
        get
        {
            return "{0}OrderFiles/".FormatThis(System.Web.Configuration.WebConfigurationManager.AppSettings["File_Folder"]);
        }
        set
        {
            this._UploadFolder = value;
        }
    }

    #endregion

}