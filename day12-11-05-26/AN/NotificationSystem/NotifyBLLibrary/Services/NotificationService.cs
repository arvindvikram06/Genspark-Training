using NotifyModelLibrary;
using NotifyBLLibrary.Interfaces; using NotifyDALLibrary.Interfaces;
using NotifyDALLibrary.Repositories;
using NotifyModelLibrary.Exceptions;
using NotifyBLLibrary.Validators;
using NotifyBLLibrary.NotificationSenders;

namespace NotifyBLLibrary.Services{
    public class NotificationService{
        private IRepository<int, Notification> _notificationRepository;
        private IRepository<int, User> _userRepository;
        private Dictionary<string,INotification> _notificationTypes; // used dictionary so we can add another type without modifying code.

        public NotificationService(IRepository<int, Notification> notificationRepository, IRepository<int, User> userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            
            _notificationTypes = new Dictionary<string, INotification>
            {
                { "email", new EmailNotificationSender() },
                { "sms", new SmsNotificationSender() }
            };
        }
        public Notification SendNotifification(int userId, string type, string message)
        {
            type = type.ToLower();

            NotificationValidator.ValidateNotification(type,message); // validate the notification

            User? user = _userRepository.GetById(userId); // get the user
            
            if (user == null)
                throw new UserNotFoundException(userId); // if user is not found, throw exception
            
            INotification notificationHandler = _notificationTypes[type]; // polymorphism

            Notification notification = new Notification(message,type,userId);

            notificationHandler.Send(user, notification); // send the notification

            return _notificationRepository.Create(notification);// save the notification
        }


        public void SendNotificationToAll(string type,string message)
        {
            List<User>? userList = _userRepository.GetAll();
            if(userList == null || userList.Count == 0)
            {
                throw new UserNotFoundException();
            }

            foreach(User user in userList) // loop and send notifcation to all users
            {
                SendNotifification(user.Id,type,message);
            }
        }

        public List<Notification> GetAllNotifications()
        {
            return _notificationRepository.GetAll(); // get all notifications
        }

        public Notification GetNotificationById(int id)
        {
            Notification? notification = _notificationRepository.GetById(id);
            if (notification == null)
                throw new Exception($"Notification with ID {id} not found.");
            return notification;
        }

        public Notification UpdateNotification(int id, string newMessage)
        {
            Notification notification = GetNotificationById(id);
            notification.Message = newMessage;
            
            Notification? updatedNotification = _notificationRepository.Update(id, notification);
            if (updatedNotification == null)
                throw new Exception("Failed to update notification.");
            
            return updatedNotification;
        }

        public Notification DeleteNotification(int id)
        {
            Notification? deletedNotification = _notificationRepository.Delete(id);
            if (deletedNotification == null)
                throw new Exception($"Failed to unsend notification. Notification with ID {id} not found.");
            
            return deletedNotification;
        }
    }
}