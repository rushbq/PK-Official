<%@ Page Title="產品資料維護" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Prod_Edit" ValidateRequest="false" %>

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
                <li><a href="<%=Application["WebUrl"] %>Prod/Search/">產品資料查詢</a></li>
                <li class="active">產品資料維護</li>
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
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Rpt_Folder">上架區域 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:CheckBoxList ID="cbl_Area" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                    <asp:CustomValidator ID="cv_check_Area" runat="server" ErrorMessage="請選擇「上架區域」" Display="Dynamic"
                                        ClientValidationFunction="check_AreaCode" ValidationGroup="Add" CssClass="styleRed help-block"></asp:CustomValidator>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Rpt_Folder">顯示未開賣</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:CheckBoxList ID="cbl_NoSellArea" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:CheckBoxList>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-xs-12 col-sm-3 col-md-3 col-lg-2">品號 <em>*</em></label>
                                <div class="col-xs-7 col-sm-6 col-md-6 col-lg-8">
                                    <asp:TextBox ID="tb_Model_No" runat="server" CssClass="form-control tip" placeholder="輸入品號關鍵字"></asp:TextBox>
                                    <asp:HiddenField ID="hf_myItemVal" runat="server" />

                                    <asp:RequiredFieldValidator ID="rfv_tb_Model_No" runat="server" ErrorMessage="請填寫「品號」" ControlToValidate="tb_Model_No" Display="Dynamic" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-xs-5 col-sm-3 col-md-3 col-lg-2">
                                    <asp:Button ID="btn_GetIcon" runat="server" Text="帶出認證符號" CssClass="btn btn-warning" OnClick="btn_GetIcon_Click" />
                                </div>
                            </div>
                            <div class="form-group">
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
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">上架狀態 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="showRadioGrp">
                                        <asp:RadioButtonList ID="rbl_Display" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_ddl_Status">產品標記 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="form-inline">
                                        <div class="showRadioGrp">
                                            <asp:RadioButtonList ID="rbl_IsNew" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                <asp:ListItem Value="Z">推薦</asp:ListItem>
                                                <asp:ListItem Value="Y">新品</asp:ListItem>
                                                <asp:ListItem Value="N" Selected="True">一般品</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Sort">自訂排序 <em>*</em></label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox ID="tb_Sort" runat="server" MaxLength="3" CssClass="form-control" placeholder="自訂排序" Width="70px" Style="text-align: center;">999</asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfv_tb_Sort" runat="server" ErrorMessage="請輸入「自訂排序」"
                                        Display="Dynamic" ControlToValidate="tb_Sort" ValidationGroup="Add" CssClass="styleRed help-block"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rv_tb_Sort" runat="server" ErrorMessage="請輸入1 ~ 999 的數字"
                                        Display="Dynamic" Type="Integer" MaximumValue="999" MinimumValue="1" ControlToValidate="tb_Sort"
                                        ValidationGroup="Add" CssClass="styleRed help-block"></asp:RangeValidator>
                                    <div class="help-block">
                                        (前台的自訂排序, 優先順序為 推薦 -> 新品 -> 自訂排序 -> 上架日期, 數字小的優先)
                                    </div>
                                </div>
                            </div>
                            
                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">目錄 / 頁次</label>
                                <div class="control-label col-sm-9 col-md-9 col-lg-10">
                                    <asp:Label ID="lb_Vol" runat="server"></asp:Label>&nbsp;/&nbsp;
                                    <asp:Label ID="lb_Page" runat="server"></asp:Label>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2" for="MainContent_tb_Tag">
                                    前台關鍵字查詢
                                </label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <div class="row">
                                        <div class="col-xs-8">
                                            <asp:TextBox ID="tb_Tag" runat="server" CssClass="form-control" placeholder="輸入關鍵字"></asp:TextBox>
                                            <input type="hidden" id="tb_TagName" />
                                            <input type="hidden" id="tb_TagID" />
                                            <span class="help-block">
                                                1.加入已存在的關鍵字：輸入關鍵字, 選擇項目後按下存檔.<br />
                                                2.新的關鍵字：填完新的名稱後,按下「新增關鍵字」
                                            </span>
                                        </div>
                                        <div class="col-xs-4">
                                            <a href="javascript:;" class="btn btn-warning" id="newTag"><span class="glyphicon glyphicon-tags"></span>&nbsp;新增關鍵字</a>
                                            <asp:TextBox ID="tb_All_itemID" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                            </asp:TextBox>
                                            <asp:TextBox ID="tb_All_itemName" runat="server" ToolTip="欄位值集合" Style="display: none;">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <ul class="list-inline" id="myTags">
                                                <asp:Literal ID="lt_myItems" runat="server"></asp:Literal>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3 col-md-3 col-lg-2">認證符號</label>
                                <div class="col-sm-9 col-md-9 col-lg-10 table-responsive">
                                    <asp:CheckBoxList ID="cbl_CertIcon" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" RepeatColumns="5" CssClass="table"></asp:CheckBoxList>
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
                            <a id="triggerSave" class="text-center" onclick="Get_Item();">
                                <i class="fa fa-floppy-o"></i>
                                <div class="status">資料存檔</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="ph_Delete" runat="server" Visible="false">
                       <%-- <div class="metro-nav-block nav-block-red">
                            <asp:LinkButton ID="lbtn_Delete" runat="server" CssClass="text-center" CausesValidation="false" OnClientClick="return confirm('是否確定刪除?')" OnClick="lbtn_Delete_Click">
                            <i class="glyphicon glyphicon-trash"></i>
                            <div class="status">刪除資料</div>
                            </asp:LinkButton>
                        </div>--%>

                        <div class="metro-nav-block nav-block-orange">
                            <a href="<%=Application["WebUrl"] %>Prod/Edit/" class="text-center">
                                <i class="fa fa-pencil-square-o"></i>
                                <div class="status">新增下一筆</div>
                            </a>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <div style="display: none;">
                <asp:HiddenField ID="hf_ModelNo" runat="server" />
                <asp:Button ID="btn_doSave" runat="server" Text="Save" OnClick="btn_Save_Click" ValidationGroup="Add" Style="display: none;" />
            </div>
        </div>
    </div>
    <!-- Form End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="Server">
    <script>
        //取得資料編號(這項宣告要放在最前方)
        var dataID = '<%=Param_thisID%>';

    </script>
    <%: Scripts.Render("~/bundles/JQ-UI-script") %>
    <%: Scripts.Render("~/bundles/blockUI-script") %>
    <%: Scripts.Render("~/bundles/group-base") %>
    <%: Scripts.Render("~/bundles/DTpicker-script") %>
    <%: Scripts.Render("~/bundles/group-datepicker") %>
    <script>
        $(function () {
            /* 偵測Tag enter */
            $("#MainContent_tb_Tag").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#newTag").trigger("click");
                    return false;
                }
            });
        });
    </script>
    <%-- Autocompelete 品號 Start --%>
    <script>
        $("#MainContent_tb_Model_No").catcomplete({
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
                $("#MainContent_tb_Model_No").val(ui.item.label);
                $("#MainContent_hf_myItemVal").val(ui.item.id);

                //觸發Click - 帶出認證符號
                $('#MainContent_btn_GetIcon').trigger('click');

            }
        });

    </script>
    <%-- Autocompelete 品號 End --%>
    <%-- Autocompelete Tag Start --%>
    <script>
        $("#MainContent_tb_Tag").autocomplete({
            minLength: 1,  //至少要輸入 n 個字元
            source: function (request, response) {
                $.ajax({
                    url: "<%=Application["WebUrl"]%>Ajax_Data/AC_ProdTags.aspx",
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
                                    value: item.label,
                                    id: item.id
                                }
                            }));
                        }
                    }
                });
            },
            select: function (event, ui) {
                $("#tb_TagName").val(ui.item.value);
                $("#tb_TagID").val(ui.item.id);

                //觸發事件, 新增項目
                Add_Item();;

                //清除輸入欄
                $(this).val("");
                event.preventDefault();
            }
        });

        /* Click事件 - 自訂Tag */
        $("#newTag").click(function () {
            //重置欄位值
            $("#tb_TagID").val('0');
            $("#tb_TagName").val($("#MainContent_tb_Tag").val());

            //新增項目
            Add_Item();
        });
    </script>
    <%-- Autocompelete Tag End --%>
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
</asp:Content>

