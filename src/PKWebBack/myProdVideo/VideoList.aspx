<%@ Page Title="產品影片清單" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="VideoList.aspx.cs" Inherits="PV_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li class="active">產品影片清單</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-12">
            <div class="panel panel-warning">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-filter"></span>
                        <span>篩選器</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#filter">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="filter" class="collapse in">
                    <div class="panel-body">
                        <!-- Filter Content Start -->
                        <div class="row">
                            <div class="col-sm-6">
                                <label>品號</label>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="20"></asp:TextBox>
                            </div>
                            <div class="col-sm-6">
                                <label></label>
                                <div class="text-right" style="vertical-align: bottom;">
                                    <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                                </div>
                            </div>
                        </div>
                        <!-- Filter Content End -->
                    </div>
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
                    <div class="pull-left">
                        <span class="fa fa-th-list"></span>&nbsp;資料列表
                    </div>
                    <div class="pull-right text-right">
                        <asp:Button ID="btn_Excel" runat="server" CssClass="btn btn-primary btn-sm" Text="匯出Excel" CausesValidation="false" OnClick="btn_Excel_Click" />

                    </div>
                    <div class="clearfix"></div>
                </div>
                <!-- Table Content Start -->
                <div id="result" class="table-responsive collapse in">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="table table-bordered table-advance table-striped table-responsive">
                                <thead>
                                    <tr>
                                        <th style="width:100px;">品號</th>
                                        <th>繁中Url</th>
                                        <th>簡中Url</th>
                                        <th>英文Url</th>
                                        <th>識別名稱</th>
                                        <th>編號</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="6">
                                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-center">
                                    <strong><%#Eval("Model_No") %></strong>
                                </td>
                                <td>
                                    <small>
                                        <a href="<%#Eval("zh-tw") %>" target="_blank"><%#Eval("zh-tw") %></a>
                                    </small>
                                </td>
                                <td>
                                    <small>
                                        <a href="<%#Eval("zh-cn") %>" target="_blank"><%#Eval("zh-cn") %></a>
                                    </small>
                                </td>
                                <td>
                                    <small>
                                        <a href="<%#Eval("en-us") %>" target="_blank"><%#Eval("en-us") %></a>
                                    </small>
                                </td>
                                <td>
                                    <small><%#Eval("Group_Name") %></small>
                                </td>
                                <td class="text-center">
                                    <%#Eval("Group_ID") %>
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
    <%: Scripts.Render("~/bundles/blockUI-script") %>
</asp:Content>

