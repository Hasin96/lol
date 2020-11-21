var IndexModule = (function () {

    // excel sheet
    

    var $createExcelSheetBtn = $(".js-create-excel-btn");

    var $containerStartDateChooser = $(".js-appointment-start-date-inp");

    $containerStartDateChooser.on("change", function () {
        $createExcelSheetBtn.attr("href", "/api/containers/" + this.valueAsDate.toISOString());
    });

    // excel sheet end

    var $template = $(".container-actions-template");

    var $moreOptionsIcon = $(".js-more-icon");
    $moreOptionsIcon.off("click");
    $moreOptionsIcon.on("click", function () {

        var containerId = $(this).attr("data-container-id");

        var options = {
            html: true,
            content: function () {
                var $el = $('<div data-more-icon-id="'+ containerId +'"></div>');

                $el.append('<a href="/home/edit/' + containerId +'"><svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2 feather feather-log-in"><path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"></path><polyline points="10 17 15 12 10 7"></polyline><line x1="15" y1="12" x2="3" y2="12"></line></svg></a>');
                $el.append('<a href="/home/edit/' + containerId + '"><svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2 feather feather-edit"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg></a>');
                $el.append('<svg xmlns="http://www.w3.org/2000/svg" data-container-id="' + containerId +'" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2 feather feather-trash-2 js-delete-container-btn"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>');

                return $el;
            },
            container: 'body',
            placement: 'right'
        }

        $(this).popover(options);
        $(this).popover("toggle");

    });

    var $confirmDeleteModal = $(".js-confirm-delete-modal");
    var $confirmDelBtn = $(".js-confirm-del-btn");

    var $delContainerBtn = $(".js-delete-container-btn");
    $delContainerBtn.off("click");
    $(document).on("click", ".js-delete-container-btn", function (e) {
        $confirmDelBtn.attr("data-container-id", $(this).attr("data-container-id"));
        $confirmDeleteModal.modal("show");
    });

    $confirmDelBtn.off("click");
    $confirmDelBtn.on("click", function () {
        var containerId = $(this).attr("data-container-id");

        $.ajax({
            url: "/api/Containers/" + containerId,
            method: "DELETE"
        })
            .done(function (result) {
                $moreOptionsIcon.popover("hide");
                $("#" + containerId).remove();
                hideShowingModal();
            })
            .fail(function (error) {
            });
    });

    //$(document).on("click", function () {
    //    if ($(this).parents(".popover").length == 0 && $(this).hasClass("popover") == false && $(this).hasClass("js-more-icons") == false && $(this).parents(".js-more-icons").length == 0)
    //        $moreOptionsIcon.popover("hide");
    //})


    // elements
    var $containerActivationDetailsFormModal = $("#activationDetailsFormModal");
    var $containerActivationDetailsForm = $(".containerActivationDetailsForm");

    // Saving container details
    $containerActivationDetailsForm.off("submit")
    $containerActivationDetailsForm.on("submit", function (e) {
        e.preventDefault();

        var containerId = $(this).find("input[name=ContainerId]").val();
        var statusId = $(this).find("input[name=NewStatusId").val(); 

        var $form = $(this);
        $.ajax({
            method: "PUT",
            url: "/api/Containers/" + containerId,
            data: $form.serialize(),
            contentType: 'application/x-www-form-urlencoded'
        })
            .done(function (result) {
                hideShowingModal();
                var bay = $form.find("select[name=BayId] option:selected").text();
                $("#" + containerId).find(".js-bay").html(bay);
                $form.find("input").val("");
                $form.find("select").val("");

                $.ajax({
                    url: "/api/containers/" + statusId + "/" + containerId,
                    contentType: "application/json charset=utf-8",
                    type: "PUT",
                })
                    .done(function (result) {
                        $("[data-status-id=" + statusId + "]").append($("#" + containerId));

                    })

                    })
                    .fail(function (error) {
                        debugger;
                    })
            .fail(function (error) {alert("fail") })

    });

    // Drag events under here

    var cards = document.querySelectorAll(".js-custom-container-card");
    cards.forEach(function (el) {
        el.addEventListener("dragstart", dragstart_handler);
    });

    var statusCol = document.querySelectorAll(".js-status-column");
    statusCol.forEach(function (el) {
        el.addEventListener("dragover", dragover_handler);
        el.addEventListener("drop", drop_handler)
    })

    function dragstart_handler(ev) {
        // Add the target element's id to the data transfer object
        ev.dataTransfer.setData("card-id/container-id", ev.target.id);
        ev.dataTransfer.setData("old-status-id", $(ev.target).parents(".card-body").attr("data-status-id"));
    }

    function dragover_handler(ev) {
        ev.preventDefault();
        ev.dataTransfer.dropEffect = "move";
    }

    function drop_handler(ev) {
        ev.preventDefault();

        var oldStatusId = ev.dataTransfer.getData("old-status-id");
        // Get the id of the target and add the moved element to the target's DOM
        var containerId = ev.dataTransfer.getData("card-id/container-id");
        var newStatusId = $(ev.target).attr("data-status-id");
        var c = $("#" + containerId);
        var bay = c.find(".js-bay").text();
        console.log(newStatusId);
        if ((bay == "" || bay == null) && newStatusId > 2) {
            $containerActivationDetailsForm.find("input[name=ContainerId]").val(containerId);
            $containerActivationDetailsForm.find("input[name=NewStatusId]").val(newStatusId);
            $containerActivationDetailsFormModal.modal("show");
            return;
        }

        $.ajax({
            url: "/api/containers/" + newStatusId + "/" + containerId,
            contentType: "application/json charset=utf-8",
            type: "PUT",
        })
            .done(function (result) {
                ev.target.appendChild(document.getElementById(containerId));
            })
            .fail(function (error) {
                debugger;
            })

    }

    var init = function () {
    }

    return {
        init: init
    }

})();