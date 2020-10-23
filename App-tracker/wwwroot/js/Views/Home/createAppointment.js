var create_appointment_module = (function () {

    //... Elements
    var $form = $("form");

    var $containerTypesWrapper = $form.find(".js-container-types");
    var $containerTypesRadioBtns = $containerTypesWrapper.find("input[type=radio]");

    var $containerDepartmentsWrapper = $form.find(".js-container-departments");
    var $containerDepartmentsRadioBtns = $containerDepartmentsWrapper.find("input[type=radio]");

    var init = function () {
        $containerTypesRadioBtns.off("change");
        $containerTypesRadioBtns.on("change", changeSelectedRadioBtns);

        $containerDepartmentsRadioBtns.off("change");
        $containerDepartmentsRadioBtns.on("change", changeSelectedRadioBtns);
    };

    function changeSelectedRadioBtns() {

        $(this).parent("label")
            .addClass("btn-custom-primary")
            .siblings()
            .removeClass("btn-custom-primary");

    }

    return {
        init: init
    }

})();