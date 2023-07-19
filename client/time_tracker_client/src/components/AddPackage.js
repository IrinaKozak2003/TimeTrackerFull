import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { Form } from 'react-bootstrap';
import React, { useState, useEffect } from 'react';
import Select from 'react-select';
import { Card, Row } from 'react-bootstrap';
import { fetchUsers, fetchUsersById } from '../http/userApi';
import { observer } from 'mobx-react-lite';
import { useContext } from 'react';
import { Context } from '../index';
import { createPackage, fetchPackageById, updatePackage } from '../http/packageApi';

const  AddPackage= observer(({isUpdate = false}) => {
  const { user, user_package } = useContext(Context);
  const [show, setShow] = useState(false);
  const [selectedPeople, setSelectedPeople] = useState([]);
  const [loading, setLoading] = useState(true);
  const [options, setOptions] = useState([]);
  const [name, setName] = useState('');
  const [budget, setBudget] = useState('');
  const [description, setDescription] = useState('');
  const owner ={
    UserId: user.user.id,
    userName: user.user.userName
  }
const onClick = async() => {
    const data = {
        packageName: name,
        packageBudget: budget,
        UsedBudget:0,
        status: 'active',
        packageDescription: description,
        owner: owner, 
        users: selectedPeople.map((person) => person.value),
        usedPackageBudget: 0
    };
    await createPackage(data);
    window.location.reload();
    setShow(false)
  }
  const UpdateClick = async() => {
    const data = {
      id: user_package.selectedPack.id,
      packageName: name,
      packageBudget: budget,
      usedPackageBudget:user_package.selectedPack.usedPackageBudget,
      status: 'active',
      packageDescription: description,
      users: selectedPeople.map((person) => person.value.id),
    }
    await updatePackage(data);
    window.location.reload();
    setShow(false);
  }
  useEffect(() => {
    const fetchData = async () => {
      try {
        const users = await fetchUsers();
        setOptions(users.map((user) => ({ value: user, label: user.userName })));
        setLoading(false);
      } catch (error) {
        console.error(error);
      }
    };

    fetchData();
    const setPack=()=>{
    if (isUpdate) {
      user_package.selectedPack.id &&
     fetchPackageById(user_package.selectedPack.id).then(data=>{
      setName(data.packageName);
      setBudget(data.packageBudget);
      setDescription(data.packageDescription);  
      Promise.all(
        data.userIds.map((user) =>
          fetchUsersById(user).then((u) => ({
            value: u,
            label: u.userName,
          }))
        )
      ).then((people) => setSelectedPeople(people));
     
      })
    }}
    setPack();
  }, [user_package.selectedPack]);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  const handleSelectChange = (selectedOptions) => {
    setSelectedPeople(selectedOptions);
  };

  return (
    <>{!isUpdate?
      <Row className="d-flex justify-content-center" style={{ margin: '10px' }}>
        <Card style={{ width: '320.5px', height: '180px' }} onClick={handleShow}>
          <Card.Body style={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
            <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" fill="currentColor" className="bi bi-plus-lg" viewBox="0 0 16 16">
              <path fillRule="evenodd" d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2Z"/>
            </svg>
          </Card.Body>
        </Card>
      </Row>: <Button variant="link" onClick={handleShow} style={{ position: 'absolute', top: '0px', right: '0px' }}>
    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil" viewBox="0 0 16 16">
      <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z" />
    </svg>
  </Button>}


      <Modal show={show} onHide={handleClose} backdrop="static">
        <Modal.Header >
          {isUpdate&&isUpdate?<Modal.Title>Add Package</Modal.Title>:<Modal.Title>Update Package</Modal.Title>}
        </Modal.Header>
        <Modal.Body>
          <Form.Label>Package name</Form.Label>
          <Form.Control type="text" placeholder="Enter package name" value={name}  onChange={(e) => setName(e.target.value)} />
          <Form.Label>Package budget</Form.Label>
          <Form.Control type="time" placeholder="Enter package budget" value={budget} onChange={(e) => setBudget(e.target.value)} />
          {!isUpdate? <div>
          <Form.Label>Select users</Form.Label>
          <div>
            {loading ? (
              <div>Loading...</div>
            ) : (
              <Select options={options} value={selectedPeople} isMulti onChange={handleSelectChange} />
            )}
            <div>
              Selected People: 
              {selectedPeople.map((person) => (
                
                <span key={person.value.id}> {person.label},</span>
              ))}
            </div>
          </div>
          </div>:null}
          <Form.Group className="mb-3" controlId="exampleForm.ControlTextarea1">
            <Form.Label>Package Description</Form.Label>
            <Form.Control as="textarea" rows={3} value={description} onChange={(e) => setDescription(e.target.value)} />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
          {isUpdate&&isUpdate?<Button variant="primary" onClick={UpdateClick}>Update</Button>:
          <Button variant="primary" onClick={onClick}>Add</Button>}
        </Modal.Footer>
      </Modal>
    </>
  );
});

export default AddPackage;
