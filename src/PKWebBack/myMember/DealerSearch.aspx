<%@ Page Title="經銷商查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="DealerSearch.aspx.cs" Inherits="Member_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="page-header">
                <h2>經銷商查詢</h2>
            </div>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <i class="fa fa-filter"></i>
                    <span>資料篩選</span>
                </div>
                <div class="panel-body">
                    <!-- Filter Content Start -->
                    <div class="row">
                        <div class="col-sm-7">
                            <label>關鍵字</label>
                            <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="編號, 名稱" MaxLength="20"></asp:TextBox>
                        </div>
                        <div class="col-sm-5">
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
            <!-- Nav tabs -->
            <asp:Literal ID="lt_Tabs" runat="server"></asp:Literal>

            <div class="tab-content">
                <div class="tab-pane active">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <div class="pull-left">
                                <span class="fa fa-th-list"></span>&nbsp;資料列表
                            </div>
                            <div class="pull-right text-right">
                                <%--<asp:Button ID="btn_Excel" runat="server" CssClass="btn btn-primary btn-sm" Text="匯出Excel" CausesValidation="false" OnClick="btn_Excel_Click" />--%>
                            </div>
                            <div class="clearfix"></div>
                        </div>
                        <!-- Table Content Start -->
                        <div id="result" class="table-responsive">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                                <LayoutTemplate>
                                    <table class="table table-bordered table-advance table-striped">
                                        <thead>
                                            <tr>
                                                <th style="width: 15%">編號</th>
                                                <th>名稱</th>
                                                <th style="width: 15%">會員關聯數</th>
                                                <th style="width: 15%">&nbsp;</th>
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
                                            <strong><%#Eval("ID") %></strong>
                                        </td>
                                        <td>
                                            <%#Eval("Label") %>
                                        </td>
                                        <td class="text-center">
                                            <%#Eval("MemberCnt") %>
                                        </td>
                                        <td class="text-center">
                                            <a href="<%=Application["WebUrl"] %>Member/Search/?srh=1&dealerid=<%#Eval("ID") %>&Tab=2" class="btn btn-primary">
                                                <i class="fa fa-eye fa-lg"></i>
                                            </a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="text-center styleReddark" style="padding: 60px 0px 60px 0px;">
                                        <h3><i class="fa fa-exclamation-triangle" aria-hidden="true"></i>&nbsp;查無資料</h3>
                                    </div>
                                </EmptyDataTemplate>

                            </asp:ListView>
                        </div>
                        <!-- Table Content End -->
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Result End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
</asp:Content>

