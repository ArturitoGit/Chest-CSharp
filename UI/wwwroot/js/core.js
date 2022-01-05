const { ipcRenderer } = require("electron") ;

async function openChest ( password )
{
    console.log("Try to open chest with password : " + password)

    return Promise.resolve(
        {
            success: password == "test"
        }
    )
}

async function deleteAccount ( account )
{
    const reply = ipcRenderer.sendSync("delete-account", "account-name") ;
    console.log("new function has been detected")
    console.log( reply ) ;
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
    console.log("change password from ")
    console.log(oldPassword)
    console.log("to")
    console.log(newPassword)

    return Promise.resolve(
        {
            success: true,
            error: "Test error message ..."
        }
    )
}

async function addAccount (account)
{
    console.log("try to add account : ")
    console.log(account)

    return Promise.resolve(
        {
            success: false,
            error_msg: "Invalid username ..."
        }
    )
}

async function generatePassword ( 
    size,
    upper, lower, symbols, numbers,
    mandatory_letters )
{
    console.log("generate password with parameters :")
    console.log(arguments)
    
    return Promise.resolve(
        {
            password: "Petit Henry"
        }
    )
}

async function setClipboard ( content )
{
    console.log(`clipboard content set to "${content}" !`)
}



const GetAccounts = 
    () => [
        {
            Name: "La banque postale",
            id: 0,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Overleaf",
            id: 1,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Uber drinks",
            id: 2,
            link: "http://test-address.com",
            username: "Arturitio",
            password: "29081999"
        },
        {
            Name: "La banque postale",
            id: 3,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Overleaf",
            id: 4,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Uber Eats",
            id: 5,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "La banque postale",
            id: 6,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Overleaf",
            id: 7,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
        {
            Name: "Uber Eats",
            id: 8,
            link: "http://test-address.com",
            username: "Arturitoo",
            password: "29081999"
        },
    ]