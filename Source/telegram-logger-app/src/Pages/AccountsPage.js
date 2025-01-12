import React from "react";
import AccountItem from '../Components/AccountItem/AccountItem'
import plusIcon from '../img/icons8-plus.svg'

export class AccountsPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
    }

    render = () => {
        return (
        <ul className="account-list">
            {
                this.props.source.map(x => <AccountItem item={x} />)
            }
            <li className="accountItem addNew">
                <div className="addNewAccountIcon">
                    <img src={plusIcon}></img>
                </div>
                <div className="addNewAccountText">
                    <span>Добавить новый аккаунт</span>
                </div>
            </li>
        </ul>);
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