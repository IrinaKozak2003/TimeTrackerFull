import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import { registration } from '../http/userApi';
import validator from 'validator';



function Registration() {
  const [show, setShow] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [username, setUsername] = useState('');
  const [isChecked, setIsChecked] = useState(false);

  const handleCheckboxChange = (e) => {
    setIsChecked(e.target.checked);
  };
  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const handleConfirmPasswordChange = (event) => {
    setConfirmPassword(event.target.value);
  };

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };
  


  const click = async () => {
    try {
      const isEmailValid = validator.isEmail(email);
      if (!isEmailValid) {
        alert('Please enter a valid email address.');
        return;
      }

     
        if (password !== confirmPassword) {
          alert('Password and confirm password do not match.');
          return;
        }
        const response = await registration(email, password, confirmPassword, username, isChecked);
        setShow(false)
        window.location.reload();
    } catch (e) {
        alert(e.response.data.message);
        }
    }
  return (
    <>
      <Button variant="primary" onClick={handleShow}>
       Add User
      </Button>

      <Modal show={show} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title></Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
              <Form.Label>Email address</Form.Label>
              <Form.Control
                type="email"
                placeholder="name@example.com"
                autoFocus
                onChange={handleEmailChange}
                value={email}
              />
            </Form.Group>
            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
              <Form.Label>Username</Form.Label>
              <Form.Control
                type="text"
                placeholder="example"
                autoFocus
                onChange={handleUsernameChange}
                value={username}
              />
            </Form.Group>
            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
              <Form.Label>Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="password"
                autoFocus
                onChange={handlePasswordChange}
                value={password}
              />
            </Form.Group>
            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
              <Form.Label>Confirm Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="password"
                autoFocus
                onChange={handleConfirmPasswordChange}
                value={confirmPassword}
              />
            </Form.Group>
            <Form>
      <Form.Check
        type="checkbox"
        label="Admin"
        checked={isChecked}
        onChange={handleCheckboxChange}
      />
    </Form>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
          <Button variant="primary" onClick={click}>
            Save Changes
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

export default Registration;