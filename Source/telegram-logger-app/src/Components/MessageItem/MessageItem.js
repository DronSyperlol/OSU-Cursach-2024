import react from "react"
import './MessageItem.css'

export default class MessageItem extends react.Component {
    constructor(props){
        super(props);
        this.fromId = (this.props.item.prevChanges.length > 0 ? this.props.item.prevChanges.filter(x => x.prevId === undefined || x.prevId === null)[0] : this.props.item).fromId;
    }

    render = () => {
        switch (this.props.item.type) {
            case "Specifications":
                return (<this.spec />);
            case "Message":
                return (<this.message />);
            case "DeletedMessage":
                return (<this.delmsg />);
            default: (<span>unknown type</span>)
        }
    }


    spec = () => {
        return (
            <li key={this.props.item.messageId} className="messageListItem special" 
            title={new Date(this.props.item.logTime)}
            ref={this.props.onRef}>
                <span>{this.props.item.message}</span>
            </li>
        );
    }
    
    message = () => {
        return (
            <li key={this.props.item.messageId} className={`messageListItem 
                ${this.fromId === -1 ? "senderIsMe" : "senderIsOther"}`} 
                title={new Date(this.props.item.logTime)}
                ref={this.props.onRef}>
                <div className="messageHeader">
                    <span><i>{this.fromId}</i>:</span>
                </div>
                <div className="messageData">
                    <span>{this.props.item.message}</span>
                </div>
                <div className="messageTime">
                    <span>{this.convertTime(this.props.item.messageDate)}</span>
                </div>
            </li>
        )
    }

    delmsg = () => {
        <li key={this.props.item.messageId} className={`messageListItem
            ${this.fromId === -1 ? "senderIsMe" : "senderIsOther"} messageDeleted`}
            title={new Date(this.props.item.logTime)}
            ref={this.props.onRef}>
            <div className="messageHeader">
                <span>{this.fromId}</span>
            </div>
            <div className="messageData">
                <span>{this.props.item.message}</span>
            </div>
        </li>
    }

    convertTime = (unixTimeWithMilliseconds) => {
        let date = new Date(unixTimeWithMilliseconds);
        return `${date.getDate()}.${date.getMonth() + 1}.${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`;
    }
}