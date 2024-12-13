import './App.css';
import { Loading } from './Animations/get-animation';

function App() {
  return (
    <div className="App">
        <Loading/>
        <p>Загружаю данные...</p>
    </div>
  );
}
export default App;
