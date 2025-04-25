import './MessageItem.css'

export default function Loading({height, width}) {
    const defaultOptions = {
        loop: true,
        autoplay: true,
        animationData: animationData,
        rendererSettings: {
          preserveAspectRatio: "xMidYMid slice"
        }
      };
    
    return (
      <li key={this.props.item.peerId} className="dialogListItem">
      <div className="dialogWatch" title="Отслеживать этот чат">
          <input type="checkbox" 
              defaultChecked={this.props.item.isTarget} 
              onChange={(cb) => this.props.onChecked(cb.currentTarget.checked, this.props.item.peerId, this.props.item.accessHash)}/>
      </div>
      <div className="dialogItem" onClick={() => {this.props.onSelected(this.props.item)}}>
          <img className="dialogImage" height={avatarSize} width={avatarSize} src={this.props.item.photoUrl} alt="avatar"/>
          <div className="dialogInfo">
              <span className="dialogTitle">{this.props.item.title}</span> <br/>
              <span className="dialogTopMessage">{this.props.item.topMessage}</span>
          </div>
      </div>
  </li>
    );
}