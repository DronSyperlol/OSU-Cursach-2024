import './App.css';
import React from 'react';
import _api from './Tools/Api';
import { StartPage } from './Pages/StartPage';
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
    this.state = {
      currentPageContent: (<StartPage />)
    };
    App.drawPage = this.drawPage;
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
  // componentDidUpdate = (prevProps, prevState, snapShot) => {
  // }
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
                App.drawPage(<DialogsPage app={App} source={response.dialogs} phoneNumber={phoneNumber}/>);
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