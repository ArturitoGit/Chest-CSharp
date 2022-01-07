const { ipcRenderer } = require("electron") ;

async function openChest ( password )
{
    console.log("Try to open chest with password : " + password)

    return JSON.parse(await ipcRenderer.sendSync("open-chest", password))

    return Promise.resolve(
        {
            Success: password == "test"
        }
    )
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

async function editAccount ( oldAccount, newAccount )
{
    console.log("edit account from")
    console.log(oldAccount)
    console.log("to")
    console.log(newAccount)

    return Promise.resolve(
        {
            success: true,
            account:
                {
                    id: oldAccount.id,
                    Name: newAccount.Name,
                    link: newAccount.link,
                    username: newAccount.username,
                    password: newAccount.password
                }
        }
    )
}

async function editPassword (oldPassword, newPassword)
{
    var result = await ipcRenderer.sendSync("set-password", 
    {
        OldPassword: oldPassword,
        NewPassword: newPassword
    })

    return result
}

async function createPassword (password)
{
    var result = await ipcRenderer.sendSync("create-password", password)
    return JSON.parse(result)
}

async function isPasswordRegistered ()
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

async function GetAccounts ()
{
    var result = await ipcRenderer.sendSync("get-accounts", null)
    return JSON.parse(result)
}



// const GetAccounts = 
//     () => [
//         {
//             Name: "La banque postale",
//             id: 0,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Overleaf",
//             id: 1,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Uber drinks",
//             id: 2,
//             link: "http://test-address.com",
//             username: "Arturitio",
//             password: "29081999"
//         },
//         {
//             Name: "La banque postale",
//             id: 3,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Overleaf",
//             id: 4,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Uber Eats",
//             id: 5,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "La banque postale",
//             id: 6,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Overleaf",
//             id: 7,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//         {
//             Name: "Uber Eats",
//             id: 8,
//             link: "http://test-address.com",
//             username: "Arturitoo",
//             password: "29081999"
//         },
//     ]