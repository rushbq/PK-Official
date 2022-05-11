<%@ Page Title="活動內容設定" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit_Sub.aspx.cs" Inherits="Expo_Edit_Sub" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/Venobox-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Page_LastUrl %>">
                    <asp:Literal ID="lt_NavSubject" runat="server">單頭資料</asp:Literal></a></li>
                <li class="active">語系內容設定</li>
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
                        <span>基本設定</span>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">識別名稱</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Group_Name" runat="server" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">目前語系</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Lang" runat="server" CssClass="styleBlue"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">標題 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Title" runat="server" CssClass="form-control tip" placeholder="標題" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Expo_Title" runat="server" ErrorMessage="請填寫「標題」" ControlToValidate="tb_Expo_Title" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">次標題</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_SubTitle" runat="server" CssClass="form-control tip" placeholder="次標題" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">簡述 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Desc" runat="server" CssClass="form-control tip" placeholder="簡述" MaxLength="500" ToolTip="字數上限 250 字" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Expo_Desc" runat="server" ErrorMessage="請填寫「簡述」" ControlToValidate="tb_Expo_Desc" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">網址 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Url" runat="server" CssClass="form-control tip" placeholder="網址" MaxLength="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Expo_Url" runat="server" ErrorMessage="請輸入「網址」" ControlToValidate="tb_Expo_Url" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rev_tb_Expo_Url" runat="server" ErrorMessage="「網址」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Expo_Url" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                         
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">小圖</label>
                                <div class="col-sm-6 col-md-6 col-lg-8">
                                    <div class="pull-left">
                                        <asp:FileUpload ID="fu_Files_Small" runat="server" />
                                        <asp:HiddenField ID="hf_OldFile_Small" runat="server" />
                                    </div>
                                    <div class="pull-right">
                                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="help-block">
                                        <kbd>建議大小：1200*692</kbd>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-3 col-lg-2">
                                    <asp:PlaceHolder ID="ph_files_Small" runat="server" Visible="false">
                                        <asp:Literal ID="lt_dwUrl_Small" runat="server"></asp:Literal>&nbsp;
                                      <asp:LinkButton ID="lbtn_DelFile_Small" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Small_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">大圖 <small>(顯示在明細頁)</small></label>
                                <div class="col-sm-6 col-md-6 col-lg-8">
                                    <div class="pull-left">
                                        <asp:FileUpload ID="fu_Files_Big" runat="server" />
                                        <asp:HiddenField ID="hf_OldFile_Big" runat="server" />
                                    </div>
                                    <div class="pull-right">
                                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="help-block">
                                        <kbd>建議大小：1060*1000</kbd>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-3 col-lg-2">
                                    <asp:PlaceHolder ID="ph_files_Big" runat="server" Visible="false">
                                        <asp:Literal ID="lt_dwUrl_Big" runat="server"></asp:Literal>&nbsp;
                                      <asp:LinkButton ID="lbtn_DelFile_Big" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Big_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Button ID="btn_Base_Save" runat="server" Text="儲存設定" CssClass="btn btn-primary" OnClick="btn_Save_Click" ValidationGroup="Add" OnClientClick="blockBox1('Add', '資料處理中...');" />
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
                        <a href="<%=Page_LastUrl %>" class="text-center">
                            <i class="glyphicon glyphicon-circle-arrow-left"></i>
                            <div class="status">返回單頭資料</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Form End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });

        });
    </script>

    <%-- VenoBox Start --%>
    <%: Scripts.Render("~/bundles/Venobox-script") %>
    <script>
        $(function () {
            /* Venobox */
            $('.zoomPic').venobox({
                border: '10px',
                bgcolor: '#ffffff'
            });
        });
    </script>
    <%-- VenoBox End --%>

</asp:Content>
