﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MongoTest.Models
{
    public class ProductService
    {
        IGridFSBucket gridFS;   // файловое хранилище
        IMongoCollection<Product> Products; // коллекция в базе данных
        public ProductService()
        {
            // строка подключения
            string connectionString = "mongodb://localhost:27017/test";
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            // получаем доступ к файловому хранилищу
            gridFS = new GridFSBucket(database);
            // обращаемся к коллекции Products
            Products = database.GetCollection<Product>("Products");
        }
        // получаем все документы, используя критерии фальтрации
        public async Task<IEnumerable<Product>> GetProducts(int? minPrice, int? maxPrice, string name)
        {
            // строитель фильтров
            var builder = new FilterDefinitionBuilder<Product>();
            var filter = builder.Empty; // фильтр для выборки всех документов
            // фильтр по имени
            if (!String.IsNullOrWhiteSpace(name))
            {
                filter = filter & builder.Regex("Name", new BsonRegularExpression(name));
            }
            if (minPrice.HasValue)  // фильтр по минимальной цене
            {
                filter = filter & builder.Gte("Price", minPrice.Value);
            }
            if (maxPrice.HasValue)  // фильтр по максимальной цене
            {
                filter = filter & builder.Lte("Price", maxPrice.Value);
            }

            return await Products.Find(filter).ToListAsync();
        }

        // получаем один документ по id
        public async Task<Product> GetProduct(string id)
        {
            return await Products.Find(new BsonDocument("_id", new ObjectId(id))).FirstOrDefaultAsync();
        }
        // добавление документа
        public async Task Create(Product p)
        {
            await Products.InsertOneAsync(p);
        }
        // обновление документа
        public async Task Update(Product p)
        {
            await Products.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        // удаление документа
        public async Task Remove(string id)
        {
            await Products.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
