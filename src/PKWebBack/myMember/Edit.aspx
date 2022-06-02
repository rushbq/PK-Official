<%@ Page Title="會員資料" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Member_Edit" ValidateRequest="false" %>

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
                <li><a href="<%=Application["WebUrl"] %>Member/Search/">會員查詢</a></li>
                <li class="active">會員資料</li>
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
                        <span>會員基本資料</span>
                    </div>
                    <div class="pull-right">
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="col-xs-2 control-label">帳號</label>
                                <div class="col-xs-10 form-control-static">
                                    <strong>
                                        <asp:Literal ID="lt_Email" runat="server"></asp:Literal></strong>
                                    <asp:Literal ID="lt_Social" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">帳號狀態</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                </div>
                                <label class="col-xs-2 control-label">會員身份</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Label ID="lb_MemType" runat="server" CssClass="label label-info"></asp:Label>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">公司</label>
                                <div class="col-xs-10 form-control-static">
                                    <asp:Literal ID="lt_Company" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <!-- 申請經銷商 Start -->
                            <asp:Panel ID="pl_DealerApply" runat="server" Visible="false">
                                <div class="bq-callout orange">
                                    <h4>經銷商關聯設定</h4>
                                    <div class="form-group">
                                        <label class="col-xs-2 control-label">申請狀態</label>
                                        <div class="col-xs-10 form-control-static">
                                            <asp:Label ID="lb_DealerCheck" runat="server" CssClass="label label-default"></asp:Label>
                                            <asp:Label ID="lb_DealerStatus" runat="server" CssClass="label label-default"></asp:Label>
                                            <asp:PlaceHolder ID="pl_ViewApply" runat="server" Visible="false">&nbsp;
                                                <a href="#" data-toggle="modal" data-target="#modal_Dealer">查看申請單&nbsp;<i class="fa fa-external-link"></i></a>
                                            </asp:PlaceHolder>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <label class="col-xs-2 control-label">ERP客戶代號</label>
                                        <div class="col-xs-10 form-inline">
                                            <asp:TextBox ID="tb_CustID" runat="server" CssClass="form-control tip" placeholder="輸入客戶關鍵字"></asp:TextBox>
                                            <asp:HiddenField ID="hf_CustID" runat="server" />
                                            <asp:HiddenField ID="hf_CustName" runat="server" />
                                            <asp:RequiredFieldValidator ID="rfv_tb_CustID" runat="server" ErrorMessage="請選擇「客戶代號」"
                                                Display="Dynamic" ControlToValidate="tb_CustID" ValidationGroup="Dealer" CssClass="styleRed help-block"></asp:RequiredFieldValidator>

                                            <asp:Button ID="btn_SetDealer" runat="server" Text="設定關聯" CssClass="btn btn-warning" ValidationGroup="Dealer" OnClientClick="return confirm('是否確認此編號無誤?')" OnClick="btn_SetDealer_Click" />
                                            <asp:Button ID="Button1" runat="server" Text="申請駁回" CssClass="btn btn-danger" CausesValidation="false" OnClientClick="return confirm('是否確認要駁回此申請?')" OnClick="lbtn_RejectDealer_Click" />
                                        </div>
                                    </div>
                                    <div class="alert alert-warning"><i class="fa fa-exclamation-triangle"></i>&nbsp;設定完成後, 請提醒客戶<strong>重新登入</strong>, 才能取得新身份</div>
                                </div>
                                <!-- /// 經銷商申請單 Modal Start /// -->
                                <div id="modal_Dealer" class="modal fade" tabindex="-1" role="dialog" style="display: none;">
                                    <div class="modal-dialog">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button>
                                                <h4 class="modal-title">經銷商申請單</h4>
                                            </div>
                                            <div class="modal-body">
                                                <asp:Repeater ID="myForm" runat="server" OnItemDataBound="myForm_ItemDataBound">
                                                    <ItemTemplate>
                                                        <div class="form-horizontal">
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">公司名稱<br />Company Name</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CompName") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">公司資本額<br />Company Capital</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CompCaptital") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">總裁/董事長<br />President / CEO</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CEO") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">電話<br />Tel No.</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("Tel") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">傳真<br />Fax No.</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("Fax") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">網站<br />Website</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("WebSite") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">街道地址<br />Street Address</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("Address") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">市<br />City</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("City") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">省<br />State</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("State") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">郵遞區號<br />ZIP/Postal</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("ZIP") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">國家<br />Country</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("Country") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">員工數<br />Member of Employees</label>
                                                                <div class="col-xs-2">
                                                                   <strong><%#Eval("EmpNum1") %></strong>
                                                                </div>
                                                                <div class="col-xs-2">
                                                                    (+Engineer)&nbsp;&nbsp;<u><%#Eval("EmpNum2") %></u>&nbsp;
                                                                </div>
                                                                <div class="col-xs-2">
                                                                    (+Others)&nbsp;&nbsp;<u><%#Eval("EmpNum3") %></u>&nbsp;
                                                                </div>
                                                                <div class="col-xs-2">
                                                                    (=Total)&nbsp;&nbsp;<u><%#Eval("EmpNum4") %></u>&nbsp;
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">聯絡人1<br />Contact Person 1</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Name1") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">部門/職稱<br />Dept./Title</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Dept1") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">分機<br />Direct Line/Ext.</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Line1") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">電子郵件<br />E-mail</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Email1") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">聯絡人2<br />Contact Person 2</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Name2") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">部門/職稱<br />Dept./Title</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Dept2") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">分機<br />Direct Line/Ext.</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Line2") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">電子郵件<br />E-mail</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("CP_Email2") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">業務類型<br />Business Type</label>
                                                                <div class="col-xs-8">
                                                                    <%#Show_cbItems(Eval("BusinessType").ToString(),"BusinessType",Eval("LangCode").ToString()) %>
                                                                    <div>
                                                                        <%#Eval("BusinessType_Other") %>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">分公司<br />Branch Office</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("BranchOffice") %>
                                                                </div>
                                                            </div>
                                                            <div class="form-group">
                                                                <label class="col-xs-4 control-label label-default">年營業額</label>
                                                                <div class="col-xs-8">
                                                                    <%#Eval("Annual") %>
                                                                </div>
                                                            </div>

                                                            <asp:PlaceHolder ID="ph_FieldOfEnglish" runat="server">
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Establish tool product line since year</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Eval("ProductYear") %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Annual turn over of tools product in US$</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Eval("AnnualProduct") %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Agent / Distributor for famous brands</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Eval("AgentBrands") %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Main market &amp; ratio</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Eval("MainMarket").ToString().Replace("\r","<br/>") %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Major product line &amp; ratio of annual turn over</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Eval("MajorProduct").ToString().Replace("\r","<br/>") %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Supplier location &amp; ratio</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Show_cbItems(Eval("SupplierLocation").ToString(),"SupplierLocation",Eval("LangCode").ToString()) %>
                                                                        <div>
                                                                            <%#Eval("SupplierLocation_Other") %>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">Business Field &amp; ratio (percentage)</label>
                                                                    <div class="col-xs-8">
                                                                        <div class="row">
                                                                            <div class="col-xs-9 col-sm-9">
                                                                                <label>Computer, Communication &amp; Consumer Industry</label>
                                                                            </div>
                                                                            <div class="col-xs-3 col-sm-3">
                                                                                <%#Eval("BusniessField_Per1") %> %
                                                                            </div>
                                                                        </div>
                                                                        <div style="padding-left: 10px;">
                                                                            <%#Show_cbItems(Eval("BusniessField_Opt1").ToString(),"BusinessFieldA",Eval("LangCode").ToString()) %>
                                                                        </div>
                                                                        <div class="row" style="padding-top: 20px;">
                                                                            <div class="col-xs-9 col-sm-9">
                                                                                <label>Machinery &amp; Hardware</label>
                                                                            </div>
                                                                            <div class="col-sm-3 col-sm-3">
                                                                                <%#Eval("BusniessField_Per2") %> %
                                                                            </div>
                                                                        </div>
                                                                        <div style="padding-left: 10px;">
                                                                            <%#Show_cbItems(Eval("BusniessField_Opt2").ToString(),"BusinessFieldB",Eval("LangCode").ToString()) %>
                                                                        </div>
                                                                        <div class="row" style="padding-top: 20px;">
                                                                            <div class="col-xs-9 col-sm-9">
                                                                                <label>Construction Engineering</label>
                                                                            </div>
                                                                            <div class="col-sm-3 col-sm-3">
                                                                                <%#Eval("BusniessField_Per3") %> %
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </asp:PlaceHolder>

                                                            <asp:PlaceHolder ID="ph_FieldOfChinese" runat="server">
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">公司屬性</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Show_cbItems(Eval("CompType").ToString(),"CompanyType",Eval("LangCode").ToString()) %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">營業項目</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Show_cbItems(Eval("BusinessItem").ToString(),"BusinessItem",Eval("LangCode").ToString()) %>
                                                                    </div>
                                                                </div>
                                                                <div class="form-group">
                                                                    <label class="col-xs-4 control-label label-default">銷售對象</label>
                                                                    <div class="col-xs-8">
                                                                        <%#Show_cbItems(Eval("SalesTarget").ToString(),"SalesTarget",Eval("LangCode").ToString()) %>
                                                                    </div>
                                                                </div>
                                                            </asp:PlaceHolder>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                            <div class="modal-footer">
                                                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!-- /// 經銷商申請單 Modal End /// -->
                            </asp:Panel>
                            <!-- 申請經銷商 End -->
                            <div class="form-group">
                                <label class="col-xs-2 control-label">生日</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_Birthday" runat="server"></asp:Literal>
                                </div>
                                <label class="col-xs-2 control-label">註冊日</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_RegDate" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">姓名</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_FirstName" runat="server"></asp:Literal>
                                    <asp:Literal ID="lt_LastName" runat="server"></asp:Literal>
                                </div>
                                <label class="col-xs-2 control-label">性別</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_Sex" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">電話</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_Tel" runat="server"></asp:Literal>
                                </div>
                                <label class="col-xs-2 control-label">手機</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_Mobile" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">國家</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_Country" runat="server"></asp:Literal>
                                </div>
                                <label class="col-xs-2 control-label">資料回填</label>
                                <div class="col-xs-4 form-control-static">
                                    <asp:Literal ID="lt_IsWrite" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-xs-2 control-label">地址</label>
                                <div class="col-xs-10 form-control-static">
                                    <asp:Literal ID="lt_Address" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>
                        <!-- Content End -->
                    </div>
                    <asp:Panel ID="pl_Sign" runat="server" CssClass="floatSign" Visible="false">
                        <span class="styleGraylight"><i class="fa fa-ban"></i>&nbsp;帳 戶 已 停 用</span>
                    </asp:Panel>
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
                    <asp:PlaceHolder ID="ph_disable" runat="server" Visible="false">
                        <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Disable" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定停用?')" OnClick="lbtn_Disable_Click">
                            <i class="fa fa-ban"></i>
                            <div class="status">停用帳號</div>
                            </asp:LinkButton>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_enable" runat="server" Visible="false">
                        <div class="metro-nav-block nav-block-blue">
                            <asp:LinkButton ID="lbtn_Enable" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定啟用?')" OnClick="lbtn_Enable_Click">
                            <i class="fa fa-refresh"></i>
                            <div class="status">啟用帳號</div>
                            </asp:LinkButton>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
    <!-- Form End -->
    <!-- Dealer Info Start -->
    <asp:PlaceHolder ID="ph_DealerInfo" runat="server" Visible="false">
        <div class="row">
            <div class="col-sm-9 col-md-10">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-list"></span>
                            <span>經銷商資料</span>
                        </div>
                        <div class="pull-right">
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <!-- Table Content Start -->
                    <div id="result" class="collapse in">
                        <div class="panel-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">客戶代號</label>
                                    <div class="col-xs-10 form-control-static">
                                        <strong>
                                            <asp:Literal ID="lt_MA001" runat="server"></asp:Literal></strong>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">客戶簡稱</label>
                                    <div class="col-xs-10 form-control-static">
                                        <asp:Literal ID="lt_MA002" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">客戶全稱</label>
                                    <div class="col-xs-10 form-control-static">
                                        <asp:Literal ID="lt_MA003" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">客戶EMail</label>
                                    <div class="col-xs-10 form-control-static">
                                        <asp:Literal ID="lt_MA009" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">交易幣別</label>
                                    <div class="col-xs-10 form-control-static">
                                        <asp:Literal ID="lt_MA014" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label class="col-xs-2 control-label">出貨地址</label>
                                    <div class="col-xs-10 form-control-static">
                                        <asp:Literal ID="lt_MA027" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
            <div class="col-sm-3 col-md-2"></div>
        </div>
    </asp:PlaceHolder>
    <!-- Dealer Info End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        //取得資料編號(這項宣告要放在最前方)
        var dataID = '<%=Param_thisID%>';

    </script>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/group-base") %>

    <%-- Autocompelete 客戶 Start --%>
    <script>
        $("#MainContent_tb_CustID").autocomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/AC_Customer.aspx",
                    data: {
                        q: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    label: "(" + item.id + ") " + item.label,
                                    value: "(" + item.id + ") " + item.label,
                                    name: item.label,
                                    id: item.id
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                $("#MainContent_hf_CustID").val(ui.item.id);
                $("#MainContent_hf_CustName").val(ui.item.name);

            }
        });

    </script>
    <%-- Autocompelete 客戶 End --%>
</asp:Content>

