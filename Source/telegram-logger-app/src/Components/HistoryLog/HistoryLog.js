import react from "react"
import './HistoryLog.css'

export default class HistoryLog extends react.Component {
    // constructor(props){
    //     super(props);
    // }

    render = () => {
        return (<li className="HistoryLog">
            <span className="LogTime">Время лога: {this.convertTime(this.props.item.logTime)}</span>
            {
                this.props.item.type === "MessageDeleted" ? (<this.deleted />) : (<this.edited />)
            }
        </li>)
    }

    edited = () => {
        return (<div className="MessageLog">
            <span className="LogOwner"><i>{this.props.item.fromId}</i>:</span>
            <span className="LogData">{this.props.item.message}</span>
            <span className="MessageDate">{this.convertTime(this.props.item.messageDate)}</span>
        </div>)
    }

    deleted = () => {
        return (<div className="DeleteLog">
            <span>Сообщение удалено</span>
        </div>)
    }
    
    convertTime = (unixTimeWithMilliseconds) => {
        let date = new Date(unixTimeWithMilliseconds);
        return `${date.getDate()}.${date.getMonth() + 1}.${date.getFullYear()} ${date.getHours()}:${date.getMinutes()}`;
    }
}