<%@ Page Title="國別維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Country_Edit" ValidateRequest="false" %>

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
                <li><a href="<%=Application["WebUrl"] %>Country/Search/">國別查詢</a></li>
                <li class="active">國別維護</li>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">洲別 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfv_ddl_AreaCode" runat="server" ErrorMessage="請選擇「洲別」" ControlToValidate="ddl_AreaCode" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">國家區碼 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <asp:TextBox ID="tb_Country_Code" runat="server" CssClass="form-control tip" placeholder="國家區碼" MaxLength="2" ToolTip="字數上限 2 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Country_Code" runat="server" ErrorMessage="請填寫「國家區碼」" ControlToValidate="tb_Country_Code" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">國旗 <em>*</em></label>
                                <div class="col-sm-6 col-md-6 col-lg-8">
                                    <div class="pull-left">
                                        <asp:FileUpload ID="fu_Files" runat="server" />
                                        <asp:HiddenField ID="hf_OldFile" runat="server" />
                                    </div>
                                    <div class="pull-right">
                                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="help-block">
                                        (上傳功能僅限PC使用)
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-3 col-lg-2">
                                    <asp:PlaceHolder ID="ph_files" runat="server" Visible="false">
                                        <asp:Literal ID="lt_dwUrl" runat="server"></asp:Literal>&nbsp;
                                      <asp:LinkButton ID="lbtn_DelFile" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">顯示狀態</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">國家名 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="table-responsive">
                                        <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                                            <LayoutTemplate>
                                                <table class="table table-bordered table-advance table-striped">
                                                    <tbody>
                                                        <asp:PlaceHolder ID="ph_Items" runat="server" />
                                                    </tbody>
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="text-center">
                                                        <%#Eval("LangName") %>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="tb_Country_Name" runat="server" CssClass="form-control" placeholder="國家名" MaxLength="30" ToolTip="字數上限 15 字" Text='<%#Eval("Country_Name") %>'></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfv_tb_Country_Name" runat="server" ErrorMessage="請填寫「國家名」" ControlToValidate="tb_Country_Name" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>

                                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("LangCode") %>' />
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
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
                </div>
            </div>
            <div style="display: none;">
                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
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
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                blockBox1('Add', '資料處理中...');
                $('#MainContent_btn_doSave').trigger('click');
            });

            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });


            //滑動到指定內容區 Start
            <% if (Param_thisID != "")
               { %>
            //取得元素
            var _thisID = $('#data');

            //滑動至指定ID
            $('html, body').animate({
                scrollTop: $(_thisID).offset().top - 50
            }, 600);

            <%}%>
            //滑動到指定內容區 End

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
                bgcolor: '#ffffff',
                numeratio: true,
                infinigall: true
            });
        });
    </script>
    <%-- VenoBox End --%>
</asp:Content>

