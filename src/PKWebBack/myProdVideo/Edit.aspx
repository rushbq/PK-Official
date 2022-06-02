<%@ Page Title="產品影片維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="PV_Edit" ValidateRequest="false" %>

<%@ Import Namespace="ExtensionMethods" %>
<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
    <%: Styles.Render("~/bundles/JQ-UI-css") %>
    <%: Styles.Render("~/bundles/DTpicker-css") %>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="Server">
    <!-- Page Header Start -->
    <div class="row">
        <div class="col-md-12">
            <ol class="breadcrumb">
                <li><a href="<%=Application["WebUrl"] %>">首頁</a></li>
                <li><a href="<%=Application["WebUrl"] %>PV/Search/">產品影片查詢</a></li>
                <li class="active">產品影片維護</li>
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
                        <span>群組基本設定</span>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">資料編號</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <p class="form-control-static">
                                        <strong>
                                            <asp:Label ID="lb_DataID" runat="server" Text="系統自動編號" CssClass="styleGreen"></asp:Label></strong>
                                    </p>
                                </div>
                            </div>

                            <div class="form-group hide">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_cbl_Area">區域 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:CheckBoxList ID="cbl_Area" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                    <asp:CustomValidator ID="cv_check_Area" runat="server" ErrorMessage="請選擇「區域」" Display="Dynamic"
                                        ClientValidationFunction="check_AreaCode" ValidationGroup="Add" CssClass="styleRed help-block"></asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">識別名稱 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Group_Name" runat="server" CssClass="form-control tip" placeholder="識別名稱" MaxLength="150" ToolTip="字數上限 70 字"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Group_Name" runat="server" ErrorMessage="請填寫「識別名稱」" ControlToValidate="tb_Group_Name" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <%--<div class="form-group" style="display:none;">
                                <!-- 前台已移除判斷 -->
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架日期 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_StartDate">
                                            <asp:TextBox ID="show_sDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>

                                        </div>
                                        ~
                                        <div class="input-group date showDate" data-link-field="MainContent_tb_EndDate">
                                            <asp:TextBox ID="show_eDate" runat="server" CssClass="form-control text-center" ReadOnly="true"></asp:TextBox>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-remove"></span></span>
                                            <span class="input-group-addon"><span class="glyphicon glyphicon-time"></span></span>
                                        </div>
                                    </div>
                                    <div class="help-block">
                                        (若結束時間空白,系統將自動填入開始時間 +5 年)
                                    </div>
                                    <div>
                                        <asp:RequiredFieldValidator ID="rfv_sDate" runat="server" ErrorMessage="請選擇「開始時間」" ControlToValidate="tb_StartDate" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                        <asp:TextBox ID="tb_StartDate" runat="server" Style="display: none"></asp:TextBox>
                                        <asp:TextBox ID="tb_EndDate" runat="server" Style="display: none"></asp:TextBox>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架狀態</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                    <%--2021/3/18 remove (Nikki)
                                        <div class="help-block">
                                        <asp:Label ID="lb_SetMsg" runat="server" Text="(目前語系資料尚未完成設定,無法修改上架狀態)"></asp:Label>
                                    </div>--%>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">所屬區塊</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_ClassType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                            <asp:ListItem Value="A" Selected="True">工具</asp:ListItem>
                                            <asp:ListItem Value="B">玩具</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Sort">排序 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「排序」"
                                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                    <div class="help-block">
                                        (前台顯示的排序,若排序相同,則依照上架開始日期由新到舊排序)
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">
                                    關聯產品品號 <i class="fa fa-link"></i><em>*</em>
                                    <br />
                                    (產品連結條件)</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_myFilterItem" runat="server" CssClass="form-control tip" placeholder="輸入品號關鍵字"></asp:TextBox>
                                    <input type="hidden" id="tb_myItemID" />
                                    <span class="help-block">(輸入品號關鍵字, 選擇項目後會自動加入列表)</span>

                                    <div>
                                        <asp:TextBox ID="val_Items" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                        </asp:TextBox>
                                        <ul class="list-inline" id="myItemView">
                                            <asp:Literal ID="lt_myItems" runat="server"></asp:Literal>
                                        </ul>
                                    </div>
                                    <asp:RequiredFieldValidator ID="rfv_val_Items" runat="server" ErrorMessage="請建立「關聯產品品號」" ControlToValidate="val_Items" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>

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
    <!-- Sub Details Start -->
    <div class="row">
        <div class="col-sm-9 col-md-10">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <div class="pull-left">
                        <span class="glyphicon glyphicon-list"></span>
                        <span>各語系設定狀態</span>
                    </div>
                    <div class="pull-right">
                        
                    </div>
                    <div class="clearfix"></div>
                </div>
                <!-- Table Content Start -->
                <div id="result" class="collapse in">
                    <div class="panel-body">
                        <div class="btn-group btn-group-justified btn-group-lg">
                            <asp:Literal ID="lt_DataSet" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
                <!-- Table Content End -->
            </div>
        </div>
        <div class="col-sm-3 col-md-2"></div>
    </div>
    <!-- Sub Details End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <script>
        //取得資料編號(這項宣告要放在最前方)
        var dataID = '<%=Param_thisID%>';

        //判斷核取方塊選項(ClientValidate) - 區域
        function check_AreaCode(sender, args) {
            var flagNum = 0;
            var optList = document.getElementById("MainContent_cbl_Area");
            var inArr = optList.getElementsByTagName('input');
            for (var i = 0; i < inArr.length; i++) {
                if (inArr[i].type == "checkbox") {
                    if (inArr[i].checked == true) {
                        flagNum += 1;
                    }
                }
            }
            if (flagNum == 0) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }
        }

        /* 頁面載入後 */
        $(function () {
            //Click事件, 觸發儲存
            $("#triggerSave").click(function () {
                Get_Item();
                blockBox1('Add', '資料處理中...');
                $('#MainContent_btn_doSave').trigger('click');
            });

            /* tooltip */
            $('.tip').tooltip({
                html: true,
                //trigger: 'focus',  /* click | hover | focus */
                placement: 'bottom' /*  top | bottom | left | right | auto */
            });

            //使用jQueryUI 將radio Group
            $(".showRadioGrp").buttonset();

            //滑動到指定內容區 Start
            if (dataID != '') {
                //取得元素
                var _thisID = $('#data');

                //滑動至指定ID
                $('html, body').animate({
                    scrollTop: $(_thisID).offset().top - 50
                }, 600);
            }
            //滑動到指定內容區 End

        });
    </script>

    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <%: Scripts.Render("~/bundles/group-datepicker") %>

    <%-- Autocompelete Start --%>
    <script>
        $("#MainContent_tb_myFilterItem").catcomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ModelNo.aspx",
                    data: {
                        q: request.term
                    },
                    type: "POST",
                    dataType: "json",
                    success: function (data) {
                        if (data != null) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.label,
                                    category: item.category,
                                    value: item.label,
                                    id: item.id
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                $("#tb_myItemID").val(ui.item.id);

                //觸發事件, 新增項目
                Add_Item();

                //清除輸入欄
                $(this).val("");
                event.preventDefault();
            }
        });

        //----- 動態欄位 Start -----
        /* 新增項目 */
        function Add_Item() {
            var ObjId = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");
            var ObjVal = $("#tb_myItemID").val();
            if (ObjVal == "") {
                alert('欄位空白!');
                return;
            }
            var NewItem = '<li id="li_' + ObjId + '" style="padding-top:5px;">';
            NewItem += '<input type="hidden" class="item_ID" value="' + ObjVal + '" />';
            NewItem += '<a href="javascript:Delete_Item(\'' + ObjId + '\');" class="btn btn-success">' + ObjVal + '&nbsp;<span class="glyphicon glyphicon-trash"></span></a>';
            NewItem += '</li>';

            //將項目append到指定控制項
            $("#myItemView").append(NewItem);
        }

        /* 刪除項目 */
        function Delete_Item(TarObj) {
            $("#li_" + TarObj).remove();
        }

        function Delete_AllItem() {
            $("#myItemView li").each(
               function (i, elm) {
                   $(elm).remove();
               });
        }

        /* 取得各項目欄位值*/
        function Get_Item() {
            //取得控制項, ServerSide
            var fld_itemID = $("#MainContent_val_Items");

            //清空欄位值
            fld_itemID.val('');

            //巡覽項目, 填入值
            $("#myItemView li .item_ID").each(
                function (i, elm) {
                    var OldCont = fld_itemID.val();
                    if (OldCont == '') {
                        fld_itemID.val($(elm).val());
                    } else {
                        fld_itemID.val(OldCont + ',' + $(elm).val());
                    }
                }
            );

        }
        //----- 動態欄位 End -----

    </script>
    <%-- Autocompelete End --%>
</asp:Content>

