using System;
using NotifyModelLibrary;
using NotifyBLLibrary.Services;
using NotifyFELibrary.Interfaces; 
using NotifyBLLibrary.Interfaces; 
using NotifyDALLibrary.Interfaces;
using NotifyDALLibrary.Repositories;
using NotifyDALLibrary.Contexts;
using System.Collections.Generic;

namespace NotifyFELibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            NotifyContext context = new NotifyContext();
            IRepository<int, User> userRepository = new UserRepository(context);
            IRepository<int, Notification> notificationRepository = new NotificationRepository(context);
            UserService userService = new UserService(userRepository);
            NotificationService notificationService = new NotificationService(notificationRepository, userRepository);
            IUserInteraction userInteraction = new UserInteractionService(userService, notificationService);

            MenuHandler menuHandler = new MenuHandler(userInteraction);
            menuHandler.Run();
        }
    }
}