// Disable and enebable documentation file upload box submit button
const submitDocumentation = document.getElementById('submitDocumentationFile');
const documentationFile = document.getElementById('inputDocumentationFile');

submitDocumentation.disabled = true;
documentationFile.addEventListener("change", function (event) {
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
layoutFile.addEventListener("change", function (event) {
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