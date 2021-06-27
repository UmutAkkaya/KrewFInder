using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using DatabaseLayer.Models;

namespace DatabaseLayer
{
    public class CollectionName : Attribute
    {
        public String Value { get; }

        public CollectionName(String collectionName)
        {
            Value = collectionName;
        }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException()
        {
        }

        public DatabaseException(string message)
            : base(message)
        {
        }
    }

    public class InvalidModelException : DatabaseException
    {
        public InvalidModelException()
        {
        }

        public InvalidModelException(string message)
            : base(message)
        {
        }
    }

    public class DatabaseNotInitializedException : DatabaseException
    {
        public DatabaseNotInitializedException() :
            base("Mongo DB Connection is not initialized. Make sure you call InitializeDatabase before performing any database oprations.")
        {
        }

        public DatabaseNotInitializedException(String message)
            : base(message)
        {
        }
    }

    public static class MongoDBLayer
    {

        private static MongoClient _client;
        private static IMongoDatabase _database;
        public static readonly int DEFAULT_PORT = 27017;

        public static void InitializeDatabase(String connectionString, String database)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(database);
        }

        // Client code MUST call this before performing any actual database operations
        public static void InitializeDatabase(String hostname, int port, String username, String password, String connectionDatabase, String database)
        {
            // mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
            InitializeDatabase(String.Format("mongodb://{0}:{1}@{2}:{3}/{4}", username, password, hostname, port, connectionDatabase), database);
        }

        public static void InitializeDefaultDatabase()
        {
                InitializeDatabase("ec2-18-233-156-157.compute-1.amazonaws.com", DEFAULT_PORT, "admin", "h7WsssrE3sseqkHb4DssAGrv9CQ9c", "admin", "default");
        }

        // This is hardcoded to test database so that someone doesn't type something... unwise.
        public static void DropTestDatabase(string database)
        {
            if (_database is null)
                throw new DatabaseNotInitializedException();
            if (!database.StartsWith("test_"))
                throw new DatabaseException("Tried dropping non-test database. Bad");
            _client.DropDatabase(database);
        }

        public static IMongoCollection<TModel> GetCollectionForModel<TModel>() where TModel : DatabaseModel
        {
            if (!(typeof(TModel).GetCustomAttributes(typeof(CollectionName)).FirstOrDefault() is CollectionName collectionNameAttribute))
            {
                throw new InvalidModelException(
                    String.Format(
                        "Type {0} does not appear to be a valid model. Models must specify CollectionName attribute", nameof(TModel))
                );
            }
            return _database.GetCollection<TModel>(collectionNameAttribute.Value);
        }

        public static async Task<T> FindOneById<T>(ObjectId objectRef) where T : DatabaseModel
        {
            if (_database is null)
                throw new DatabaseNotInitializedException();

            var collection = GetCollectionForModel<T>();
            var document = (await collection.FindAsync(Builders<T>.Filter.Eq("_id", objectRef))).FirstOrDefault();
            return document;
        }

        public static async Task<T> FindOneById<T>(String id) where T : DatabaseModel
        {
            return ObjectId.TryParse(id, out var objectId) ? await FindOneById<T>(objectId) : null;
        }

        public static async Task<IEnumerable<T>> FindById<T>(IEnumerable<ObjectId> objectIds) where T : DatabaseModel
        {
            if (_database is null)
                throw new DatabaseNotInitializedException();

            var queryFilter = Builders<T>.Filter;
            var collection = GetCollectionForModel<T>();
            var documents = await collection.FindAsync(queryFilter.In("_id", objectIds));
            return documents.ToEnumerable();
        }

        public static async Task<T> FindOne<T>(Expression<Func<T, bool>> predicate) where T : DatabaseModel
        {
            var query = await GetCollectionForModel<T>().FindAsync<T>(predicate);
            return await query.FirstOrDefaultAsync();
        }

        public static async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> predicate) where T : DatabaseModel
        {
            var query = await GetCollectionForModel<T>().FindAsync<T>(predicate);
            return query.ToEnumerable();
        }

        public static async Task UpsertOne<T>(T document) where T : DatabaseModel
        {
            if (_database is null)
                throw new DatabaseNotInitializedException();

            var collection = GetCollectionForModel<T>();
            await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", document.Id), document, new UpdateOptions { IsUpsert = true });

        }

        public static async Task DeleteOne<T>(ObjectId id) where T : DatabaseModel
        {
            if (_database is null)
                throw new DatabaseNotInitializedException();

            var collection = GetCollectionForModel<T>();
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }

        public static async Task DeleteOne<T>(T document) where T : DatabaseModel
        {
            await DeleteOne<T>(document.Id);
        }

        public static async Task DeleteOne<T>(String id) where T : DatabaseModel
        {
            if (ObjectId.TryParse(id, out var objectId))
            {
                await DeleteOne<T>(objectId);
            }
        }

    }
}
