import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { getUser } from './util/getUser';
import { UserClient } from './util/clients/userClient';

function App() {
  const [user, setUser] = useState("");
  const [count, setCount] = useState(0)

  const login = () => {
    const userClient = new UserClient();
    window.location.replace(userClient.getLoginUrl());
  }

  const logout = async () => {
    const userClient = new UserClient();
    await userClient.logout();
  }

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
      <button onClick={() => login()}>Login</button>
      <br />
      <br />
      <button onClick={() => logout()}>Logout</button>
      <br />
      <br />
      <button onClick={() => getUser().then(u => setUser(JSON.stringify(u)))}>Get</button>
      <br />
      <p>{user}</p>
    </>
  )
}

export default App
