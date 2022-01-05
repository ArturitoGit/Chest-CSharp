
// Import the templates of all of the 
ImportTemplate("login.html", DisplayLoginPage )
ImportTemplate("accounts.html")
ImportTemplate("account-edit.html")
ImportTemplate("account.html")
ImportTemplate("password.html")
ImportTemplate("password-edit.html")

function ImportTemplate ( template_path, cb )
{
    // Create a div and load the template inside
    $("<div/>").load("html/" + template_path, function () {
        // Add this div inside the templates part of the DOM
        $(this).appendTo("#templates")
        // Call the callback function if there is one
        if (typeof(cb) == 'function') cb()
    });
}

