using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.Collections;
using MongoDB.Bson;

namespace Xync.Utils
{
    public class Logger
    {
        static MongoClient _client = new MongoClient(Constants.NoSqlConnection);
        public static async Task Error(Exception exception, string title)
        {
            try
            {
                Event error = new Event
                {
                    Source = exception.Source,
                    Message = exception.Message,
                    Title = title,
                    StackTrace = exception.StackTrace,
                    Exception = exception,
                    Type = exception.GetType().ToString(),
                    MessageType=Message.MessageType.Error,
                    CreatedDateTime=DateTime.Now
                };

                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                await collection.InsertOneAsync(error);
            }
            catch (Exception ex)
            {
                ToFile(ex);

            }
        }
        public static async Task Success(string message, string title)
        {
            try
            {
                Event error = new Event
                {
                    Source = string.Empty,
                    Message = message,
                    Title = title,
                    StackTrace = string.Empty,
                    Exception = null,
                    Type = string.Empty,
                    MessageType=Message.MessageType.Success,
                    CreatedDateTime = DateTime.Now
                };

                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                await collection.InsertOneAsync(error);
            }
            catch (Exception ex)
            {
                ToFile(ex);
            }
        }
        public static async Task<long> DeleteAllErrors()
        {
            try
            {
                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                var filter = Builders<Event>.Filter.Eq(x => x.MessageType, Message.MessageType.Error);
                var result = await collection.DeleteManyAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                return default(long);
            }
        }
        public static async Task<long> DeleteAllEvents()
        {
            try
            {
                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                var filter = Builders<Event>.Filter.Empty;
                var result = await collection.DeleteManyAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                return default(long);
            }
        }
        public static async Task<long> DeleteAllOther()
        {
            try
            {
                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                var filter = Builders<Event>.Filter.Ne(x => x.MessageType, Message.MessageType.Error);
                var result = await collection.DeleteManyAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                return default(long);
            }
        }
        public static async Task<long> DeleteError(string id)
        {
            try
            {
                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Event>("XYNC_Messages");
                var filter = Builders<Event>.Filter.Eq(x=>x.Id, ObjectId.Parse(id));
                var result = await collection.DeleteOneAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                return default(long);
            }
        }
        public static async Task<IList<Event>> GetEvents(int page,int count)
        {
            var db = _client.GetDatabase(Constants.NoSqlDB);
            var collection = db.GetCollection<Event>("XYNC_Messages").AsQueryable();
            var query = (from _error in collection
                        orderby _error.CreatedDateTime descending
                        select _error).Skip(page*count).Take(count);
            return await query.ToListAsync();
        }
        static void ToFile(Exception ex)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Message");
                stringBuilder.AppendLine("-----------------");
                stringBuilder.AppendLine(ex.Message);
                stringBuilder.AppendLine("Stack Trace");
                stringBuilder.AppendLine("-----------------");
                stringBuilder.AppendLine(ex.StackTrace);
                System.IO.File.WriteAllText(@"C:\Users\Public","xync_log_"+Guid.NewGuid());
            }
            catch
            {

            }
        }
    }

}
