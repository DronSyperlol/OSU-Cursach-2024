import Auth from './Api/Auth'
import Account from './Api/Account'

function init(apiAuthData) {
    Auth.init(apiAuthData);
    Account.init(apiAuthData);
};

const toExport = 
{ 
    init,
    Auth, 
    Account, 
};

export default toExport