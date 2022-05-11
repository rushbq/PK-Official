<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdReg.aspx.cs" Inherits="ProdReg" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/member-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
    <%: Styles.Render("~/bundles/steps-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container">

        <div class="row login">
            <div class="login-area col-sm-7">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_資料提供").ToString()%></div>
                </div>
                <div class="form-horizontal">
                    <div class="mySteps">
                        <!-- progressbar -->
                        <ul class="progressbar">
                            <li class="active"><%=this.GetLocalResourceObject("nav_填寫發票訊息").ToString()%></li>
                            <li><%=this.GetLocalResourceObject("nav_填寫產品資料").ToString()%></li>
                            <li><%=this.GetLocalResourceObject("nav_完成註冊").ToString()%></li>
                        </ul>
                        <div class="stepContainer">
                            <!-- Step 1 -->
                            <fieldset data-rel="step1">
                                <div class="form-group has-error">
                                    <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_發票號碼").ToString()%></label>
                                    <div class="col-md-9">
                                        <asp:TextBox ID="tb_InvoiceNo" runat="server" CssClass="form-control myEnter" autocomplete="off"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_InvoiceNo" runat="server" Display="Dynamic"
                                            ControlToValidate="tb_InvoiceNo" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="col-md-3 text-right"><%=this.GetLocalResourceObject("txt_購買日期").ToString()%></label>
                                    <div class="col-md-9 form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_BuyDate">
                                            <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center myEnter" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </div>
                                        <asp:TextBox ID="tb_BuyDate" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_BuyDate" runat="server" Display="Dynamic"
                                            ControlToValidate="tb_BuyDate" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="text-right">
                                    <input type="button" class="next btn btn-primary" value="<%=this.GetLocalResourceObject("btn_下一步").ToString()%>" />
                                </div>
                            </fieldset>
                            <!-- Step 2 -->
                            <fieldset data-rel="step2">
                                <div class="form-group has-error">
                                    <label class="col-xs-12"><%=this.GetLocalResourceObject("txt_產品資料").ToString()%></label>
                                    <div class="col-xs-12">
                                        <asp:TextBox ID="tb_myFilterItem" runat="server" CssClass="form-control myEnter"></asp:TextBox>
                                        <input type="hidden" id="tb_getItemID" />
                                        <input type="hidden" id="tb_getItemName" />
                                    </div>
                                    <div class="col-xs-12">(請輸入關鍵字,並選擇下拉選單上的商品)</div>
                                </div>

                                <div data-rel="data-list">
                                    <ul class="list-group" id="myItemList">
                                        <asp:Literal ID="lt_ViewList" runat="server"></asp:Literal>
                                    </ul>

                                    <asp:TextBox ID="myValues" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_myValues" runat="server" Display="None"
                                        ControlToValidate="myValues" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                </div>
                                <div class="text-right">
                                    <input type="button" class="previous btn btn-default" value="<%=this.GetLocalResourceObject("btn_上一步").ToString()%>" />
                                    <input type="button" class="next btn btn-primary" value="<%=this.GetLocalResourceObject("btn_下一步").ToString()%>" />
                                </div>
                            </fieldset>
                            <!-- Step 3 -->
                            <fieldset data-rel="step3">
                                <h4>即將完成</h4>
                                <p>恭喜您即將完成，請填入驗證碼並按下完成註冊。</p>
                                <hr />
                                <!-- 驗證碼 Start -->
                                <div class="form-group has-error">
                                    <label class="col-sm-12 col-md-3 text-right"><%=this.GetLocalResourceObject("txt_驗證碼").ToString()%></label>
                                    <div class="col-sm-12 col-md-9">
                                        <asp:TextBox ID="tb_VerifyCode" runat="server" MaxLength="5" CssClass="form-control myEnter" rel="nofollow"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfv_tb_VerifyCode" runat="server" Display="Dynamic"
                                            ControlToValidate="tb_VerifyCode" ValidationGroup="Add"><div class="alert alert-danger"><%=this.GetLocalResourceObject("tip_Require").ToString()%></div></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="col-md-offset-3 col-md-9">
                                        <asp:Image ID="img_Verify" runat="server" ImageAlign="Middle" />
                                        <a id="chg-Verify" href="javascript:void(0)"><i class="fa fa-refresh"></i></a>
                                    </div>
                                </div>
                                <!-- 驗證碼 End -->

                                <div class="text-right">
                                    <input type="button" class="previous btn btn-default" value="<%=this.GetLocalResourceObject("btn_上一步").ToString()%>" />
                                    <input type="button" id="triggerSave" class="finish btn btn-success" value="<%=this.GetLocalResourceObject("txt_傳送").ToString() %>" />

                                    <asp:Button ID="btn_Submit" runat="server" OnClick="btn_Submit_Click" ValidationGroup="Add" Style="display: none" />
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" HeaderText="Please check the form." />
                                </div>
                            </fieldset>
                        </div>
                    </div>
                </div>
            </div>

            <div class="login-area col-sm-5">
                <div class="page-title">
                    <div class="header"><%=this.GetLocalResourceObject("txt_說明表頭").ToString()%></div>
                </div>
                <div class="login-info">
                    <%=this.GetLocalResourceObject("txt_說明內文").ToString()%>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/steps-script") %>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/dynamic-Item-script") %>
    <script>
        $(function () {
            /* 偵測enter */
            $(".myEnter").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    //不動作
                    event.preventDefault();
                }
            });

            /* 驗證碼 */
            $('#chg-Verify').click(function () {
                document.getElementById('MainContent_img_Verify').src = '<%=Application["WebUrl"] %>myHandler/Ashx_CreateValidImg.ashx?r=' + Math.random();
            });

            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                //取得動態欄位值
                Get_Item('myItemList', 'MainContent_myValues');

                //呼叫BlockUI
                blockBox1('Add', 'Processing...');

                //觸發儲存
                $('#MainContent_btn_Submit').trigger('click');
            });
        });
    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: true,     //顯示today
                todayHighlight: true,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
            });
        });

    </script>
    <%-- DatePicker End --%>
    <%-- Autocompelete Start --%>
    <script>
        $(function () {
            $("#MainContent_tb_myFilterItem").catcomplete({
                minLength: 1,  //至少要輸入 n 個字元
                source: function (request, response) {
                    $.ajax({
                        url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                        data: {
                            q: request.term
                        },
                        type: "POST",
                        dataType: "json",
                        success: function (data) {
                            if (data != null) {
                                response($.map(data, function (item) {
                                    return {
                                        label: item.label + ' (' + item.id + ')',
                                        category: item.category,
                                        value: item.label,
                                        id: item.id
                                    }
                                }));
                            }
                        }
                    });
                },
                select: function (event, ui) {
                    $("#tb_getItemID").val(ui.item.id);
                    $("#tb_getItemName").val(ui.item.value);

                    //呼叫動態欄位, 新增項目
                    Add_Item("myItemList", "tb_getItemID", "tb_getItemName", true);

                    //清除輸入欄
                    $(this).val("");
                    event.preventDefault();
                }
            });

        });
    </script>
    <%-- Autocompelete End --%>
</asp:Content>

