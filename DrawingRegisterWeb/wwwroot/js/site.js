﻿
/*
		Table sorting by <th> elelemnts in ascending and descending 
	order, once clicked on <th> - shows arrow icon near <th> element.
	Icons implamented from https://icons.getbootstrap.com/. This 
	sorting works only on string values only.
*/

function sortTable(n) {
	var table, rows, switching, i, x, y, shouldSwitch, dir, icons, icon, switchcount = 0;
	table = document.getElementById("sortTable");
	icons = document.getElementsByClassName("sortIcon");
	icon = table;
	switching = true;
	dir = "asc";

	Array.from(icons).forEach(i => i.classList.remove(
		"bi-caret-up-fill", "bi-caret-down-fill"));

	while (switching) {
		switching = false;
		rows = table.rows;
		for (i = 1; i < (rows.length - 1); i++) {
			shouldSwitch = false;
			x = rows[i].getElementsByTagName("TD")[n];
			y = rows[i + 1].getElementsByTagName("TD")[n];

			if (dir == "asc") {
				if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			} else if (dir == "desc") {
				if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			}
		}
		if (shouldSwitch) {
			rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
			switching = true;
			switchcount++;
		} else {
			if (switchcount == 0 && dir == "asc") {
				dir = "desc";
				switching = true;
			}
		}
	}

	if (dir == "asc") {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-up-fill");
	} else {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-down-fill");
	}
}



/*
	Bootstrap: 
		Tooltip - Get Tooltip function
		Popover - Get Popover function
*/

$(document).ready(function () {
	$('[data-bs-toggle="tooltip"]').tooltip();
});

$(document).ready(function () {
	$('[data-bs-toggle="popover"]').popover();
});




/*
		Project State Warning alert display controll from local storage.
	In ProjectStates/Index button #alertState has Onclick function => turnOffStateAlert()
*/

const getLocalAlert = localStorage.getItem('alertNone');
const alert = document.getElementById("alertState");

function turnOffStateAlert() {
	Swal.fire({
		title: 'Do you want to save the changes?',
		showDenyButton: true,
		showCancelButton: true,
		confirmButtonText: 'Save',
		confirmButtonColor: '#dc3545',
		denyButtonText: `Don't save`,
		denyButtonColor: '#0d6efd'
	}).then((result) => {
		if (result.isConfirmed) {
			Swal.fire('Saved!', '', 'success')
			localStorage.setItem("alertNone", "off")
			alert.style.display = "none";
		} else if (result.isDenied) {
			Swal.fire('Changes are not saved', '', 'info')
		}
	})
}




/*
		sweetalert2 upload load screen
*/
function load() {
	Swal.fire({
		title: 'Uploading...',
		html: 'Please wait...',
		allowEscapeKey: false,
		allowOutsideClick: false,
		didOpen: () => {
			Swal.showLoading()
		}
	});
}




/*
		SORT TALBE ONLY FOR DOCUMENTATION TOGLE TAB

		TODO: select table not from id...

		Table sorting by <th> elelemnts in ascending and descending 
	order, once clicked on <th> - shows arrow icon near <th> element.
	Icons implamented from https://icons.getbootstrap.com/. This 
	sorting works only on string values only.
*/

function sortTableDoc(n) {
	var table, rows, switching, i, x, y, shouldSwitch, dir, icons, icon, switchcount = 0;
	table = document.getElementById("sortTableDoc");
	icons = document.getElementsByClassName("sortIcon");
	icon = table;
	switching = true;
	dir = "asc";

	Array.from(icons).forEach(i => i.classList.remove(
		"bi-caret-up-fill", "bi-caret-down-fill"));

	while (switching) {
		switching = false;
		rows = table.rows;
		for (i = 1; i < (rows.length - 1); i++) {
			shouldSwitch = false;
			x = rows[i].getElementsByTagName("TD")[n];
			y = rows[i + 1].getElementsByTagName("TD")[n];

			if (dir == "asc") {
				if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			} else if (dir == "desc") {
				if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			}
		}
		if (shouldSwitch) {
			rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
			switching = true;
			switchcount++;
		} else {
			if (switchcount == 0 && dir == "asc") {
				dir = "desc";
				switching = true;
			}
		}
	}

	if (dir == "asc") {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-up-fill");
	} else {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-down-fill");
	}
}




/*
		SORT TALBE ONLY FOR LAYOUTS TOGLE TAB

		TODO: select table not from id...

		Table sorting by <th> elelemnts in ascending and descending 
	order, once clicked on <th> - shows arrow icon near <th> element.
	Icons implamented from https://icons.getbootstrap.com/. This 
	sorting works only on string values only.
*/

function sortTableLay(n) {
	var table, rows, switching, i, x, y, shouldSwitch, dir, icons, icon, switchcount = 0;
	table = document.getElementById("sortTableLay");
	icons = document.getElementsByClassName("sortIcon");
	icon = table;
	switching = true;
	dir = "asc";

	Array.from(icons).forEach(i => i.classList.remove(
		"bi-caret-up-fill", "bi-caret-down-fill"));

	while (switching) {
		switching = false;
		rows = table.rows;
		for (i = 1; i < (rows.length - 1); i++) {
			shouldSwitch = false;
			x = rows[i].getElementsByTagName("TD")[n];
			y = rows[i + 1].getElementsByTagName("TD")[n];

			if (dir == "asc") {
				if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			} else if (dir == "desc") {
				if (x.innerHTML.toLowerCase() < y.innerHTML.toLowerCase()) {
					shouldSwitch = true;
					break;
				}
			}
		}
		if (shouldSwitch) {
			rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
			switching = true;
			switchcount++;
		} else {
			if (switchcount == 0 && dir == "asc") {
				dir = "desc";
				switching = true;
			}
		}
	}

	if (dir == "asc") {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-up-fill");
	} else {
		icon.getElementsByTagName("I")[n].classList.add("bi-caret-down-fill");
	}
}