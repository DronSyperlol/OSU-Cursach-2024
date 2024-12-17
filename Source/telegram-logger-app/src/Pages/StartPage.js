import React from "react";
import { Loading } from "../Components/Loading/Loading";

const animationSpeed = 700;

export class StartPage extends React.Component {
  constructor(props) {
    super(props);
    let initData = window.Telegram.WebApp.initData
    if (initData === "") initData = process.env.REACT_APP_DEV_INITDATA;
    let decoded = URL.parse("http://example/?" + initData);
    this.userName = JSON.parse(decoded.searchParams.get("user"))["first_name"];
  }
  
  state =
  {
    animationDots: "."
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
        <h3>Приветствую {this.userName}!</h3>
        <Loading height={250} width={250} />
        <p>Загружаю данные{this.state.animationDots}</p>
      </div>);
  }
}