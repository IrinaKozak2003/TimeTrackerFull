import React, {createContext} from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import UserStore from './store/userStore';
import PackageStore from './store/packageStore';




export const Context = createContext(null)

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
    <Context.Provider value={{
         user: new UserStore(),
         user_package: new PackageStore()
    }}>
      <App />
    </Context.Provider>,
);