/*
        Bar and Doughnut Charts. Charts using Chart.js library.
    Requires to add a link: https://cdn.jsdelivr.net/npm/chart.js@4.2.1/dist/chart.umd.min.js.
*/

const url = "/DrawingRegisters/GetData";
const barCharSelect = document.getElementById("floatingSelectProject");
const DoughnutSelect = document.getElementById("floatingSelectStates");

barCharSelect.addEventListener("change", (event) => {
    refreshCharts();
    getFromAPI(url, createChartsFromData);
});
DoughnutSelect.addEventListener("change", (event) => {
    refreshCharts();
    getFromAPI(url, createChartsFromData);
});




/*
        Fetch JSON Data from DrawingRegistersController API 
    and draws charts, based of selectList items and JSON Data.
*/

function getFromAPI(url, callback) {
    var obj;
    fetch(url)
        .then((res) => res.json())
        .then((data) => (obj = data))
        .then(() => callback(obj));
}

getFromAPI(url, createChartsFromData);

function createChartsFromData(data) {
    const barContainer = document.getElementById("barContainer");
    const doughnutContainer = document.getElementById("doughnutContainer");
    const drawingRegister = data.register;
    const selectedBar = document.getElementById("floatingSelectProject").value;
    const selectedDoughnut = document.getElementById("floatingSelectStates").value;
    const barOptions = {
        plugins: {
            legend: { display: false },
        },
        scales: {
            x: {
                grid: { display: false },
            },
            y: {
                grid: { display: false },
                ticks: { precision: 0 },
            },
        }
    };
    const doughnutOptions = { responsive: true, maintainAspectRatio: false };

    let barLabels, barData, barDataSum, dougnutLabels, dougnutData, dougnutColor, dougnutDataSum;

    // Choose Data from the API call that represents the SelectList element
    for (let i = 0; i < data.myBarChartList.length; i++) {
        if (data.myBarChartList[i].projectNumber == selectedBar) {
            barLabels = data.myBarChartList[i].drawingNumber;
            barData = data.myBarChartList[i].drawingFilesCount;
            barDataSum = barData.reduce((a, b) => a + b, 0);
        }
    }

    for (let i = 0; i < data.myDoughnutChartList.length; i++) {
        if (data.myDoughnutChartList[i].config == selectedDoughnut) {
            dougnutLabels = data.myDoughnutChartList[i].states;
            dougnutData = data.myDoughnutChartList[i].projectsCount;
            dougnutColor = data.myDoughnutChartList[i].color;
            dougnutDataSum = dougnutData.reduce((a, b) => a + b, 0);
        }
    }

    console.log(barDataSum);
    console.log(dougnutDataSum);

    // Bar Chart
    if (drawingRegister) {
        let barchart = document.getElementById("bar").getContext("2d");

        new Chart(barchart, {
            type: "bar",
            options: barOptions,
            data: {
                labels: barLabels,
                datasets: [
                    {
                        label: "Drawing Files",
                        data: barData,
                        backgroundColor: "#0d6efd",
                    },
                ],
            },
        });
    }

    // Doughnut Chart
    if (drawingRegister && dougnutDataSum != 0) {
        let doughnutchart = document.getElementById("doughnut").getContext("2d");

        new Chart(doughnutchart, {
            type: "doughnut",
            options: doughnutOptions,
            data: {
                labels: dougnutLabels,
                datasets: [
                    {
                        data: dougnutData,
                        backgroundColor: dougnutColor,
                        hoverOffset: 4,
                    },
                ],
            },
        });
    }

    // Add messages to charts if there is no data
    if (barDataSum == 0) {
        const newPara = document.createElement("p");

        newPara.innerText = "This project has no Drawing files...";
        newPara.className = "text-center text-secondary position-absolute top-50 start-50 translate-middle";
        barContainer.appendChild(newPara);
    }

    if (dougnutDataSum == 0) {
        const newPara = document.createElement("p");

        newPara.innerText = "There aren't any projects in selected status...";
        newPara.className = "text-center text-secondary position-absolute top-50 start-50 translate-middle";
        doughnutContainer.appendChild(newPara);
    }
}




/*
        Rebuild Charts Canvas after selectItem has been changed.
    Chart.js requires to destroy canvas elements to change data.
*/

function refreshCharts() {
    const barContainer = document.getElementById("barContainer");
    const doughnutContainer = document.getElementById("doughnutContainer");
    let newBarchart = document.createElement("canvas");
    let newdoughnutchart = document.createElement("canvas");

    // Rebuild bar chart
    while (barContainer.firstChild) {
        barContainer.firstChild.remove();
    }

    newBarchart.setAttribute("id", "bar");
    barContainer.appendChild(newBarchart);

    // Rebuild doughnut chart
    while (doughnutContainer.firstChild) {
        doughnutContainer.firstChild.remove();
    }

    newdoughnutchart.setAttribute("id", "doughnut");
    doughnutContainer.appendChild(newdoughnutchart);
}
