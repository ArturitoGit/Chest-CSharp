function onAccountPageLoaded (account)
{
    const back_btn = document.getElementById("btn_back")
    const delete_btn = document.getElementById("btn_delete")
    const edit_btn = document.getElementById("btn_edit")
    const link_field = document.getElementById("link-field")
    const username_field = document.getElementById("username-field")
    const password_field = document.getElementById("password-field")
    const name_field = document.getElementById("name-field")

    const btn_copy_link = document.getElementById("btn_copy_link")
    const btn_copy_username = document.getElementById("btn_copy_username")
    const btn_copy_password = document.getElementById("btn_copy_password")

    console.log("decrypt password is going to be sent, here is the account : ")
    console.log(account)

    // Get the account infos from the core project
    var result = decryptPassword(account)
    var password = result.Success ? result.ClearPassword : "???"

    // Back action
    back_btn.onclick = () => DisplayAccountsPage() 

    fillFields(account, password)
    // Fill the fields of the account
    function fillFields (account, password)
    {
        link_field.innerHTML = account.Link || ""
        username_field.innerHTML = account.Username || ""
        name_field.innerHTML = account.Name
        password_field.innerHTML = password || ""
    }

    // Delete btn action
    delete_btn.onclick = () => 
    {
        // Delete the account
        deleteAccount( account )

        // Display the menu
        DisplayAccountsPage() 
    }

    // Edit btn action
    edit_btn.onclick = () => DisplayAccountEditPage( account ) ;

    btn_copy_link.onclick = () => 
    {
        ResetAllCopyIcons()
        setClipboard(link_field.innerText)
        CheckCopyIcon(btn_copy_link)
    }

    btn_copy_password.onclick = () => 
    {
        ResetAllCopyIcons()
        setClipboard(password_field.innerText)
        CheckCopyIcon(btn_copy_password)
    }

    btn_copy_username.onclick = () =>
    {
        ResetAllCopyIcons()
        setClipboard(username_field.innerText)
        CheckCopyIcon(btn_copy_username)
    }

    function ResetAllCopyIcons () {
        Array.from(document.getElementsByClassName("bi bi-clipboard-check"))
            .forEach(i => UnCheckCopyIcon(i))
    }

}