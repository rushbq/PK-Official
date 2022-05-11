<%@ Page Title="會員資料查詢" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Search.aspx.cs" Inherits="Member_Search" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <div class="page-header">
                <h2>會員資料查詢
                    <small>
                        <a>會員管理</a>&nbsp;
                        <i class="fa fa-chevron-right"></i>&nbsp;<span>會員資料查詢</span>
                    </small>
                </h2>
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
                        <div class="col-sm-3">
                            <label>洲別</label>
                            <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <label>國家</label>
                            <select id="ddl_Country" class="form-control"></select>
                            <asp:TextBox ID="tb_DataValue" runat="server" Style="display: none;"></asp:TextBox>
                        </div>
                        <div class="col-sm-6">
                            <label>關鍵字</label>
                            <asp:TextBox ID="tb_Keyword" runat="server" CssClass="form-control" placeholder="Email, 會員名, 公司名" MaxLength="20"></asp:TextBox>
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
                                <asp:Button ID="btn_Excel" runat="server" CssClass="btn btn-primary btn-sm" Text="匯出Excel" CausesValidation="false" OnClick="btn_Excel_Click" />
                            </div>
                            <div class="clearfix"></div>
                        </div>
                        <!-- Table Content Start -->
                        <div id="result" class="table-responsive">
                            <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                                <LayoutTemplate>
                                    <table class="table table-bordered table-advance table-striped">
                                        <thead>
                                            <tr>
                                                <th style="width: 35%">Email / 姓名</th>
                                                <th style="width: 15%">帳號狀態</th>
                                                <th style="width: 15%">回填資料</th>
                                                <th style="width: 20%">註冊日</th>
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
                                        <td>
                                            <p>
                                                <strong><%#Eval("Mem_Account") %></strong>
                                            </p>
                                            <p>
                                                <label class="label label-default"><%#Eval("Country_Name") %></label>
                                                <asp:Literal ID="lt_Social" runat="server"></asp:Literal>
                                                <%#Eval("FirstName") %>&nbsp;<%#Eval("LastName") %>
                                            </p>
                                            <p class="text-danger"><%#Eval("CustName") %></p>
                                        </td>
                                        <td class="text-center">
                                            <asp:Label ID="lb_Status" runat="server"></asp:Label>
                                        </td>
                                        <td class="text-center">
                                            <%#fn_Desc.PubAll.YesNo(Eval("IsWrite").ToString()) %>
                                        </td>
                                        <td class="text-center">
                                            <%#Eval("Create_Time").ToString().ToDateString("yyyy-MM-dd") %>
                                        </td>
                                        <td class="text-center">
                                            <a href="<%=Application["WebUrl"] %>Member/Edit/<%# Server.UrlEncode(Cryptograph.MD5Encrypt(Eval("Mem_ID").ToString(), Application["DesKey"].ToString()))%>/" class="btn btn-primary">
                                                <i class="fa fa-pencil fa-lg"></i>
                                            </a>

                                            <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Mem_ID") %>' />
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

