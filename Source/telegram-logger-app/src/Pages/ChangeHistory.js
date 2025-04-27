import React from "react";
import MessageItem from "../Components/MessageItem/MessageItem";
import BackButton from "../Components/BackButton/BackButton";
import { DialogsPage } from "./DialogsPage";
import { MessagesPage } from "./MessagesPage";

export class ChangeHistory extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
        this.listEnd = React.createRef();
        this.listEndSet = false
        if (ChangeHistory.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                                 // Конструктор кстати тоже вызывается 2 раза 
            this.props.app.stackPush(this.props);
            ChangeHistory.initCalled = true;
        }
    }


    render = () => {
        return (
        <div className="history-list-grid">
            <div className="history-list-title-box">
                <BackButton height={35} width={20} onClick={this.back}/>
                <h1 className="history-list-title">Список сообщений:</h1>
            </div>
        </div>);
    }

    back = () => {
        this.props.app.stackPop();
        ChangeHistory.initCalled = false;
        let olderProps = this.props.app.stackPop();
        this.props.app.stackPush(olderProps);
        this.props.app.drawPage(<MessagesPage app={this.props.app} source={olderProps.source}/>);
    }
}


