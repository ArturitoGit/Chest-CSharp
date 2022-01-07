function DisplayAccountsPage ()
{
    // Load the page
    SetPageContent("accounts-page")
    // Execute the javascript associated with the page
    onAccountsPageLoaded()
}

function DisplayLoginPage ()
{
    SetPageContent("login-page")
    onLoginPageLoaded()
}

function DisplayAccountEditPage ( account )
{
    SetPageContent("account-edit-page")
    onAccountEditPageLoaded( account )
}

function DisplayPasswordEditPage ()
{
    SetPageContent("password-edit-page")
    onPasswordEditPageLoaded()
}

function DisplayNewAccountEditPage (account)
{
    SetPageContent("account-edit-page")
    onNewAccountEditPageLoaded(account)
}

function DisplayAccountPage ( account )
{
    SetPageContent("account-page")
    onAccountPageLoaded( account )
}

function DisplayPasswordPage ( account, create )
{
    SetPageContent("password-page")
    onPasswordPageLoaded( account, create ) 
}

function SetPageContent ( page_id )
{
    const pages_div = document.querySelector("#pages")

    // Reset the content of the page
    pages_div.innerHTML = ""

    if ("content" in document.createElement("template")) 
    {
        // Get the template of the page
        var template = document.querySelector(`#${page_id}`);

        // Create a clone
        var clone = document.importNode(template.content, true);

        // Add it to the pages part of the body
        pages_div.appendChild(clone) ;
    }
    else {
        alert("HMTL templates not supported ...")
    }
}

// COPY BUTTON ACTIONS

function CheckCopyIcon ( icon )
{
    icon.className = "bi bi-clipboard-check"
}

function UnCheckCopyIcon ( icon )
{
    icon.className = "bi bi-clipboard"
}