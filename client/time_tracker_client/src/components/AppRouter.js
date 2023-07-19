import React from 'react';
import { Routes, Route} from 'react-router-dom'
import { observer } from 'mobx-react'; 
import { publicRoutes } from '../routes';
import { authRoutes } from '../routes';
import { useContext } from 'react';
import { Context } from '../index';



const AppRouter = observer(() => {
const {user} = useContext(Context);
console.log(user.isAuth);
    return (
        <Routes>
          
            {publicRoutes.map(({path, element}) =>
                <Route key={path} path={path} Component={element} exact/>
            )} 
            
            {user.isAuth && authRoutes.map(({path, element}) =>
                <Route key={path} path={path} Component={element} exact/>
            )}
            
          

       
        </Routes>
        
    );
});

export default AppRouter;