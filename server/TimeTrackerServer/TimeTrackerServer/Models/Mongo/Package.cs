using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace TimeTrackerServer.Models
{
    public class Package
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")] public string PackageName { get; set; } = null!;

        public TimeSpan PackageBudget { get; set; }
        public List<Budget> Budgets { get; set; } = null!;
        public string PackageDescription { get; set; } = null!;
        public string Status { get; set; } = null!;

        [JsonIgnore] // Исключаем свойство Users из сериализации
        public List<string> UserIds { get; set; }

  
    }
}