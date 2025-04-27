import React from "react";
import Loading from "../Components/Loading/Loading";

const animationSpeed = 700;

export class LoadingPage extends React.Component {
  constructor(props) {
    super(props);
    this.state =
    {
      animationDots: "."
    }
  }
  
  componentDidMount = () => {
    this.setState({
      animationDots: "."
    });
  }

  componentDidUpdate = () => {
    setTimeout(() => {
      if (this.state.animationDots.length === 3)
        this.setState({
          animationDots: ""
        });
      else
        this.setState({
          animationDots: this.state.animationDots + "."
        });
    }, animationSpeed);
  }

  render = () => {
    return (
      <div>
        <h3>{this.props.headText}</h3>
        <Loading height={250} width={250} />
        <p>{this.props.loadText}{this.state.animationDots}</p>
      </div>);
  }
}