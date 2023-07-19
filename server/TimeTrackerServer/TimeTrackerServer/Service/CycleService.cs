using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;

namespace TimeTrackerServer.Service;

public class CycleService
{
  private readonly IMongoCollection<Cycle> _cyclesCollection;
  
  public CycleService(
    IOptions<TimeTrackerDatabaseSettings> timeTrackerDatabaseSettings)
  {
    var mongoClient = new MongoClient(
      timeTrackerDatabaseSettings.Value.ConnectionString);

    var mongoDatabase = mongoClient.GetDatabase(
      timeTrackerDatabaseSettings.Value.DatabaseName);

    _cyclesCollection = mongoDatabase.GetCollection<Cycle>(
      timeTrackerDatabaseSettings.Value.CycleCollectionName);
  }
  //get all cycles
  public async Task<List<Cycle>> GetAllCycles()
  {
    return await _cyclesCollection.Find(_ => true).ToListAsync();
  }
  //get cycle by id
  public async Task<Cycle?> GetCycleById(string id)
  {
    return await _cyclesCollection.Find(cycle => cycle.Id.ToString() == id).FirstOrDefaultAsync();
  }
  //create cycle
  public async Task<Cycle> CreateCycle(Cycle cycle)
  {
    
    await _cyclesCollection.InsertOneAsync(cycle);
    return cycle;
  }
  //update cycle
  public async Task UpdateCycle(string id, Cycle cycleIn)
  {
    await _cyclesCollection.ReplaceOneAsync(cycle => cycle.Id.ToString() == id, cycleIn);
  }
  //delete cycle
  public async Task DeleteCycle(string id)
  {
    await _cyclesCollection.DeleteOneAsync(cycle => cycle.Id.ToString() == id);
  }
  //get cycles by owner
  public async Task<List<Cycle>> GetCyclesByOwner(string ownerId)
  {
    return await _cyclesCollection.Find(cycle => cycle.UserId.ToString() == ownerId).ToListAsync();
  }
  //get cycles by package
  public async Task<List<Cycle>> GetCyclesByPackage(string packageId)
  {
    return await _cyclesCollection.Find(cycle => cycle.PackageId.ToString() == packageId).ToListAsync();
  }
  public async Task<List<Cycle>> GetCyclesByPackageAndUser(string packageId, string userId)
  {
    return await _cyclesCollection.Find(cycle => cycle.PackageId.ToString() == packageId && cycle.UserId.ToString()== userId).ToListAsync();
  }
    


}