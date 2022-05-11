<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="List.aspx.cs" Inherits="myOrder_List" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="310" />
        <!-- 路徑導航 End -->

        <div class="panel panel-success">
            <!-- Default panel contents -->
            <div class="panel-heading">
                <h4><%=this.GetLocalResourceObject("title_歷史訂單").ToString()%></h4>
            </div>
            <div class="panel-body">
                <div class="pull-left form-inline">
                    <div class="form-group input-group">
                        <asp:TextBox ID="filter_Keyword" runat="server" CssClass="form-control myEnter" MaxLength="50"></asp:TextBox>
                        <div class="input-group-btn">
                            <button type="button" id="trigger-keySearch" class="btn btn-success"><span class="fa fa-search"></span></button>
                            <asp:Button ID="btn_KeySearch" runat="server" Text="Search" OnClick="btn_KeySearch_Click" Style="display: none;" />
                        </div>
                    </div>
                    <div class="form-group">
                        <%--<asp:DropDownList ID="ddl_ProdClass" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="btn_Search_Click"></asp:DropDownList>--%>
                    </div>
                    <%--<asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />--%>
                </div>
                <div class="pull-right">
                    <a href="<%=Application["WebUrl"] %>EO/unShip" class="btn btn-warning"><%=this.GetLocalResourceObject("txt_未出貨訂單").ToString()%></a>
                    <a href="<%=Application["WebUrl"] %>EO/Step1" class="btn btn-primary"><span class="fa fa-plus"></span>&nbsp;<%=this.GetLocalResourceObject("txt_建立訂單").ToString()%></a>
                </div>
                <div class="clearfix"></div>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemDataBound="lvDataList_ItemDataBound">
                <LayoutTemplate>
                    <table class="table table-striped table-bordered">
                        <thead>
                            <tr>
                                <th class="text-center"><asp:Literal ID="lt_header1" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center"><asp:Literal ID="lt_header2" runat="server"></asp:Literal>
                                </th>
                                <th class="text-center"><asp:Literal ID="lt_header3" runat="server"></asp:Literal>
                                </th>
                                <th>&nbsp;
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:PlaceHolder ID="ph_Items" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="text-center">
                            <%#Eval("Create_Time").ToString() %>
                        </td>
                        <td class="text-center">
                            <%#Eval("TraceID") %>
                        </td>
                        <td class="text-center">
                            <%#Eval("StatusName") %>
                        </td>
                        <td class="text-center">
                            <asp:Literal ID="lt_Url" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                </EmptyDataTemplate>
            </asp:ListView>
        </div>
        <div class="row">
            <div class="col-xs-12 text-right">
                <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        $(function () {
            //[搜尋][查詢鈕] - 觸發關鍵字快查
            $("#trigger-keySearch").click(function () {
                $("#MainContent_btn_KeySearch").trigger("click");
            });

            //[搜尋][Enter鍵] - 觸發關鍵字快查
            $("#MainContent_filter_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_KeySearch").trigger("click");

                    e.preventDefault();
                }
            });

        });
    </script>
</asp:Content>

