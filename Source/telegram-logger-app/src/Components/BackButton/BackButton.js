import react from "react"
import './BackButton.css'
import imgSrc from '../../img/icons8-back-arrow.svg'

export default class BackButton extends react.Component {
    render = () => {
        return (<button 
            className="backButton"
            onClick={this.props.onClick}>
            <img 
                src={imgSrc}
                height = {this.props.height}
                width = {this.props.width}
                alt="back"
            />
        </button>)
    }
}
