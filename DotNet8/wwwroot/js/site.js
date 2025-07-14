document.addEventListener("DOMContentLoaded", function () {

    let editorDelete = document.getElementById("deleteInput");
    if (editorDelete) {
        editorDelete.addEventListener("click", function (event) {
            event.preventDefault();
            if (confirm("Want to Delete?")) {
                document.getElementById("deleteForm").submit();
            }
        });
    }

    let editorRepeat = document.querySelector("select#Repeat")
    if (editorRepeat) {
        editorRepeat.addEventListener("change", function (e) {
            let everyXdaysField = document.querySelector("#everyXdaysField");
            let selected = e.target.value;
            let selectedText = e.target.querySelector('option[value="' + selected + '"]').text;
            if (selectedText == "EveryXdays") {
                everyXdaysField.style.display = "block";
            } else {
                everyXdaysField.style.display = "none";
            }
        });
    }

});