import { MAIN_ROUTER, LOGIN_ROUTER, REGISTRATION_ROUTER, PACKAGE_ROUTER} from "./utils/consts";
import Main from "./pages/main";
import Auth from "./pages/Auth"
import Package from "./pages/Package"


export const authRoutes =[

    {
        path: MAIN_ROUTER,
        element: Main
    },
    {
        path: PACKAGE_ROUTER+'/'+':id',
        element: Package
    }


]
export const publicRoutes=[
    {
        path: LOGIN_ROUTER,
        element: Auth
    },
  

    

]

