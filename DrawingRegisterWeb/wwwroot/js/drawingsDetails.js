//Disable and enebable file upload box submit button
const submitfile = document.getElementById('submitFile');
const inputFile = document.getElementById('inputFile');

if (inputFile != null) {
	inputFile.addEventListener("change", function (event) {
		const target = event.target

		// Restrict from uploading files begger then 10 MB
		if (target.files && target.files[0]) {
			const maxAllowedSize = 10 * 1024 * 1024;
			if (target.files[0].size > maxAllowedSize) {
				Swal.fire({
					icon: 'error',
					title: 'Oops...',
					text: 'The maximum filesize is limited to 10 MB...'
				});
				target.value = '';
			}
		}

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