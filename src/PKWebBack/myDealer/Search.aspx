<%@ Page Title="銷售據點查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Dealer_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li class="active">銷售據點查詢</li>
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
                            <div class="col-sm-4">
                                <label>國家</label>
                                <select id="ddl_Country" class="form-control"></select>
                                <asp:TextBox ID="tb_DataValue" runat="server" Style="display: none;"></asp:TextBox>
                            </div>
                            <div class="col-sm-4">
                                <label>經銷商</label>
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
                    <a href="<%=Application["WebUrl"] %>Dealer/Edit/" class="text-center">
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
                                        <th style="width: 5%">編號</th>
                                        <th style="width: 25%">經銷商名稱</th>
                                        <th style="width: 37%">其他資訊</th>
                                        <th style="width: 10%">狀態</th>
                                        <th style="width: 8%">排序</th>
                                        <th style="width: 15%">&nbsp;</th>
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
                                    <%#Eval("Dealer_ID") %>
                                </td>
                                <td>
                                    <p>
                                        <span class="label label-warning"><%#Eval("AreaName") %></span>
                                        <span class="label label-primary"><%#Eval("Country_Name") %></span>
                                        <span class="label label-info"><%#Eval("CityName") %></span>
                                    </p>
                                    <p><%#Eval("Dealer_Name") %></p>
                                </td>
                                <td>
                                    <p><i class="fa fa-map-marker fa-fw"></i>&nbsp;<%#Eval("Dealer_Location") %></p>
                                    <p><i class="fa fa-envelope fa-fw"></i>&nbsp;<%#Eval("Dealer_Email") %></p>
                                </td>
                                <td class="text-center">
                                    <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                </td>
                                <td class="text-center">
                                    <%#Eval("Sort") %>
                                </td>
                                <td class="text-center">
                                    <a href="<%=Application["WebUrl"] %>Dealer/Edit/<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("Dealer_ID").ToString(), Application["DesKey"].ToString()))%>/" class="btn btn-primary">
                                        <i class="fa fa-pencil fa-lg"></i>
                                    </a>&nbsp;
                                    <asp:LinkButton ID="lbtn_Del" CommandName="Del" runat="server" CssClass="btn btn-danger" OnClientClick="return confirm('是否確定刪除?')">
                                        <i class="fa fa-trash-o fa-lg"></i>
                                    </asp:LinkButton>
                                    <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Dealer_ID") %>' />
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
    <%-- 連動式選單 Start --%>
    <script type="text/javascript">
        /* ----- Init Start ----- */
        $(function () {

            //宣告
            var menu_AreaCode = 'select#MainContent_ddl_AreaCode';
            var menu_Country = 'select#ddl_Country';

            //onchange事件 - 洲別
            $(menu_AreaCode).change(function () {
                var GetVal = $(menu_AreaCode + ' option:selected').val();
                //取得國家
                GetCountries(GetVal);

            });

            //onchange事件 - 國家
            $(menu_Country).change(function () {
                var GetVal = $(menu_Country + ' option:selected').val();

                //填入選擇的國家
                $("#MainContent_tb_DataValue").val(GetVal);

            });

            //觸發器 - 若洲別有帶預設值, 則自動觸發onchange
            $(menu_AreaCode).trigger("change");

        });
        /* ----- Init End ----- */

        /* 取得洲別國家 - 連動選單 Start */
        function GetCountries(AreaCode) {
            //宣告 - 取得物件,國家
            var myMenu = $('select#ddl_Country');
            myMenu.empty();
            myMenu.append($('<option></option>').val('').text('loading.....'));

            //判斷洲別編號是否空白
            if (AreaCode.length == 0) {
                SetMenuEmpty(myMenu);
                return false;
            }

            //這段必須加入, 不然會有No Transport的錯誤
            jQuery.support.cors = true;

            //API網址
            var uri = 'https://api.prokits.com.tw/place/countries/zh-tw/?AreaCode=' + AreaCode + '&showAll=N';

            // Send an AJAX request
            $.getJSON(uri)
                .done(function (data) {
                    //清空選項
                    myMenu.empty();

                    //加入選項
                    myMenu.append($('<option></option>').val('').text('-- 請選擇 --'));
                    $.each(data, function (key, item) {
                        myMenu.append($('<option></option>').val(item.CountryCode).text(item.CountryName + ' (' + item.CountryCode + ')'))
                    });

                    var getVal = $("#MainContent_tb_DataValue").val();
                    myMenu.val(getVal);

                })
                .fail(function (jqxhr, textStatus, error) {
                    var err = textStatus + ", " + error;
                    alert("無法取得資料\n\r" + err);
                });
        }

        //重設選單
        function SetMenuEmpty(menuID) {
            //清空選項
            menuID.empty();

            //加入空白選項
            menuID.append($('<option></option>').val('').text('-- 請先選擇洲別 --'));
        }
        /* 取得洲別國家 - 連動選單 End */
    </script>
    <%-- 連動式選單 End --%>
</asp:Content>

