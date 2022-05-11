<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="unShipList.aspx.cs" Inherits="myOrder_unShipList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="myBody" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="310" />
        <!-- 路徑導航 End -->

        <div class="panel panel-warning">
            <!-- Default panel contents -->
            <div class="panel-heading">
                <div class="pull-left">
                    <h4><%=this.GetLocalResourceObject("title_未交貨訂單").ToString()%></h4>
                </div>
                <div class="pull-right">
                    <a href="<%=Application["WebUrl"] %>EO/List" class="btn btn-default"><i class="fa fa-home" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("txt_返回歷史訂單").ToString()%></a>
                    <asp:LinkButton ID="lbtn_Export" runat="server" CssClass="btn btn-success" OnClick="lbtn_Export_Click"></asp:LinkButton>
                </div>
                <div class="clearfix"></div>
            </div>
            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                <LayoutTemplate>
                    <table id="listTable" class="table table-bordered table-striped">
                        <thead>
                            <tr>
                                <th><asp:Literal ID="lt_header1" runat="server"></asp:Literal></th>
                                <th class="no-sort"><asp:Literal ID="lt_header2" runat="server"></asp:Literal></th>
                                <th class="no-sort"><asp:Literal ID="lt_header3" runat="server"></asp:Literal></th>
                                <th class="no-sort"><asp:Literal ID="lt_header4" runat="server"></asp:Literal></th>
                                <th class="no-sort"><asp:Literal ID="lt_header5" runat="server"></asp:Literal></th>
                                <th class="no-sort"><asp:Literal ID="lt_header6" runat="server"></asp:Literal></th>
                                <th><asp:Literal ID="lt_header7" runat="server"></asp:Literal></th>
                                <th><asp:Literal ID="lt_header8" runat="server"></asp:Literal></th>
                            </tr>
                            <tbody>
                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                            </tbody>
                        </thead>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="text-center"><%#Eval("OrderDate") %></td>
                        <td><%#Eval("ModelNo") %></td>
                        <td class="text-center"><%#Eval("BuyCnt") %></td>
                        <td class="text-center"><%#Eval("UnShipCnt") %></td>
                        <td class="text-right"><%#String.Format("{0:#,##0.00;($#,##0.00);Zero}", Eval("UnitPrice")) %></td>
                        <td class="text-right"><%#String.Format("{0:#,##0.00;($#,##0.00);Zero}", Eval("UnShipPrice")) %></td>
                        <td class="text-center"><%#Eval("PreShipDate") %></td>
                        <td><%#Eval("OrderFullID") %></td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="exception-info">
                        <div class="icon"><i class="fa fa-exclamation-triangle"></i></div>
                        <div class="message">Oops! <%=Resources.resPublic.txt_查無資料 %></div>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>

        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.11/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/dataTables.bootstrap.min.js"></script>

    <script>
        $(function () {
            /*
             [使用DataTable]
             注意:標題欄須與內容欄數量相等
           */
            $('#listTable').DataTable({
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false,      //筆數資訊
                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 'no-sort',
                    "orderable": false,
                }]

            });
        });

    </script>
    <%-- DataTable End --%>
</asp:Content>

