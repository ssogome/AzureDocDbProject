using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Configuration;



namespace AzureDocDbProject
{
    using Models;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System.Configuration;


    public static class DocumentDBRepository
    {
        private static string databaseId;
        private static string collectionId;
        private static Database database;
        private static DocumentCollection collection;
        private static DocumentClient client;

        private static string DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = ConfigurationManager.AppSettings["database"];
                }

                return databaseId;
            }
        }

        private static string CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return collectionId;
            }
        }

        private static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        private static DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        private static DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                                    .Where(c => c.Id == CollectionId)
                                    .AsEnumerable()
                                    .FirstOrDefault();

            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }

            return col;
        }

        private static Database ReadOrCreateDatabase()
        {
            var db = Client.CreateDatabaseQuery()
                                 .Where(d => d.Id == DatabaseId)
                                 .AsEnumerable()
                                 .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        public static List<Item> GetIncompleteItems()
        {
            return Client.CreateDocumentQuery<Item>(Collection.DocumentsLink)
                       .Where(d => !d.Completed)
                       .AsEnumerable()
                       .ToList<Item>();
        }

    }
}