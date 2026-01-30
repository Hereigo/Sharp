window.onerror = function (message, source, lineno, colno, error) {
    console.error("JS Error:", message);
    // Notify WPF
    if (window.chrome?.webview) {
        window.chrome.webview.postMessage({
            type: "js-error",
            message,
            source,
            lineno,
            colno
        });
    }
    alert(message);
};

function createTableFromJSON(data) {
    const table = document.getElementById("dataTable");
    const tbody = table.querySelector("tbody");
    // Clear previous content
    tbody.innerHTML = "";
    if (!data || data.length === 0) return;
    // const thead = table.querySelector("thead");
    // const headerRow = document.createElement("tr");
    // Object.keys(data[0]).forEach(key => {
    //     const th = document.createElement("th");
    //     th.textContent = key;
    //     headerRow.appendChild(th);
    // });
    // thead.appendChild(headerRow);
    let ind = 1;
    data.forEach(record => {
        const row = document.createElement("tr");
        const td0 = document.createElement("td");
        td0.classList.add('border');
        td0.textContent = ind++;
        row.appendChild(td0);
        const td1 = document.createElement("td");
        td1.classList.add('border');
        td1.innerHTML = '<input type="checkbox"/>';
        row.appendChild(td1);
        Object.values(record).forEach(value => {
            const td = document.createElement("td");
            td.classList.add('border');
            td.textContent = value;
            row.appendChild(td);
        });
        tbody.appendChild(row);
    });
}

// fetch("data.json") // or your API URL
//     .then(res => res.json())
//     .then(json => createTableFromJSON(json));

window.receiveData = function (json) {
    // console.log(obj[0].fileName);
    createTableFromJSON(json);
};

function sendMessage() {
    chrome.webview.postMessage("Hello from Web UI!");
}

function toggleCollapseBasedOnWidth() {
    const $collapsibleDiv = $('#collapsibleDiv');
    const screenWidth = $(window).width();

    if (screenWidth < 768) {
        $collapsibleDiv.collapse('hide');
        $collapsibleDiv.addClass('collapse');
    } else {
        $collapsibleDiv.collapse('show');
        $collapsibleDiv.removeClass('collapse');
    }
}

$(document).ready(function () {

    toggleCollapseBasedOnWidth();

    $(window).resize(function () {
        toggleCollapseBasedOnWidth();
    });

    $('#toggleButton').click(function () {
        const $collapsibleDiv = $('#collapsibleDiv');
        $collapsibleDiv.collapse('toggle');
    });

    // call WPF
    //chrome.webview.postMessage("Hello from Web UI!");

    document.getElementById("folderInput").addEventListener("change", (e) => {
        if (e.target.files.length < 1) {
            alert('No files.');
        } else {
            let dirName = e.target.files[0].webkitRelativePath.split('/')[0];

            chrome.webview.postMessage(dirName);

            // for (const file of e.target.files) {
            //     console.log('1 -', file.webkitRelativePath.split('/')[0]);
            // }
        }
    });

});