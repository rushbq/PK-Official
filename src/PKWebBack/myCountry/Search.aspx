<%@ Page Title="國別查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Country_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li class="active">國別查詢</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Filter Start -->
    <div class="row">
        <div class="col-sm-8 col-md-10">
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
                            <div class="col-sm-4">
                                <label>洲別</label>
                                <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-sm-6">
                                <label>國家名</label>
                                <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="輸入關鍵字" MaxLength="20"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="text-right">
                                    <asp:Button ID="btn_Search" runat="server" CssClass="btn btn-success" Text="開始查詢" ValidationGroup="Search" OnClick="btn_Search_Click" OnClientClick="blockBox2_NoMsg()" />
                                </div>
                            </div>
                        </div>
                        <!-- Filter Content End -->
                    </div>
                </div>
            </div>
        </div>
        <div class="col-sm-4 col-md-2">
            <div class="metro-nav metro-fix-view">
                <div class="metro-nav-block nav-light-blue">
                    <a href="<%=Application["WebUrl"] %>Country/Edit/" class="text-center">
                        <i class="fa fa-pencil-square-o"></i>
                        <div class="status">新增資料</div>
                    </a>
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
                        <span class="glyphicon glyphicon-list"></span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#result">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
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
                                        <th style="width: 10%">洲別</th>
                                        <th style="width: 45%">國家名</th>
                                        <th style="width: 20%">國旗</th>
                                        <th style="width: 10%">狀態</th>
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
                                    <%#Eval("AreaName") %>
                                </td>
                                <td>
                                    <p><span class="label label-warning"><%#Eval("Country_Code") %></span></p>
                                    <p><%#Eval("Country_Name") %></p>
                                </td>
                                <td align="center">
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

