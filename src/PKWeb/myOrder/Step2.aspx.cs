using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using eOrder.Controllers;
using eOrder.Models;
using PKLib_Method.Methods;

public partial class myOrder_Step2 : SecurityCheckDealer
{

    //設定FTP連線參數
    private FtpMethod _ftp = new FtpMethod(fn_FTP.myFtp_Username, fn_FTP.myFtp_Password, fn_FTP.myFtp_ServerUrl);
    public string ErrMsg;


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

                //set resource
                this.lbtn_Reset.OnClientClick = "return confirm('{0}')".FormatThis(this.GetLocalResourceObject("btn_Confirm").ToString());
            }


        }
        catch (Exception)
        {

            throw;
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
                DBName = fld.DB_Name,
                Staus = fld.Status

            }).FirstOrDefault();

        //Check Data
        if (query == null)
        {
            //No data ...redirect
            Response.Redirect(Application["WebUrl"] + "EO/List");
            return;
        }

        //檢查訂單是否已完成
        if (query.Staus.Equals(4))
        {
            Response.Redirect(Application["WebUrl"] + "EO/Step1");
            return;
        }

        //載入單身資料
        LookupDetailData();


        //----- 資料整理:填入資料 -----
        string TraceID = query.TraceID;

        this.lt_TraceID.Text = TraceID;

        query = null;

    }

    private void LookupDetailData()
    {
        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDetailTempList(Req_DataID);

        //----- 資料繫結 -----
        this.lvDataList.DataSource = query;
        this.lvDataList.DataBind();

        //Check null
        if (this.lvDataList.Items.Count == 0)
        {
            Response.Redirect("{0}EO/Step1-1/{1}".FormatThis(Application["WebUrl"], Req_DataID));
            return;
        }
        //填入表頭的多語系文字
        ((Literal)this.lvDataList.FindControl("lt_Header1")).Text = this.GetLocalResourceObject("txt_Header1").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header2")).Text = this.GetLocalResourceObject("txt_Header2").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header3")).Text = this.GetLocalResourceObject("txt_Header3").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header4")).Text = this.GetLocalResourceObject("txt_Header4").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header5")).Text = this.GetLocalResourceObject("txt_Header5").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header6")).Text = this.GetLocalResourceObject("txt_Header6").ToString();
        ((Literal)this.lvDataList.FindControl("lt_Header7")).Text = this.GetLocalResourceObject("txt_Header7").ToString();

    }

    protected void lvDataList_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            //判斷數量是否符合, 不符合則自動修正為最小量  -- 20180620移除設定, 只提示
            Int16 Get_MinQty = Convert.ToInt16(((Literal)e.Item.FindControl("lt_MinQty")).Text);
            Int16 Get_BuyQty = Convert.ToInt16(((TextBox)e.Item.FindControl("tb_BuyQty")).Text);
            Literal lt_Msg = (Literal)e.Item.FindControl("lt_Msg");
            if (Get_BuyQty < Get_MinQty)
            {
                lt_Msg.Text = "<div class=\"text-danger\">{0}</div>".FormatThis(this.GetLocalResourceObject("tip4").ToString());

                //change Qty -- 20180620移除設定
                //((TextBox)e.Item.FindControl("tb_BuyQty")).Text = Get_MinQty.ToString();
            }


            //取得資料, 是否通過檢查
            string Get_IsPass = DataBinder.Eval(dataItem.DataItem, "IsPass").ToString();


            //顯示相關欄位
            e.Item.FindControl("ph_Edit").Visible = Get_IsPass.Equals("Y");
            e.Item.FindControl("ph_Lock").Visible = Get_IsPass.Equals("N");

            //Set resources
            ((LinkButton)e.Item.FindControl("btn_Edit")).Text = this.GetLocalResourceObject("btn_修改").ToString();
            ((LinkButton)e.Item.FindControl("btn_Del")).Text = this.GetLocalResourceObject("btn_移除").ToString();
            ((LinkButton)e.Item.FindControl("btn_Del")).OnClientClick = "return confirm('{0}')".FormatThis(this.GetLocalResourceObject("btn_Comfirm").ToString());
            ((RangeValidator)e.Item.FindControl("rv_tb_BuyQty")).ErrorMessage = this.GetLocalResourceObject("txt_數字").ToString();


        }
    }


    protected void lvDataList_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得資料值
            string Get_DataID = ((HiddenField)e.Item.FindControl("hf_DataID")).Value;
            Int16 Get_MOQ = Convert.ToInt16(((Literal)e.Item.FindControl("lt_MOQ")).Text);
            Int16 Get_MinQty = Convert.ToInt16(((Literal)e.Item.FindControl("lt_MinQty")).Text);
            Int16 Get_BuyQty = Convert.ToInt16(((TextBox)e.Item.FindControl("tb_BuyQty")).Text);
            Int16 Now_BuyQty = Get_BuyQty;
            string msg = "";

            //宣告
            eOrderingRepository _data = new eOrderingRepository();

            //判斷按鈕
            switch (e.CommandName)
            {
                case "doEdit":
                    //判斷:輸入值Get_BuyQty >= 最小量Get_MinQty,未成立直接帶最小量 -- 20180620移除設定, 只提示
                    if (Get_BuyQty < Get_MinQty)
                    {
                        msg = this.GetLocalResourceObject("tip1").ToString() + "\\n";

                        //自動帶最小量 -- 20180620移除設定
                        //((TextBox)e.Item.FindControl("tb_BuyQty")).Text = Get_MinQty.ToString();
                        //Now_BuyQty = Get_MinQty;
                    }

                    //判斷:輸入值Get_BuyQty >= Get_MOQ,未成立只有提示
                    if (Get_BuyQty < Get_MOQ)
                    {
                        msg += this.GetLocalResourceObject("tip2").ToString();
                    }

                    //----- 更新資料 -----
                    _data.Update_Qty(Req_DataID, Get_DataID, Now_BuyQty);


                    break;

                case "doDel":
                    //----- 刪除資料 -----
                    _data.Delete_Item(Req_DataID, Get_DataID);


                    break;
            }

            //release
            _data = null;


            //若訊息不為空值,則顯示alert
            if (!msg.Equals(""))
            {
                CustomExtension.AlertMsg(msg, "");
                return;
            }

            //導向本頁
            Response.Redirect(PageUrl);
        }
    }


    #endregion


    #region -- 按鈕事件 --
    /// <summary>
    /// 重新上傳:刪除資料及檔案
    /// </summary>
    protected void lbtn_Reset_Click(object sender, EventArgs e)
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
        //取得Grid 所有欄位值
        List<RefTempColumn> columnList = new List<RefTempColumn>();
        string alertMsg = "";

        for (int row = 0; row < this.lvDataList.Items.Count; row++)
        {
            ListView grid = this.lvDataList;

            Int16 Get_MinQty = Convert.ToInt16(((Literal)grid.Items[row].FindControl("lt_MinQty")).Text);
            Int16 Get_BuyQty = Convert.ToInt16(((TextBox)grid.Items[row].FindControl("tb_BuyQty")).Text);
            Int32 Get_DataID = Convert.ToInt32(((HiddenField)grid.Items[row].FindControl("hf_DataID")).Value);
            String Get_ModelNo = ((Literal)grid.Items[row].FindControl("lt_ModelNo")).Text;

            //AlertMsg
            if (Get_BuyQty < 1)
            {
                alertMsg += "{0}, {1}\\n".FormatThis(Get_ModelNo, this.GetLocalResourceObject("txt_數字").ToString());
            }
            
            //-- 20180620移除設定
            //if (Get_BuyQty < Get_MinQty)
            //{
            //    alertMsg += "{0}, {1}={2}\\n".FormatThis(Get_ModelNo, this.GetLocalResourceObject("txt_Header4").ToString(), Get_MinQty);
            //}

            var col = new RefTempColumn
            {
                Data_ID = Get_DataID,
                BuyCnt = Get_BuyQty
            };

            columnList.Add(col);
        }

        //欄位檢查
        if (!string.IsNullOrEmpty(alertMsg))
        {
            CustomExtension.AlertMsg(alertMsg, "");
            return;
        }


        //----- 宣告:資料參數 -----
        eOrderingRepository _data = new eOrderingRepository();

        //建立基本資料參數, 用途:資料篩選條件
        var baseData = new ImportData
        {
            Data_ID = new Guid(Req_DataID),
            TraceID = this.lt_TraceID.Text
        };

        //do update
        if (!_data.Update_AllQty(Req_DataID, columnList.AsQueryable(), out ErrMsg))
        {
            //[Log]
            string Msg = "單身數量更新失敗(Step2)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            this.ph_errMessage.Visible = true;
            return;
        }


        //建立單身資料,取價/庫存/庫存判斷/生成EDI訂單號
        if (!_data.Create_Detail(baseData, out ErrMsg))
        {
            //[Log]
            string Msg = "單身資料檔建立失敗(Step2)...\n" + ErrMsg;
            _data.Create_Log(baseData, Msg, out ErrMsg);

            this.ph_errMessage.Visible = true;
            return;
        }

        //OK
        Response.Redirect(Application["WebUrl"] + "EO/Step3/" + baseData.Data_ID);

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
    /// 設定參數 - 本頁Url
    /// </summary>
    public string PageUrl
    {
        get
        {
            return "{0}EO/Step2/{1}".FormatThis(Application["WebUrl"], Req_DataID);
        }
        set
        {
            this._PageUrl = value;
        }
    }
    private string _PageUrl;


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