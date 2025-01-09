import React from "react";

export class NewAccountPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      currentForm: this.inputPhone(),
      phone: "",
      anonymValue: "",
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
        <input type="number" placeholder="Номер телефона" onChange={(input) => this.setState({phone: input.target.value})}/>
        <button type="button" onClick={() => {
          this.props.api.Account.newAccount(this.props.auth, this.state.phone)
          .then((response) => {
            console.log(response);
            if (response.status == "verification_code") {
              document.getElementById("inputPhone").reset();
              this.setState({currentForm: this.inputCode()});
            }
          })
          .catch(err => {
            console.log(err);
          });
        }}>Отправить номер</button>
      </form>
    );
  }

  inputCode = () => {
    return (
      <form id="inputCode">
        <input type="number" placeholder="Код верификации" onChange={(input) => this.setState({anonymValue: input.target.value})}/>
        <button type='button' onClick={() => {
          this.props.api.Account.setCode(this.props.auth, this.state.phone, this.state.anonymValue)
          .then((response) => {
            console.log(response);
            if (response.status == "password") {
              document.getElementById("inputCode").reset();
              this.setState({currentForm: this.inputPassword()});
            }
          }).catch(err => {
            console.log(err);
          });
        }}>Отправить код</button>
      </form>
    );
  }
  inputPassword = () => {
    return (
      <form id="inputPassword">
        <input type="password" placeholder="Облачный пароль" onChange={(input) => this.setState({anonymValue: input.target.value})}/>
        <button type='button' onClick={() => {
          this.props.api.Account.setPassword(this.props.auth, this.state.phone, this.state.anonymValue)
          .then((response) => {
            document.getElementById("inputPassword").reset();
            console.log(response);
          }).catch(err => {
            console.log(err);
          });

        }}>Отправить пароль</button>
      </form>
    );
  }
}