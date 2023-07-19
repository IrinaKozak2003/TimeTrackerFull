import React, { useContext, useEffect } from 'react';
import { Card, Row } from 'react-bootstrap';
import { observer } from 'mobx-react-lite';
import { ProgressBar } from 'react-bootstrap';
import { useState } from 'react';


import { Link } from 'react-router-dom';
import { Context } from '../index';
import AddPackage from './AddPackage';


const PackageItem = observer(({ packageItem }) => {
  const [showModal, setShowModal] = useState(false);
  const {user_package, user} = useContext(Context);
  useEffect(() => {
    user_package.setSelectedPack(packageItem);
  }, [packageItem]);


return (
  <Row className="d-flex" style={{ margin: '10px' }}>
  
    <Card key={packageItem.id} style={{ width: '320.5px', height: '180px' }}>
      <Card.Text style={{ marginRight: '20px', marginTop: '10px', padding: '0px',marginLeft:'20px', marginBottom:  '0px' }}>{packageItem.status}</Card.Text>
    {user.isAdmin?
     <AddPackage isUpdate={true}/>:null}
     <Link to={`/package/${packageItem.id}`} style={{ textDecoration: 'none' }}>
      <Card.Body style={{ padding: '20px', marginTop: '30px' }}>
        <Card.Title>{packageItem.packageName}</Card.Title>
        <ProgressBar now={packageItem.usedBudgetInPresents} label={`${packageItem.usedBudgetInPresents}%`} visuallyHidden />
        <Card.Text>Total Time: {packageItem.totalTime}</Card.Text>
      </Card.Body> 
      </Link>
      {packageItem.owner ? <Card.Text style={{ textAlign: 'right', marginRight: '20px', marginBottom: '10px' }}>Owner</Card.Text> : null}
      
    </Card>
 
</Row>)

});

export default PackageItem;
