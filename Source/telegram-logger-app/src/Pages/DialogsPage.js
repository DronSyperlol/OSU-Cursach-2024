import React from "react";
import DialogItem from "../Components/DialogItem/DialogItem";

export class DialogsPage extends React.Component {
    static initCalled = false;

    render = () => {
        return (
        <div className="dialog-list-grid">
            <h1>Чаты для отслеживания</h1>
            <ul className="dialog-list">
                {
                    this.props.source.map(x => <DialogItem item={x} onSelected={this.dialogSelected} onChecked={this.dialogChecked}/>)
                }
            </ul>
        </div>);
    }

    dialogSelected = (sender) => {
        this.props.app.Api.Target.getSavedLogs(this.props.phoneNumber, sender.peerId)
        .then((response) => {
            console.log(response);
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
}