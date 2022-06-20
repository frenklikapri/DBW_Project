export function showModalWithId(modalId) {
    $(`#${modalId}`).modal("show")
}

export function hideModalWithId(modalId) {
    $(`#${modalId}`).modal("hide")
}

export function showSuccess(message, title = "Success") {
    setToastrOptions();
    toastr["success"](message, title)
}

export function showError(message, title = "Error") {
    setToastrOptions();
    toastr["error"](message, title)
}

export function setToastrOptions() {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}