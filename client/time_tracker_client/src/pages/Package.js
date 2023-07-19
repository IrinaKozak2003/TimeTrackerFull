import { observer } from "mobx-react-lite";
import React, { useContext, useEffect } from "react";
import { ProgressBar, Form } from "react-bootstrap";
import Select from 'react-select';
import { useLocation } from "react-router-dom";
import { GetPackageBudgets, deletePackage, downloadPackage, fetchPackageById,  updateUsersInPackage } from "../http/packageApi";
import { fetchCycles } from "../http/cycleApi";
import { fetchUsersById } from "../http/userApi";
import { fetchUsers } from "../http/userApi";
import AddPackage from "../components/AddPackage";
import AddBudget from "../components/AddBudget";
import { Context } from "../index";
import { Button } from "react-bootstrap";
import BudgetItem from "../components/BudgetItem";
import { useNavigate } from "react-router-dom";

//создай массив из пользователей


const Package = observer(()=>{
    const navigate = useNavigate();
    const location = useLocation();
    const package_id = location.pathname.split('/')[2];
    const {user_package, user} = useContext(Context)
    let [loading, setLoading] = React.useState(true);
    let [userTime, setUserTime] = React.useState('00:00:00');
    const [options, setOptions] = React.useState([]);
    const [budget, setBudget] = React.useState(false);
    let [selectedPeople, setSelectedPeople] = React.useState([]);

    
    useEffect(() => {
        const fetchData = async () => {
          try {
            const users = await fetchUsers();
            setOptions(users.map((user) => ({ value: user, label: user.userName })));
            setLoading(false);
            await fetchPackageById(package_id).then((data) => {
              setLoading(false);
              Promise.all(
                data.userIds.map((user) =>
                  fetchUsersById(user).then((u) => ({
                    value: u,
                    label: u.userName,
                  }))
                )
              ).then((people) => setSelectedPeople(people));
              user_package.setSelectedPack(data);
            });
            await fetchCycles(package_id, user.user.id).then((data) => {
              setLoading(false);
              data.map((d) =>
                setUserTime((prevUserTime) => addTimeIntervals(prevUserTime, d.cycleTime))
              );
            });
            await GetPackageBudgets(package_id).then((data) => {
              setLoading(false);
              setBudget(data);

            });
          
            function addTimeIntervals(time1, time2) {
              console.log(time1, time2);
              const [time1Hours, time1Minutes] = time1.split(':').map(Number);
              const [time2Hours, time2Minutes] = time2.split(':').map(Number);
      
              const sumHours = time1Hours + time2Hours;
              const sumMinutes = time1Minutes + time2Minutes;
      
              const resultHours = sumHours + Math.floor(sumMinutes / 60);
              const resultMinutes = sumMinutes % 60;
      
              return `${resultHours.toString().padStart(2, '0')}:${resultMinutes.toString().padStart(2, '0')}`;
            }
          } catch (error) {
            console.error(error);
          }
        };
      
        fetchData();
      }, [package_id, user.user.id, user_package]);
      const handleSelectChange = (selectedOptions) => {
        const uniqueOptions = [...new Set(selectedOptions)]; // Remove duplicate options
        setSelectedPeople(uniqueOptions);
      };
      
    
      const download =(async () => {
        try {
            await downloadPackage(user_package.selectedPack.id)
            window.location.reload();
        } catch(e){
            console.error(e)
        }})

      
         const  deletePackageClick = async() => {
          await deletePackage(user_package.selectedPack.id);
          navigate('/main');
        }
        const handleUpdateUsers = async () => {
          try {
            const userIds = selectedPeople.map((person) => person.value.userId);
 
            await updateUsersInPackage(package_id, userIds);
            window.location.reload();
            // Handle success or any additional logic after updating the users
          } catch (error) {
            console.error(error);
            // Handle error or display an error message to the user
          }
        };
        
    if (loading) return 'Loading...';
    else
    return(<div>

<h1>
  {user_package.selectedPack.packageName} {user.isAdmin && (
    <div style={{ display: 'flex', alignItems: 'center' }}>
      <AddPackage isUpdate={true} />

      <Button
        variant="link"
        onClick={deletePackageClick}
        style={{ marginLeft: '10px' }} // Adjust the spacing as needed
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="16"
          height="16"
          fill="currentColor"
          className="bi bi-trash3"
          viewBox="0 0 16 16"
        >
          <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5ZM11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H2.506a.58.58 0 0 0-.01 0H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 0 0 0 0-1h-.995a.59.59 0 0 0-.01 0H11Zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5h9.916Zm-7.487 1a.5.5 0 0 1 .528.47l.5 8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 0 0 1 .528-.47ZM8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5Z" />
        </svg>
      </Button>
    </div>
  )}
</h1>

        {user.isAdmin?
     <AddPackage isUpdate={true}/>:null}
        <h2>Total Budjet</h2>
        <div>{user_package.selectedPack.packageBudget}</div>
        {user.isAdmin &&
        <AddBudget />}
          {budget&&budget.map((b) => (
            
            <div key={b.id}>
              <BudgetItem props={b} />
            </div>
          ))}
        <ProgressBar now={(user_package.selectedPack.usedPackageBudget/user_package.selectedPack.packageBudget)*100} />
        <Form.Label>Select users</Form.Label>
        {user.isAdmin?
          <div>
            {loading ? (
              <div>Loading...</div>
            ) : (
              <Select options={options} value={selectedPeople} isMulti onChange={handleSelectChange} />
            )}
            </div>:null}
            <div>
              Selected People: 
              {selectedPeople.map((person) => (
                
                <span key={person.value.id}> {person.label},</span>
              ))}
            </div>
            {user.isAdmin?
           < div>
         
          <Button variant="primary" onClick={handleUpdateUsers}></Button> 
       
        <Button onClick={download}>
            download 
        </Button></div>:null}
  

{!user.isAdmin?
<div> 
     <div> Your Time</div>
    <div>{userTime}</div>
    </div>:null}
        </div>
    )
})


export default Package;