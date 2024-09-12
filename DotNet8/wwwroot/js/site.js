document.addEventListener("DOMContentLoaded", function () {

    var deleteInput = document.getElementById("deleteInput");
    if (deleteInput) {
        deleteInput.addEventListener("click", function (event) {
            event.preventDefault();
            if (confirm("Want to Delete?")) {
                document.getElementById("deleteForm").submit();
            }
        });
    }

    document.querySelector("select#Repeat").addEventListener("change", function (e) {
        let everyXdaysField = document.querySelector("#everyXdaysField");
        let selected = e.target.value;
        let selectedText = e.target.querySelector('option[value="' + selected + '"]').text;
        if (selectedText == "EveryXdays") {
            everyXdaysField.style.display = "block";
        } else {
            everyXdaysField.style.display = "none";
        }
    });

});