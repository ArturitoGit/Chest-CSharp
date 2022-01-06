function onPasswordEditPageLoaded () {
    
    const old_password_field = document.getElementById("input-old")
    const new_password_field = document.getElementById("input-new")
    const new2_password_field = document.getElementById("input-new2")
    const btn_submit = document.getElementById("btn_validate")
    const btn_back = document.getElementById("btn_back")
    const span_error = document.getElementById("password-error-msg")

    btn_back.onclick = () => DisplayAccountsPage()

    btn_submit.onclick = async () => 
    {   
        // Reset the error message
        span_error.style.visibility = "hidden"

        // Check that the two new pwd field contain the same value
        if (new_password_field.value != new2_password_field.value)
        {
            span_error.innerText = "Confirmation doesn't match the first password ..."
            span_error.style.visibility = "visible" 
            return
        }

        // Call the core request
        var result = await editPassword(old_password_field.value, new_password_field.value) ;
        
        if (!result.success)
        {
            span_error.innerText = result.error
            span_error.style.visibility = "visible"
            return
        }

        // If success return to the accounts page
        DisplayAccountsPage()
    }

}