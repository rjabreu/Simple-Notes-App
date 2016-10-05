var api = 'api/Notes';

$(document).ready(function () {
    

    $('#button-save').click(function () {
        postData();
    });

    $('#button-find').click(function () {
        var id = $('#note-id').val();
        var password = $('#password').val();
        readData(id,password);
    });

    //getAllNotes(); if needed to display all notes
});



function postData() {
    var title = $('#title').val();
    var content = $('#content').val();
    var password = $('#password').val();

    var newNote = {
        id: 0,
        title: title,
        content: content,
        password: password
    };

    $.ajax({
        type: "POST",
        data: JSON.stringify(newNote),
        url: api + "/CreateNote",
        contentType: "application/json",
        success: function (data) {
            console.log(data);
            $('#message').text("The note with id " + data + " was saved successfully. Add some more!");
        }
    });
}

function readData(id,password) {
    $.getJSON(api + "/GetNote?id="+id+"&password="+password, function (data) {
        $('.note-title').append(data.Title);
        $('.note-id').append(data.Id);
        $('.note-content').append(data.Content);
    });
}

function getAllNotes() {
    $.getJSON(api + "/GetAllNotes", function (data) {
        
        if (data.length) {
            
            data.forEach(function (note) {
                console.log(note)
                var item = '<li>' + note.Title + '</li>';
                $('.notes').append(item);
            });
           
        }
    });
}