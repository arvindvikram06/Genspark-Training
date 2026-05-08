using System;
using NotifyModelLibrary;
using NotifyBLLibrary.Services;
using NotifyFELibrary.Interfaces; 
using NotifyBLLibrary.Interfaces; 
using NotifyDALLibrary.Interfaces;
using NotifyDALLibrary.Repositories;
using System.Collections.Generic;

namespace NotifyFELibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, User> userRepository = new UserRepository();
            IRepository<int, Notification> notificationRepository = new NotificationRepository();
            UserService userService = new UserService(userRepository);
            NotificationService notificationService = new NotificationService(notificationRepository, userRepository);
            IUserInteraction userInteraction = new UserInteractionService(userService, notificationService);

            MenuHandler menuHandler = new MenuHandler(userInteraction);
            menuHandler.Run();
        }
    }
}