class Pipeline {
    Move(element) {
        var pipelineId = $(element).attr("data-pipeline");
        var Id = $(element).attr("data-id");
        var stageId = $(element).attr("data-stage");
        var moveforward = $(element).attr("data-move-forward") == "yes";

        $("#Deal_DealId").val(Id);
        $("#Deal_PipelineId").val(pipelineId);
        $("#Deal_StageId").val(stageId);


        $.ajax({
            type: 'POST',
            url: '/Deal/Move',
            dataType: 'json',
            data: { id: Id, PipelineId: pipelineId, StageId: stageId, Moveforward: moveforward },
            success: function (data) {
                
                if (data.nok != null)
                {
                    $.smallBox({
                        title: "Error",
                        content: "<i class='fa fa-clock-o'></i> <i> ..</i>" + data.nok,
                        color: "#ff6a00",
                        iconSmall: "fa fa-thumbs-down bounce animated",
                        timeout: 4000
                    });
                    
                } else
                {
                  location.reload(); 
                    //$('#' + data.stageId).append($('#deal_' + Id));
                    //$('#Qty-' + data.stageId).html(data.qty);
                    //$('#Amout-' + data.stageId).html(data.subTotal);
                }


            }
        });
    }

    CallAddWindows(element, modal) {
        var stageId = $(element).attr("data-stage");
        $("#StageIdAdd").val(stageId);
        var element = document.getElementById('StageIdAdd');
        element.value = stageId;
        $(modal).modal();
    }

    CallWindows(element, modal) {
        var pipelineId = $(element).attr("data-pipeline");
        var Id = $(element).attr("data-id");
        var stageId = $(element).attr("data-stage");
        $("#Deal_DealId").val(Id);
        $("#Deal_PipelineId").val(pipelineId);
        $("#Deal_StageId").val(stageId);
        var element = document.getElementById('StageId');
        element.value = stageId;
        $(modal).modal();
    }

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
                                label: item.contactId + ", " + item.firstName + (item.lastName ? ", " + item.lastName : ", " + item.firstName),
                                value: item.contactId + ", " + item.firstName + (item.lastName ? ", " + item.lastName : ", " + item.firstName)
                            }
                        }));
                    }
                })
            },
            appendTo: "#resultContact",
            select: function (event, ui) {
                $("#ContactIds").val(ui.item ? ui.item.value : "0, " + this.value + ", Create New");
                $("#Deal_Name").val(this.value);
                if (!ui.item) {
                    $("#resultContactText").html("Create New" + this.value);

                }
            },
            change: function (event, ui) {
                $("#ContactIds").val(ui.item ? ui.item.value : "0, " + this.value + ", Create New");
                $("#Deal_Name").val(this.value);
                if (!ui.item) {
                    $("#resultContactText").html("Create New " + this.value);
                }
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
                                value: item.companyId + ", " + item.firstName + (item.lastName ? ", " + item.lastName : ", " + item.firstName)
                            }
                        }));
                    }
                })
            },
            appendTo: "#resultCompany",
            select: function (event, ui) {
                $("#CompanyIds").val(ui.item ? ui.item.value : "0, " + this.value + ", Create New");
                $("#Deal_Name").val(this.value);
                if (!ui.item) {
                    $("#resultCompanyText").html("Create New" + this.value);

                }
                //alert(ui.item ? this.value = "Selected: " + ui.item.value : "Nothing selected, input was " + this.value);
            },
            change: function (event, ui) {
                $("#CompanyIds").val(ui.item ? ui.item.value : "0, " + this.value + ", Create New");
                $("#Deal_Name").val(this.value);
                if (!ui.item) {
                    $("#resultCompanyText").html("Create New " + this.value);
                }
            }

        });
    }


}

var pipeline = new Pipeline();