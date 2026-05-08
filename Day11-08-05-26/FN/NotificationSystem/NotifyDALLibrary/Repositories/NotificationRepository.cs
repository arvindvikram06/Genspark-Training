
using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;

namespace NotifyDALLibrary.Repositories;

public class NotificationRepository : IRepository<int, Notification>
{

    private Dictionary<int, Notification> _notificationsList = new Dictionary<int, Notification>();


    public Notification Create(Notification notification)
    {
        _notificationsList.Add(notification.Id,notification);
        return notification;
    }

    public List<Notification> GetAll()
    {
        return _notificationsList.Values.ToList();
    }

    // future implementation
    public Notification? GetById(int key) 
    {
        if(_notificationsList.ContainsKey(key))
        {
            return _notificationsList[key];
        }
        return null;
    }

    // future implementation
    public Notification? Update(int key, Notification notification)
    {
        if(_notificationsList.ContainsKey(key) && notification != null)
        {
            _notificationsList[key] = notification;
            return notification;
        }
        return null;
    }

    // future implementation
    public Notification? Delete(int key)
    {
        if(_notificationsList.ContainsKey(key))
        {
            Notification notification = _notificationsList[key];
            _notificationsList.Remove(key);
            return notification;
        }
        return null;
    }

}
