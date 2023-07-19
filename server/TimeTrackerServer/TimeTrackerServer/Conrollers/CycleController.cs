using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;
using TimeTrackerServer.Service;

namespace TimeTrackerServer.Conrollers
{
    [Route("api/cycle")]
    [Authorize]
    [ApiController]
    public class CycleController : ControllerBase
    {
        private readonly CycleService _cycleService;
        private readonly PackagesService _packagesService;

        public CycleController(CycleService cycleService, PackagesService packagesService)
        {
            _cycleService = cycleService;
            _packagesService = packagesService;
        }

        [HttpGet]
        [Authorize]
        [Route("package/cycle/{packageId}")]
        public async Task<IActionResult> GetCyclesByPackage(string packageId)
        {
            try
            {
                var result = await _cycleService.GetCyclesByPackage(packageId);
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
        [Route("package/cycle/{packageId}/{userId}")]
        public async Task<IActionResult> GetCyclesByPackage(string packageId, string UserId)
        {
            try
            {
                var result = await _cycleService.GetCyclesByPackageAndUser(packageId, UserId);
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
       
        

        [HttpPut]
        [Authorize]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateCycle([FromBody] Cycle cycle, string id)
        {
            try
            {
                await _cycleService.UpdateCycle(id, cycle);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while updating the cycle.");
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteCycle(string id)
        {
            try
            {
                await _cycleService.DeleteCycle(id);
                return Ok();
            }
            catch (Exception e)
            {
                // Log the exception or handle it as per your application's requirements
                return StatusCode(500, "An error occurred while deleting the cycle.");
            }
        }
    }
}