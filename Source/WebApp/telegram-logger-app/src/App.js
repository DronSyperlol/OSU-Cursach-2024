import './App.css';

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <p>
          Edit <code>src/App.js</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
        <List items={JSON.stringify(["item1","item2","sss","item 95"])}/>
      </header>
    </div>
  );
}

function List(props) {
  var source = JSON.parse(props.items);
  let counter = 0;
  return (
    <div className='ListDiv'>
      {source.map((item) => <ListItem index={counter++} name={item}/>)}
    </div>
  );
}

function ListItem(props) {
  return (
    <div>
      <b Id={props.index}>{props.name}</b>
      <br></br>
    </div>
  );
}

export default App;
