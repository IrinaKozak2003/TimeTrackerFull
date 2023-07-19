import React, { useState } from "react";
import { useLocation } from "react-router-dom";
import { useContext } from "react";
import { Context } from "../index";
import { useNavigate } from "react-router-dom";
import validator from 'validator';

import {
  MDBBtn,
  MDBContainer,
  MDBRow,
  MDBCol,
  MDBCard,
  MDBCardBody,
  MDBInput,
} from 'mdb-react-ui-kit';
import {  login } from "../http/userApi";
import { observer } from "mobx-react-lite";
import { MAIN_ROUTER } from "../utils/consts";


const Auth = observer(() => {
  const {user} = useContext(Context)
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigate = useNavigate();

  const click = async () => {
    try {
      const isEmailValid = validator.isEmail(email);
      if (!isEmailValid) {
        alert('Please enter a valid email address.');
        return;
      }

     
    
        const response = await login(email, password);
       
        user.setUser(response);
        navigate(MAIN_ROUTER);


        if (response['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'Admin') {
          user.setIsAdmin(true);
        }
        
        user.setIsAuth(true);
        
      
    } catch (e) {
      alert(e.response.data.message);
    }
  };

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

 

  return (
    <MDBContainer fluid>
      <MDBRow className='d-flex justify-content-center align-items-center h-100'>
        <MDBCol col='12'>
          <MDBCard className='bg-dark text-white my-5 mx-auto' style={{ borderRadius: '1rem', maxWidth: '400px' }}>
            <MDBCardBody className='p-5 d-flex flex-column align-items-center mx-auto w-100'>
              <h2 className="fw-bold mb-2 text-uppercase"> Login </h2>
              <p className="text-white-50 mb-5"> Please enter your login and password!</p>

              <MDBInput wrapperClass='mb-4 mx-5 w-100' labelClass='text-white' label='Email' id='formControlLg' type='email' size="lg" onChange={handleEmailChange} />
              <MDBInput wrapperClass='mb-4 mx-5 w-100' labelClass='text-white' label='Password' id='formControlLg' type='password' size="lg" onChange={handlePasswordChange} />
             

              <div style={{ marginTop: '-20px' }}>
                <MDBBtn
                  outline
                  className='mx-2 px-'
                  color='white'
                  size='lg'
                  disabled={!email || !password }
                  onClick={click}
                >
                  Login 
                </MDBBtn>
              </div>
            </MDBCardBody>
          </MDBCard>
        </MDBCol>
      </MDBRow>
    </MDBContainer>
  );
});
export default Auth;
