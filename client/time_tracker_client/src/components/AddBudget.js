import { useEffect, useState } from 'react';
import Button from 'react-bootstrap/Button';
import Form from 'react-bootstrap/Form';
import Modal from 'react-bootstrap/Modal';
import { AddPackageBudget, UpdatePackageBudget } from '../http/packageApi';
import { useContext } from 'react';
import { Context } from '../index';
import { observer } from 'mobx-react-lite';

function AddBudget({isUpdate = false, budget = null} ) {
    
  const [show, setShow] = useState(false);
    const [name, setName] = useState('');
    const [present, setPresent] = useState('');
    const [usedBudget, setUsedBudget] = useState('');
    const [comment, SetComment] = useState('');
    const {user_package, user} = useContext(Context);
    const ClickSave = async() => {
       

        if(isUpdate){
          let data = null
          if(user.isAdmin){
             data = {
                budgetId: budget.id,
                name: name,
                present: present,
                usedBuget: usedBudget,
            };}
            else{
               data = {
                budgetId: budget.id,
                name: name,
                present: present,
                usedBuget: usedBudget,
                comment: comment,
                userId: user.user.id,
                isUser: true};
            };
            await UpdatePackageBudget(user_package.selectedPack.id,data);
        }else{
        const data = {
           
            name: name,
            present: present,
            usedBuget: usedBudget,
        };
        await AddPackageBudget(user_package.selectedPack.id,data);}
        setShow(false);
        window.location.reload();}
    useEffect(() => {
        console.log(isUpdate);
        if (isUpdate) {
            console.log(budget);
            setName(budget.budgetName);
            setPresent(budget.present);
            setUsedBudget(budget.usedBudget);
        }
    }, []);
  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  

  return (
    <>
      <Button variant="primary" onClick={handleShow}>
        {!isUpdate?
      <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" fill="currentColor" className="bi bi-plus-lg" viewBox="0 0 16 16">
      <path fillRule="evenodd" d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2Z"/>
    </svg>: <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil" viewBox="0 0 16 16">
      <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708l-10 10a.5.5 0 0 1-.168.11l-5 2a.5.5 0 0 1-.65-.65l2-5a.5.5 0 0 1 .11-.168l10-10zM11.207 2.5 13.5 4.793 14.793 3.5 12.5 1.207 11.207 2.5zm1.586 3L10.5 3.207 4 9.707V10h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.293l6.5-6.5zm-9.761 5.175-.106.106-1.528 3.821 3.821-1.528.106-.106A.5.5 0 0 1 5 12.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.468-.325z" />
    </svg>}
      </Button>

      <Modal show={show} onHide={handleClose}>
        <Modal.Header >
          <Modal.Title>Add Budget</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            {user.isAdmin&&
            <div>
            <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
              <Form.Label>Title</Form.Label>
              <Form.Control
                type="text"
        
                autoFocus
                value={name}
                onChange={(e) => setName(e.target.value)}
              />
            </Form.Group>
            <Form.Group
              className="mb-3"
              controlId="exampleForm.ControlTextarea1"
            >
              <Form.Label>Presrnt</Form.Label>
              <Form.Control type="text"
             
              value={present}
              onChange={(e) => setPresent(e.target.value) } />
            </Form.Group></div>}
            {isUpdate && !user.isAdmin&&
            <div>
            <Form.Group
              className="mb-3"
              controlId="exampleForm.ControlTextarea1"
            >
              <Form.Label>UsedPackage</Form.Label>
              <Form.Control type="text" rows={3}
              value={usedBudget} onChange={(e) => setUsedBudget(e.target.value)} />
            </Form.Group>
             <Form.Group
             className="mb-3"
             controlId="exampleForm.ControlTextarea1"
           >
             <Form.Label>Comment</Form.Label>
             <Form.Control type="textarea" rows={3}
             value={comment} onChange={(e) => SetComment(e.target.value)} />
           </Form.Group>
           </div>}
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
          <Button variant="primary" onClick={ClickSave}>
            Save Changes
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

export default AddBudget;