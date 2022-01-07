function onPasswordPageLoaded ( account, create )
{
    const check_upper = document.getElementById("input-upper")
    const check_lower = document.getElementById("input-lower")
    const check_symbols = document.getElementById("input-symbols")
    const check_numbers = document.getElementById("input-numbers")
    const input_size = document.getElementById("input-password-size")
    const label_size = document.getElementById("preview-size")
    const btn_submit = document.getElementById("btn_validate")
    const password_preview = document.getElementById("span-password")
    const btn_back = document.getElementById("btn_back")
    const btn_generate = document.getElementById("btn_generate")
    const input_mandatory = document.getElementById("input-mandatory")
    const btn_copy = document.getElementById("btn_copy")

    // Update the displayed password
    if (account != null && account.password != "")
    {
        password_preview.innerHTML = account.password
    }

    // On size-input change 
    input_size.oninput = () =>
    {
        label_size.innerHTML = input_size.value
    }
    // First update of the size preview
    input_size.oninput()

    // On Button click
    btn_submit.onclick = () =>
    {
        // Update the account password attribute
        account.password = password_preview.innerText
        
        // Go back to the right page
        create ? DisplayNewAccountEditPage(account) : DisplayAccountEditPage(account)
    }

    btn_generate.onclick = () => 
    {
        // Send a request for a random password
        let result = generatePassword(
            input_size.value,
            check_upper.checked == '1',
            check_lower.checked == '1',
            check_symbols.checked == '1',
            check_numbers.checked == '1',
            input_mandatory.value
        )

        // Update the displayed password
        password_preview.innerHTML = result.Password 

        // Update the copy button
        UnCheckCopyIcon(btn_copy.lastChild)
    }

    btn_copy.onclick = () => 
    {
        // Copy the password to the clipboard
        setClipboard ( password_preview.innerText )
        
        // Change the image of the button
        CheckCopyIcon(btn_copy.lastChild)
    }

    btn_back.onclick = () => create ? DisplayNewAccountEditPage(account) : DisplayAccountEditPage( account )

}