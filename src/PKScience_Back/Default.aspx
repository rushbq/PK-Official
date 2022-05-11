<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="jumbotron">
                <h2>本網站建議使用Chrome瀏覽器，<a href="https://www.google.com/chrome/" target="_blank">點我下載</a></h2>
            </div>
        </div>
    </div>
    <div class="row">
        <asp:PlaceHolder ID="ph_mySearch" runat="server">
            <div class="col-md-6">
               <%-- <div class="panel panel-primary">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <h4>自訂報表查詢(Top 5)</h4>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#pal1" class="styleWhite">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="pal1" class="list-group collapse in">
                        <asp:Literal ID="lt_CU_List" runat="server"></asp:Literal>
                    </div>
                </div>--%>
            </div>
        </asp:PlaceHolder>
        <div class="col-md-6">
            <%-- <div class="panel panel-danger">
                <div class="panel-heading">
                    <div class="pull-left">
                        我的待辦事項
                    </div>
                    <div class="pull-right">
                        <a href="#more" class="showTip styleGreen" data-toggle="tooltip" data-placement="left" title="View all">
                            <span class="glyphicon glyphicon-list"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="list-group">
                    <a href="#" class="list-group-item">[專案工作] API 站台(Web資料交換)</a>
                    <a href="#" class="list-group-item">[專案工作] [產品中心] 新需求</a>
                    <a href="#" class="list-group-item">[程式處理] [產品中心] 新增產品訊息功能</a>
                    <a href="#" class="list-group-item">[專案工作] [25週年] 週年慶網站</a>
                </div>
            </div>--%>
        </div>
    </div>


    <%--  <div class="row">
        <div class="col-md-6">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <div class="pull-left">
                        <h4>2014 寶工實業 公司年度目標</h4>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#pal1" class="styleWhite">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="pal1" class="list-group collapse in">
                    <a class="list-group-item">
                        <h4 class="list-group-item-heading">業績目標</h4>
                        <p class="list-group-item-text">
                            外銷目標 $1,380萬美金, 挑戰 $1450萬美金；<br />
                            內銷目標 $4,000萬台幣, 挑戰 $4,200萬台幣；<br />
                            暢聯目標 $4,500人民幣, 挑戰 $5,000萬人民幣
                        </p>
                    </a>
                    <a class="list-group-item">
                        <h4 class="list-group-item-heading">專案目標</h4>
                        <p class="list-group-item-text">
                            籌劃2015年寶工25旅遊慶週年
                        </p>
                    </a>
                    <a class="list-group-item">
                        <h4 class="list-group-item-heading">永續目標</h4>
                        <p class="list-group-item-text">
                            持續改善提案，佔部門/個人 (20%)<br />
                            積極發展電子商務，以中國為據點發展全球電子商務
                        </p>
                    </a>
                </div>
            </div>
        </div>
        <div class="col-md-6">
            <div class="panel panel-primary">
                <!-- Default panel contents -->
                <div class="panel-heading">
                    <div class="pull-left">
                        <h4>2014 暢聯貿易 公司年度目標</h4>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#pal2" class="styleWhite">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="pal2" class="list-group collapse in">
                    <a class="list-group-item">銷售目標4,500萬人民幣,挑戰5,000萬人民幣
                    </a>
                    <a class="list-group-item">積極開拓新客戶,業績目標450萬人民幣
                    </a>
                    <a class="list-group-item">深耕電子商務,目標1,100萬人民幣,挑戰目標1,500萬人民幣
                    </a>
                    <a class="list-group-item">強化推廣Pro'skit新開發商品,業績目標500萬人民幣
                    </a>
                    <a class="list-group-item">落實執行產品發想,每季至少提案2項開發評估商品  
                    </a>
                    <a class="list-group-item">優化部門改造工程,每季至少提案2項部門改善執行方案
                    </a>
                    <a class="list-group-item">籌備暢聯10周年/上海寶工15周年/台灣寶工20周年慶典活動
                    </a>
                </div>
            </div>
        </div>
    </div>
   

    <!-- test -->
    <div class="row">
        <div class="col-lg-6">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        我的差勤記錄
                    </div>
                    <div class="pull-right">
                        <a href="#more" class="showTip styleGreen" data-toggle="tooltip" data-placement="left" title="View all">
                            <span class="glyphicon glyphicon-list"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div class="panel-body">
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs" role="tablist">
                        <li class="active"><a href="#home" role="tab" data-toggle="tab">資訊</a></li>
                        <li><a href="#waiting" role="tab" data-toggle="tab">待核假單</a></li>
                        <li><a href="#passed" role="tab" data-toggle="tab">已核假單</a></li>
                    </ul>

                    <!-- Tab panes -->
                    <div class="tab-content">
                        <div class="tab-pane fade in active" id="home">
                            <ul class="list-group">
                                <li class="list-group-item">
                                    <span class="badge badge-important">10</span>
                                    剩餘特休
                                </li>
                                <li class="list-group-item">
                                    <span class="badge badge-success">3</span>
                                    已請特休
                                </li>
                            </ul>
                        </div>
                        <div class="tab-pane fade" id="waiting">
                            <div class="table-responsive">
                                <table class="table table-advance table-striped">
                                    <thead>
                                        <tr>
                                            <th>假別</th>
                                            <th>日期起訖</th>
                                            <th>時數</th>
                                            <th>單別/單號</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <%for (int row = 0; row < 5; row++)
                                          { %>
                                        <tr>
                                            <td>特休</td>
                                            <td>2012-10-25 09:00<br />
                                                2012-10-26 18:00
                                            </td>
                                            <td>
                                                <span>2</span>&nbsp;天
                                            <span>0</span>&nbsp;小時

                                            </td>
                                            <td>STD002<br />
                                                0000011446 
                                            </td>
                                            <td class="text-center">
                                                <button type="button" class="btn btn-warning">
                                                    <span class="glyphicon glyphicon-folder-open"></span>
                                                </button>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                            <div>
                                <div class="pull-left styleGraylight">(只列出最近5筆記錄)</div>
                                <div class="pull-right"><a href="#">more...</a></div>
                                <div class="clearfix"></div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="passed">
                            <div class="table-responsive">
                                <table class="table table-advance table-striped">
                                    <thead>
                                        <tr>
                                            <th>假別</th>
                                            <th>日期起訖</th>
                                            <th>時數</th>
                                            <th>單別/單號</th>
                                            <th>&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <%for (int row = 0; row < 5; row++)
                                          { %>
                                        <tr>
                                            <td>特休</td>
                                            <td>2012-12-25 09:00<br />
                                                2012-12-26 18:00
                                            </td>
                                            <td>
                                                <span>2</span>&nbsp;天
                                             <span>0</span>&nbsp;小時
                                            </td>
                                            <td>STD002<br />
                                                0000011446 
                                            </td>
                                            <td class="text-center">
                                                <button type="button" class="btn btn-success">
                                                    <span class="glyphicon glyphicon-folder-open"></span>
                                                </button>
                                            </td>
                                        </tr>
                                        <%} %>
                                    </tbody>
                                </table>
                            </div>
                            <div>
                                <div class="pull-left styleGraylight">(只列出最近5筆記錄)</div>
                                <div class="pull-right"><a href="#">more...</a></div>
                                <div class="clearfix"></div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </div>
        <div class="col-lg-6">
        </div>
    </div>
    --%>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        $(function () {
            /* tooltip */
            $('.showTip').tooltip();
        });
    </script>
</asp:Content>
