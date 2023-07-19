import { $authHost, $host } from "./index";

export const createCycle = async (data) => {
    try {
      const token = localStorage.getItem('token');
  
      const response = await $authHost.post("/api/cycle/create", {
        UserId: data.userId,
        CycleTime: data.cycleTime.toString(),
        Budget: data.budget,
        PackageId: data.packageId,
        Comment: data.comment
      }, {
        headers: { "Authorization": `Bearer ${token}` }
      });
      return response.data;
    } catch (error) {
      console.error(error);
    }
  };
  export const fetchCycles = async (packageId, userId) => {
    var tocken = localStorage.getItem('token');
    const response = await $authHost.get("/api/cycle/package/cycle/"+packageId+"/"+userId, {
        headers: { "Authorization": `Bearer ${tocken}` }
    });
    return response.data}

  