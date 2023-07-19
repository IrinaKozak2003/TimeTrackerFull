using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;
using TimeTrackerServer.Service;
using OfficeOpenXml;

namespace TimeTrackerServer.Conrollers
{
    [ApiController]
    [Route("api")]

    public class PackageController : ControllerBase
    {
        private readonly PackagesService _packagesService;
        private readonly CycleService _cycleService;
        private readonly AuthenticationService _authenticationService;

        public PackageController(PackagesService packagesService, CycleService cycleService, AuthenticationService authenticationService)
        {
            _packagesService = packagesService;
            _cycleService = cycleService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("all/package")]
        public IActionResult Get()
        {
            try
            {
                var result = _packagesService.GetAllPackages();
                return Ok(result);
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while retrieving packages.");
            }
        }

        [HttpGet]
        [Route("package/{id}")]
        public async Task<IActionResult> GetPackageById(string id)
        {
            try
            {
                var result = await _packagesService.GetPackageById(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while retrieving the package.");
            }
        }



        [HttpGet]
        [Authorize]
        [Route("package/user/{userId}")]
        public async Task<IActionResult> GetPackagesByUser(string userId)
        {
            try
            {
                var result = await _packagesService.GetPackagesByUser(userId);
                return Ok(result);
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while retrieving packages.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("package/create")]
        public async Task<IActionResult> CreatePackage(CreatePackageRequest package)
        {
            try
            {
                var result = await _packagesService.CreatePackage(package);
                return Ok(result);
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while creating the package." + e.Message);
            }
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPut("package/{id}")]
        public async Task<IActionResult> UpdatePackage(string id, CreatePackageRequest packageIn)
        {
            try
            {
                Package package = new Package
                {
                    Id = packageIn.Id,
                    PackageName = packageIn.PackageName,
                    PackageBudget = TimeSpan.Parse(packageIn.PackageBudget),
                    PackageDescription = packageIn.PackageDescription,
                    Status = packageIn.Status

                };
                await _packagesService.UpdatePackage(id, package);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while updating the package.");
            }
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpDelete("package/{id}")]
        public async Task<IActionResult> DeletePackage(string id)
        {
            try
            {
                await _packagesService.DeletePackage(id);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while deleting the package.");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("download/{id}")]
        public async Task<IActionResult> ExportPackage(string id)
        {
            Package package = await _packagesService.GetPackageById(id);
            var cycles = await _cycleService.GetCyclesByPackage(id);
            using (var packageExcelPackage = new ExcelPackage())
            {
                var worksheet = packageExcelPackage.Workbook.Worksheets.Add("Packages");

                worksheet.Cells[ 1, 1].Value = package.PackageName;
                worksheet.Cells[ 1, 2].Value = package.PackageBudget;
                worksheet.Cells[ 1, 3].Value = package.PackageDescription;
                worksheet.Cells[ 1, 5].Value = package.Status;
  
                worksheet.Cells[1, 7].Value = "Users";
                var i = 2;
                foreach (var userId in package.UserIds)
                {
                    ApplicationUser user = await _authenticationService.GetUserById(userId);
                    worksheet.Cells[i, 7].Value = user.UserName;
                    var cyclesByUser = await _cycleService.GetCyclesByPackageAndUser(id, userId);
                     
                        var j = 1;
                
                        foreach (var cycle in cyclesByUser)
                        {
                            // Заполните ячейки для информации о цикле
                            worksheet.Cells[i + j, 1].Value = cycle.CycleTime;
                            worksheet.Cells[i + j, 2].Value = cycle.BudgetId;
                            worksheet.Cells[i + j, 3].Value = cycle.Comment;
                    
                            j++;
                        }
                
                        i += j;
                    }
                   
                
        
                // Сохраните Excel-документ и отправьте его для скачивания
                var stream = new MemoryStream(packageExcelPackage.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "package.xlsx");
            }
                
                
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("package/{id}/updateUsers")]
        public async Task<IActionResult> UpdateUsers(string id, UpdateUsersRequest request)
        {
            try
            {
               
                await _packagesService.UpdateUsersInPackage(id, request.UserIds);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while updating the package.");
            }

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("package/budget/create/{id}")]
        public async Task<IActionResult> CreateBudget(string id, BudgetRequest budgetRequest)
        {
            try
            {
                Budget budget = new Budget
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    BudgetName = budgetRequest.BudgetName,
                    Present = budgetRequest.Present,
                    UsedBudget = TimeSpan.Parse(budgetRequest.UsedBudget)
                };
                
                await _packagesService.AddBudgetInPackage(id, budget);
                
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while creating the package." + e.Message);
            }
        }

        [Authorize]
        [HttpPut("package/budget/{id}")]
        public async Task<IActionResult> UpdateBudget(string id,BudgetRequest budgetRequest)
        {
            if (budgetRequest.IsUser)
            {
                Cycle cycle = new Cycle
                {
                    BudgetId = budgetRequest.Id,
                    Comment = budgetRequest.Comment,
                    CycleTime = TimeSpan.Parse(budgetRequest.UsedBudget),
                    PackageId = id,
                    UserId = budgetRequest.UserId
                };
                _cycleService.CreateCycle(cycle);

            }
            Budget budget = new Budget
            {
                Id = budgetRequest.Id,
                BudgetName = budgetRequest.BudgetName,
                Present = budgetRequest.Present,
                UsedBudget = TimeSpan.Parse(budgetRequest.UsedBudget)
                
            };
            try
            {
                await _packagesService.UpdateBudgetInPackage(id, budgetRequest);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while updating the package.");
            }
        }

        [Authorize]
        [Authorize(Roles = "Admin")]
        [HttpPost("package/budget/{id}")]
        public async Task<IActionResult> DeletePackage(string id, BudgetRequest budgetRequest)
        {
            try
            {
                Budget budget = new Budget
                {
                    Id = budgetRequest.Id,
                    BudgetName = budgetRequest.BudgetName,
                    Present = budgetRequest.Present,
                    UsedBudget = TimeSpan.Parse(budgetRequest.UsedBudget)
                };
                await _packagesService.DeleteBudgetInPackage(id, budget);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while deleting the package.");
            }
        }
        [HttpGet]
        [Authorize]
        [Route("all/budgets/{id}")]
        public async Task<IActionResult> GetBudget(string id)
        {
            try
            {
                var result = await _packagesService.GetBudgetInPackage(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while retrieving packages.");
            }
        }

    }
}
