$(".myContentText img").addClass("carousel-inner img-responsive img-rounded");
$(function () {
    function htsHide() {
        $(".hovertreeshowlayer img").remove();
        $(".hovertreeshowcover").hide();
        $(".hovertreeshowlayer").hide();
    }
    $(".myContentText img").on("click", function (e) {
        if ($(".hovertreeshowcover").length == 0) {
            $("body").append("<div class='hovertreeshowcover'></div>").append("<div class='hovertreeshowlayer'></div>")

            $(".hovertreeshowcover").on("click", function () { htsHide(); })
            $(".hovertreeshowlayer").on("click", function () { htsHide(); })
        }
        else {
            $(".hovertreeshowcover").show();
            $(".hovertreeshowlayer").show();
        }
        var mytop = $(this).offset().top;
        $(".hovertreeshowlayer").append("<img src='" + this.src + "' style='margin-top:" + mytop + "px;' />")
    })
})