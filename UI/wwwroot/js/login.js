function onLoginPageLoaded ()
{
    const msg_error = document.getElementById('msg-error')
    const password_field = document.getElementById('input_pwd')
    const btn_submit = document.getElementById('btn-submit')

    console.log("test") ;

    btn_submit.onclick = async () =>
    {
        // Reset the error msg
        msg_error.style.visibility = "hidden"

        // Call the login request
        var result = await openChest(password_field.value)

        console.log("result from login page : ")
        console.log(result)

        // If wrong password
        if (!result.Success)
        {
            msg_error.style.visibility = "visible"
            return
        }

        // if right password
        DisplayAccountsPage()
    } 
}