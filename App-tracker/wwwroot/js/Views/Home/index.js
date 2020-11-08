var IndexModule = (function () {

    // elements
    var $containerActivationDetailsFormModal = $("#activationDetailsFormModal");
    var $containerActivationDetailsForm = $(".containerActivationDetailsForm");

    // Saving container details
    $containerActivationDetailsForm.off("submit")
    $containerActivationDetailsForm.on("submit", function (e) {
        e.preventDefault();

        var containerId = $(this).find("input[name=ContainerId]").val();
        var statusId = $(this).find("input[name=NewStatusId").val(); 
        $.ajax({
            method: "PUT",
            url: "/api/Containers/" + containerId,
            data: $(this).serialize(),
            contentType: 'application/x-www-form-urlencoded'
        })
            .done(function (result) {
                hideShowingModal();
                $(this).find("input select").val("");

                $.ajax({
                    url: "/api/containers/" + statusId + "/" + containerId,
                    contentType: "application/json charset=utf-8",
                    type: "PUT",
                })
                    .done(function (result) {
                        $("[data-status-id=" + statusId + "]").append($("#" + containerId))
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

        if (oldStatusId < 3 && newStatusId > 2) {
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
        alert("init");
    }

    return {
        init: init
    }

})();