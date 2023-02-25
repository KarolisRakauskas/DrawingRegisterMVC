// Disable and enebable documentation file upload box submit button
const submitDocumentation = document.getElementById('submitDocumentationFile');
const documentationFile = document.getElementById('inputDocumentationFile');

submitDocumentation.disabled = true;
documentationFile.addEventListener("change", function () {
    if (documentationFile.value == "") {
        submitDocumentation.disabled = true;
    }
    else {
        submitDocumentation.disabled = false;
    }
});




// Disable and enebable Layout file upload box submit button
const submitLayout = document.getElementById('submitLayoutFile');
const layoutFile = document.getElementById('inputLayoutFile');

submitLayout.disabled = true;
layoutFile.addEventListener("change", function () {
    if (layoutFile.value == "") {
        submitLayout.disabled = true;
    }
    else {
        submitLayout.disabled = false;
    }
});




//Save Last Opened nav-tab in Session Storage
$(function () {
    $('.nav-tabs a').on('show.bs.tab', function () {
        sessionStorage.setItem('lastTab', $(this).attr('href'));
    });
    var lastTab = sessionStorage.getItem('lastTab');
    if (lastTab) {
        $('[href="' + lastTab + '"]').tab('show');
    }
});