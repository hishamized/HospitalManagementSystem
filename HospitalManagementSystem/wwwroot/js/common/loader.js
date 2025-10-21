function run_ajax_loader() {
    $(".ajaxBlock").waitMe({ effect: "img", text: "Please wait...", bg: "rgba( 255, 255, 255, 0.7)", color: "#000", sizeW: "", sizeH: "", source: "/images/loader.gif" });
}

$(document).ajaxSend(function (event, request, opt) {
    switch (opt.url) {
        case "/Dashboard/GetMainStats" :
            break;
        default:
            run_ajax_loader();
    }
});

$(document).ajaxError(function () {
    $(".ajaxBlock").waitMe("hide");
});

$(document).ajaxComplete(function () {
    $(".ajaxBlock").waitMe("hide");
});

$(document).ajaxStop(function () {
    $(".ajaxBlock").waitMe("hide");
    $.jTimeout().resetExpiration();
});
