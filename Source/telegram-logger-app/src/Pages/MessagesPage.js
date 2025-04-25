import React from "react";
import MessageItem from "../Components/MessageItem/MessageItem";

export class DialogsPage extends React.Component {
    static initCalled = false;

    render = () => {
        return (
        <div className="messages-list-grid">
            <h1>Список сообщений:</h1>
            <ul className="messages-list">
                {
                    this.props.source.map(x => <MessageItem item={x} onSelected={this.messageSelected}/>)
                }
            </ul>
        </div>);
    }

    messageSelected = (sender) => {
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
}