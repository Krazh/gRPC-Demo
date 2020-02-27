using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
        public override Task<ReplyMessage> CreateUser(UserTransferModel request, ServerCallContext context)
        {
            UserModel user = new UserModel
            {
                UserName = request.UserName
            };

            if (request.Password == request.ConfirmedPassword)
                user.Password = PasswordHelper.HashPassword(request.Password);
            else
                return Task.FromResult(new ReplyMessage {IsSuccess = false});

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

        [Authorize("Administrators")]
        public override Task<ReplyMessage> DeleteUser(UserRequestModel request, ServerCallContext context)
        {
            var result = _db.DeleteRecord<UserModel>(_table, request.Id);

            return Task.FromResult(new ReplyMessage {IsSuccess = result.IsAcknowledged});
        }

        [AllowAnonymous]
        public override Task<LoginReply> LoginUser(UserTransferModel request, ServerCallContext context)
        {
            var users = _db.LoadRecords<UserModel>(_table);
            var user = users.First(x => x.UserName == request.UserName);
            var isSuccessValidatePassword = PasswordHelper.ValidatePassword(request.Password, user.Password);

            return Task.FromResult(isSuccessValidatePassword ? new LoginReply {ClientToken = "lol", IsSuccess = true} : new LoginReply{IsSuccess = false});
        }
    }
}
