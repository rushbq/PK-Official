<%@ Page Title="主題產品推廣" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Promo_Edit" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Application["WebUrl"] %>Promo/Search/">主題產品推廣</a></li>
                <li class="active">資料維護</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Form Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>群組基本設定</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#data">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">資料編號</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_DataID" runat="server" Text="系統自動編號" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Rpt_Folder">區域 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:CheckBoxList ID="cbl_Area" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                    <asp:CustomValidator ID="cv_check_Area" runat="server" ErrorMessage="請選擇「區域」" Display="Dynamic"
                                        ClientValidationFunction="check_AreaCode" ValidationGroup="Add" CssClass="styleRed help-block"></asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">識別名稱 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Group_Name" runat="server" CssClass="form-control tip" placeholder="識別名稱" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Group_Name" runat="server" ErrorMessage="請填寫「識別名稱」" ControlToValidate="tb_Group_Name" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架日期 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_StartDate">
                                            <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>

                                        </div>
                                        ~
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_EndDate">
                                            <asp:TextBox ID="show_eDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>
                                        </div>
                                    </div>
                                    <div class="help-block">
                                        (若結束時間空白,系統將自動填入開始時間 +5 年)
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ID="rfv_sDate" runat="server" ErrorMessage="請選擇「開始時間」" ControlToValidate="tb_StartDate" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:TextBox ID="tb_StartDate" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="tb_EndDate" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架狀態</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="false"></asp:RadioButtonList>
                                    </div>
                                    <div class="help-block">
                                        <asp:Label ID="lb_SetMsg" runat="server" Text="(目前語系資料尚未完成設定,無法修改上架狀態)"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Sort">排序 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                    <div class="help-block">
                                        (前台顯示的排序,若排序相同,則依照上架開始日期由新到舊排序)
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Literal ID="lt_CreateInfo" runat="server"><p class="form-control-static help-block">資料新增中...</p></asp:Literal>
                                    <asp:Literal ID="lt_UpdateInfo" runat="server"></asp:Literal>
                                </div>

                            </div>

                            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
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
                    <asp:PlaceHolder ID="ph_Save" runat="server">
                        <div class="metro-nav-block nav-block-blue">
                            <a id="triggerSave" class="text-center">
                                <i class="fa fa-floppy-o"></i>
                                <div class="status">資料存檔</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_Delete" runat="server" Visible="false">
                        <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定刪除?')" OnClick="lbtn_Delete_Click">
                            <i class="glyphicon glyphicon-trash"></i>
                            <div class="status">刪除資料</div>
                            </asp:LinkButton>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div style="display: none;">
                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
            </div>
        </div>
    </div>
    <!-- Form End -->
    <!-- Sub Details Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-list"></span>
                        <span>各語系設定狀態</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#result">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <!-- Table Content Start -->
                <div id="result" class="collapse in">
                    <div class="panel-body">
                        <div class="btn-group btn-group-justified btn-group-lg">
                            <asp:Literal ID="lt_PromoSet" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <!-- Table Content End -->
            </div>
        </div>
        <div class="col-sm-3 col-md-2"></div>
    </div>
    <!-- Sub Details End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        //取得資料編號(這項宣告要放在最前方)
        var dataID = '<%=Param_thisID%>';

    </script>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/group-base") %>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <%: Scripts.Render("~/bundles/group-datepicker") %>
</asp:Content>

