<%@ Page Title="訂單記錄" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="myOrders_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
             <div class="page-header">
                <h2>訂單記錄
                    <small>
                        <a>訂單查詢</a>&nbsp;
                        <i class="fa fa-chevron-right"></i>&nbsp;<span>訂單記錄</span>
                    </small>
                </h2>
            </div>
        </div>
    </div>
    <!-- Page Header End -->
    <h2>~~~~ 功能未開發 ~~~~</h2>
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-12">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <div>
                        <span class="glyphicon glyphicon-filter"></span>
                        <span>資料篩選</span>
                    </div>
                </div>
                <div class="panel-body">
                    <!-- Filter Content Start -->
                    <div class="row">
                        <div class="col-sm-4">
                            <label>狀態</label>
                            <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="col-sm-6">
                            <label>客戶代號 / 客戶名稱 / 訂單編號</label>
                            <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="40"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="text-right">
                                <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" />
                            </div>
                        </div>
                    </div>
                    <!-- Filter Content End -->
                </div>
            </div>
        </div>
    </div>

    <!-- Filter End -->
    <!-- Result Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <span class="glyphicon glyphicon-list"></span>
                    <span>資料列表</span>
                </div>
                <!-- Table Content Start -->
                <div id="result" class="table-responsive">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                        <LayoutTemplate>
                            <table class="table table-bordered table-advance table-striped">
                                <thead>
                                    <tr>
                                        <th>訂單編號</th>
                                        <th>客戶</th>
                                        <th>下單日期</th>
                                        <th>總金額</th>
                                        <th>狀態</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="5">
                                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-center">
                                    <%#Eval("AreaName") %>
                                </td>
                                <td>
                                    <p><span class="label label-warning"><%#Eval("Country_Code") %></span></p>
                                    <p><%#Eval("Country_Name") %></p>
                                </td>
                                <td>
                                    <asp:Literal ID="lt_FileThumb" runat="server"></asp:Literal>
                                </td>
                                <td class="text-center">
                                    <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                </td>
                                <td class="text-center">
                                    <a href="<%=Application["WebUrl"] %>Country/Edit/<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("Country_Code").ToString(), Application["DesKey"].ToString()))%>/" class="btn btn-primary">
                                        <i class="fa fa-pencil fa-lg"></i>
                                    </a>&nbsp;
                                    <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除?')">
                                        <i class="fa fa-trash-o fa-lg"></i>
                                    </asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Country_Code") %>' />
                                    <asp:HiddenField ID="hf_OldFile" runat="server" Value='<%#Eval("Country_Flag") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="text-center styleReddark" style="padding: 60px 0px 60px 0px;">
                                <h3><span class="glyphicon glyphicon-exclamation-sign"></span>&nbsp;查無資料</h3>
                            </div>
                        </EmptyDataTemplate>

                    </asp:ListView>
                </div>
                <!-- Table Content End -->
            </div>
        </div>
    </div>
    <!-- Result End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">

</asp:Content>

