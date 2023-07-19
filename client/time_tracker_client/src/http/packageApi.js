import { $authHost, $host } from "./index";
import FileSaver from 'file-saver';


export const createPackage = async (data) => {
    try {

      const token = localStorage.getItem('token');
      const response = await $authHost.post("/api/package/create", {
        PackageName: data.packageName,
        PackageBudget: data.packageBudget,
        Status: data.status,
        PackageDescription: data.packageDescription,
        Owner: data.owner,
        Users:data.users,
        UsedPackageBudget: data.usedPackageBudget
        },{
            headers: { "Authorization": `Bearer ${token}` }
        })
        
        return response.data;
    } catch (error) {
        console.error(error);
        // Handle any errors appropriately
        }
    };

    export const fetchPackages = async (id) => {
        var tocken = localStorage.getItem('token');
        const response = await $authHost.get("/api/package/user/"+id, {
            headers: { "Authorization": `Bearer ${tocken}` }
        });
        return response.data
    }
    export const fetchPackageById = async (id) => {
        const response = await $authHost.get("/api/package/"+id);
        return response.data
    }
    export const deletePackage = async (id) => {
        const response = await $authHost.delete("/api/package/"+id,{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }

        });
        return response
    }
    export const updatePackage = async (data) => {
        const response = await $authHost.put("/api/package/"+data.id, {
            Id: data.id,
            PackageName: data.packageName,
            PackageBudget: data.packageBudget,
            Status: data.status,
            PackageDescription: data.packageDescription,
            UserIds:data.users,
            UsedPackageBudget: data.usedPackageBudget
        },
        {
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
            });
        return response
    }
    export const downloadPackage = async (id) => {
        $authHost.get('/api/download/'+id, { responseType: 'blob' })
        .then(response => {
            const file = new Blob([response.data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            FileSaver.saveAs(file, 'packages.xlsx');
        })
        .catch(error => {
            console.error('Error exporting packages:', error);
        });
    }
    export const getAllPackages = async () => {
        const response = await $authHost.get("/api/all/package",{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.result
    }
    export const updateUsersInPackage = async (id, users) => {
        console.log(users)
        const response = await $authHost.put("/api/package/"+id+"/updateUsers",{
            UserIds: users
        },{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.result
    }
    export const updatePackageStatus = async (id, status) => {
        const response = await $authHost.put("/api/package/"+id+"/updateStatus",{
            Status: status
        },{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.result
    }
    export const AddPackageBudget = async (id, budget) => {
     
        const response = await $authHost.post("/api/package/budget/create/"+id,{
            Id: "0",
            BudgetName: budget.name,
            Present: budget.present,
            UsedBudget:"00:00:00",
        },{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.result
    }
    export const UpdatePackageBudget = async (id, budget) => {
      let  response ={}
        if(budget.isUser==true){
        
             response = await $authHost.put("/api/package/budget/"+id,{
                Id: budget.budgetId,
                BudgetName: budget.name,
                Present: budget.present,
                UsedBudget: budget.usedBuget,
                IsUser: budget.isUser,
                UserId: budget.userId,
                Comment: budget.comment,
            },{
                headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
            });
        }else{
         response = await $authHost.put("/api/package/budget/"+id,{
            Id: budget.budgetId,
            BudgetName: budget.name,
            Present: budget.present,
            UsedBudget: budget.usedBuget,
        },{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });}
        return response.data.result
    }
    export const DeletePackageBudget = async (id, budget) => {
        console.log(budget)
        const response = await $authHost.post("/api/package/budget/"+id,{
            Id: budget.id,
            BudgetName: budget.budgetName,
            Present: budget.present,
            UsedBudget: budget.usedBudget,
        },{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data.result
    }
    export const GetPackageBudgets = async (id) => {
        const response = await $authHost.get("/api/all/budgets/"+id,{
            headers: { "Authorization": `Bearer ${localStorage.getItem('token')}` }
        });
        return response.data
        }
    

