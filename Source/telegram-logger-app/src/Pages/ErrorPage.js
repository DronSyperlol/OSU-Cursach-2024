import React from "react";
import svgSource from '../img/error-svgrepo-com.svg'

export class ErrorPage extends React.Component {
  render = () => {
    return (
      <div>
        <img height="50%" width="50%" src={svgSource} alt=""/>
        <h1>Ошибка в работе сайта!</h1>
        <span>Описание ошибки: {this.props.description}</span>
      </div>);
  }
}