function getYear(val) {
    var year = new Date().getFullYear();
    $("#year").html("<option value=-2>Chọn</option>");
    for (var i = year; i >= 1970; i--) {
        $("#year").append("<option value=" + i + ">" + i + "</option>");
    }
    $("#year").val(val);
}
function showLoadingImage() {
    $("#progressbar").show();
}
function hideLoadingImage() {
    $("#progressbar").hide();
}
function tolower() {
    var temp = $("#name").val();
    $("#name").val(temp.toLowerCase());
}
function detectmob() {
    //alert(navigator.userAgent);
    if (navigator.userAgent.match(/Android/i)
     || navigator.userAgent.match(/webOS/i)
     || navigator.userAgent.match(/iPhone/i)
     || navigator.userAgent.match(/iPad/i)
     || navigator.userAgent.match(/iPod/i)
     || navigator.userAgent.match(/BlackBerry/i)
     || navigator.userAgent.match(/Windows Phone/i)
     || navigator.userAgent.match(/Firefox/i) 
     ) {
        return true;
    }
    else {
        return false;
    }
}