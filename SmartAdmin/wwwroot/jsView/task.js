class Task {

    CompleteContact(element) {
        $(element).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Contact/ListJson',
                    type: "POST",
                    dataType: "json",
                    data: { id: $("#Contact").val(), terms: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.firstName,
                                value: item.contactId
                            }
                        }));
                    }
                })
            },
            select: function (event, ui) {
                $("#ContactId").val(ui.item ? ui.item.value : "");
                $("#ContactIds").val(ui.item ? ui.item.label : "");
            },
            change: function (event, ui) {
                $("#ContactId").val(ui.item ? ui.item.value : "");
                $("#ContactIds").val(ui.item ? ui.item.label : "");
            }

        });
    }

    CompleteCompany(element) {
        $(element).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Company/ListJson',
                    type: "POST",
                    dataType: "json",
                    data: { terms: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.companyId + ", " + item.firstName + (item.lastName ? ", " + item.lastName : ", " + item.firstName),
                                value: item.companyId
                            }
                        }));
                    }
                })
            },
            select: function (event, ui) {
                $("#CompanyId").val(ui.item ? ui.item.value : "");
                $("#CompanyIds").val(ui.item ? ui.item.label : "");
            },
            change: function (event, ui) {
                $("#CompanyId").val(ui.item ? ui.item.value : "");
                $("#CompanyIds").val(ui.item ? ui.item.label : "");
            }

        });
    }



    CompleteDeal(element) {
        $(element).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/Deal/ListJson',
                    type: "POST",
                    dataType: "json",
                    data: { terms: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.dealId + ", " + item.name,
                                value: item.dealId
                            }
                        }));
                    }
                })
            },
            select: function (event, ui) {
                $("#DealId").val(ui.item ? ui.item.value : "");
                $("#DealIds").val(ui.item ? ui.item.label : "");
            },
            change: function (event, ui) {
                $("#DealId").val(ui.item ? ui.item.value : "");
                $("#DealIds").val(ui.item ? ui.item.label : "");
            }

        });

    }

}

var task = new Task();