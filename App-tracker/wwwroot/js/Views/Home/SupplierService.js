var SupplierService = (function () {

    var apiUrl = "/api/Suppliers";

    var addSupplier = function (supplier, done, fail) {
        $.ajax({
            method: "POST",
            url: apiUrl,
            data: supplier,
            contentType: 'application/x-www-form-urlencoded'
        })
            .done(done)
            .fail(fail);
    };

    return {
        addSupplier: addSupplier
    };

})();
