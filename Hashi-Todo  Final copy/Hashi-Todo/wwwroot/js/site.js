function DoneTodo(i) {
    $.ajax({
        url: 'https://localhost:5001/Todo/Done',
        type: 'POST',
        data: {
            id: i
        },
        success: function () {
            window.location.reload();
        }
    });
}

function formMaker(i) {
    $.ajax({
        url: 'https://localhost:5001/Todo/PopulateForm',
        type: 'GET',
        data: {
            id: i
        },
        dataType: 'json',
        success: function (response) {
            $("#Todo_Name").val(response.name);
            $("#Todo_Id").val(response.id);
            $("#form-button").val("Update Todo");
            $("#form-action").attr("action", "/Todo/Update");
        }
    });
}
