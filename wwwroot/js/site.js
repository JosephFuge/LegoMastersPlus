// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(document).ready(function () {
//    var acceptCookieConsent = localStorage.getItem("acceptCookieConsent");
//    if (acceptCookieConsent != "true") {
//        $("#cookieConsentContainer").fadeIn(200);
//    }
//});

//function acceptCookieConsent() {
//    localStorage.setItem("acceptCookieConsent", "true");
//    $("#cookieConsentContainer").fadeOut(200);
//}

$(document).ready(function () {
    var acceptCookieConsent = localStorage.getItem("acceptCookieConsent");
    var shownInSession = sessionStorage.getItem("cookieNotificationShown");
    if (acceptCookieConsent != "true") {
        if (shownInSession != "true") {
            $('#cookieConsentModal').modal('show');
        }
    } else if (shownInSession != "true") {
        sessionStorage.setItem("cookieNotificationShown", "true");
    }

    $('.show-cookie-btn').click(function () {
        sessionStorage.setItem("cookieNotificationShown", "true");
        $('#cookieConsentModal').modal('show');
    });


    $('.close-modal').click(function () {
        sessionStorage.setItem("cookieNotificationShown", "true");
        $('#cookieConsentModal').modal('hide');
    });

    $("#acceptCookieConsent").click(function () {
        localStorage.setItem("acceptCookieConsent", "true");
        sessionStorage.setItem("cookieNotificationShown", "true");

        $.ajax({
            type: "POST",
            url: "/Home/Consent",
            success: function () {
                $('#cookieConsentModal').modal('hide');
            }
        });
    });
});