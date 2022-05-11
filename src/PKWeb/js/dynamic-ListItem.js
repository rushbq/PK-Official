/*
    [動態欄位]
    1. 使用前確認公用js是否有日期格式化的函式 (e.g.:Date.prototype.Format = function (fmt)...)
    2. 儲存時要使用 Get_Item(...)
    3. 需搭配bootstrap
*/

/* [新增項目]
   myListID = 項目清單編號
   myItemID = 資料來源-編號
   myItemName = 資料來源-名稱
   showID = 是否要顯示編號
*/
function Add_Item(myListID, myItemID, myItemName, showID) {
    //取得參數值
    var ObjID = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");     //自動編號
    var ObjValID = $("#" + myItemID).val();     //資料來源 - 編號
    var ObjValName = $("#" + myItemName).val();     //資料來源 - 名稱
    if (ObjValID == "") {
        alert('Field is empty.');
        return;
    }

    //填入Html
    var NewItem = '<li id="li_' + ObjID + '" class="list-group-item">';

    if (showID) {
        NewItem += ' <div class="list-group-item-heading">';
        NewItem += '     <label class="pull-left label label-warning">' + ObjValID + '</label>';
        NewItem += '     <span class="pull-right">';
        NewItem += '     <button type="button" class="btn btn-default btn-xs" onclick="Delete_Item(\'' + ObjID + '\');"><i class="fa fa-times"></i></button>';
        NewItem += '    </span>';
        NewItem += ' </div>';
        NewItem += ' <div class="clearfix"></div>';
        NewItem += ' <p class="list-group-item-text">' + ObjValName + '</p>';
    } else {
        NewItem += ' <p class="list-group-item-text">';
        NewItem += '     <span class="pull-left">' + ObjValName + '</span>'
        NewItem += '     <span class="pull-right">';
        NewItem += '     <button type="button" class="btn btn-default btn-xs" onclick="Delete_Item(\'' + ObjID + '\');"><i class="fa fa-times"></i></button>';
        NewItem += '    </span>';
        NewItem += ' </p>';
        NewItem += ' <div class="clearfix"></div>';
    }

    //隱藏欄位, 儲存時調用(放在li裡, 若要調動需調整Get_Item(..))
    NewItem += '<input type="hidden" class="item_ID" value="' + ObjValID + '" />';
    NewItem += '<input type="hidden" class="item_Name" value="' + ObjValName + '" />';

    NewItem += '</li>';



    //將項目append到指定控制項
    $("#" + myListID).append(NewItem);
}

/* 刪除指定項目 */
function Delete_Item(myItemID) {
    $("#li_" + myItemID).remove();
}

/* 刪除所有項目 */
function Delete_AllItem(myListID) {
    $("#" + myListID + " li").each(
       function (i, elm) {
           $(elm).remove();
       });
}

/* [取得各項目欄位值]
   myListID = 項目清單編號
   myValField = 存放項目參數值集合的欄位
*/
function Get_Item(myListID, myValField) {
    //取得存放資料的控制項, <欄位X>
    var myFldItem = $("#" + myValField);

    //清空此欄位
    myFldItem.val('');

    //取得項目清單值, 組合後填入<欄位X>, 以逗號分隔
    $("#" + myListID + " li .item_ID").each(
        function (index, element) {
            var OldCont = myFldItem.val();
            if (OldCont == '') {
                myFldItem.val($(element).val());
            } else {
                myFldItem.val(OldCont + ',' + $(element).val());
            }
        }
    );
}