$("#sidebarToggle, #sidebarToggleTop").on('click', function (e) {
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");

    if ($(".sidebar").hasClass("toggled")) {
        $('.sidebar .collapse').collapse('hide');
        SetCookie("IsSidebar", "toggled");
    } else {
        SetCookie("IsSidebar", "");
    };
});

// Close any open menu accordions when window is resized below 768px
$(window).resize(function () {
    if ($(window).width() < 768) {
        $('.sidebar .collapse').collapse('hide');
    };
});

// Prevent the content wrapper from scrolling when the fixed side navigation hovered over
$('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function (e) {
    if ($(window).width() > 768) {
        var e0 = e.originalEvent,
            delta = e0.wheelDelta || -e0.detail;
        this.scrollTop += (delta < 0 ? 1 : -1) * 30;
        e.preventDefault();
    }
});

// Scroll to top button appear
$(document).on('scroll', function () {
    var scrollDistance = $(this).scrollTop();
    if (scrollDistance > 100) {
        $('.scroll-to-top').fadeIn();
    } else {
        $('.scroll-to-top').fadeOut();
    }
});

// Smooth scrolling using jQuery easing
$(document).on('click', 'a.scroll-to-top', function (e) {
    var $anchor = $(this);
    $('html, body').stop().animate({
        scrollTop: ($($anchor.attr('href')).offset().top)
    }, 1000, 'easeInOutExpo');
    e.preventDefault();
});

//페이징 클릭
$(document).on("click", "a[data-dt-idx]", function (event) {
    var page = $(this).attr('data-dt-idx');
    if (page == "0") return;

    $('#hPageNumber').val(page);

    fn_Search();
});

$(document).on("change", "#dataTableSelect", function (event) {
    $('#hPageSize').val($(this).val());
    $('#hPageNumber').val("1");

    SetCookie("hPageSize", $('#hPageSize').val());

    fn_Search();
});

$(document).on("click", "#btnSearch", function (event) {
    if ($("#dataTables_filterWord").val().length <= 2 && $("#dataTables_filterWord").val() != "") {
        alert("검색은 2글자 이상 검색해 주십시오.");
        return false;
    }

    $('#hPageNumber').val("1");
    $("#hArea").val($("#dataTables_filterArea").val());
    $("#hWord").val($("#dataTables_filterWord").val());

    fn_Search();
});

$(document).on("keyup", "#dataTables_filterWord", function (event) {
    var Key = (event.target) ? event.which : event.keyCode;
    if (Key == 13) {
        if ($("#dataTables_filterWord").val().length < 2 && $("#dataTables_filterWord").val() != "") {
            alert("검색은 2글자 이상 검색해 주십시오.");
            return false;
        }

        $('#hPageNumber').val("1");
        $("#hArea").val($("#dataTables_filterArea").val());
        $("#hWord").val($("#dataTables_filterWord").val());

        fn_Search();
    }
});

$(document).on("click", "#dataTable thead th", function (event) {
    $('#hSortType').val($(this).attr("aria-title"));
    $('#hSortColumn').val($(this).attr("aria-title"));

    if ($('#hSortOrder').val() == "ASC") {
        $('#hSortOrder').val("DESC");
    } else {
        $('#hSortOrder').val("ASC");
    }

    fn_Search();
});

var fn_Search = function (isRefresh) {
    var result = $.post("/Notice/Board", $("#formBoard").serialize());
    result.done(function (reqHtml) {
        var $reqHtml = $(reqHtml);


        var containerformBoard = $reqHtml.find("#formBoard").html();
        $("#formBoard").html(containerformBoard);

        var containerdataTable_length = $reqHtml.find("#dataTable_length").html();
        $("#dataTable_length").html(containerdataTable_length);
        var containerdataTables_filter = $reqHtml.find("#dataTables_filter").html();
        $("#dataTables_filter").html(containerdataTables_filter);

        var containerdataTable = $reqHtml.find("#dataTable").html();
        $("#dataTable").html(containerdataTable);

        var containerpaging = $reqHtml.find("#paging").html();
        $("#paging").html(containerpaging);

        // Event Setting
    });
    result.fail(function (e) {
        //console.log("/Frame/Board : " + e);
    });
}