using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudApp
{
    public class CosmosHelper
    {
        private CosmosClient client;

        public CosmosHelper(string uri, string key)
        {
            client = new CosmosClient(uri, key);
        }

        public CosmosClient GetClient()
        {
            return client;
        }

        // --------------------------------------------------------------------
        // DB + Container Creation
        // --------------------------------------------------------------------
        public async Task<(bool created, string status)> CreateDatabaseAsync(string db)
        {
            DatabaseResponse response = await client.CreateDatabaseIfNotExistsAsync(db);
            return (response.StatusCode == System.Net.HttpStatusCode.Created,
                    response.StatusCode.ToString());
        }

        public async Task<(bool created, string status)> CreateTableAsync(string db, string table)
        {
            Database dbObj = client.GetDatabase(db);
            ContainerResponse response =
                await dbObj.CreateContainerIfNotExistsAsync(table, "/id");

            return (response.StatusCode == System.Net.HttpStatusCode.Created,
                    response.StatusCode.ToString());
        }

        // --------------------------------------------------------------------
        // Get all DBs
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDatabasesAsync()
        {
            List<string> dbNames = new List<string>();

            FeedIterator<DatabaseProperties> iterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (iterator.HasMoreResults)
            {
                foreach (var db in await iterator.ReadNextAsync())
                    dbNames.Add(db.Id);
            }

            return dbNames;
        }

        // --------------------------------------------------------------------
        // Count all DBs
        // --------------------------------------------------------------------
        public async Task<int> CountDatabasesAsync()
        {
            int count = 0;

            FeedIterator<DatabaseProperties> iterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (iterator.HasMoreResults)
            {
                foreach (var db in await iterator.ReadNextAsync())
                    count++;
            }

            return count;
        }

        // --------------------------------------------------------------------
        // Get all tables in all DBs
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesAsync()
        {
            List<string> tables = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (var db in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = client.GetDatabase(db.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    while (tableIterator.HasMoreResults)
                    {
                        foreach (var table in await tableIterator.ReadNextAsync())
                            tables.Add($"{db.Id} - {table.Id}");
                    }
                }
            }

            return tables;
        }

        // --------------------------------------------------------------------
        // Overload: tables in a specific DB
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesAsync(string dbName)
        {
            List<string> tables = new List<string>();
            Database dbObj = client.GetDatabase(dbName);

            FeedIterator<ContainerProperties> iterator = dbObj.GetContainerQueryIterator<ContainerProperties>();
            while (iterator.HasMoreResults)
            {
                foreach (var table in await iterator.ReadNextAsync())
                    tables.Add(table.Id);
            }

            return tables;
        }

        // --------------------------------------------------------------------
        // DBs starting with prefix
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsStartingWithAsync(string prefix)
        {
            List<string> filtered = new List<string>();

            FeedIterator<DatabaseProperties> iterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (iterator.HasMoreResults)
            {
                foreach (var db in await iterator.ReadNextAsync())
                {
                    if (db.Id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        filtered.Add(db.Id);
                }
            }

            return filtered;
        }

        // --------------------------------------------------------------------
        // Tables with length filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesLongerThanAsync(int minLength)
        {
            List<string> result = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (var db in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = client.GetDatabase(db.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    while (tableIterator.HasMoreResults)
                    {
                        foreach (var table in await tableIterator.ReadNextAsync())
                        {
                            if (table.Id.Length > minLength)
                                result.Add($"{db.Id}-{table.Id}");
                        }
                    }
                }
            }

            return result;
        }

        // --------------------------------------------------------------------
        // DBs containing a specific table
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsContainingTableAsync(string tableName)
        {
            List<string> result = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (var db in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = client.GetDatabase(db.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    bool found = false;

                    while (tableIterator.HasMoreResults && !found)
                    {
                        foreach (var table in await tableIterator.ReadNextAsync())
                        {
                            if (table.Id == tableName)
                            {
                                result.Add(db.Id);
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        // --------------------------------------------------------------------
        // Count tables in each DB
        // --------------------------------------------------------------------
        public async Task<List<string>> CountTablesPerDBAsync()
        {
            List<string> result = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (var db in await dbIterator.ReadNextAsync())
                {
                    int count = 0;
                    Database dbObj = client.GetDatabase(db.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    while (tableIterator.HasMoreResults)
                    {
                        foreach (var table in await tableIterator.ReadNextAsync())
                            count++;
                    }

                    result.Add($"{db.Id} - {count} tables");
                }
            }

            return result;
        }

        // --------------------------------------------------------------------
        // Exact table count filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsWithExactTableCountAsync(int num)
        {
            List<string> result = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                client.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (var db in await dbIterator.ReadNextAsync())
                {
                    int count = 0;
                    Database dbObj = client.GetDatabase(db.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    while (tableIterator.HasMoreResults)
                    {
                        foreach (var table in await tableIterator.ReadNextAsync())
                            count++;
                    }

                    if (count == num)
                        result.Add(db.Id);
                }
            }

            return result;
        }
    }
}
