class Address {
    CompleteAdress(element) {
        var e = document.getElementById("Address_StateProvinceId");
        var address_StateProvinceId = e.options[e.selectedIndex].value;
        $(element).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/City/ListJson',
                    type: "POST",
                    dataType: "json",
                    data: { id: $("#CityIdF").val(), terms: request.term, stateprovinceId: address_StateProvinceId },
                    success: function (data) {
                        response($.map(data, function (item) {
                            debugger;
                            return {
                                label: item.cityId + ", " + item.name + " " + "(" + item.stateProvinceName + "-" + item.stateProvinceCode+")" ,
                                value: item.cityId
                            }
                        }));
                    }
                })
            },
            appendTo: "#result",
            select: function (event, ui) {
                $("#Address_CityId").val(ui.item ? ui.item.value : "");
                $("#CityIdF").val(ui.item ? ui.item.label : "");
            },
            change: function (event, ui) {
                $("#Address_CityId").val(ui.item ? ui.item.value : "");
                $("#CityIdF").val(ui.item ? ui.item.label : "");
            }

        });
    }

    RemoveAddress(element) {
        var $this = $(element).parent().parent();
        $.ajax({
            dataType: "json",
            type: "POST",
            url: "/Address/MarkDelete",
            data: { id: $(element).attr("data-id") },
            success: function (response) {
                debugger;
                // once clicked - add class, copy to memory then remove and add to sortable3
                $this.slideUp(500, function () {
                    $this.remove();
                });
            }
        });

    }

   

    Makeditable(e) {
        $(e).editable({
            url: "/Address/PostInfo",
            success: function (response, newValue) {
                $.smallBox({
                    title: "Change",
                    content: "<i class='fa fa-clock-o'></i> <i> 1 seconds ago...</i>" + newValue,
                    color: "#296191",
                    iconSmall: "fa fa-thumbs-up bounce animated",
                    timeout: 4000
                });
            },
            error: function (response, newValue) {
                $.smallBox({
                    title: "Error",
                    content: "<i class='fa fa-clock-o'></i> <i>1 seconds ago...</i>",
                    color: "#ff6a00",
                    iconSmall: "fa fa-thumbs-down bounce animated",
                    timeout: 4000
                });
            }
        });

    }





}

var address = new Address();