import React from "react";
import MessageItem from "../Components/MessageItem/MessageItem";
import BackButton from "../Components/BackButton/BackButton";
import { DialogsPage } from "./DialogsPage";
import { ChangeHistoryPage } from "./ChangeHistoryPage";

export class MessagesPage extends React.Component {
    static initCalled = false;

    constructor(props) {
        super(props);
        this.listEnd = React.createRef();
        this.listEndSet = false
        if (MessagesPage.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                                 // Конструктор кстати тоже вызывается 2 раза 
            this.props.app.stackPush(this.props);
            MessagesPage.initCalled = true;
        }
    }

    setRef = (el) => {
        if (this.listEndSet === false)
        {
            this.listEnd = el;
            this.listEndSet = true;
        }
    }

    componentDidMount = () => {
        if (this.listEnd && this.listEnd.scrollIntoView) {
            this.listEnd.scrollIntoView({ behavior: "smooth" });
        }
    }

    render = () => {
        return (
        <div className="messages-list-grid">
            <div className="messages-list-title-box">
                <BackButton height={35} width={20} onClick={this.back}/>
                <h1 className="messages-list-title">Список сообщений:</h1>
            </div>
            {this.getList(this.props.source)}
        </div>);
    }

    getList = (source) => {
        if (source === undefined || source === null || source.length === undefined || source.length === 0) {
            return(
                <div>
                    Нет сохранённых сообщений
                </div>
            );
        }
        return(
            <ul className="messages-list">
                {
                    this.props.source.map(x => <MessageItem item={x} onSelected={this.messageSelected} onRef={this.setRef}/>)
                }
            </ul>
        );
    }

    messageSelected = (sender) => {
        let toOpen = this.props.source.filter(x => x.messageId === sender.messageId)[0];
        let convertObject = (toConvert) => {
            return {
                fromId: toConvert.fromId,
                message: toConvert.message,
                messageId: toConvert.messageId,
                messageDate: toConvert.messageDate,
                logTime: toConvert.logTime,
                type: toConvert.type,
            }
        }
        let source = [convertObject(toOpen)].concat(toOpen.prevChanges.map(x => convertObject(x)));
        this.props.app.drawPage(<ChangeHistoryPage app={this.props.app} source={source}/>);
        console.log(`messageSelected from ${sender.messageId}`);
    }


    back = () => {
        this.props.app.stackPop();
        MessagesPage.initCalled = false;
        let olderProps = this.props.app.stackPop();
        this.props.app.stackPush(olderProps);
        this.props.app.drawPage(<DialogsPage app={this.props.app} source={olderProps.source} phoneNumber={olderProps.phoneNumber}/>);
    }
}


