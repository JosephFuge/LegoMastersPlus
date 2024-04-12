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


// cookies:

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

// remove product: 
$(document).ready(function () {
    $('a.btn-danger').click(function (event) {
        event.preventDefault(); // Prevent the default link behavior

        var deleteUrl = $(this).data('url'); // Use the URL from the data-url attribute

        $('#deleteConfirmationModal').modal('show'); // Show the confirmation modal

        $('.close-modal').click(function () {
            $('#cookieConsentModal').modal('hide');
        });

        $('#confirmDelete').off().click(function () {
            $.ajax({
                type: "POST",
                url: deleteUrl, // Use the URL fetched from the button's data-url attribute
                success: function () {
                    $('#deleteConfirmationModal').modal('hide');
                    location.reload(); // Reload the page to reflect changes
                },
                error: function () {
                    alert('Failed to delete the product.'); // Handle error
                }
            });
        });
    });
});

// delete customer: 
$(document).ready(function () {
    $('a.deleteCustomerButton').click(function (event) {
        event.preventDefault(); // Prevent the default link behavior

        var deleteUrl = $(this).data('url'); // Use the URL from the data-url attribute

        $('#customerDeleteConfirmationModal').modal('show'); // Show the confirmation modal

        $('.close-modal').click(function () {
            $('#customerDeleteConfirmationModal').modal('hide');
        });

        $('#customerConfirmDelete').off().click(function () {
            $.ajax({
                type: "POST",
                url: deleteUrl, // Use the URL fetched from the button's data-url attribute
                success: function () {
                    $('#customerDeleteConfirmationModal').modal('hide');
                    location.reload(); // Reload the page to reflect changes
                },
                error: function () {
                    alert('Failed to delete the product.'); // Handle error
                }
            });
        });
    });
});

// delete user: 
$(document).ready(function () {
    $('a.deleteUserButton').click(function (event) {
        event.preventDefault(); // Prevent the default link behavior

        var deleteUrl = $(this).data('url'); // Use the URL from the data-url attribute

        $('#userDeleteConfirmationModal').modal('show'); // Show the confirmation modal

        $('.close-modal').click(function () {
            $('#userDeleteConfirmationModal').modal('hide');
        });

        $('#userConfirmDelete').off().click(function () {
            $.ajax({
                type: "POST",
                url: deleteUrl, // Use the URL fetched from the button's data-url attribute
                success: function () {
                    $('#userDeleteConfirmationModal').modal('hide');
                    location.reload(); // Reload the page to reflect changes
                },
                error: function () {
                    alert('Failed to delete the product.'); // Handle error
                }
            });
        });
    });
});

//confirm cart item remove

$(document).ready(function () {
    $('#deleteConfirmationModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var productID = button.data('productid'); // Extract product ID from data-* attributes
        // Ensure you have a returnUrl set correctly in your model, or adjust as necessary
        

        // Set the form's action to include the handler
        // Make sure to replace '/YourPage' with the actual path to your Razor Page
        $(this).find('#deleteForm').attr('action', '/Cart?handler=Remove');

        // Update the form inputs
        $(this).find('input[name="product_ID"]').val(productID);
        
    });

    // This part is optional, shown here for completeness
    // If you need AJAX submission, you'd handle it here
});


