function onNewAccountEditPageLoaded (account)
{
    const btn_back = document.getElementById("btn_back")
    const name_input = document.getElementById("input-name")
    const link_input = document.getElementById("input-link")
    const username_input = document.getElementById("input-username")
    const password_input = document.getElementById("input-password")
    const btn_submit = document.getElementById("btn_validate")
    const error_span = document.getElementById("error-msg")
    const btn_pwd = document.getElementById("generate-button")
    const title_field = document.getElementById("title-field")

    console.log("account received : " + account) 

    
    btn_back.onclick = () => DisplayAccountsPage()
    btn_pwd.onclick = () => DisplayPasswordPage(getCurrentPassword(), true)

    // Fill the input fields if some are given as parameter
    if (account != null)
    {
        name_input.value = account.Name || ""
        link_input.value = account.link || ""
        username_input.value = account.username || ""
        password_input.value = account.password || ""
    }

    // Change the title to "create account"
    title_field.innerHTML = "Create account"


    btn_submit.onclick = async () => {

        // Hide the error msg
        error_span.style.visibility = "hidden"

        // Try to add the account
        var result = await addAccount (
            {
                Name: name_input.value,
                link: link_input.value,
                username: username_input.value,
                password: password_input.value
            }
        )
        
        if (result.Success)
        {
            // Go back to the accounts page
            DisplayAccountsPage()
            return
        }

        // If failure print the error msg
        error_span.innerHTML = result.Errors[0] || "Unknown error ..."
        error_span.style.visibility = "visible"
    }

    function getCurrentPassword ()
    {
        return {
            Name: name_input.value,
            link: link_input.value,
            username: username_input.value,
            password: password_input.value 
        }
    }
}

function onAccountEditPageLoaded ( account )
{
    const btn_back = document.getElementById("btn_back")
    const name_input = document.getElementById("input-name")
    const link_input = document.getElementById("input-link")
    const username_input = document.getElementById("input-username")
    const password_input = document.getElementById("input-password")
    const btn_submit = document.getElementById("btn_validate")
    const error_span = document.getElementById("error-msg")
    const btn_pwd = document.getElementById("generate-button")

    btn_back.onclick = () => DisplayAccountsPage()
    btn_pwd.onclick = () => DisplayPasswordPage(getCurrentPassword(), false)
    
    // Fill the input fields
    name_input.value = account.Name || ""
    link_input.value = account.link || ""
    username_input.value = account.username || ""
    password_input.value = account.password || ""


    btn_submit.onclick = async () => {

        // Send the request
        var result = editAccount(
            account, 
            {
                Name: name_input.value,
                link: link_input.value,
                username: username_input.value,
                password: password_input.value 
            }
        )

        if (result.Success)
        {
            DisplayAccountPage(result.account)
            return
        }

        // If failure print the error msg
        error_span.innerHTML = result.Errors[0] || "Unknown error ..."
        error_span.style.visibility = "visible"
    }

    function getCurrentPassword ()
    {
        return {
            Name: name_input.value,
            link: link_input.value,
            username: username_input.value,
            password: password_input.value 
        }
    }

}