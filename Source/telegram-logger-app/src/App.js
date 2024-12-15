import './App.css';
import { Loading } from './Components/Loading/Loading';
import { http_post } from './Tools/httpRequest';
import { useEffect , useState } from 'react';

function App() {
  const [mainPageContent, setMainPageContent] = useState((
    <div className="Loading">
      <Loading height={250} width={250}/>
      <p>Загружаю данные...</p>
    </div>));
  
  useEffect(() => {
    http_post(process.env.REACT_APP_BACKEND_URL+"auth/logIn", {initData: "sdf"})
    .then(() => {
      setMainPageContent(
        <div className = "LoadedContent">
          <h1>HELLO!!!</h1>
        </div>
      );
    })
    .catch(() => {
      setMainPageContent(
        <div className="ErrorView">
          <h1>ERROR!!!</h1>
        </div>
      );
    });
    
  }, []);

  return (
    <div className="App">
        {mainPageContent}
    </div>
  );
}



export default App;