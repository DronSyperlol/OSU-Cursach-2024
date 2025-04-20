import Auth from './Api/Auth'
import Account from './Api/Account'
import Target from './Api/Target'

function init(apiAuthData) {
    Auth.init(apiAuthData);
    Account.init(apiAuthData);
    Target.init(apiAuthData);
};

const toExport = 
{ 
    init,
    Auth, 
    Account, 
    Target,
};

export default toExport