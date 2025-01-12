import react from "react"
import './DialogItem.css'

export default class DialogItem extends react.Component {
    // constructor(props){
    //     super(props);
    // }

    render = () => {
        const avatarSize = 70;
        return (
            <li className="dialogItem" onClick={() => {this.props.onSelected(this.props.item.peerId)}}>
                <div className="dialogImage" >
                    <img height={avatarSize} width={avatarSize} src={this.props.item.photoUrl} alt="avatar"/>
                </div>
                <div className="dialogInfo">
                    <span className="dialogTitle">{this.props.item.title}</span> <br/>
                    <span className="dialogTopMessage">{this.props.item.topMessage}</span>
                </div>
            </li>
        );
    }
}