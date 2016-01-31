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