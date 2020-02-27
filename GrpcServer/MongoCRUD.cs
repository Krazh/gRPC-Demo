using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GrpcServer
{
    public class MongoCRUD
    {
        private IMongoDatabase _db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            _db = client.GetDatabase(database);
        }

        public bool InsertRecord<T>(string table, T record)
        {
            var collection = _db.GetCollection<T>(table);
            try
            {
                collection.InsertOne(record);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public List<T> LoadRecords<T>(string table)
        {
            var collection = _db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordsById<T>(string table, string id)
        {
            var collection = _db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(id));

            return collection.Find(filter).First();
        }

        public ReplaceOneResult UpsertRecord<T>(string table, string id, T record)
        {
            var collection = _db.GetCollection<T>(table);

            return collection.ReplaceOne(
                new BsonDocument("_id", ObjectId.Parse(id)), 
                record,
                new ReplaceOptions() {IsUpsert = true});
        }

        public DeleteResult DeleteRecord<T>(string table, string id)
        {
            var collection = _db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(id));

            return collection.DeleteOne(filter);

        }
    }
}