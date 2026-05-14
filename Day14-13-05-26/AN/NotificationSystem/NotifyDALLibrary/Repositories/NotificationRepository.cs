using Npgsql;
using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;
using NotifyDALLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using NotifyModelLibrary.Exceptions;
namespace NotifyDALLibrary.Repositories;

public class NotificationRepository : IRepository<int, Notification>
{
    private readonly NotifyContext _context;


    public NotificationRepository(NotifyContext context)
    {
        _context = context;
    }

    public Notification Create(Notification notification)
    {
        try
        {
             _context.Notifications.Add(notification);
            _context.SaveChanges();
            return notification;
        }
        catch(DbUpdateException ex)
        {
            throw new DatabaseException("Could not create notification in database", ex);
        }
    }


    public Notification? GetById(int key) 
    {
        try
        {
            return _context.Notifications.Find(key);
        }
        catch (Exception ex)
        {
            throw new DatabaseException($"Error retrieving notification with ID {key}", ex);
        }
    }

    public Notification? Update(int key, Notification notification)
    {
        try
        {
            var updatedEntity = _context.Notifications.Update(notification);
            _context.SaveChanges();
            return updatedEntity.Entity;
        }
        catch(DbUpdateException ex)
        {
            throw new DatabaseException("Could not update notification in database", ex);
        }
    }

    public List<Notification> GetAll()
    {
        try
        {
            return _context.Notifications.AsNoTracking().ToList();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Error retrieving all notifications", ex);
        }
    }

    

    public Notification? Delete(int key)
    {   
        try
        {
            Notification? notification = GetById(key);
            if(notification == null) return null;
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
            return notification;
        }
        catch(DbUpdateException ex)
        {
            throw new DatabaseException("Could not delete notification from database", ex);
        }
    }
}
