import React from "react";
import Loading from '../Components/Loading/Loading'

const loadingSize = 150;

export class NewAccountPage extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      currentForm: this.inputPhone(),
      phone: "",
      anonymValue: "",
    };
    this.statuses = {
      logged: "Logged in",
      code: "verification_code",
      password: "password",
    }
  }
 
  render = () => {
    return (
      <div>
        <h2>Добавление нового аккаунта</h2>
        {this.state.currentForm}
      </div>);
  }

  inputPhone = () => {
    let handleSend = () => {
      document.getElementById("inputPhone").reset();
      this.setState({currentForm: <Loading height={loadingSize} width={loadingSize} />});
      this.props.app.Api.Account.newAccount(this.state.phone)
      .then((response) => {
        console.log(response);
        if (response.status === this.statuses.code) {
          this.setState({currentForm: this.inputCode()});
        }
        else if (response.status === this.statuses.logged) {
          this.props.onLogged(response.phone);
        }
      })
      .catch(err => {
        console.log(err);
      });
    };
    return (
      <form id="inputPhone">
        <input type="number" placeholder="Номер телефона" 
          onChange={(input) => this.setState({phone: input.target.value})}
          onKeyDown={e => e.key === 'Enter' ? handleSend() : ''}/>
        <button type="button" onClick={handleSend}>Отправить номер</button>
      </form>
    );
  }

  inputCode = () => {
    let handleSend = () => {
      document.getElementById("inputCode").reset();
      this.setState({currentForm: <Loading height={loadingSize} width={loadingSize}/>});
      this.props.app.Api.Account.setCode(this.state.phone, this.state.anonymValue)
      .then((response) => {
        console.log(response);
        if (response.status === this.statuses.password) {
          this.setState({currentForm: this.inputPassword()});
        }
        else if (response.status === this.statuses.logged) {
          this.props.onLogged(response.phone);
        }
      }).catch(err => {
        console.log(err);
      });
    };
    return (
      <form id="inputCode">
        <input type="number" placeholder="Код верификации" 
          onChange={(input) => this.setState({anonymValue: input.target.value})}
          onKeyDown={e => e.key === 'Enter' ? handleSend() : ''} />
        <button type='button' onClick={handleSend}>Отправить код</button>
      </form>
    );
  }
  inputPassword = () => {
    let handleSend = () => {
      document.getElementById("inputPassword").reset();
      this.setState({currentForm: <Loading height={loadingSize} width={loadingSize}/>});
      this.props.app.Api.Account.setPassword(this.state.phone, this.state.anonymValue)
      .then((response) => {
        console.log(response);
        if (response.status === this.statuses.logged) {
          this.props.onLogged(response.phone);
        }
      }).catch(err => {
        console.log(err);
      });
    };
    return (
      <form id="inputPassword">
        <input type="password" placeholder="Облачный пароль" 
          onChange={(input) => this.setState({anonymValue: input.target.value})}
          onKeyDown={e => e.key === 'Enter' ? handleSend() : ''} />
        <button type='button' onClick={handleSend}>Отправить пароль</button>
      </form>
    );
  }
}