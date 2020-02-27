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
            Console.WriteLine("Please enter password to login");
            var loginPassword = Console.ReadLine();

            var user = client.GetUserById(new UserRequestModel{Id = currentUser.Id });

            var isLoggedIn = ValidatePassword(loginPassword, user.Password);

            if (isLoggedIn)
                Console.WriteLine($"You have successfully logged in. Hashed password is {user.Password}");

            Console.ReadLine();
        }

        public static string GetRandomSalt()
        {
            return BCryptHelper.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCryptHelper.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string hashedPassword)
        {
            return BCryptHelper.CheckPassword(password, hashedPassword);
        }
    }
}
