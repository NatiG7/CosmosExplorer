using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudApp
{
    public class CosmosHelper
    {
        private CosmosClient cosmosClient;
        public CosmosHelper(string uri, string key)
        {
            cosmosClient = new CosmosClient(uri, key);
        }
        public CosmosClient GetClient()

        {
            return cosmosClient;
        }
        // --------------------------------------------------------------------
        // DB + Container Creation
        // --------------------------------------------------------------------
        public async Task<(bool created, string status)> CreateDatabaseAsync(string db)
        {
            DatabaseResponse dbResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(db);
            return (dbResponse.StatusCode == System.Net.HttpStatusCode.Created,
                    dbResponse.StatusCode.ToString());
        }
        public async Task<(bool created, string status)> CreateTableAsync(string db, string table)
        {
            Database dbObj = cosmosClient.GetDatabase(db);
            ContainerResponse cntrResponse =
                await dbObj.CreateContainerIfNotExistsAsync(table, "/id");

            return (cntrResponse.StatusCode == System.Net.HttpStatusCode.Created,
                    cntrResponse.StatusCode.ToString());
        }
        // --------------------------------------------------------------------
        // Get all DBs
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDatabasesAsync()
        {
            List<string> dbNames = new List<string>();

            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();

            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                    dbNames.Add(currentDBprop.Id);
            }

            return dbNames;
        }
        // --------------------------------------------------------------------
        // Count all DBs
        // --------------------------------------------------------------------
        public async Task<int> CountDatabasesAsync()
        {
            int numOfDbs = 0;
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                    numOfDbs++;
            }
            return numOfDbs;
        }
        // --------------------------------------------------------------------
        // Get all tables in all DBs
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesAsync()
        {
            List<string> tables = new List<string>();
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = cosmosClient.GetDatabase(currentDBprop.Id);
                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();
                    while (tableIterator.HasMoreResults)
                    {
                        foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                            tables.Add($"{currentDBprop.Id} - {currentTBprop.Id}");
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
            Database dbObj = cosmosClient.GetDatabase(dbName);
            FeedIterator<ContainerProperties> tableIterator = dbObj.GetContainerQueryIterator<ContainerProperties>();
            while (tableIterator.HasMoreResults)
            {
                foreach (ContainerProperties currentTable in await tableIterator.ReadNextAsync())
                    tables.Add(currentTable.Id);
            }
            return tables;
        }
        // --------------------------------------------------------------------
        // DBs starting with prefix
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsStartingWithAsync(string prefix)
        {
            List<string> filteredDbs = new List<string>();
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    if (currentDBprop.Id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                        filteredDbs.Add(currentDBprop.Id);
                }
            }
            return filteredDbs;
        }
        // --------------------------------------------------------------------
        // Tables with length filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesLongerThanAsync(int minLength)
        {
            List<string> result = new List<string>();
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = cosmosClient.GetDatabase(currentDBprop.Id);
                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();
                    while (tableIterator.HasMoreResults)
                    {
                        foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                        {
                            if (currentTBprop.Id.Length > minLength)
                                result.Add($"{currentDBprop.Id}-{currentTBprop.Id}");
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
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    Database dbObj = cosmosClient.GetDatabase(currentDBprop.Id);
                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();
                    bool found = false;
                    while (tableIterator.HasMoreResults && !found)
                    {
                        foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                        {
                            if (currentTBprop.Id == tableName)
                            {
                                result.Add(currentDBprop.Id);
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
            List<string> result = [];
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    int numOfTBs = 0;
                    Database dbObj = cosmosClient.GetDatabase(currentDBprop.Id);
                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();
                    while (tableIterator.HasMoreResults)
                    {
                        foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                            numOfTBs++;
                    }
                    result.Add($"{currentDBprop.Id} - {numOfTBs} tables");
                }
            }
            return result;
        }
        // --------------------------------------------------------------------
        // Count tables in a DB
        // --------------------------------------------------------------------
        public async Task<int> CountTablesInDBAsync(string dbName)
        {
            int numOfTBs = 0;
            Database dbObj = cosmosClient.GetDatabase(dbName);
            FeedIterator<ContainerProperties> tableIterator =
                dbObj.GetContainerQueryIterator<ContainerProperties>();
            while (tableIterator.HasMoreResults)
            {
                foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                    numOfTBs++;
            }
            return numOfTBs;
        }
        public async Task<int> CountAllTablesAsync()
        {
            int tableCount = 0;

            List<string> DBs = await GetDatabasesAsync();
            foreach (string dbName in DBs)
            {
                int countInDb = await CountTablesInDBAsync(dbName);
                tableCount += countInDb;
            }
            return tableCount;
        }
        public async Task<bool> DatabaseExistsAsync(string dbName)
        {
            if (string.IsNullOrWhiteSpace(dbName))
                return false;

            List<string> allDbs = await GetDatabasesAsync();

            return allDbs.Contains(dbName, StringComparer.OrdinalIgnoreCase);
        }
        // --------------------------------------------------------------------
        // Exact table count filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsWithExactTableCountAsync(int num)
        {
            List<string> result = new List<string>();
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            while (dbIterator.HasMoreResults)
            {
                foreach (DatabaseProperties currentDBprop in await dbIterator.ReadNextAsync())
                {
                    int count = 0;
                    Database dbObj = cosmosClient.GetDatabase(currentDBprop.Id);

                    FeedIterator<ContainerProperties> tableIterator =
                        dbObj.GetContainerQueryIterator<ContainerProperties>();

                    while (tableIterator.HasMoreResults)
                    {
                        foreach (ContainerProperties currentTBprop in await tableIterator.ReadNextAsync())
                            count++;
                    }
                    if (count == num)
                        result.Add(currentDBprop.Id);
                }
            }
            return result;
        }
    }
}
