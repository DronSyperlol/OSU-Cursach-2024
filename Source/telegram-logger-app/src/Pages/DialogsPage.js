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

    dialogSelected = (peerId) => {
        console.log(`dialogSelected from ${peerId}`);
    }

    dialogChecked = (cbState, peerId) => {
        console.log(`dialogChecked from ${peerId} with cbState: ${cbState}`);
    }
}