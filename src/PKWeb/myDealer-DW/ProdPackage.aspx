<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ProdPackage.aspx.cs" Inherits="myDealer_ProdPackage" %>

<%@ Register Src="~/myController/Ascx_Navi.ascx" TagName="Ascx_Navi" TagPrefix="ucNavi" %>
<%@ Register Src="~/myController/Ascx_FloatBar.ascx" TagName="Ascx_FloatBar" TagPrefix="ucFloatBar" %>
<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/distributor-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="container df-container-margin">
        <!-- 路徑導航 Start -->
        <ucNavi:Ascx_Navi ID="Ascx_Navi1" runat="server" Param_CurrID="306" />
        <!-- 路徑導航 End -->
        <!-- Content Start -->
        <!-- 左側浮動選單, MD LG 螢幕顯示 Start -->
        <div class="float-menu visible-md visible-lg">
            <ucFloatBar:Ascx_FloatBar ID="Ascx_FloatBar1" runat="server" Param_CurrItem="2" Param_MenuType="floatbar" />
        </div>
        <!-- 左側浮動選單, MD LG 螢幕顯示 End -->
        <div>
            <!-- Title -->
            <div class="download-title">
                <div class="pull-left"><%=Resources.resPublic.bar_包材檔案 %></div>
                <!-- 功能選擇 -->
                <div class="pull-right visible-xs visible-sm">
                    <ucFloatBar:Ascx_FloatBar ID="Ascx_FloatBar2" runat="server" Param_CurrItem="2" Param_MenuType="dropdownmenu" />
                </div>
                <div class="clearfix"></div>
            </div>
            <!-- 下載內容 Start -->
            <div class="download-content">
                <table id="listTable" class="table table-bordered table-striped">
                    <thead>
                        <tr>
                            <th style="width: 30%"><%=this.GetLocalResourceObject("title_產品名稱").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_彩盒").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_彩標").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_貼紙").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_卡片").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_Pounch袋").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_袖套").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_吊卡").ToString()%></th>
                            <th><%=this.GetLocalResourceObject("title_其他").ToString()%></th>
                        </tr>
                    </thead>
                </table>
                <div id="divModal"></div>
            </div>
            <!-- 下載內容 End -->
        </div>
        <!-- Content End -->
    </div>

</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/floatBar-script") %>
    <%-- DataTable Start --%>
    <link href="<%=CDNUrl %>plugin/dataTables/dataTables.bootstrap.min.css" rel="stylesheet" />
    <script src="<%=CDNUrl %>plugin/dataTables/jquery.dataTables.min.js"></script>
    <script src="<%=CDNUrl %>plugin/dataTables/dataTables.bootstrap.min.js"></script>
    <script>
        $(function () {
            //使用DataTable
            var table =
             $('#listTable').DataTable({
                 "processing": true,
                 "serverSide": true,
                 "ajax": {
                     "url": "<%=Application["WebUrl"]%>Ajax_Data/GetData_ProdPackage.ashx",
                     "type": "POST"
                 },
                 "searching": true,  //搜尋
                 "ordering": false,   //排序(此功能待研究)
                 "paging": true,     //分頁
                 "info": true,      //頁數資訊
                 "lengthChange": false,  //是否顯示筆數選單
                 //自訂顯示欄位
                 "columns": [
                     {
                         //圖片/品號/品名
                         "data": function (source, type, val) {
                             var showID = source.ID
                             var showName = source.Label
                             var imgUrl = '<%=Param_ImgUrl%>' + fixedEncodeURIComponent(showID) + '/'
                             //Html
                             var html = '<div class="media"><div class="media-left"><img class="media-object" alt="" src="' + imgUrl + '" data-holder-rendered="true" /></div>' +
                                 '<div class="media-body"><h4 class="media-heading">' + showID + '</h4>' + showName + '</div></div>'

                             return html;
                         }
                     },
                    {//彩盒<4>
                        "data": function (source) {
                            return urlFormat(source, source.myFile1, 4);
                        },
                        "className": 'text-center myModal'
                    },
                    {//彩標<5>
                        "data": function (source) {
                            return urlFormat(source, source.myFile2, 5);
                        },
                        "className": 'text-center myModal'
                    },
                    {//貼紙<6>
                        "data": function (source) {
                            return urlFormat(source, source.myFile3, 6);
                        },
                        "className": 'text-center myModal'
                    },
                    {//卡片<7>
                        "data": function (source) {
                            return urlFormat(source, source.myFile4, 7);
                        },
                        "className": 'text-center myModal'
                    },
                    {//Pounch袋<9>
                        "data": function (source) {
                            return urlFormat(source, source.myFile5, 9);
                        },
                        "className": 'text-center myModal'
                    },
                    {//袖套<11>
                        "data": function (source) {
                            return urlFormat(source, source.myFile6, 11);
                        },
                        "className": 'text-center myModal'
                    },
                    {//吊卡<12>
                        "data": function (source) {
                            return urlFormat(source, source.myFile7, 12);
                        },
                        "className": 'text-center myModal'
                    },
                    {//其他<99>
                        "data": function (source) {
                            return urlFormat(source, source.myFile99, 99);
                        },
                        "className": 'text-center myModal'
                    }
                 ],
                 "pageLength": 20,   //顯示筆數預設值
                 "language": {
                     //自訂頁數資訊
                     "info": 'Total <strong class="text-success">_TOTAL_</strong> ,Current page <strong class="text-success">_PAGE_</strong>/_PAGES_'
                 },
                 //捲軸設定
                 "scrollY": '50vh',
                 "scrollCollapse": true,
                 "scrollX": true
             });

            //回傳欄位Url
            function urlFormat(data, rowCnt, col) {
                var html = '';
                if (rowCnt > 0) {
                    var showID = data.ID
                    var rowID = data.RowRank
                    var url = '<%=Application["WebUrl"]%>myDealer-DW/html_PackFiles.aspx?DataID=' + fixedEncodeURIComponent(showID) + '&c=' + fixedEncodeURIComponent(col);
                    html = '<a href="' + url + '" class="btn btn-default" data-toggle="modal" data-target="#remoteModal-' + rowID + '"><i class="fa fa-folder-open" aria-hidden="true"></i></a>';
                }

                return html;
            }

            //Click事件, 加入遠端Modal模組
            $('#listTable tbody').on('click', 'td.myModal', function () {
                //取得上一層tr
                var tr = $(this).closest('tr');
                //取得資料
                var row = table.row(tr);
                //將modal加入暫存div
                $('#divModal').html(myFormat(row.data()))

            });
            //暫存div
            function myFormat(d) {
                var rowID = d.RowRank

                return '<div class="modal fade" id="remoteModal-' + rowID + '" tabindex="-1" role="dialog" aria-labelledby="remoteModalLabel" aria-hidden="true">'
                    + '<div class="modal-dialog"><div class="modal-content"></div></div></div>';
            }
        });
    </script>
    <%-- DataTable End --%>
</asp:Content>

