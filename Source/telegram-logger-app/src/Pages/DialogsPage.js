import React from "react";
import DialogItem from "../Components/DialogItem/DialogItem";
import { MessagesPage } from "../Pages/MessagesPage";
import BackButton from "../Components/BackButton/BackButton";
import { AccountsPage } from "./AccountsPage";

export class DialogsPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
        if (DialogsPage.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                                // Конструктор кстати тоже вызывается 2 раза 
            this.props.app.stackPush(this.props);
            DialogsPage.initCalled = true;
        }
    }

    render = () => {
        return (
        <div className="dialog-list-grid">
            <div className="dialog-list-title-box">
                <BackButton height={35} width={20} onClick={this.back}/>
                <h1 className="dialog-list-title">Чаты для отслеживания</h1>
            </div>
            <ul className="dialog-list">
                {
                    this.props.source.map(x => <DialogItem item={x} onSelected={this.dialogSelected} onChecked={this.dialogChecked}/>)
                }
            </ul>
        </div>);
    }

    dialogSelected = (sender) => {
        this.props.app.drawLoadingPage("", "Достаю сообщения из архива");
        this.props.app.Api.Target.getSavedLogs(this.props.phoneNumber, sender.peerId)
        .then((response) => {
            console.log(response);
            this.props.app.drawPage(<MessagesPage app={this.props.app} source={response.logs}/>);
        })
        .catch((ex) => {
            console.log(ex.message)
        });
        console.log(`dialogSelected from ${sender.peerId}`);
    }

    dialogChecked = (cbState, peerId, accessHash) => {
        this.props.app.Api.Target.updateTarget(this.props.phoneNumber, peerId, accessHash, cbState)
        .then((response) => {
            console.log(response);
        })
        .catch((ex) => {
            console.log(ex.message)
        });
        console.log(`dialogChecked from ${peerId} with cbState: ${cbState}`);
    }

    back = () => {
        this.props.app.stackPop();
        DialogsPage.initCalled = false;
        let olderProps = this.props.app.stackPop();
        this.props.app.stackPush(olderProps);
        this.props.app.drawPage(<AccountsPage app={this.props.app} source={olderProps.source}/>);
    }
}