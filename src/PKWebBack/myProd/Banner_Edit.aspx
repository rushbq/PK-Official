<%@ Page Title="產品表頭宣傳維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Banner_Edit.aspx.cs" Inherits="Country_Edit" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li class="active">各種產品列表-表頭宣傳維護</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Form Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <span class="glyphicon glyphicon-edit"></span>
                    <span>基本設定</span>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">功能區塊 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <asp:DropDownList ID="ddl_ProdArea" runat="server" CssClass="form-control detectChange"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddl_AreaCode" runat="server" ErrorMessage="請選擇「功能區塊」" ControlToValidate="ddl_ProdArea" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">產品類型 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <asp:DropDownList ID="ddl_ProdType" runat="server" CssClass="form-control detectChange"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Country_Code" runat="server" ErrorMessage="請填寫「產品類型」" ControlToValidate="ddl_ProdType" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">語系 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Lang" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="detectChange">
                                            <asp:ListItem Value="zh-tw" Selected="True">繁體中文</asp:ListItem>
                                            <asp:ListItem Value="zh-cn">简体中文</asp:ListItem>
                                            <asp:ListItem Value="en-us">English</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">關聯分類</label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <asp:DropDownListGP ID="ddl_ProdClass" runat="server" CssClass="form-control detectChange"></asp:DropDownListGP>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Button ID="btn_Base_Save" runat="server" Text="開始設定" CssClass="btn btn-primary" OnClick="btn_BaseSave_Click" ValidationGroup="Add" OnClientClick="blockBox1('Add', '資料處理中...');" />
                                </div>
                            </div>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />
                        </div>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <div class="FormBtn-fixPos">
                <div class="metro-nav metro-fix-view">
                    <div class="metro-nav-block nav-block-green">
                        <a href="<%=Page_SearchUrl %>" class="text-center">
                            <i class="fa fa-list"></i>
                            <div class="status">返回列表</div>
                        </a>
                    </div>
                    <div class="metro-nav-block nav-block-orange">
                        <a href="<%=Application["WebUrl"] %>Prod/BannerEdit/" class="text-center">
                            <i class="fa fa-pencil-square-o"></i>
                            <div class="status">新增下一筆</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-sm-9 col-md-10">
            <asp:PlaceHolder ID="ph_DetailMsg" runat="server">
                <div class="well well-lg">
                    <h4>↑↑↑↑↑ 請先完成基本設定 ↑↑↑↑↑</h4>
                </div>
            </asp:PlaceHolder>
            <div id="triggerChange" class="well well-lg" style="display: none;">
                <h4 class="text-danger">
                    <span class="glyphicon glyphicon-exclamation-sign"></span>
                    偵測到選項變更，請選擇完畢後，按下「開始設定」</h4>
            </div>

            <asp:PlaceHolder ID="ph_Detail" runat="server" Visible="false">
                <div id="detail" class="panel panel-success">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <i class="fa fa-cube"></i>
                            <span>詳細內容</span>
                        </div>
                        <div class="pull-right">
                            <asp:Button ID="btn_DetailSave" runat="server" Text="詳 細 內 容 存 檔" CssClass="btn btn-sm btn-warning" OnClick="btn_DetailSave_Click" OnClientClick="blockBox1('Edit', '資料處理中...');" />
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div class="panel-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3">識別名稱</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="tb_Subject" runat="server" CssClass="form-control" placeholder="後台辨識用, 最多 70 字" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-12">自訂內容&nbsp;(<em>若前台不顯示, 請按「原始碼」後清空內容</em>)</label>
                                <div class="col-sm-12">
                                    <asp:TextBox ID="tb_Content1" runat="server" TextMode="MultiLine" CssClass="ckeditor"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <div class="col-sm-3 col-md-2"></div>
    </div>
    <!-- Form End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/CKEditor-script") %>
    <script>
        $(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                blockBox1('Add', '資料處理中...');
                $('#MainContent_btn_doSave').trigger('click');
            });

            //使用jQueryUI 將radio Group
            $(".showRadioGrp").buttonset();


            //偵測設定變更
            $(".detectChange").change(function () {
                $("#triggerChange").show();
                $("#detail").hide();
            });

            /* 基本設定區功能 */
            //定義事件:功能區塊
            var prodArea = $("#MainContent_ddl_ProdArea")
            prodArea.on("myMenu1Event", function (event) {
                var getVal = $(this).find(':selected').val();  //選單值

                //關聯分類啟用否
                var eleProdClass = $("#MainContent_ddl_ProdClass");
                if (getVal == "C") {
                    eleProdClass.prop("disabled", "");
                } else {
                    eleProdClass.prop("disabled", "disabled");
                    eleProdClass[0].selectedIndex = 0;
                }
            });

            //init:功能區塊
            prodArea.trigger("myMenu1Event");

            //onchange:功能區塊
            prodArea.on('change', function () {
                $(this).trigger("myMenu1Event");
            });
        });
    </script>

</asp:Content>

