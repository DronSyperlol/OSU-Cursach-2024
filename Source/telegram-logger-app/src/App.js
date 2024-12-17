import './App.css';
import React from 'react';
import { StartPage } from './Pages/StartPage';
import { ErrorPage } from './Pages/ErrorPage';
import Api from './Tools/Api';

export default class App extends React.Component {
  
  static initCalled = false;
  static pingTimer = undefined;

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

  render = () => {
    return (
      <div className="App">
          {this.state.currentPageContent}
      </div>
    );
  }

  init = async () => {
    Api.Auth.logIn()
      .then(() => {
        this.drawPage(
          <div>
            <h1>Hello!</h1>
          </div>
        )
      })
      .catch((ex) => {
        this.drawPage(
          <ErrorPage description={ex.status + " | " + ex.statusText}/>
        );
      });
  }
}