using Npgsql;
using NotifyDALLibrary.Interfaces;
using NotifyModelLibrary;

namespace NotifyDALLibrary.Repositories;

public class NotificationRepository : AbstractRepository, IRepository<int, Notification>
{
    public Notification Create(Notification notification)
    {
        NpgsqlConnection _connection = GetDBConnection();
        string query = $"insert into notifications (user_id, message, notification_type) values('{notification.UserId}','{notification.Message}','{notification.NotificationType}')";
        NpgsqlCommand command = new NpgsqlCommand(query, _connection);
        try
        {
            _connection.Open();
            command.ExecuteNonQuery();
        }
        finally
        {
            _connection.Close();
        }

        return notification;
    }

    public List<Notification> GetAll()
    {
        NpgsqlConnection _connection = GetDBConnection();
        List<Notification> notifications = new List<Notification>();
        string query = "select * from notifications";
        NpgsqlCommand command = new NpgsqlCommand(query, _connection);

        try
        {
            _connection.Open();
            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                notifications.Add(new Notification(
                    (int)reader["id"],
                    reader["message"].ToString() ?? "",
                    reader["notification_type"].ToString() ?? "",
                    (int)reader["user_id"]
                ));
            }
            return notifications;
        }
        finally
        {
            _connection.Close();
        }
    }

    public Notification? GetById(int key) 
    {
        NpgsqlConnection _connection = GetDBConnection();
        string query = $"select * from notifications where id = {key}";
        NpgsqlCommand command = new NpgsqlCommand(query, _connection);
        try
        {
            _connection.Open();
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Notification(
                    (int)reader["id"],
                    reader["message"].ToString() ?? "",
                    reader["notification_type"].ToString() ?? "",
                    (int)reader["user_id"]
                );
            }
        }
        finally
        {
            _connection.Close();
        }
        return null;
    }

    public Notification? Update(int key, Notification notification)
    {
        NpgsqlConnection _connection = GetDBConnection();
        string query = $"update notifications set message='{notification.Message}' where id = '{key}'";
        NpgsqlCommand command = new NpgsqlCommand(query, _connection);
        try
        {
            _connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows > 0) return notification;
            return null;
        }
        finally
        {
            _connection.Close();
        }
    }

    public Notification? Delete(int key)
    {
        NpgsqlConnection _connection = GetDBConnection();
        string query = $"delete from notifications where id = {key}";
        NpgsqlCommand command = new NpgsqlCommand(query, _connection);
        Notification? notification = GetById(key);
        try
        {
            _connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            if (affectedRows > 0) return notification;
            return null;
        }
        finally
        {
            _connection.Close();
        }
    }
}
