import React, { useContext, useEffect } from 'react';
import { NavDropdown, Modal, Button, Form, Navbar, Container } from 'react-bootstrap';
import { observer } from 'mobx-react-lite';
import { Context } from '../index';
import Registration from './Registration';
import { deleteUser, fetchUsersAll, logout, updateUser } from '../http/userApi';
import { useNavigate } from 'react-router-dom';

const Menu = observer(() => {
  const navigate = useNavigate();
const [showModal, setShowModal] = React.useState(false);
const [selectedUser, setSelectedUser] = React.useState(null);
const [users, setUsers] = React.useState([]);
const { user } = useContext(Context);

useEffect(() => {
  if(user.IsAuth)
fetchUsersAll().then((data) => {
setUsers(data);
});
}, [user.IsAuth]);


const handleModalShow = (user) => {
setSelectedUser(user);

setShowModal(true);
};

const handleModalClose = () => {
setSelectedUser(null);
setShowModal(false);
};

const handleUserUpdate = async () => {
// Выполните необходимую логику обновления информации пользователя
updateUser(selectedUser);

handleModalClose(); // Закройте модальное окно после обновления
};
const deleteUserClick = async (userId) => {
  await deleteUser(userId);
}
const handleLogout = () => {
user.setIsAuth(false);
navigate('/login');
logout();



};

return (
<Navbar className="bg-body-tertiary">
<Container>
<Navbar.Brand href="/main">TimeTracker</Navbar.Brand>
<Navbar.Toggle />
{user.isAdmin ? <Registration /> : null}
<NavDropdown title={user.user.userName} id="basic-nav-dropdown" style={{ color: 'white' }}>
<NavDropdown.Item onClick={handleLogout}>Выход</NavDropdown.Item>
</NavDropdown>
{user.isAdmin ? 
<NavDropdown title="Пользователи" id="basic-nav-dropdown" style={{ color: 'white' }}>
{users.map((user) => (
<NavDropdown.Item key={user.userId} onClick={() => handleModalShow(user)}>
{user.userName}

</NavDropdown.Item>
))}
</NavDropdown>
: null}
</Container>

  {/* Модальное окно для редактирования информации пользователя */}
  <Modal show={showModal} onHide={handleModalClose}>
    <Modal.Header closeButton>
      <Modal.Title>Редактирование информации пользователя</Modal.Title>
    </Modal.Header>
    <Modal.Body>
      {/* Форма редактирования информации пользователя */}
      <Form>
        <Form.Group className="mb-3" controlId="formUserName">
          <Form.Label>Имя пользователя:</Form.Label>
          <Form.Control
            type="text"
            value={selectedUser?.userName || ''}
            onChange={(e) => setSelectedUser({ ...selectedUser, userName: e.target.value })}
          />
        </Form.Group>
        
      </Form>
    </Modal.Body>
    <Modal.Footer>
      <Button variant="secondary" onClick={handleModalClose}>
        Закрыть
      </Button>
      <Button variant="danger" onClick={() => deleteUserClick(selectedUser.userId)}>
        Удалить
      </Button>
      <Button variant="primary" onClick={handleUserUpdate}>
        Сохранить
      </Button>
    </Modal.Footer>
  </Modal>
</Navbar>
);
});

export default Menu;