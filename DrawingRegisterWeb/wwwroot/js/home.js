// Control img id = "HomeIntroduction" size
const homeIntroduction = document.getElementById("HomeIntroduction");
const marginForImg = document.getElementById("marginForHomeIntroduction");

window.addEventListener("resize", changeImgSize);

changeImgSize();
showImg();

function changeImgSize() {
	if (window.innerWidth > window.innerHeight) {
		homeIntroduction.style.width = "30vh";
		marginForImg.style.marginBottom = "20vh";
	}
	else {
		homeIntroduction.style.width = "30vw";
		marginForImg.style.marginBottom = "20vw";
	}
}

function showImg() {
	homeIntroduction.style.display = "block";
}

// Relocate on click to drawsql site
function sqlDiagram() {
	window.location.href = "https://drawsql.app/teams/solo-project/diagrams/drawing-register-v2";
}