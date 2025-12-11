using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System.Net;

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
        // Internal Helper
        // --------------------------------------------------------------------
        private async Task<List<T>> ExecuteQueryAsync<T>(FeedIterator<T> iterator)
        {
            List<T> results = new List<T>();

            while (iterator.HasMoreResults)
            {
                foreach (T item in await iterator.ReadNextAsync())
                    results.Add(item);
            }
            return results;
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
            FeedIterator<DatabaseProperties> dbIterator = cosmosClient
                        .GetDatabaseQueryIterator<DatabaseProperties>();
            List<DatabaseProperties> props = await ExecuteQueryAsync(dbIterator);
            return props.Select(p => p.Id).ToList();
        }
        // --------------------------------------------------------------------
        // Count all DBs
        // --------------------------------------------------------------------
        public async Task<int> CountDatabasesAsync()
        {
            return (await GetDatabasesAsync()).Count;
        }
        // --------------------------------------------------------------------
        // Get all tables in all DBs
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesAsync()
        {
            List<string> allTables = [];
            List<string> allDbs = await GetDatabasesAsync();
            foreach (string db in allDbs)
            {
                List<string> tablesInThisDB = await GetTablesAsync(db);
                foreach (string table in tablesInThisDB)
                    allTables.Add($"{db} - {table}");
            }
            return allTables;
        }
        // --------------------------------------------------------------------
        // Overload: tables in a specific DB
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesAsync(string dbName)
        {
            try
            {
                Database dbObj = cosmosClient.GetDatabase(dbName);
                FeedIterator<ContainerProperties> tableIterator = dbObj.GetContainerQueryIterator<ContainerProperties>();
                List<ContainerProperties> props = await ExecuteQueryAsync(tableIterator);
                return props.Select(c => c.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error accessing database '{dbName}': {ex.Message}", ex);
            }
        }
        // --------------------------------------------------------------------
        // DBs starting with prefix
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsStartingWithAsync(string prefix)
        {
            return (await GetDatabasesAsync())
            .Where(db => db.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();
        }
        // --------------------------------------------------------------------
        // Tables with length filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetTablesLongerThanAsync(int minLength)
        {
            List<string> DBsContainingTable = [];
            List<string> allDbs = await GetDatabasesAsync();
            foreach (string db in allDbs)
            {
                List<string> tablesInThisDB = await GetTablesAsync(db);
                foreach (string table in tablesInThisDB)
                    if (table.Length > minLength)
                        DBsContainingTable.Add($"{db} - {table}");
            }
            return DBsContainingTable;
        }
        // --------------------------------------------------------------------
        // DBs containing a specific table
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsContainingTableAsync(string tableName)
        {
            List<string> DBsContainingTable = [];
            List<string> allDbs = await GetDatabasesAsync();
            foreach (string db in allDbs)
            {
                List<string> tablesInThisDB = await GetTablesAsync(db);
                if (tablesInThisDB.Contains(tableName))
                    DBsContainingTable.Add($"{db}");
            }
            return DBsContainingTable;
        }
        // --------------------------------------------------------------------
        // Count tables in each DB
        // --------------------------------------------------------------------
        public async Task<List<string>> CountTablesPerDBAsync()
        {
            List<string> dbTableCounts = [];
            List<string> allDbs = await GetDatabasesAsync();
            foreach (string db in allDbs)
            {
                try
                {
                    List<string> tablesInThisDB = await GetTablesAsync(db);
                    dbTableCounts.Add($"{db} - Tables: {tablesInThisDB.Count}");
                }
                catch (Exception)
                {
                    dbTableCounts.Add($"{db} - Error: Could not retrieve tables.");
                }
            }
            return dbTableCounts;
        }
        // --------------------------------------------------------------------
        // Count tables in a DB
        // --------------------------------------------------------------------
        public async Task<int> CountTablesInDBAsync(string dbName)
        {
            var tables = await GetTablesAsync(dbName);
            return tables.Count;
        }
        public async Task<int> CountAllTablesAsync()
        {
            int totCount = 0;
            List<string> allDbs = await GetDatabasesAsync();
            foreach (string db in allDbs)
                totCount += await CountTablesInDBAsync(db);
            return totCount;
        }
        public async Task<int> CountDBsContainingTableAsync()
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
        public async Task<bool> ItemExistsAsync(string dbName, string containerName, string id)
        {
            var item = await GetItemFromCosmosAsync(dbName, containerName, id);
            return item != null;
        }
        // --------------------------------------------------------------------
        // Exact table count filter
        // --------------------------------------------------------------------
        public async Task<List<string>> GetDBsWithExactTableCountAsync(int num)
        {
            List<string> result = [];
            FeedIterator<DatabaseProperties> dbIterator =
                cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>();
            List<DatabaseProperties> dbProps = await ExecuteQueryAsync(dbIterator);
            foreach (DatabaseProperties currentDBprop in dbProps)
            {
                int tableCount = await CountTablesInDBAsync(currentDBprop.Id);
                if (tableCount == num)
                {
                    result.Add(currentDBprop.Id);
                }
            }
            return result;
        }
        public async Task<string> GetDbsWithTablesStartingWithAsync(string userStr)
        {
            List<string> listOfDbsFound = [];
            List<string> allDbsStartWith = await GetDBsStartingWithAsync(userStr);
            foreach (string currentDbName in allDbsStartWith)
            {
                List<string> tables = await GetTablesAsync(currentDbName);
                if (tables.Any(t => t.StartsWith(userStr, StringComparison.OrdinalIgnoreCase)))
                {
                    listOfDbsFound.Add(currentDbName);
                }
            }
            return string.Join(", ", listOfDbsFound);
        }
        public async Task<string> GetLongestDbNameStartingWith(string userStr)
        {
            List<string> allDbsStartWith = await GetDBsStartingWithAsync(userStr);
            List<string> filteredDbs = [];
            foreach (string dbName in allDbsStartWith)
            {
                List<string> tables = await GetTablesAsync(dbName);
                if (tables.Any(t => t.StartsWith(userStr, StringComparison.OrdinalIgnoreCase)))
                {
                    filteredDbs.Add(dbName);
                }
            }
            if (filteredDbs.Count == 0)
                return string.Empty;
            int maxLen = filteredDbs.Max(db => db.Length);
            List<string> longestNames = filteredDbs
                .Where(db => db.Length == maxLen)
                .ToList();
            return string.Join(", ", longestNames);
        }
        public async Task<string> GetDbsWithMostTablesAsync()
        {
            List<string> allDbs = await GetDatabasesAsync();

            if (allDbs.Count == 0)
                return "No database exists in the current cloud account";
            List<string> resultDbs = new List<string>();
            int maxCount = 0;
            bool hasTables = false;
            foreach (string db in allDbs)
            {
                int count = await CountTablesInDBAsync(db);
                if (count > 0) hasTables = true;
                if (count > maxCount)
                {
                    maxCount = count;
                    resultDbs.Clear();
                    resultDbs.Add(db);
                }
                else if (count == maxCount && count > 0) resultDbs.Add(db);
            }
            if (!hasTables)
                return "There are no tables in any of the databases.";

            return string.Join(", ", resultDbs);
        }
        public async Task<JObject?> GetItemFromCosmosAsync(string dbName,
                                                string containerName, string id)
        {
            // Get item by id
            Database db = cosmosClient.GetDatabase(dbName);
            Container container = db.GetContainer(containerName);
            try
            {
                var response = await container.ReadItemAsync<JObject>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        public async Task ReplaceItemInCosmosAsync(string dbName, string containerName, string id, JObject item)
        {
            // Replace item
            Database db = cosmosClient.GetDatabase(dbName);
            Container container = db.GetContainer(containerName);
            await container.ReplaceItemAsync(item, id, new PartitionKey(id));
        }
        public async Task SaveItemToCosmosAsync(string dbName, string containerName, StudentInfo item)
        {
            Database db = cosmosClient.GetDatabase(dbName);
            Container container = await db.CreateContainerIfNotExistsAsync(containerName, "/id");

            await container.CreateItemAsync(item, new PartitionKey(item.id));
        }
        public async Task<bool> DeleteItemFromCosmosAsync(string dbName, string containerName, string id)
        {
            if (string.IsNullOrEmpty(id))
                return false; // indicate failure

            Database db = cosmosClient.GetDatabase(dbName);
            Container container = db.GetContainer(containerName);

            try
            {
                await container.DeleteItemAsync<JObject>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }
        public async Task SaveJsonItemToCosmosAsync(string dbName, string containerName, JObject item)
        {
            Database db = cosmosClient.GetDatabase(dbName);
            Container dbTable = await db.CreateContainerIfNotExistsAsync(containerName, "/id");
            string? id = item["id"]?.ToString();
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
                item["id"] = id;
            }
            await dbTable.CreateItemAsync(item, new PartitionKey(id));
        }
        public async Task<bool> TableExistsAsync(string dbName, string tableName)
        {
            try
            {
                Database db = cosmosClient.GetDatabase(dbName);
                Container container = db.GetContainer(tableName);
                await container.ReadContainerAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<string>> GetComplexFilteredDatabasesAsync()
        {
            List<string> dbList = await GetDatabasesAsync();
            List<string> results = new List<string>();
            foreach (string db in dbList)
            {
                int tableCount = await CountTablesInDBAsync(db);
                if ((db.Length % 2 != 0) && (tableCount >= 3 || tableCount == 0))
                {
                    results.Add($"{db} - Tables : {tableCount}");
                }
            }
            return results;
        }
        public async Task<Container?> GetTableAsync(string table)
        {
            List<string> allTables = await GetTablesAsync();
            string? foundTable = allTables.FirstOrDefault(t => t.EndsWith($" - {table}"
                                                , StringComparison.OrdinalIgnoreCase));
            if (foundTable == null) return null;
            string[] parts = foundTable.Split([" - "], StringSplitOptions.None);
            string dbName = parts[0];
            return cosmosClient.GetDatabase(dbName).GetContainer(table);
        }
        public async Task<List<string>> GetItemsInTableAsync(string table)
        {
            Container? tableContainer = await GetTableAsync(table);
            if (tableContainer == null) return [];
            FeedIterator<JObject> itemIterator = tableContainer.GetItemQueryIterator<JObject>();
            List<JObject> itemList = await ExecuteQueryAsync(itemIterator);
            return itemList.Select(item => item["id"]?.ToString() ?? "Unknown").ToList();
        }
        public async Task<int> CountItemsInTableAsync(string table)
        {
            return (await GetItemsInTableAsync(table)).Count;
        }
        public async Task<int> CountAllItemsAsync()
        {
            int totCount = 0;
            List<string> allTables = await GetTablesAsync();
            foreach (string table in allTables)
            {
                string[] parts = table.Split([" - "], StringSplitOptions.None);
                string tableName = parts[1];
                totCount += await CountItemsInTableAsync(tableName);
            }
            return totCount;
        }
    }
}
