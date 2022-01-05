function onAccountsPageLoaded()
{
    const accounts_div = document.getElementById('accounts')
    const search_input = document.getElementById('search_input') ;
    const add_button = document.getElementById('add_button')
    const btn_edit_password = document.getElementById("btn_edit_password")

    add_button.onclick = () => DisplayNewAccountEditPage() 
    btn_edit_password.onclick = () => DisplayPasswordEditPage()

    search_input.addEventListener('input', _event =>
    {
        ResetAccountView()
        var filter_results = FilterAccounts(GetAccounts(),search_input.value) ;
        if (filter_results.length <= 0) 
            accounts_div.innerHTML = 
                `<span id="no_match_msg">
                    No account name contains "${search_input.value}" ...
                </span>`
        else
            FillAccounts(filter_results) ;
    })

    const FilterAccounts = (accounts, search) =>
    {
        var search_lc = search.toLowerCase() ;
        var result = [] ;
        accounts.forEach(ac => {
            var matches = (ac.Name.toLowerCase().includes(search_lc)) ;
            if (matches) result.push(ac)
        }) ;
        return result ;
    }

    const FillAccounts = ( accounts ) => 
            // For each account
            accounts.forEach( account => 
                // Append account view to the container
                accounts_div.appendChild(CreateAccountView(account)))


    const CreateAccountView = account =>
    {
        var element = document.createElement('p')
        element.innerHTML = 
            `${account.Name}
            <i class="bi bi-arrow-right"></i>`
        element.onclick = () => onAccountClicked(account)
        element.style.cursor = "pointer"
        return element
    }

    const ResetAccountView = () => accounts_div.innerHTML = "" ;

    function onAccountClicked ( account )
    {
        DisplayAccountPage( account ) ;
    }

    // Fill the accounts in the page
    FillAccounts(GetAccounts()) ;
}