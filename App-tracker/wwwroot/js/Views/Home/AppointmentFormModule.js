﻿var AppointmentFormModule = (function () {

    // -- JS SUPPLIER SELECTOR STARTS 

    var $chosenSuppliers = $(".js-chosen-suppliers");
    var $supplierSelectList = $(".js-supplier-select");

    $supplierSelectList.off("change");
    $supplierSelectList.on("change", function () {
        var supplier = $(this).find(":selected").text();

        $('<p class="badge badge-light"> ' + supplier 
            + ' <svg xmlns="http://www.w3.org/2000/svg" data-supplier-id="' + $(this).val() + '" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="deselect-supplier-icon js-deselect-supplier-icon feather feather-x"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>'
            + '<p> ')
            .appendTo($chosenSuppliers);

        var currentSelectedSuppliers = $supplierSelect.val() || [];
        currentSelectedSuppliers.push($(this).val());

        $supplierSelect.val(currentSelectedSuppliers);

        $(this).val("");
    });

    $chosenSuppliers.on("click", ".js-deselect-supplier-icon", function (e) {

        var currentSelectedSuppliers = $supplierSelect.val();

        for (var i = 0; i < currentSelectedSuppliers.length; i++) {
            if (currentSelectedSuppliers[i] == $(this).attr("data-supplier-id")) {
                console.log("De selecting supplier with the ID of : " + currentSelectedSuppliers[i]);
                currentSelectedSuppliers.splice(i, 1);
                $supplierSelect.val(currentSelectedSuppliers);
                $(this).parent(".badge").remove();
                break;
            }
        }
    });


    // -- JS SUPPLIER SELECTOR ENDS
    //... Data
    var data = {};

    //... Elements
    var $form = $("form");
    var $addRowBtn = $form.find(".js-add-row-btn");
    var $confirmDelCommentBtn = $(".js-confirm-del-row-btn");
    var $confirmDelModal = $("#confirmDelete");
    var $commentsWrapper = $form.find(".js-comments-wrapper");
    var $supplierSelect = $form.find("select[name=SupplierIds]");

    var $commentTemplate = $(".js-comment-template");

    var $containerTypesWrapper = $form.find(".js-container-types");
    var $containerTypesRadioBtns = $containerTypesWrapper.find("input[type=radio]");

    var $containerDepartmentsWrapper = $form.find(".js-container-departments");
    var $containerDepartmentsRadioBtns = $containerDepartmentsWrapper.find("input[type=radio]");

    var $supplierForm = $(".addSupplierForm");
    var $addSupplierSuccessMsg = $(".js-add-supplier-success-msg");

    // Binds
    $containerTypesRadioBtns.off("change");
    $containerTypesRadioBtns.on("change", changeSelectedRadioBtns);

    $containerDepartmentsRadioBtns.off("change");
    $containerDepartmentsRadioBtns.on("change", changeSelectedRadioBtns);

    $addRowBtn.off("click");
    $addRowBtn.on("click", addRow);

    $form.on("click", ".js-del-row-btn", showConfirmForSavedCommentOrDeleteNonSavedComment);
    $confirmDelCommentBtn.off("click");
    $confirmDelCommentBtn.on("click", confirmDelete);

    $commentsWrapper.on("input", "input[name$=Comment]", function () {
        var index = $(this).attr("data-idx");

        data.containerComments[index].comment = $(this).val();
        console.log(data.containerComments[index]);
        
    });

    $supplierForm.off("submit");
    $supplierForm.on("submit", handleAddSupplier);
    (function $supplierFormInputChangeEvent() {
        var timerId;
        $supplierForm.find("input").on("input", function () {
            var $el = $(this);
            var value = $el.val();
            if (value != $el.data("lastval")) {

                $el.data("lastval", value);
                clearTimeout(timerId);

                timerId = setTimeout(function () {
                   $el
                        .removeClass("input-invalid")
                        .siblings(".invalid-feedback")
                        .removeClass("d-block");
                }, 250);
            }
        });
    }());

    var init = function (containerComments) {
        $containerTypesRadioBtns.each(function (i, el) { $(el).is(":checked") ? $(el).change() : null; });
        $containerDepartmentsRadioBtns.each(function (i, el) { $(el).is(":checked") ? $(el).change() : null; });

        data.containerComments = containerComments;

        renderComments();
    }

    function confirmDelete() {
        var containerCommentId = $(this).attr("data-id");

        $.ajax({
            url: "/api/ContainerComments/" + containerCommentId,
            method: "DELETE"
        })
            .done(function (result) {
                for (var i = 0; i < data.containerComments.length; i++) {
                    if (data.containerComments[i].id == containerCommentId) {
                        data.containerComments.splice(i, 1);
                        hideShowingModal();
                        var $rowToDel = $commentsWrapper.find(".row[data-id=" + containerCommentId + "]");
                        $rowToDel.remove();
                        break;
                    }
                }
            })
            .fail(function (error) {
                var $rowThatHasError = $commentsWrapper.find(".row[data-id=" + containerCommentId + "]");
                $rowThatHasError.find(".js-comment-error-msg").text(error.responseText).addClass("d-block");
                hideShowingModal();
            });
    }

    function showConfirmForSavedCommentOrDeleteNonSavedComment() {
        var containerCommentId = $(this).attr("data-id");
        var containerComment = $(this).val();

        if (containerCommentId == 0) {
            for (var i = 0; i < data.containerComments.length; i++) {
                if (data.containerComments[i].id == containerCommentId && data.containerComments[i].comment == containerComment) {
                    data.containerComments.splice(i, 1);
                    renderComments();
                    break;
                }
            }
        }
        else {
            $confirmDelCommentBtn.attr("data-id", containerCommentId);
            $confirmDelModal.modal("show");
        }
        
    }
    

    function changeSelectedRadioBtns() {

        $(this).parent("label")
            .addClass("btn-custom-primary")
            .siblings()
            .removeClass("btn-custom-primary");

    }

    function renderComments() {
        var templateString = $commentTemplate.html();
        var htmlComments = "";

        for (var i = 0; i < data.containerComments.length; i++) {
            var htmlComment = templateString
                .replace(/{{id}}/g, data.containerComments[i].id)
                .replace(/{{containerId}}/g, data.containerComments[i].containerId)
                .replace(/{{comment}}/g, data.containerComments[i].comment)
                .replace(/{{i}}/g, i);

            htmlComments += htmlComment;
        }

        $commentsWrapper.children().remove();
        $commentsWrapper.append(htmlComments);
    }

    function addRow() {

        var comment = {
            id: 0,
            containerId: 0,
            comment: ""
        }

        data.containerComments.push(comment);
        console.log(data.containerComments);

        renderComments();
    }

    function addRowOld() {
        var curRowCount = $commentRows.length;

        var changeName, newInpName;
        if (curRowCount != 0) {
            changeName = $commentRows.last().attr("name");
            newInpName = changeName.replace("[" + (curRowCount - 1) + "]", "[" + curRowCount + "]");
        } else {
            newInpName = "Comments[0].Comment";
        }

        var $row = $("<div>").attr("class", "mb-2 row");

        var $colOne = $("<div>").attr("class", "col-11 pr-1");
        var $colTwo = $("<div>").attr("class", "col-1 pl-1");

        var $inp = $("<input/>").attr({ type: "text", name: newInpName, id: "test" + curRowCount, class: "form-control" });
        $inp.appendTo($colOne);

        $("<div>").attr("class", "btn btn-danger js-del-row").html("&#x2715;").appendTo($colTwo);

        $colOne.appendTo($row);
        $colTwo.appendTo($row);

        $row.appendTo($commentsWrapper);

        $commentRows = $commentsWrapper.find(".row input");
    }

    function handleAddSupplier(e) {
        e.preventDefault();
        SupplierService.addSupplier($supplierForm.serialize(), addSupplierSuccess, addSupplierFail);
    }

    function addSupplierSuccess(supplier) {
        hideShowingModal();

        $addSupplierSuccessMsg.html("Successfully added new supplier {{supplier}}");
        $addSupplierSuccessMsg.html($addSupplierSuccessMsg.html().replace("{{supplier}}", supplier.supplier));
        $addSupplierSuccessMsg.slideDown();

        $supplierSelect.append($('<option>', {
            value: supplier.id,
            text: supplier.supplier
        }))
            .val(supplier.id);

        $supplierForm.find("input").val("");

    }

    function addSupplierFail(error) {
        $supplierForm
            .find("input")
            .addClass("input-invalid")
            .siblings(".invalid-feedback")
            .text(error.responseText)
            .addClass("d-block");
    }

   

    return {
        init: init
    }

})();