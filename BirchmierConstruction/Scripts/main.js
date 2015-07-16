var isShowing = false;

$("li.info").hide();
$("p.ifNoContacts").hide();
$("#showhideresourceinfo").click(function () {
    if (isShowing) {
        $("li.info").hide();
        $("p.ifNoContacts").hide();
        $("#showhideresourceinfo").html("Show Employees");
        isShowing = false;
    }
    else {
        $("li.info").show();
        $("p.ifNoContacts").show();
        $("#showhideresourceinfo").html("Hide Employees");
        isShowing = true;
    }
});

function AJAX(url, data, callback, type, errorFunc) {
    $.ajax(
        {
            url: url,
            data: data,
            type: type == undefined ? "GET" : type,
            success: function (result) {
                if (callback)
                    callback(result);
            },
            error: function (result) {
                if (errorFunc)
                    errorFunc(result);
            }
        });
};

var saveBaseLine = function (me, id) {
    AJAX("/Home/SaveBaseLine",
        { id: id, save: me.checked },
        function (result) {
            $('#saveMessage').fadeTo(100, 1).html(result.Message).css('color', result.Success ? 'green' : 'red')
                .fadeTo(1800, 0);
        }, "POST",
        function (result) {
            $('#saveMessage').fadeTo(100, 1).html(result).css('color', 'red').fadeTo(1800, 0);
        });
};

var Delete = function (me, prop) {
    var id = me.id.split("_")[1];
    AJAX("/Home/Delete" + prop, { projID: id }, function (result) {
        alert(result.Result);
        window.location = '/Home/Index';
    }, "POST");
};

$(document).ready(function () {
    var InitEditing = function (prop) {
        $('[id^="Edit' + prop + '"]').click(function () {
            var propped = prop + '_';
            var id = $(this).attr('id').split("_")[1];
            var editing = $('#' + propped + id).attr('class');
            var taskProp = $('#' + propped + id).html();

            if (editing == 'pre_edit') {
                $(this).attr('src', "/Resources/save.png");
                $(this).attr('title', "Save");
                $('#' + propped + id).html('<input type="text" value="' + taskProp + '"/>');
                $('#' + prop + 'Cell_' + id).append('<a id="cancelEdit' + propped + id + '" onclick="cancelEditTask(this, \'' + taskProp + '\', \'' + prop + '\')" href="javascript:void(0)"><img style="width:14px;height:14px;" src="/Resources/cancel.png" title="Cancel"/></a>');
                $('#' + propped + id).attr('class', 'in_edit');
            }
            else {
                var val = $('#' + propped + id + " input").val();
                if ((val.trim() !== "" && prop === "TaskName") || prop === "TaskPredecessors") {
                    $('#cancelEdit' + propped + id).remove();
                    $(this).attr('src', "/Resources/pencil.png");
                    $(this).attr('title', "Edit");
                    AJAX("/Home/Update" + prop, { taskid: id, name: val }, function (result) {
                        $('#' + propped + result.TaskId).html(result.TaskName);
                        $('#' + propped + id).attr('class', 'pre_edit');
                    });
                }
            }
        });
    };

    InitEditing("TaskName");
    InitEditing("TaskPredecessors");
});

function cancelEditTask(e, TaskProp, prop) {
    var id = $(e).attr('id').split("_")[1];
    $('#cancelEdit' + prop + '_' + id).remove();
    $('#Edit' + prop + '_' + id).attr('src', '/Resources/pencil.png');
    $('#Edit' + prop + '_' + id).attr('title', "Edit");

    $('#' + prop + '_' + id).html(TaskProp);
    $('#' + prop + '_' + id).attr('class', 'pre_edit');
};

function changeCompletion(e) {
    var taskElement = $(e).attr('id');
    var taskId = taskElement.split("_")[1];
    var percentComplete = $(e).val();
    AJAX("/Home/UpdateCompletion", { taskid: taskId, percent: percentComplete }, function (result) {
        if (result.Success) {
            $('#taskCompletion_' + result.TaskId).html(result.Percent + "%");

            $('#taskRow_' + result.TaskId).css('background-color', result.Percent == 100 ? 'lightblue' : 'white');
        }
        else
            alert(result.Message);
    });
}

function changeResource(e) {
    var taskElement = $(e).attr('id');
    var taskId = taskElement.split("_")[1];
    var resourceId = $(e).val();
    AJAX("/Home/UpdateResource", { taskid: taskId, resourceid: resourceId }, function (result) {
        if (result.Success) {
            $('#taskResource_' + result.TaskId).html(result.ResourceID == null ? result.Resource : "<a href='/Resource/ResourceDetails/" + result.ResourceID + "'>" + result.Resource + "</a>");
        }
        else
            alert(result.Message);
    });
}