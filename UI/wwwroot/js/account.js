function onAccountPageLoaded (account)
{
    const back_btn = document.getElementById("btn_back")
    const delete_btn = document.getElementById("btn_delete")
    const edit_btn = document.getElementById("btn_edit")
    const link_field = document.getElementById("link-field")
    const username_field = document.getElementById("username-field")
    const password_field = document.getElementById("password-field")

    const btn_copy_link = document.getElementById("btn_copy_link")
    const btn_copy_username = document.getElementById("btn_copy_username")
    const btn_copy_password = document.getElementById("btn_copy_password")

    // Back action
    back_btn.onclick = () => DisplayAccountsPage() 

    fillFields(account)
    // Fill the fields of the account
    function fillFields (account)
    {
        link_field.innerHTML = account.link || ""
        username_field.innerHTML = account.username || ""
        password_field.innerHTML = account.password || ""
    }

    // Delete btn action
    delete_btn.onclick = () => 
    {
        deleteAccount( account )
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