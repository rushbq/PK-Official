<%@ Page Title="展覽活動內容設定" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit_Sub.aspx.cs" Inherits="Expo_Edit_Sub" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/Venobox-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
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
                <li><a href="<%=Page_LastUrl %>">
                    <asp:Literal ID="lt_NavSubject" runat="server">單頭資料</asp:Literal></a></li>
                <li class="active">語系內容設定</li>
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
                    </div>
                    <div class="clearfix"></div>
                </div>
                <div id="data" class="collapse in">
                    <div class="panel-body">
                        <!-- Content Start -->
                        <div class="form-horizontal">
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">識別名稱</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Group_Name" runat="server" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">目前語系</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_Lang" runat="server" CssClass="styleBlue"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">公告日期 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_Expo_PubDate">
                                            <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span>
                                        </div>
                                        <asp:TextBox ID="tb_Expo_PubDate" runat="server" Style="display: none"></asp:TextBox>

                                        <asp:RequiredFieldValidator ID="rfv_tb_Expo_PubDate" runat="server" ErrorMessage="請選擇「公告日期」" ControlToValidate="tb_Expo_PubDate" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">標題 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Title" runat="server" CssClass="form-control tip" placeholder="標題" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Expo_Title" runat="server" ErrorMessage="請填寫「標題」" ControlToValidate="tb_Expo_Title" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">簡述 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Desc" runat="server" CssClass="form-control tip" placeholder="簡述" MaxLength="500" ToolTip="字數上限 250 字" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Expo_Desc" runat="server" ErrorMessage="請填寫「簡述」" ControlToValidate="tb_Expo_Desc" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">網站</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Expo_Website" runat="server" CssClass="form-control tip" placeholder="網站" MaxLength="250" ToolTip="字數上限 200 字"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="rev_tb_Expo_Website" runat="server" ErrorMessage="「網站」輸入格式不正確" ValidationExpression="http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?" ControlToValidate="tb_Expo_Website" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">地理位置 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <asp:TextBox ID="tb_Expo_Location" runat="server" CssClass="form-control tip" placeholder="地址" MaxLength="150" ToolTip="字數上限 150 字" Width="70%"></asp:TextBox>
                                        <input type="button" class="btn btn-warning" value="查詢" onclick="codeAddress()">
                                    </div>

                                    <div style="padding: 5px 5px;">
                                        <div id="map-canvas" style="width: 100%; height: 400px;"></div>
                                    </div>

                                    <div class="form-inline">
                                        <label>Lat.</label>
                                        <asp:TextBox ID="tb_Expo_Lat" runat="server" CssClass="form-control" placeholder="座標-Lat"></asp:TextBox>

                                        <label>Long.</label>
                                        <asp:TextBox ID="tb_Expo_Lng" runat="server" CssClass="form-control" placeholder="座標-Long"></asp:TextBox>
                                    </div>
                                    <div class="help-block styleRed">
                                        <ul>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Expo_Location" runat="server" ErrorMessage="<li>請填寫「地址」並按下查詢.</li>" ControlToValidate="tb_Expo_Location" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Expo_Lat" runat="server" ErrorMessage="<li>「座標-Lat」尚未取得.</li>" ControlToValidate="tb_Expo_Lat" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                            <asp:RequiredFieldValidator ID="rfv_tb_Expo_Lng" runat="server" ErrorMessage="<li>「座標-Long」尚未取得.</li>" ControlToValidate="tb_Expo_Lng" Display="Dynamic" ValidationGroup="Add"></asp:RequiredFieldValidator>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">小圖</label>
                                <div class="col-sm-6 col-md-6 col-lg-8">
                                    <div class="pull-left">
                                        <asp:FileUpload ID="fu_Files" runat="server" />
                                        <asp:HiddenField ID="hf_OldFile" runat="server" />
                                    </div>
                                    <div class="pull-right">
                                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="help-block">
                                        (此圖將顯示在首頁/列表頁，上傳功能僅限PC使用，<kbd>建議大小：619*412</kbd>)
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-3 col-lg-2">
                                    <asp:PlaceHolder ID="ph_files" runat="server" Visible="false">
                                        <asp:Literal ID="lt_dwUrl" runat="server"></asp:Literal>&nbsp;
                                      <asp:LinkButton ID="lbtn_DelFile" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">攤位圖</label>
                                <div class="col-sm-6 col-md-6 col-lg-8">
                                    <div class="pull-left">
                                        <asp:FileUpload ID="fu_Files_Booth" runat="server" />
                                        <asp:HiddenField ID="hf_OldFile_Booth" runat="server" />
                                    </div>
                                    <div class="pull-right">
                                        <code>上傳限制：<%=FileExtLimit.Replace("|",", ") %></code>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="help-block">
                                        (此圖將顯示在攤位資訊，上傳功能僅限PC使用)
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-3 col-lg-2">
                                    <asp:PlaceHolder ID="ph_files_Booth" runat="server" Visible="false">
                                        <asp:Literal ID="lt_dwUrl_Booth" runat="server"></asp:Literal>&nbsp;
                                      <asp:LinkButton ID="lbtn_DelFile_Booth" runat="server" CssClass="btn btn-warning" OnClick="lbtn_DelFile_Booth_Click" CausesValidation="false" OnClientClick="return confirm('是否確定刪除檔案?')"><i class="fa fa-trash"></i></asp:LinkButton>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-12 text-right">
                                    <asp:Button ID="btn_Base_Save" runat="server" Text="儲存設定" CssClass="btn btn-primary" OnClick="btn_Save_Click" ValidationGroup="Add" OnClientClick="blockBox1('Add', '資料處理中...');" />
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
                        <a href="<%=Page_LastUrl %>" class="text-center">
                            <i class="glyphicon glyphicon-circle-arrow-left"></i>
                            <div class="status">返回單頭資料</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Form End -->
    <!-- Photos Start -->
    <asp:PlaceHolder ID="ph_NewsBlock" runat="server" Visible="false">
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
                                        <asp:Button ID="btn_Block_Save" runat="server" Text="開始上傳" CssClass="btn btn-primary" OnClick="btn_BlockSave_Click" ValidationGroup="BlockAdd" OnClientClick="blockBox1('BlockAdd', '資料處理中...');" />
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
                                        1. 此頁圖文排版僅供參考，實際畫面呈現以前台為主。<br />
                                        2. 拖曳區塊可自訂排序，排序完成後請按下「儲存版面排序」。
                                    </div>
                                </div>
                                <div class="col-xs-4 text-right">
                                    <asp:Button ID="btn_SaveBlock1" runat="server" Text="↓ 儲存版面排序 ↓" CssClass="btn btn-success" OnClick="btn_SaveSort_Click" ValidationGroup="List" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div>
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
                                </div>
                            </div>
                            <div class="text-right">
                                <asp:Button ID="btn_SaveBlock2" runat="server" Text="↑ 儲存版面排序 ↑" CssClass="btn btn-success" OnClick="btn_SaveSort_Click" ValidationGroup="List" />
                            </div>
                        </div>
                    </div>
                    <!-- Table Content End -->
                </div>
            </div>
            <div class="col-sm-3 col-md-2"></div>
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
    <script>
        $(function () {
            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });

            //延遲1秒
            setTimeout(function () {
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

            }, 1000);

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

            /* MultiFile 多筆上傳, 當瀏覽器為 ie 時才啟用 */
            if ($.browser.msie || (!!navigator.userAgent.match(/Trident\/7\./))) {
                $('#MainContent_fu_Photos').MultiFile({
                    STRING: {
                        remove: 'X' //移除圖示
                    },
                    accept: '<%=FileExtLimit %>' //副檔名限制
                });
            }

            /* lazyload */
            $("img.lazy").lazyload({
                effect: "fadeIn",
                event: "loadme"
            });
            //觸發Lazyload
            $("img.lazy").trigger("loadme");

        });

    </script>
    <%-- DatePicker Start --%>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <script>
        $(function () {
            $('.showDate').datetimepicker({
                format: 'yyyy/mm/dd',   //目前欄位格式
                //linkField: 'dtp_input2',    //鏡像欄位對應
                linkFormat: 'yyyy/mm/dd',   //鏡像欄位格式
                todayBtn: true,     //顯示today
                todayHighlight: true,   //將today設置高亮
                autoclose: true,    //選擇完畢後自動關閉
                startView: 2,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
                maxView: 4,
                minView: 2,
                forceParse: false
                //showMeridian: true, //顯示AM/PM
            });
        });

    </script>
    <%-- DatePicker End --%>

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

    <%-- GMAP API Start --%>
    <script src="https://maps.googleapis.com/maps/api/js?v=3.exp"></script>
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
            var address = document.getElementById('MainContent_tb_Expo_Location').value;
            var fldlat = document.getElementById('MainContent_tb_Expo_Lat');
            var fldlng = document.getElementById('MainContent_tb_Expo_Lng');

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

    </script>
    <%-- GMAP API End --%>
</asp:Content>
