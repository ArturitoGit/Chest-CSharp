function onNewAccountEditPageLoaded ()
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
    btn_pwd.onclick = () => DisplayPasswordPage(getCurrentPassword())

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

        // For debugging purpose log the result
        console.log(result)

        if (result.success)
        {
            // Go back to the accounts page
            DisplayAccountsPage()
            return
        }

        // If failure print the error msg
        error_span.innerHTML = result.error_msg || "Unknown error ..."
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

    btn_back.onclick = () => DisplayAccountPage( account )
    btn_pwd.onclick = () => DisplayPasswordPage(getCurrentPassword())
    
    // Fill the input fields
    name_input.value = account.Name || ""
    link_input.value = account.link || ""
    username_input.value = account.username || ""
    password_input.value = account.password || ""


    btn_submit.onclick = async () => {

        console.log("Beginning of the edit account procedure")

        // Send the request
        var result = await editAccount(
            account, 
            {
                Name: name_input.value,
                link: link_input.value,
                username: username_input.value,
                password: password_input.value 
            }
        )

        if (result.success)
        {
            DisplayAccountPage(result.account)
            return
        }

        // If failure print the error msg
        error_span.innerHTML = result.error_msg || "Unknown error ..."
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