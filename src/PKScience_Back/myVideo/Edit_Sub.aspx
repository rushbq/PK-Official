<%@ Page Title="影片內容設定" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit_Sub.aspx.cs" Inherits="Video_Edit_Sub" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/Venobox-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">發佈日期 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_PubDate">
                                            <asp:TextBox ID="show_PubDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>

                                        </div>
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="請選擇「發佈日期日期」" ControlToValidate="tb_PubDate" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:TextBox ID="tb_PubDate" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">標題 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Data_Title" runat="server" CssClass="form-control tip" placeholder="標題" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Data_Title" runat="server" ErrorMessage="請填寫「標題」" ControlToValidate="tb_Data_Title" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
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
                                        <kbd>建議大小：619*412</kbd>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">影片來源 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:RadioButtonList ID="rbl_Data_UrlSource" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="showRadioGrp">
                                        <asp:ListItem Value="1">Youtube</asp:ListItem>
                                        <asp:ListItem Value="2">Youku優酷</asp:ListItem>
                                        <asp:ListItem Value="3">Vimeo</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:RequiredFieldValidator ID="rfv_rbl_Data_UrlSource" runat="server" ErrorMessage="請選擇「影片來源」" ControlToValidate="rbl_Data_UrlSource" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <div class="alert alert-danger">
                                        <strong>貼上Youku優酷網址請注意</strong>：<br />
                                        Step1: 複製「通用代碼」<br />
                                        Step2: 取得影片網址，&lt;iframe src="http://player.xxx"&gt;
                                        ，請複製src裡頭的http://player.xxx (不含雙引號)
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">網址 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Data_Url" runat="server" CssClass="form-control tip" placeholder="網址" MaxLength="250"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Data_Url" runat="server" ErrorMessage="請輸入「網址」" ControlToValidate="tb_Data_Url" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="rev_tb_Data_Url" runat="server" ErrorMessage="「網址」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Data_Url" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
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

            //使用jQueryUI 將radio Group
            $(".showRadioGrp").buttonset();
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

    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                //linkField: 'dtp_input2',    //鏡像欄位對應
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: true,     //顯示today
                todayHighlight: true,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 2,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
                //showMeridian: true, //顯示AM/PM
            });
        });

    </script>
    <%-- DatePicker End --%>
</asp:Content>
