const { ipcRenderer } = require("electron") ;

function openChest ( password )
{
    return JSON.parse(ipcRenderer.sendSync("open-chest", password))
}

function deleteAccount ( account )
{
    console.log("account to be deleted : ")
    console.log(account) 
    ipcRenderer.sendSync("delete-account", account.Id) ;
}

function decryptPassword (account)
{
    return JSON.parse(ipcRenderer.sendSync("decrypt-password", account))
}

function editAccount ( oldAccount, newAccount )
{

    var result = ipcRenderer.sendSync("edit-account", newAccount)

    return JSON.parse(result)
}

function editPassword (oldPassword, newPassword)
{
    var result = ipcRenderer.sendSync("set-password", 
    {
        OldPassword: oldPassword,
        NewPassword: newPassword
    })

    return result
}

function createPassword (password)
{
    var result = ipcRenderer.sendSync("create-password", password)
    return JSON.parse(result)
}

function isPasswordRegistered ()
{
    return JSON.parse(ipcRenderer.sendSync("is-password-registered", null))
}

function addAccount (account)
{
    console.log("try to add account : ")
    console.log(account)

    var result = ipcRenderer.sendSync("add-account", 
    {
        Name: account.Name,
        AccountClearPassword: account.password,
        Link: account.link,
        Username: account.username
    }) 

    return JSON.parse(result) ;
}

function generatePassword ( 
    size,
    upper, lower, symbols, numbers,
    mandatory_letters )
{
    console.log("generate password result :")
    console.log(arguments)

    var result = ipcRenderer.sendSync("generate-password", 
    {
        PasswordLength: parseInt(size), 
        UseUpperAlphabet: upper,
        UseLowerAlphabet: lower, 
        UseNumbers: numbers,
        UseSymbols: symbols,
        ForcedSubsets: Array.from(mandatory_letters)
    })
    
    return JSON.parse(result)
}

async function setClipboard ( content )
{
    console.log(`clipboard content set to "${content}" !`)
}

function GetAccounts ()
{
    var result = ipcRenderer.sendSync("get-accounts", null)
    return JSON.parse(result)
}