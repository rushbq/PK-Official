<%@ Page Title="銷售據點維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Dealer_Edit" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/Venobox-css") %>
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <style>
        .dragMe {
            cursor: all-scroll;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Application["WebUrl"] %>Dealer/Search/">銷售據點查詢</a></li>
                <li class="active">銷售據點維護</li>
            </ol>
        </div>
    </div>
    <!-- Page Header End -->
    <!-- Form Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-edit"></span>
                        <span>基本設定</span>
                    </div>
                    <div class="pull-right">
                        <a data-toggle="collapse" href="#data">
                            <span class="glyphicon glyphicon-sort"></span>
                        </a>
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">資料編號</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_DataID" runat="server" Text="系統自動編號" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">洲別 / 國家 / 地區<em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10 form-inline">
                                    <div class="form-inline">
                                        <asp:DropDownList ID="ddl_AreaCode" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_AreaCode_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:DropDownList ID="ddl_Country" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_Country_SelectedIndexChanged">
                                            <asp:ListItem Value="">請先選擇洲別</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:DropDownList ID="ddl_City" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="">請先選擇國家</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfv_Country" runat="server" ErrorMessage="請選擇「國家」" ControlToValidate="ddl_Country" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">經銷商名稱 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Dealer_Name" runat="server" CssClass="form-control tip" placeholder="經銷商名稱" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Dealer_Name" runat="server" ErrorMessage="請填寫「經銷商名稱」" ControlToValidate="tb_Dealer_Name" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">地理位置 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-group">
                                        <div class="col-sm-12">
                                            <div class="form-inline">
                                                <label>地址:</label>
                                                <asp:TextBox ID="tb_Dealer_Location" runat="server" CssClass="form-control tip" placeholder="地址" MaxLength="150" ToolTip="字數上限 150 字" Width="70%"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-sm-12">
                                            <label>
                                                如何取得座標:&nbsp;
                                                <a href="<%=Application["WebUrl"] %>myDealer/GetLatLng1.jpg" target="_blank">方式一</a>、
                                                <a href="<%=Application["WebUrl"] %>myDealer/GetLatLng2.png" target="_blank">方式二</a>
                                            </label>
                                            <div class="form-inline">
                                                <div class="input-group margin-bottom-sm">
                                                    <span class="input-group-addon">Lat</span>
                                                    <asp:TextBox ID="tb_Dealer_Lat" runat="server" CssClass="form-control" placeholder="座標-Lat" MaxLength="30"></asp:TextBox>
                                                </div>
                                                <div class="input-group margin-bottom-sm">
                                                    <span class="input-group-addon">Long</span>
                                                    <asp:TextBox ID="tb_Dealer_Lng" runat="server" CssClass="form-control" placeholder="座標-Long" MaxLength="30"></asp:TextBox>
                                                </div>
                                                <br />
                                                <small>(2018/7, google API開始收費,故無法使用自動取得座標)</small>
                                            </div>
                                        </div>

                                    </div>

                                    <%--
                                     <div class="form-inline">
                                        <asp:TextBox ID="tb_Dealer_Location" runat="server" CssClass="form-control tip" placeholder="地址" MaxLength="150" ToolTip="字數上限 150 字" Width="70%"></asp:TextBox>
                                        <input type="button" class="btn btn-warning" value="查詢" onclick="codeAddress()" />
                                    </div>

                                    <div style="padding: 5px 5px;">
                                        <div id="map-canvas" style="width: 100%; height: 400px;"></div>
                                    </div>

                                    <div class="form-inline">
                                        <label>Lat.</label>
                                        <asp:TextBox ID="tb_Dealer_Lat" runat="server" CssClass="form-control" placeholder="座標-Lat"></asp:TextBox>

                                        <label>Long.</label>
                                        <asp:TextBox ID="tb_Dealer_Lng" runat="server" CssClass="form-control" placeholder="座標-Long"></asp:TextBox>
                                    </div>
                                    <div class="help-block styleRed">
                                        <ul>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Dealer_Location" runat="server" ErrorMessage="<li>請填寫「地址」並按下查詢.</li>" ControlToValidate="tb_Dealer_Location" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Dealer_Lat" runat="server" ErrorMessage="<li>「座標-Lat」尚未取得.</li>" ControlToValidate="tb_Dealer_Lat" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Dealer_Lng" runat="server" ErrorMessage="<li>「座標-Long」尚未取得.</li>" ControlToValidate="tb_Dealer_Lng" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                        </ul>
                                    </div>--%>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">聯絡資訊</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group margin-bottom-sm">
                                            <span class="input-group-addon"><i class="fa fa-user fa-fw"></i></span>
                                            <asp:TextBox ID="tb_Dealer_Contact" runat="server" CssClass="form-control" placeholder="聯絡人" MaxLength="30"></asp:TextBox>
                                        </div>
                                        <div class="input-group margin-bottom-sm">
                                            <span class="input-group-addon"><i class="fa fa-fax fa-fw"></i></span>
                                            <asp:TextBox ID="tb_Dealer_Fax" runat="server" CssClass="form-control" placeholder="傳真" MaxLength="30"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="input-group margin-bottom-sm">
                                        <span class="input-group-addon"><i class="fa fa-envelope-o fa-fw"></i></span>
                                        <asp:TextBox ID="tb_Dealer_Email" runat="server" CssClass="form-control" placeholder="Email address" MaxLength="150"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="rev_tb_Dealer_Email" runat="server" ErrorMessage="「Email」輸入格式不正確" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="tb_Dealer_Email" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                    </div>
                                    <div class="input-group margin-bottom-sm">
                                        <span class="input-group-addon"><i class="fa fa-globe fa-fw"></i></span>
                                        <asp:TextBox ID="tb_Dealer_Website" runat="server" CssClass="form-control" placeholder="Website" MaxLength="250"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="rev_tb_Dealer_Website" runat="server" ErrorMessage="「網站」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Dealer_Website" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                    </div>
                                    <div class="form-inline">
                                        <div class="input-group margin-bottom-sm">
                                            <span class="input-group-addon"><i class="fa fa-phone fa-fw"></i></span>
                                            <asp:TextBox ID="tb_Tel" runat="server" CssClass="form-control" placeholder="電話 (可多筆新增)" MaxLength="30"></asp:TextBox>
                                            <span class="input-group-btn">
                                                <a class="btn btn-default" href="javascript:;" id="newTag"><i class="fa fa-plus"></i></a>
                                            </span>
                                        </div>
                                        <input type="hidden" id="tb_TagName" />
                                        <input type="hidden" id="tb_TagID" />
                                        <div>
                                            <asp:TextBox ID="tb_All_itemID" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                            </asp:TextBox>
                                            <asp:TextBox ID="tb_All_itemName" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                            </asp:TextBox>
                                            <ul class="list-inline" id="myTags">
                                                <asp:Literal ID="lt_myItems" runat="server"></asp:Literal>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">顯示狀態</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">排序 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">備註</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Remark" runat="server" CssClass="form-control tip" placeholder="備註" MaxLength="300" ToolTip="字數上限 150 字" TextMode="MultiLine" Rows="3"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Literal ID="lt_CreateInfo" runat="server"><p class="form-control-static help-block">資料新增中...</p></asp:Literal>
                                    <asp:Literal ID="lt_UpdateInfo" runat="server"></asp:Literal>
                                </div>

                            </div>

                            <asp:HiddenField ID="hf_flag" runat="server" Value="Add" />
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Add" ShowMessageBox="true" ShowSummary="false" />
                        </div>
                        <!-- Content End -->
                    </div>
                </div>
            </div>
        </div>
        <!-- buttons -->
        <div class="col-sm-3 col-md-2">
            <div class="FormBtn-fixPos">
                <div class="metro-nav metro-fix-view">
                    <div class="metro-nav-block nav-block-green">
                        <a href="<%=Page_SearchUrl %>" class="text-center">
                            <i class="fa fa-list"></i>
                            <div class="status">返回列表</div>
                        </a>
                    </div>
                    <asp:PlaceHolder ID="ph_Save" runat="server">
                        <div class="metro-nav-block nav-block-blue">
                            <a id="triggerSave" class="text-center">
                                <i class="fa fa-floppy-o"></i>
                                <div class="status">資料存檔</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_Delete" runat="server" Visible="false">
                        <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定刪除?')" OnClick="lbtn_Delete_Click">
                            <i class="glyphicon glyphicon-trash"></i>
                            <div class="status">刪除資料</div>
                            </asp:LinkButton>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div style="display: none;">
                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
            </div>
        </div>
    </div>
    <!-- Form End -->
    <!-- Photos Start -->
    <asp:PlaceHolder ID="ph_myBlock" runat="server" Visible="false">
        <!-- Block Add Start -->
        <div class="row">
            <div class="col-sm-9 col-md-10">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <i class="fa fa-picture-o"></i>
                            <span>圖片集</span>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#block">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="block" class="collapse in">
                        <div class="panel-body">
                            <!-- Content Start -->
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label class="control-label col-sm-3 col-md-3 col-lg-2">圖片集上傳</label>
                                    <div class="col-sm-9 col-md-9 col-lg-10">
                                        <div class="pull-left">
                                            <asp:FileUpload ID="fu_Photos" runat="server" AllowMultiple="True" CssClass="tip" ToolTip="支援多筆上傳, IE瀏覽器須單筆多次點選" />
                                        </div>
                                        <div class="pull-right">
                                            <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                        </div>
                                        <div class="clearfix"></div>
                                        <div class="help-block">
                                            (上傳功能僅限PC使用)
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfv_fu_Photos" runat="server" ErrorMessage="請選擇要上傳的圖片" ControlToValidate="fu_Photos" Display="Dynamic" ValidationGroup="BlockAdd" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="text-right">
                                        <asp:Button ID="btn_Block_Save" runat="server" Text="開始上傳" CssClass="btn btn-primary" ValidationGroup="BlockAdd" OnClick="btn_BlockSave_Click" OnClientClick="blockBox1('BlockAdd', '資料處理中...');" />
                                        <asp:ValidationSummary ID="ValidationSummary2" runat="server" ValidationGroup="BlockAdd" ShowMessageBox="true" ShowSummary="false" />
                                    </div>
                                </div>
                            </div>
                            <!-- Content End -->
                        </div>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
            <div class="col-sm-3 col-md-2"></div>
        </div>
        <!-- Block Add End -->
        <!-- Block List Start -->
        <div class="row">
            <div class="col-sm-9 col-md-10">
                <div class="panel panel-info">
                    <div class="panel-heading">
                        <div class="pull-left">
                            <span class="glyphicon glyphicon-list"></span>
                            <span>圖片列表</span>
                        </div>
                        <div class="pull-right">
                            <a data-toggle="collapse" href="#blocklist">
                                <span class="glyphicon glyphicon-sort"></span>
                            </a>
                        </div>
                        <div class="clearfix"></div>
                    </div>
                    <div id="blocklist" class="collapse in">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-xs-8">
                                    <div class="well well-sm">
                                        1. 此頁排版僅供參考，實際畫面呈現以前台為主。<br />
                                        2. 拖曳區塊可自訂排序，排序完成後請按下「儲存版面排序」。
                                    </div>
                                </div>
                                <div class="col-xs-4 text-right">
                                    <asp:Button ID="btn_SaveBlock1" runat="server" Text="↓ 儲存版面排序 ↓" CssClass="btn btn-success" ValidationGroup="List" OnClick="btn_SaveSort_Click" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items" OnItemCommand="lvDataList_ItemCommand" OnItemDataBound="lvDataList_ItemDataBound">
                                        <LayoutTemplate>
                                            <ul id="draggableList" class="list-inline">
                                                <asp:PlaceHolder ID="ph_Items" runat="server" />
                                            </ul>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <li class="dragItem">
                                                <div class="thumbnail dragMe">
                                                    <asp:Literal ID="lt_FileThumb" runat="server"></asp:Literal>
                                                    <div class="caption">
                                                        <asp:Literal ID="lt_dwUrl" runat="server"></asp:Literal>
                                                        <asp:LinkButton ID="lbtn_Delete" runat="server" CommandName="Del" CssClass="btn btn-warning"
                                                            OnClientClick="return confirm('是否確定刪除!?')"><i class="fa fa-trash"></i></asp:LinkButton>

                                                        <asp:HiddenField ID="hf_DataID" runat="server" Value='<%#Eval("Pic_ID") %>' />
                                                        <asp:HiddenField ID="hf_OldFile" runat="server" Value='<%#Eval("Pic_File") %>' />
                                                        <asp:TextBox ID="tb_Sort" runat="server" Text='<%#Eval("Sort") %>' CssClass="tb-sortid hidden"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <div class="text-center styleReddark" style="margin: 0 auto;">
                                                <h3><span class="glyphicon glyphicon-exclamation-sign"></span>&nbsp;尚未上傳圖片</h3>
                                            </div>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </div>
                                <div class="text-right">
                                    <asp:Button ID="btn_SaveBlock2" runat="server" Text="↑ 儲存版面排序 ↑" CssClass="btn btn-success" ValidationGroup="List" OnClick="btn_SaveSort_Click" />
                                </div>
                            </div>
                        </div>
                        <!-- Table Content End -->
                    </div>
                </div>
                <div class="col-sm-3 col-md-2"></div>
            </div>
        </div>
        <!-- Block List End -->
    </asp:PlaceHolder>
    <!-- Photos End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/lazyload-script") %>
    <%: Scripts.Render("~/bundles/MultiFile-script") %>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>

    <%-- 一般設定 Start --%>
    <script>
        $(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                //取得動態欄位值
                Get_Item();

                //block-ui
                blockBox1('Add', '資料處理中...');

                //觸發
                $('#MainContent_btn_doSave').trigger('click');
            });

            /* Click事件 - 自訂Tag */
            $("#newTag").click(function () {
                //重置欄位值
                $("#tb_TagID").val('0');
                $("#tb_TagName").val($("#MainContent_tb_Tel").val());

                //新增項目
                Add_Item();
            });

            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });

            //使用jQueryUI 將radio Group
            $(".showRadioGrp").buttonset();

            //延遲1秒
<%--            setTimeout(function () {
                //滑動到指定內容區 Start
                <% if (Param_thisID != "")
                   { %>
                //取得元素
                var _thisID = $('#block');

                //滑動至指定ID
                $('html, body').animate({
                    scrollTop: $(_thisID).offset().top - 100
                }, 500);
                //$('#data').collapse('hide');

                <%}%>
                //滑動到指定內容區 End

            }, 1000);--%>

            /* jQuery sortable Start */
            var panelList = $('#draggableList');

            panelList.sortable({
                handle: '.dragMe',
                update: function () {
                    $('.dragItem', panelList).each(function (index, elem) {
                        var $listItem = $(elem),
                            newIndex = $listItem.index();

                        //填入排序
                        var newSort = newIndex + 1;
                        $(this).find('.tb-sortid').val(newSort);

                    });
                }
            });
            /* jQuery sortable End */

            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme"
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");

        });
    </script>
    <%-- 一般設定 End --%>


    <%-- GMAP API Start --%>
    <%-- <script src="https://maps.googleapis.com/maps/api/js?v=3.exp"></script>
    <script>
        var geocoder;
        var map;
        function initialize() {
            geocoder = new google.maps.Geocoder();
            var latlng = new google.maps.LatLng('<%=Param_Lat%>', '<%=Param_Lng%>');
            var mapOptions = {
                zoom: 15,
                center: latlng
            }
            map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

            var marker = new google.maps.Marker({
                map: map,
                position: latlng
            });
        }

        function codeAddress() {
            //宣告
            var address = document.getElementById('MainContent_tb_Dealer_Location').value;
            var fldlat = document.getElementById('MainContent_tb_Dealer_Lat');
            var fldlng = document.getElementById('MainContent_tb_Dealer_Lng');

            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    //Get Location
                    var getLocation = results[0].geometry.location;

                    map.setCenter(getLocation);
                    var marker = new google.maps.Marker({
                        map: map,
                        position: getLocation
                    });

                    //Get Lat/Long
                    fldlat.value = getLocation.lat();
                    fldlng.value = getLocation.lng();

                } else {
                    alert('Geocode was not successful for the following reason: ' + status);
                }
            });
        }

        google.maps.event.addDomListener(window, 'load', initialize);

    </script>--%>
    <%-- GMAP API End --%>

    <script>
        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjVal = $("#tb_TagName").val();
            var ObjValID = $("#tb_TagID").val();
            if (ObjVal == "") {
                alert('欄位空白!');
                return;
            }
            var NewItem = '<li id="li_' + ObjId + '" style="padding-top:5px;">';
            NewItem += '<input type="hidden" class="item_ID" value="' + ObjValID + '" />';
            NewItem += '<input type="hidden" class="item_Name" value="' + ObjVal + '" />';
            NewItem += '<a href="javascript:Delete_Item(\'' + ObjId + '\');" class="btn btn-success">' + ObjVal + '&nbsp;<span class="glyphicon glyphicon-trash"></span></a>';
            NewItem += '</li>';

            //將項目append到指定控制項
            $("#myTags").append(NewItem);
            //清空欄位
            $("#MainContent_tb_Tel").val('').focus();
        }

        /* 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }

        function Delete_AllItem() {
            $("#myTags li").each(
               function (i, elm) {
                   $(elm).remove();
               });
        }

        /* 取得各項目欄位值
        分隔符號 : ||
        */
        function Get_Item() {
            //取得控制項, ServerSide
            var fld_itemID = $("#MainContent_tb_All_itemID");
            var fld_itemName = $("#MainContent_tb_All_itemName");

            //清空欄位值
            fld_itemID.val('');
            fld_itemName.val('');

            //巡覽項目, 填入值
            $("#myTags li .item_ID").each(
                function (i, elm) {
                    var OldCont = fld_itemID.val();
                    if (OldCont == '') {
                        fld_itemID.val($(elm).val());
                    } else {
                        fld_itemID.val(OldCont + '||' + $(elm).val());
                    }
                }
            );

            $("#myTags li .item_Name").each(
                function (i, elm) {
                    var OldCont = fld_itemName.val();
                    if (OldCont == '') {
                        fld_itemName.val($(elm).val());
                    } else {
                        fld_itemName.val(OldCont + '||' + $(elm).val());
                    }
                }
            );
        }
        //----- 動態欄位 End -----
    </script>

    <%-- VenoBox Start --%>
    <%: Scripts.Render("~/bundles/Venobox-script") %>
    <script>
        $(function () {
            /* Venobox */
            $('.zoomPic').venobox({
                border: '10px',
                bgcolor: '#ffffff',
                numeratio: true,
                infinigall: true
            });
        });
    </script>
    <%-- VenoBox End --%>

    <script>
        $(function () {
            /* MultiFile 多筆上傳, 當瀏覽器為 ie 時才啟用 */
            if ($.browser.msie || (!!navigator.userAgent.match(/Trident\/7\./))) {
                $('#MainContent_fu_Photos').MultiFile({
                    STRING: {
                        remove: 'X' //移除圖示
                    },
                    accept: '<%=FileExtLimit %>' //副檔名限制
                });
            }

        });
    </script>
</asp:Content>

