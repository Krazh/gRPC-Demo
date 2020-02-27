using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GrpcServer.Services
{
    public class UserService : User.UserBase
    {
        private readonly ILogger<GreeterService> _logger;
        private MongoCRUD _db;
        private const string _table = "Users";

        public UserService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            _db = new MongoCRUD("UserDB");
        }

        public override Task<ReplyMessage> CreateUser(UserTransferModel request, ServerCallContext context)
        {
            UserModel user = new UserModel
            {
                Password = request.Password,
                UserName = request.UserName
            };
            return Task.FromResult(new ReplyMessage{IsSuccess = _db.InsertRecord(_table, user) });
        }

        public override async Task GetAllUsers(NullModel request, IServerStreamWriter<UserTransferModel> responseStream, ServerCallContext context)
        {
            var users =_db.LoadRecords<UserModel>(_table);

            foreach (UserModel userModel in users)
            {
                await responseStream.WriteAsync(new UserTransferModel{Id = userModel.Id.ToString(), Password = userModel.Password, UserName = userModel.UserName});
            }
        }

        public override Task<UserTransferModel> GetUserById(UserRequestModel request, ServerCallContext context)
        {
            var user = _db.LoadRecordsById<UserModel>(_table, request.Id);

            return Task.FromResult(new UserTransferModel
            {
                Id = user.Id.ToString(),
                Password = user.Password,
                UserName = user.UserName
            });
        }

        public override Task<UpsertReplyMessage> UpsertUser(UserTransferModel request, ServerCallContext context)
        {
            UserModel user = new UserModel{Id = ObjectId.Parse(request.Id), Password = request.Password, UserName = request.UserName};
            var response = _db.UpsertRecord(_table, request.Id, user);

            return Task.FromResult(new UpsertReplyMessage
            {
                IsUpdated = response.IsAcknowledged,
                User = request
            });
        }

        public override Task<ReplyMessage> DeleteUser(UserRequestModel request, ServerCallContext context)
        {
            var result = _db.DeleteRecord<UserModel>(_table, request.Id);

            return Task.FromResult(new ReplyMessage {IsSuccess = result.IsAcknowledged});
        }

        public override Task<LoginReply> LoginUser(UserTransferModel request, ServerCallContext context)
        {
            var users = _db.LoadRecords<UserModel>(_table);
            var user = users.First(x => x.UserName == request.UserName);

            if (user.Password == request.Password)
            {
                // Implement a client token system. 
            }

            return Task.FromResult(new LoginReply{IsSuccess = false});
        }
    }
}
