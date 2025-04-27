import React from "react";
import AccountItem from '../Components/AccountItem/AccountItem'
import plusIcon from '../img/icons8-plus.svg'
import { NewAccountPage } from './NewAccountPage'
import { DialogsPage } from "./DialogsPage";

export class AccountsPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
        if (AccountsPage.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                                        // Конструктор кстати тоже вызывается 2 раза 
            this.props.app.stackPush(this.props);
            AccountsPage.initCalled = true;
        }
    }

    render = () => {
        return (
        <div className="account-list-grid">
            <ul className="account-list">
                {
                    this.props.source.map(x => <AccountItem item={x} onSelected={this.accountSelected}/>)
                }
                <li key="last" className="accountItem addNew" onClick={() => this.props.app.drawPage(<NewAccountPage app={this.props.app}/>)}>
                    <div id="addNewAccountIcon">
                        <img src={plusIcon} alt="add"/>
                    </div>
                    <div id="addNewAccountText">
                        <span>Добавить новый аккаунт</span>
                    </div>
                </li>
            </ul>
        </div>);
    }

    accountSelected = (phoneNumber) => {
        console.log(`accountSelected from ${phoneNumber}`);
        this.props.app.drawLoadingPage("","Загружаю диалоги, доступные для отслеживания");
        this.props.app.Api.Account.getDialogs(phoneNumber)
        .then((response) => {
            console.log(response);
            this.props.app.drawPage(<DialogsPage app={this.props.app} source={response.dialogs} phoneNumber={phoneNumber} loader={this}/>);
        }).catch((ex) => {
            console.log(ex.message);
        });
    }
}