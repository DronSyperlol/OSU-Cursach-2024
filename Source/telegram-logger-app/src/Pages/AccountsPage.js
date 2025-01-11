import React from "react";
import AccountItem from '../Components/AccountItem/AccountItem'

export class AccountsPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
    }

    render = () => {
        return (
        <div>
            {
                this.props.source.map(x => <AccountItem item={x} />)
            }
        </div>);
    }

    componentDidMount = () => {
        if (AccountsPage.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
            // Конструктор кстати тоже вызывается 2 раза 
            this.init();
            console.log("account page init called");
            AccountsPage.initCalled = true;
        }
    }
    
    init = () => {

    }
}