<%@ Page Title="產品內容設定" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit_Sub.aspx.cs" Inherits="Prod_Edit_Sub" %>

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
                            <asp:PlaceHolder ID="ph_Status" runat="server">
                                <div class="form-group">
                                    <div class="col-xs-12">
                                        <strong>
                                            <span class="styleRed">資料新增中...</span></strong>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">品號</label>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">說明書網址</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Url_Manual" runat="server" CssClass="form-control tip" placeholder="網址" MaxLength="250"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="rev_tb_Url_Manual" runat="server" ErrorMessage="「網址」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Url_Manual" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">影片網址</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Url_Video" runat="server" CssClass="form-control tip" placeholder="網址" MaxLength="250"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="rev_tb_Url_Video" runat="server" ErrorMessage="「網址」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Url_Video" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
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

</asp:Content>
