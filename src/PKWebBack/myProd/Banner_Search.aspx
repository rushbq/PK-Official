<%@ Page Title="產品表頭宣傳維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Banner_Search.aspx.cs" Inherits="Prod_BannerSearch" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">

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
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-12">
            <div class="panel panel-warning">
                <div id="filter" class="collapse in">
                    <div class="panel-body form-horizontal">
                        <!-- Filter Content Start -->
                        <div class="row">
                            <div class="col-sm-3">
                                <label>產品類型</label>
                                <asp:DropDownList ID="ddl_Type" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-5">
                                <label>產品分類</label>
                                <asp:DropDownListGP ID="ddl_ProdClass" runat="server" CssClass="form-control"></asp:DropDownListGP>
                            </div>
                            <div class="col-sm-4">
                                <label></label>
                                <div class="text-right" style="vertical-align: bottom;">
                                    <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                                    <a href="<%=Application["WebUrl"] %>Prod/BannerSearch" class="btn btn-info" title="Reset"><span class="glyphicon glyphicon-refresh"></span></a>
                                    <a href="<%=Application["WebUrl"] %>Prod/BannerEdit/?area=<%:Req_Area %>" class="btn btn-danger" title="新增資料"><span class="glyphicon glyphicon-plus"></span></a>
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
            <!-- Nav tabs -->
            <asp:Literal ID="lt_Tabs" runat="server"></asp:Literal>

            <div class="tab-content">
                <div class="tab-pane active">
                    <div class="panel panel-success">
                        <div class="panel-heading">
                            <div class="pull-left">
                                <span class="glyphicon glyphicon-list"></span>
                            </div>
                            <div class="pull-right">
                            </div>
                            <div class="clearfix"></div>
                        </div>
                        <!-- Table Content Start -->
                        <div id="result" class="table-responsive collapse in">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="table table-bordered table-advance table-striped">
                                        <thead>
                                            <tr>
                                                <th class="text-center" style="width: 10%">系統序號</th>
                                                <th>識別名稱</th>
                                                <th class="text-center">語系</th>
                                                <th class="text-center">產品類型</th>
                                                <th class="text-center">類別名稱</th>
                                                <th style="width: 10%">&nbsp;</th>
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
                                            <%#Eval("SeqNo") %>
                                        </td>
                                        <td>
                                            <%#Eval("Subject") %>
                                        </td>
                                        <td class="text-center">
                                            <%#Eval("LangCode") %>
                                        </td>
                                        <td class="text-center">
                                            <%#Eval("desc產品類型") %>
                                        </td>
                                        <td class="text-center">
                                            <%#Eval("desc類別名稱") %>
                                        </td>
                                        <td class="text-center">
                                            <a href="<%=Application["WebUrl"] %>Prod/BannerEdit/<%# Eval("SeqNo").ToString()%>/" class="btn btn-primary">
                                                <i class="fa fa-pencil fa-lg"></i>
                                            </a>
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
        </div>
    </div>
    <!-- Result End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            //換頁時, 滑動到內容區
            <% if (Req_PageIdx != 1)
        { %>
            //取得元素
            var _thisID = $('#result');

            //滑動至指定ID
            $('html, body').animate({
                scrollTop: $(_thisID).offset().top - 110
            }, 600);
            <%}%>

        });
    </script>
</asp:Content>

