using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TimeTrackerServer.Dtos;
using TimeTrackerServer.Models;

namespace TimeTrackerServer.Service;

public class PackagesService
{
    private readonly IMongoCollection<Package> _packagesCollection;
    private readonly UserManager<ApplicationUser> _userManager;

    public PackagesService(
        IOptions<TimeTrackerDatabaseSettings> timePackageDatabaseSettings, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        var mongoClient = new MongoClient(
            timePackageDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            timePackageDatabaseSettings.Value.DatabaseName);

        _packagesCollection = mongoDatabase.GetCollection<Package>(
            timePackageDatabaseSettings.Value.PackageCollectionName);
    }
    //get all packages 

    public async Task<List<PackageItemResponse>> GetAllPackages()
    {
        List<Package> packages = await  _packagesCollection.Find(_ => true).ToListAsync();
        List<PackageItemResponse> packageItemResponseList = packages.Select(i => new PackageItemResponse
        {
            Id = i.Id.ToString(),
            Status = i.Status,
            PackageName = i.PackageName,
            TotalTime = i.PackageBudget,

            UsedBudgetInPresents = i.Budgets.Sum(i => i.UsedBudget.Hours) / (i.PackageBudget.Hours)
        }).ToList();

        return packageItemResponseList;
    }
    //get package by id

    public async Task<Package?> GetPackageById(string id)
    {
        return await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
    }
    //create package

    public async Task<Package> CreatePackage(CreatePackageRequest package)
    {
        // Create a new Package instance
        var newPackage = new Package
        {
            Id = ObjectId.GenerateNewId().ToString(),
            PackageName = package.PackageName,
            PackageBudget = TimeSpan.Parse(package.PackageBudget),
            Budgets = new List<Budget>(),
            PackageDescription = package.PackageDescription,
            Status = package.Status,
        };

        // Add user ids to the new package
        if (package.Users != null)
        {
            newPackage.UserIds = package.Users.Select(u => u.UserId).ToList();

            // Update the user's PackageIds field
            foreach (var user in package.Users)
            {
                var existingUser = await _userManager.FindByIdAsync(user.UserId);
                if (existingUser != null)
                {
                    existingUser.PackageIds.Add((ObjectId.Parse(newPackage.Id)));
                    await _userManager.UpdateAsync(existingUser);
                }
            }
        }

        await _packagesCollection.InsertOneAsync(newPackage);


        return newPackage;
    }


    //update package
    public async Task UpdatePackage(string id, Package packageIn)
    {
        Package package = await _packagesCollection.Find(p => p.Id == packageIn.Id).FirstOrDefaultAsync();
        package.PackageName = packageIn.PackageName;
        package.PackageBudget = packageIn.PackageBudget;
        package.PackageDescription = packageIn.PackageDescription;
        package.Status = packageIn.Status;

        await _packagesCollection.ReplaceOneAsync(pack => pack.Id.ToString() == id, package);
    }

    //delete package
    public async Task DeletePackage(string id)
    {
        await _packagesCollection.DeleteOneAsync(package => package.Id.ToString() == id);
    }

    //get packages by user
    public async Task<List<PackageItemResponse>> GetPackagesByUser(string userId)
    {
        // Retrieve the user from the database using the provided userId
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            // User not found, return an empty list or throw an exception
            return new List<PackageItemResponse>();
        }

        var packages = new List<Package>();
        // Retrieve the packages associated with the user
        var packageIds = user.PackageIds;
        foreach (var pack in packageIds)
        {
            Package package = await _packagesCollection.Find(p => p.Id == pack.ToString()).FirstOrDefaultAsync();
            if (pack != null)
            {
                packages.Add(package);
            }
        }


        List<PackageItemResponse> packageItemResponseList = packages.Select(i => new PackageItemResponse
        {
            Id = i.Id.ToString(),
            Status = i.Status,
            PackageName = i.PackageName,
            TotalTime = i.PackageBudget,

            UsedBudgetInPresents = i.Budgets.Sum(i => i.UsedBudget.Hours) / (i.PackageBudget.Hours)
        }).ToList();

        return packageItemResponseList;
    }
    //Update Users in Package

    public async Task UpdateUsersInPackage(string id, List<string> users)
    {
        var package = await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
        if (package != null)
        {
            var currentUsers = package.UserIds.ToList();

            // Add new users
            var newUsers = users.Except(currentUsers);
            foreach (var newUser in newUsers)
            {
                var existingUser = await _userManager.FindByIdAsync(newUser);
                if (existingUser != null)
                {
                    existingUser.PackageIds.Add(ObjectId.Parse(package.Id));
                    await _userManager.UpdateAsync(existingUser);
                    package.UserIds.Add(newUser);
                }
            }

            // Remove users that are no longer selected
            var removedUsers = currentUsers.Except(users);
            foreach (var removedUser in removedUsers)
            {
                var existingUser = await _userManager.FindByIdAsync(removedUser);
                if (existingUser != null)
                {
                    existingUser.PackageIds.Remove(ObjectId.Parse(package.Id));
                    await _userManager.UpdateAsync(existingUser);
                    package.UserIds.Remove(removedUser);
                }
            }

            await _packagesCollection.ReplaceOneAsync(p => p.Id == package.Id, package);
        }
    }

    public async Task AddBudgetInPackage(string id, Budget budget)
    {
        try
        {
            var package = await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
            decimal sum = 0;
            foreach (var budg in package.Budgets)
            {
                sum += budget.Present;
            }

            if (sum > 100)
            {
                throw new Exception("Invalid present");
            }

            if (package != null)
            {
                package.Budgets.Add(budget);
            }

            await _packagesCollection.ReplaceOneAsync(p => p.Id == package.Id, package);
        }
        catch (Exception e)
        {
        }
    }

    public async Task UpdateBudgetInPackage(string id, BudgetRequest budgetRequest)
    {
        var package = await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
        if (package != null)
        {
            if (budgetRequest.IsUser)
            {
            }

            Budget budget = new Budget
            {
                Id = budgetRequest.Id,
                BudgetName = budgetRequest.BudgetName,
                Present = budgetRequest.Present,
            };
            decimal sum = 0;
            foreach (var budg in package.Budgets)
            {
                sum += budget.Present;
            }

            if (sum > 100)
            {
                throw new Exception("Invalid present");
            }

            // Находим индекс старого бюджета, который нужно заменить
            int index = package.Budgets.FindIndex(b => b.Id == budget.Id);

            if (index != -1)
            {
                budget.UsedBudget =
                    TimeSpan.Parse(budgetRequest.UsedBudget) +
                    package.Budgets[index].UsedBudget; // Удаляем старый бюджет
                package.Budgets.RemoveAt(index);

                // Вставляем новый бюджет на том же месте
                package.Budgets.Insert(index, budget);
            }
        }

        await _packagesCollection.ReplaceOneAsync(p => p.Id == package.Id, package);
    }

    public async Task DeleteBudgetInPackage(string id, Budget budget)
    {
        var package = await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
        if (package != null)
        {
            int ind = package.Budgets.FindIndex(b => b.Id == budget.Id);
            package.Budgets.RemoveAt(ind);
        }

        await _packagesCollection.ReplaceOneAsync(p => p.Id == package.Id, package);
    }

    public async Task<List<Budget>> GetBudgetInPackage(string id)
    {
        var package = await _packagesCollection.Find(package => package.Id.ToString() == id).FirstOrDefaultAsync();
        if (package != null)
        {
            return package.Budgets;
        }
        else
        {
            throw new Exception("No such package");
        }
    }
}