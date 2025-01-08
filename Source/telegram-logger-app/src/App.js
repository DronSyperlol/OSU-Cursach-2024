import './App.css';
import React from 'react';
import { StartPage } from './Pages/StartPage';
import { ErrorPage } from './Pages/ErrorPage';
import { NewAccountPage } from './Pages/NewAccountPage'
import Api from './Tools/Api';

export default class App extends React.Component {
  
  static initCalled = false;
  static pingTimer = undefined;

  static apiAuthData = {
    sessionCode: "",
    userId: -1
  }

  constructor(props) {
    super(props);
    this.state = {
      currentPageContent: (<StartPage />)
    };
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

  componentDidUpdate = (prevProps, prevState, snapShot) => {
  }

  render = () => {
    return (
      <div className="App">
          {this.state.currentPageContent}
      </div>
    );
  }

  init = async () => {
    Api.Auth.logIn()
      .then((data) => {
        App.apiAuthData.sessionCode = data.sessionCode;
        App.apiAuthData.userId = data.me.id;
        App.doPing();
        if (data.accountCount === 0) {
          this.drawPage(<NewAccountPage api={Api} auth={App.apiAuthData}/>);
        }
        else {
          this.drawPage(
            <div>
              <h1>TODO...</h1>
            </div>
          )
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
      Api.Auth.ping(App.apiAuthData)
      .catch((ex) => { console.log(ex); });
    },10_000);
  }
}