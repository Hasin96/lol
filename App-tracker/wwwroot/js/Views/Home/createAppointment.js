var create_appointment_module = (function () {

    //... Elements
    var $form                = $("form");
    var $cTypesBtns       = $form.find(".js-container-types").find(".btn");

    var $cDepartmentsWrapper = $form.find(".js-container-departments");

    var init = function () {
        console.log($cTypesBtns);
        $cTypesBtns.click(changeSelectedRadioBtns);
    };

    function changeSelectedRadioBtns() {

        var $btn = $(this);
        var $cTypeId = $btn.attr("value");

        var $radioToSelect = $btn.siblings("input[value=" + $cTypeId + "]");
        $radioToSelect.prop("checked", true);
        $radioToSelect.removeClass("btn-light").addClass("btn-primary");

    }

    return {
        init: init
    }

})();