import React from "react";
import DialogItem from "../Components/DialogItem/DialogItem";

export class DialogsPage extends React.Component {
    static initCalled = false;

    // constructor(props) {
    //     super(props);
    // }

    render = () => {
        return (
        <div className="dialog-list-grid">
            <ul className="dialog-list">
                {
                    this.props.source.map(x => <DialogItem item={x} onSelected={this.dialogSelected}/>)
                }
            </ul>
        </div>);
    }

    dialogSelected = (peerId) => {
        console.log(`dialogSelected from ${peerId}`);
    }
}