import react from "react"
import './AccountItem.css'

export default class AccountItem extends react.Component {
    // constructor(props){
    //     super(props);
    // }

    render = () => {
        const avatarSize = 100;
        return (
            <li className="accountItem" onClick={() => {this.selected(this.props.item.phoneNumber)}}>
                <div className="accountImage" >
                    <img height={avatarSize} width={avatarSize} src={this.props.item.photoUrl} alt="avatar"/>
                </div>
                <div className="accountInfo">
                    <span className="accountTitle">{this.props.item.title}</span> <br/>
                    <span className="accountPhone">+{this.props.item.phoneNumber}</span> <br/>
                    <span className="accountUsername">@{this.props.item.username}</span>
                </div>
            </li>
        );
    }

    selected = (phone) => {
        console.log("clicked from "+ phone);
    }
}