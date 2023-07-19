import { BrowserRouter } from "react-router-dom";
import Menu from "./components/Menu";
import AppRouter from "./components/AppRouter";
import { observer } from "mobx-react-lite";
import { check } from "./http/userApi";
import { useContext } from "react";
import { Context } from "./index";

const App = observer(() => {
  const { user } = useContext(Context);
  check().then(data => {
    user.setUser(data);
    user.setIsAuth(true);
    if (data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'Admin') {
      user.setIsAdmin(true);
    }
  })
 
  return (
    <BrowserRouter>
     <Menu/>
      <AppRouter/>
    </BrowserRouter>
  );
})

export default App;
