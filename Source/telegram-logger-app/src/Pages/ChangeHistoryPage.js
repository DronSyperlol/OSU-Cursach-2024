import React from "react";
import BackButton from "../Components/BackButton/BackButton";
import { MessagesPage } from "./MessagesPage";
import HistoryLog from "../Components/HistoryLog/HistoryLog";

export class ChangeHistoryPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
        this.listEnd = React.createRef();
        this.listEndSet = false
        if (ChangeHistoryPage.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                                 // Конструктор кстати тоже вызывается 2 раза 
            this.props.app.stackPush(this.props);
            ChangeHistoryPage.initCalled = true;
        }
    }

    render = () => {
        return (
        <div className="history-list-grid">
            <div className="history-list-title-box">
                <BackButton height={35} width={20} onClick={this.back}/>
                <h1 className="history-list-title">Изменение сообщения:</h1>
            </div>
            <ul className="history-list">
                    {
                        this.props.source.map(x => <HistoryLog item={x} />)
                    }
                </ul>
        </div>);
    }

    back = () => {
        this.props.app.stackPop();
        ChangeHistoryPage.initCalled = false;
        let olderProps = this.props.app.stackPop();
        this.props.app.stackPush(olderProps);
        this.props.app.drawPage(<MessagesPage app={this.props.app} source={olderProps.source}/>);
    }
}


