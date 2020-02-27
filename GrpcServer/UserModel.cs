using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GrpcServer
{
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
