import React from "react";
import AccountItem from '../Components/AccountItem/AccountItem'
import plusIcon from '../img/icons8-plus.svg'
import { NewAccountPage } from './NewAccountPage'

export class AccountsPage extends React.Component {
    static initCalled = false;

    // constructor(props) {
    //     super(props);
    // }

    render = () => {
        return (
        <ul className="account-list">
            {
                this.props.source.map(x => <AccountItem item={x} />)
            }
            <li className="accountItem addNew" onClick={() => this.props.app.drawPage(<NewAccountPage app={this.props.app}/>)}>
                <div className="addNewAccountIcon">
                    <img src={plusIcon} alt="add"></img>
                </div>
                <div className="addNewAccountText">
                    <span>Добавить новый аккаунт</span>
                </div>
            </li>
        </ul>);
    }
}