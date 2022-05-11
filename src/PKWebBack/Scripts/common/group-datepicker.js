
/* 頁面載入後 */
$(function () {
    //DatePicker Start
    $('.showDate').datetimepicker({
        format: 'yyyy/mm/dd hh:ii',   //目前欄位格式
        //linkField: 'dtp_input2',    //鏡像欄位對應
        linkFormat: 'yyyy/mm/dd hh:ii',   //鏡像欄位格式
        todayBtn: true,     //顯示today
        todayHighlight: true,   //將today設置高亮
        autoclose: true,    //選擇完畢後自動關閉
        startView: 4,    //選擇器開啟後，顯示的視圖(4:10年 ; 3:12月 ; 2:該月 ; 1:該日全時段 ; 0:該時段的各個時間,預設5分間隔)
        maxView: 4,
        minView: 0,
        forceParse: false
        //,showMeridian: true //顯示AM/PM
    });

    $('#MainContent_tb_StartDate').datetimepicker()
        .on('changeDate', function (ev) {
            alert(ev);
            if (ev.date.valueOf() < date - start - display.valueOf()) {

            }
        });
    //DatePicker End
});
