<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="PriceList.aspx.cs" Inherits="PriceList" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/support-css") %>
    <%: Styles.Render("~/bundles/fancybox-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="305" />
        <!-- 路徑導航 End -->
        <!-- Content Start -->
        <div class="page-header df-page-header">
            <h3><%=this.GetLocalResourceObject("text_報價單").ToString()%></h3>
        </div>
        <div class="panel panel-info">
            <div class="panel-heading">
                <div class="pull-left">
                    <%=this.GetLocalResourceObject("text_查詢結果").ToString()%>
                </div>
                <div class="pull-right">
                    <a href="javascript:void(0)" id="exp_Pricelist" class="btn btn-success"><i class="fa fa-file-excel-o" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("text_匯出").ToString()%></a>
                    <a href="<%=Application["WebUrl"] %>PLfilter" class="btn btn-default"><i class="fa fa-filter" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("text_返回篩選").ToString()%></a>
                    <a href="<%=Application["WebUrl"] %>Report" class="btn btn-default"><i class="fa fa-home" aria-hidden="true"></i>&nbsp;<%=this.GetLocalResourceObject("text_返回").ToString()%></a>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="table-responsive" style="padding-top: 10px;">
                <table id="listTable" class="table table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>&nbsp;</th>
                            <th>Stop Offer</th>
                            <th>Item NO / Desc.</th>
                            <th>Class</th>
                            <th>Currency</th>
                            <th>Unit Price</th>
                            <th>Unit</th>
                            <th>Quote Date</th>
                            <th>MOQ</th>
                            <th>VOL</th>
                            <th>Page</th>
                            <th>Qty Inner</th>
                            <th>NW</th>
                            <th>GW</th>
                            <th>CUFT</th>
                            <th>BarCode</th>
                            <th>Packing</th>
                            <th>Ship From</th>
                            <th>Term</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
        <div class="hide">
            <asp:Button ID="btn_PriceList" runat="server" Text="Pricelist" Style="display: none;" OnClick="btn_PriceList_Click" />
        </div>
        <!-- Content End -->
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        $(function () {
            /* trigger click */
            $("#exp_Pricelist").click(function () {
                $("#MainContent_btn_PriceList").trigger("click");
            });
        });
    </script>
    <%-- DataTable Start --%>
    <link href="https://cdn.datatables.net/1.10.11/css/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/1.10.11/js/dataTables.bootstrap.min.js"></script>
    <script>
        $(function () {
            /*
             [使用DataTable]
             注意:標題欄須與內容欄數量相等
             ref:https://datatables.net/reference/option/columns.render
           */
            $('#listTable').DataTable({
                "processing": true,
                "ajax": "<%=Application["WebUrl"]%>Ajax_Data/GetFullPriceData.ashx?Cls=<%=Get_ProdCls%>",
                "searching": true,  //搜尋
                "ordering": true,   //排序
                "paging": true,     //分頁
                "info": false,      //筆數資訊
                //指定欄位值
                "columns": [
                    {
                        "orderable": false,
                        "data": null,
                        "defaultContent": ''
                    },
                    { "data": "Stop_Offer", "orderable": false },
                    { "data": "Model_No" },
                    { "data": "ClassName_<%=fn_Language.Param_Lang %>" },
                    { "data": "Currency", "orderable": false },
                    { "data": "UnitPrice" },
                    { "data": "Unit", "orderable": false },
                    { "data": "QuoteDate" },
                    { "data": "UnitMOQ" },
                    { "data": "Vol" },
                    { "data": "Page" },
                    { "data": "InnerBox_Qty", "orderable": false },
                    { "data": "InnerBox_NW", "orderable": false },
                    { "data": "InnerBox_GW", "orderable": false },
                    { "data": "InnerBox_Cuft", "orderable": false },
                    { "data": "BarCode" },
                    { "data": "Packing_<%=fn_Language.Param_Lang %>", "orderable": false },
                    { "data": "Ship_From" },
                    { "data": "TransTermValue", "orderable": false }
                ],

                //讓不排序的欄位在初始化時不出現排序圖
                "order": [],
                //自訂欄位
                "columnDefs": [{
                    "targets": 0,   //第 n 欄
                    "data": "Model_No", //欄位資料
                    "render": function (data, type, full, meta) {
                        return urlFormat(data);
                    }
                },
                {
                    "targets": 2,   //第 n 欄
                    "width": "200px",
                    "data": "Model_Name_<%=fn_Language.Param_Lang %>", //欄位資料
                    "render": function (data, type, full, meta) {
                        return nameFormat(full);
                    }
                },
                {
                    "targets": 5,   //第 n 欄
                    "data": "myPrice", //欄位資料
                    "render": function (data, type, full, meta) {
                        var ShowPrice = full.ShowPrice;

                        return (ShowPrice == 'Y') ? moneyFormat(data, 2) : '-';
                    }
                }],
                "pageLength": 25,   //顯示筆數預設值  
                //捲軸設定
                "scrollY": '60vh',
                "scrollCollapse": true,
                "scrollX": true
            });
        });

        //回傳圖片路徑Html
        function urlFormat(d) {
            var data = encodeURIComponent(d.Model_No);
            return '<a href="<%=Application["API_WebUrl"]%>myProd/' + data + '/" data-fancybox-type="iframe" class="myPhoto btn btn-default" title="' + data + '">' +
              '<i class="fa fa-file-image-o"></i></a>';
        }

        //回傳名稱Html
        function nameFormat(d) {
            var eng = d.Model_Name_<%=fn_Language.Param_Lang %>;
            var html = '<p class="text-danger"><strong>' + d.Model_No + '</strong></p>';
            html += (eng.length > 70) ? '<span title="' + eng + '">' + eng.substr(0, 70) + '...</span>' : eng;

            return html;

        }

        //金額,取小數第2位,四捨五入
        function moneyFormat(num, pos) {
            var size = Math.pow(10, pos);
            return Math.round(num * size) / size;
        }
    </script>
    <%-- DataTable End --%>
    <%-- fancybox Start --%>
    <%: Scripts.Render("~/bundles/fancybox-script") %>
    <script>
        $(function () {
            //fancybox - 圖片顯示
            $(".myPhoto").fancybox({
                fitToView: true,
                autoSize: true,
                closeClick: true,
                openEffect: 'elastic',
                closeEffect: 'elastic',
                helpers: {
                    title: {
                        type: 'over'
                    }
                }
            });

        });
    </script>
    <%-- fancybox End --%>
</asp:Content>

