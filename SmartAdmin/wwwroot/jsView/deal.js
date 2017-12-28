class Deal
{
    MakeDropZone(e)
    {
        Dropzone.autoDiscover = false;
        $(e).dropzone({
            //url: "/file/post",
            addRemoveLinks: true,
            maxFilesize: 0.5,
            dictDefaultMessage: '<span class="text-center"><span class="font-lg visible-xs-block visible-sm-block visible-lg-block"><span class="font-lg"><i class="fa fa-caret-right text-danger"></i> Drop files <span class="font-xs">to upload</span></span><span>&nbsp&nbsp<h4 class="display-inline"> (Or Click)</h4></span>',
            dictResponseError: 'Error uploading file!',
            success: function (file, response) {
                this.AddLI("Doc Adicionado");
            }
        });
    }



    Makeditable(e)
    {

        $(e).editable({
            url: "/Deal/PostInfo",
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

   
    AddComment(e)
    {
        $.ajax({
            dataType: "json",
            type: "POST",
            url: "/Deal/PostComment",
            data: { id: $(e).attr("data-id"), Comments: $("#Comments").val() },
            success: function (response) {
                $.smallBox({
                    title: "Add",
                    content: "<i class='fa fa-clock-o'></i> <i> 1 seconds ago...</i>" + response[0].msg,
                    color: "#296191",
                    iconSmall: "fa fa-thumbs-up bounce animated",
                    timeout: 4000
                });
                $("#Comments").val("");
            }
        }).done(function (response) {
            this.AddLI(response[0].msg);
        }.bind(this));
    }


    AddTask(data) {
        this.AddLI(data[0].msg);
        $.smallBox({
            title: "Task Add",
            content: "<i class='fa fa-clock-o'></i> <i> 1 seconds ago...</i>" + data[0].msg,
            color: "#296191",
            iconSmall: "fa fa-thumbs-up bounce animated",
            timeout: 4000
        });

        $("#Task_Name").val("");
        $("#DueDate").val("");
        $("#Task_Time").val("");
        $("#Task_Duration").val("");
        $("#Task_Comments").html("");
        $("#Task_Comments").val("");
        $("#Task_Done").value = false;
        $("#Task_Comments").value = "";

    }

    SetNameTask(e){
        var s = $(e).attr("data-text");
        $("#Task_Name").val($(e).attr("data-text"));
    }

    AddLI(msg) {
        
        var newLi = $(
            '<li>' +
            ' <div class="smart-timeline-icon">' +
            ' <i class="fa fa-file-text-o"></i>' +
            ' </div>' +
            '<div class="smart-timeline-time">' +
            '<small> Agora Mesmo </small>' +
            '</div>' +
            '<div class="smart-timeline-content">' +
            '<a href="javascript:void(0);"><strong>Adicionado!</strong></a>' +
            '<blockquote>' +
            '<p>' + msg + '</p>' +
            '</blockquote>' +
            '<p class="note">' +
            'Você Agora  mesmo' +
            '</p>' +
            '</div>' +
            '</li>'
        ).hide();
        $("#timelinedeal li:first").append(newLi.fadeIn(800));
    }



}



var deal = new Deal();