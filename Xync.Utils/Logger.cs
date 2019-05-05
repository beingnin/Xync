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
                Error error = new Error
                {
                    Source = exception.Source,
                    Message = exception.Message,
                    Title = title,
                    StackTrace = exception.StackTrace,
                    Exception = exception,
                    Type = exception.GetType().ToString()
                };

                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Error>("XYNC_Errors");
                await collection.InsertOneAsync(error);
            }
            catch (Exception ex)
            {

            }
        }
        public static async Task<long> DeleteAllErrors()
        {
            try
            {
                var db = _client.GetDatabase(Constants.NoSqlDB);
                var collection = db.GetCollection<Error>("XYNC_Errors");
                var filter = Builders<Error>.Filter.Empty;
               var result= await collection.DeleteManyAsync(filter);
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
                var collection = db.GetCollection<Error>("XYNC_Errors");
                var filter = Builders<Error>.Filter.Eq(x=>x.Id, ObjectId.Parse(id));
                var result = await collection.DeleteOneAsync(filter);
                return result.DeletedCount;
            }
            catch (Exception ex)
            {
                return default(long);
            }
        }
        public static async Task<IList<Error>> GetErrors()
        {
            var db = _client.GetDatabase(Constants.NoSqlDB);
            var collection = db.GetCollection<Error>("XYNC_Errors").AsQueryable();
            var query = from _error in collection
                        orderby _error.CreatedDateTime descending
                        select _error;
            return await query.ToListAsync();
        }
    }

}
