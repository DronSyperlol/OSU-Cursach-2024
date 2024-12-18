import React from "react";

export class NewAccountPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      currentForm: this.inputPhone(),
      inputValue: "",
    };
  }
  
  render = () => {
    return (
      <div>
        <h2>Добавление нового аккаунта</h2>
        {this.state.currentForm}
      </div>);
  }

  inputPhone = () => {
    return (
      <form id="inputPhone">
        <input type="number" placeholder="Номер телефона" onChange={(input) => this.setState({inputValue: input.target.value})}/>
        <button type="button" onClick={() => {
          // TODO... отправлять запрос account/newAccount
        }}>Далее</button>
      </form>
    );
  }
}