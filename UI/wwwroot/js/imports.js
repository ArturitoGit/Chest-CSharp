
// Import the templates of all of the 
ImportTemplate("accounts.html")
ImportTemplate("account-edit.html")
ImportTemplate("account.html")
ImportTemplate("password.html")

// Pages login and password-edit must be loaded before displaying anything
ImportTemplate("login.html", () =>
    ImportTemplate("password-edit.html", 
        WhenInitialImported // Display the first page
    )
)



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



async function WhenInitialImported ()
{
    // Check that there is already a password 
    var result = await isPasswordRegistered()

    // If there is no password registered create one by redirecting
    if (result.IsPasswordRegistered) 
        DisplayLoginPage() 
    else 
        DisplayPasswordEditPage(true)

}

