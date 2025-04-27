import './App.css';
import React from 'react';
import _api from './Tools/Api';
import { LoadingPage } from './Pages/LoadingPage';
import { ErrorPage } from './Pages/ErrorPage';
import { NewAccountPage } from './Pages/NewAccountPage'
import { AccountsPage } from './Pages/AccountsPage';
import { DialogsPage } from './Pages/DialogsPage';

export default class App extends React.Component {
  static initCalled = false;
  static pingTimer = undefined;
  static apiAuthData = {
    sessionCode: "",
    userId: -1
  }
  static Api = _api;
  static drawPage = undefined;
 
  constructor(props) {
    super(props);
    let initData = window.Telegram.WebApp.initData
    if (initData === "") initData = process.env.REACT_APP_DEV_INITDATA;
    let decoded = URL.parse("http://example/?" + initData);
    let userName = JSON.parse(decoded.searchParams.get("user"))["first_name"];
    this.state = {
      currentPageContent: (<LoadingPage headText={`Приветствую ${userName}!`} loadText="Загружаю данные"/>)
    };
    App.drawPage = this.drawPage;
    App.drawLoadingPage = this.drawLoadingPage;
    this.stack = [];
    App.stackPush = (element) => {
      this.stack.push(element);
    }
    App.stackPop = () => {
      return this.stack.pop();
    }
  }
  
  drawLoadingPage = (headText, loadText) => {
    this.drawPage(<LoadingPage headText={headText} loadText={loadText}/>);
  }

  drawPage = (pageComponent) => {
    this.setState({
      currentPageContent: pageComponent
    });
  }

  componentDidMount = () => {
    if (App.initCalled === false) { // Костыль потому что componentDidMount вызывается 2 раза. (Надо исправить)
                                    // Конструктор кстати тоже вызывается 2 раза 
      this.init();
      App.initCalled = true;
    }
  }
  render = () => {
    return (
      <div className="App">
          {this.state.currentPageContent}
      </div>
    );
  }
  init = async () => {
    App.Api.Auth.logIn()
      .then((data) => {
        App.apiAuthData.sessionCode = data.sessionCode;
        App.apiAuthData.userId = data.me.id;
        App.Api.init(App.apiAuthData);
        App.doPing();
        if (data.accountCount === 0) {
          this.drawPage(<NewAccountPage app={App} onLogged={(phoneNumber) => {
            App.Api.Account.getDialogs(phoneNumber)
            .then((response) => {
                console.log(response);
                this.drawPage(<DialogsPage app={App} source={response.dialogs} phoneNumber={phoneNumber}/>);
            }).catch((ex) => {
                console.log(ex.message);
            });
          }}/>);
        }
        else {
          App.Api.Account.getMyAccounts(App.apiAuthData)
          .then((data) => {
            console.log(data);
            this.drawPage(<AccountsPage app={App} source={data.accounts}/>);
          }).catch((ex) => {
            this.drawPage(<ErrorPage description={ex.message}/>);
          });
        }
      })
      .catch((ex) => {
        this.drawPage(
          <ErrorPage description={ex.status + " | " + ex.statusText}/>
        );
      });
  }

  static doPing() {
    if (App.pingTimer !== undefined) 
      clearInterval(App.pingTimer);
    App.pingTimer = setInterval(() => {
      App.Api.Auth.ping()
      .catch((ex) => { console.log(ex); });
    },10_000);
  }
}