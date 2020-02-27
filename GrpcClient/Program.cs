using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var input = new HelloRequest{ Name = "Bruce"};

            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new User.UserClient(channel);
            //List<UserTransferModel> users = new List<UserTransferModel>
            //{
            //    new UserTransferModel
            //    {
            //        UserName = "JohnnyBoy",
            //        Password = "StrongMan"
            //    },
            //    new UserTransferModel
            //    {
            //        UserName = "MangaLover",
            //        Password = "NeverDie"
            //    }
            //};
            var currentUser = await client.GetUserByIdAsync(new UserRequestModel {Id = "5e4ef65a76583c5b62147463"});

            Console.WriteLine(currentUser.UserName);
            //Console.WriteLine("Please update your password");
            //var user = new UserModel{UserName = "Bruce", Password = "1234"};
            //var reply = await client.CreateUserAsync(user);

            //var password = Console.ReadLine();
            //currentUser.Password = HashPassword(password);
            //var response = await client.UpsertUserAsync(currentUser);
            //if (response.IsUpdated)
            //    Console.WriteLine("success");

            Console.WriteLine();
            Console.WriteLine("Please enter your username:");
            var userName = Console.ReadLine();
            Console.WriteLine("Please enter password to login");
            var loginPassword = Console.ReadLine();

            var user = await client.LoginUserAsync(
                new UserTransferModel {UserName = userName, Password = loginPassword, ConfirmedPassword = loginPassword});

            if (user.IsSuccess)
                Console.WriteLine($"You have successfully logged in.");

            Console.WriteLine("Attempting to delete a user");
            var result = new ReplyMessage();
            try
            {
                 result = await client.DeleteUserAsync(new UserRequestModel {Id = "5e4f0858775b0e5550988c37"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine(result.IsSuccess);

            Console.ReadLine();
        }

        
    }
}
