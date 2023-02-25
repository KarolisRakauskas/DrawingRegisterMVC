//Disable and enebable file upload box submit button
const submitfile = document.getElementById('submitFile');
const inputFile = document.getElementById('inputFile');

if (inputFile != null) {
	inputFile.addEventListener("change", function () {
		if (inputFile.value == "") {
			submitfile.disabled = true;
		}
		else {
			submitfile.disabled = false;
		}
	});
}

//CheckBox Style Control
const checkBoxLabel = document.getElementById("checkBoxLabel");

function changeText() {
	if (checkBoxLabel.classList.contains("text-primary")) {
		checkBoxLabel.classList.remove("text-primary");
		checkBoxLabel.classList.add("text-secondary");
	} else {
		checkBoxLabel.classList.remove("text-secondary");
		checkBoxLabel.classList.add("text-primary");
	}
}