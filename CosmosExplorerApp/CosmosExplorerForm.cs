namespace CosmosExplorerApp;

using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using cloudApp;
using Newtonsoft.Json.Linq;

public partial class CosmosExplorerForm : Form
{
    private CosmosHelper helper;
    private CosmosClient? _client;
    public CosmosExplorerForm()
    {
        InitializeComponent();
    }
    private bool validateHelper()
    {
        // Validate helper
        if (helper == null)
        {
            MessageBox.Show("Client not initialized. Load keys first!",
                        "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            return false;
        }
        return true;
    }
    // ----------------------------------------------------------
    // Refresh DB list
    // ----------------------------------------------------------
    private async Task RefreshDatabasesAsync()
    {
        try
        {
            if (helper == null)
            {
                string endpoint = endpointTxtBox.Text.Trim();
                string key = pkeyTxtBox.Text.Trim();

                if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
                {
                    MessageBox.Show("Endpoint or PKey is empty!",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // Create helper once
                helper = new CosmosHelper(endpoint, key);
                _client = helper.GetClient();
            }

            // Load DB list
            listDb.Items.Clear();
            var dbs = await helper.GetDatabasesAsync();

            foreach (var db in dbs)
                listDb.Items.Add(db);

            // Update form title with connection speed
            double speedMs = await MeasureConnectionSpeedAsync();
            if (speedMs >= 0)
            {
                this.Text = $"Cosmos DB Explorer - Connected (Latency: {speedMs:F2} ms)";
            }
            else
            {
                this.Text = "Cosmos DB Explorer - Connected (Latency: N/A)";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error refreshing databases: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
    private async Task<double> MeasureConnectionSpeedAsync()
    {
        if (helper == null) return -1;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Lightweight call to Cosmos DB
            await helper.GetDatabasesAsync();
        }
        catch
        {
            return -1; // failed to connect
        }
        sw.Stop();
        return sw.Elapsed.TotalMilliseconds; // return milliseconds
    }
    // ----------------------------------------------------------
    // Load keys from App.config
    // ----------------------------------------------------------
    private async void BtnLoadKeys_Click(object sender, EventArgs e)
    {
        endpointTxtBox.Text = ConfigurationManager.AppSettings["EndPointUri"];
        pkeyTxtBox.Text = ConfigurationManager.AppSettings["PrimaryKey"];

        // Initialize helper
        helper = new CosmosHelper(
            endpointTxtBox.Text.Trim(),
            pkeyTxtBox.Text.Trim()
        );

        validateHelper();

        _client = helper.GetClient();

        await RefreshDatabasesAsync();
        await LoadDatabasesIntoComboBox();
    }
    // ----------------------------------------------------------
    // Create Database
    // ----------------------------------------------------------
    private async void BtnCreateDb_Click(object sender, EventArgs e)
    {
        string endpoint = endpointTxtBox.Text.Trim();
        string key = pkeyTxtBox.Text.Trim();
        string dbName = dbNameTxtBox.Text.Trim();
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
        {
            MessageBox.Show("Endpoint or PKey is empty!",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
            return;
        }
        if (!validateHelper())
            return;

        (bool created, string status) result = await helper.CreateDatabaseAsync(dbName);
        MessageBox.Show(
            $"Database created? {result.created} (Status: {result.status})",
            "DB Creation",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        await RefreshDatabasesAsync();
        await LoadDatabasesIntoComboBox();
    }
    private async void BtnCountDatabases_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper())
                return;
            int count = await helper.CountDatabasesAsync();
            MessageBox.Show($"Total databases: {count}", "Database Count",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error counting databases: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async Task<int> CountTablesPerDB(object sender, EventArgs e)
    {
        if (!validateHelper())
            return 0;
        List<string> dbs = await helper.GetDatabasesAsync();
        int countTables = 0;
        foreach (string db in dbs)
        {
            int countTable = await helper.CountTablesInDBAsync(db);
            countTables += countTable;
        }
        return countTables;
    }
    private async void BtnFilterDb_Click(object sender, EventArgs e)
    {
        if (!validateHelper())
            return;
        string prefix = txtDbPrefix.Text.Trim();
        if (string.IsNullOrEmpty(prefix))
        {
            MessageBox.Show("Please enter a prefix.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        List<string> filteredDbs = [];
        cmbFilteredDbs.Items.Clear();
        // checkbox logic
        if (chkMatchTables.Checked && chkLongestNames.Checked)
        {
            MessageBox.Show("Please select only one option at a time.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (chkMatchTables.Checked)
        {
            string output = await helper.GetDbsWithTablesStartingWithAsync(prefix);
            filteredDbs = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries).ToList();
            if (filteredDbs.Count == 0)
            {
                MessageBox.Show("No databases found with matching tables.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        else if (chkLongestNames.Checked)
        {
            string output = await helper.GetLongestDbNameStartingWith(prefix);
            filteredDbs = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries).ToList();
            if (filteredDbs.Count == 0)
            {
                MessageBox.Show("No databases found with that prefix.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        else
        // default behavior
        {
            filteredDbs = await helper.GetDBsStartingWithAsync(prefix);
        }
        foreach (string db in filteredDbs)
            cmbFilteredDbs.Items.Add(db);
        // Auto-select first item
        if (cmbFilteredDbs.Items.Count > 0)
            cmbFilteredDbs.SelectedIndex = 0;
        else
            MessageBox.Show("No databases found with that prefix.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private async void BtnRefreshTables_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper())
                return;
            string? selectedDb = comboDbTables.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedDb))
            {
                MessageBox.Show("No database selected!",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            listTables.Items.Clear();
            List<string> tables = await helper.GetTablesAsync(selectedDb);
            foreach (string table in tables) listTables.Items.Add(table);
            if (listTables.Items.Count > 0)
                listTables.SelectedIndex = 0;

            lblTableCount.Text = $"Tables/Containers: {tables.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error refreshing tables/containers: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
    private async void BtnCountAllTables_Click(object sender, EventArgs e)
    {
        if (!validateHelper())
            return;
        try
        {
            btnCountAllTables.Enabled = false;
            btnCountAllTables.Text = "Counting...";
            int tableCount = await helper.CountAllTablesAsync();
            lblTotalTables.Text = $"Total Tables/Containers in all DBs: {tableCount}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error counting tables: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCountAllTables.Enabled = true;
            btnCountAllTables.Text = "Count All Tables in All DBs";
        }
    }
    private async void BtnCreateContainer_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper())
                return;
            string? selectedDb = comboDbTables.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedDb))
            {
                MessageBox.Show("Please select a database first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string tableName = txtContainerName.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Container name is required.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            (bool created, string status) = await helper.CreateTableAsync(selectedDb, tableName);
            if (created)
                MessageBox.Show($"Table created: {tableName}", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show($"Table existed already. Status: {status}", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtContainerName.Clear();
            BtnRefreshTables_Click(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating table: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnListDbsWithTableCount_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper())
                return;
            listDb.Items.Clear();
            List<string> dbs = await helper.GetDatabasesAsync();
            foreach (string db in dbs)
            {
                int tableCount = await helper.CountTablesInDBAsync(db);
                listDb.Items.Add($"{db} - Tables: {tableCount}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error listing DBs with table counts: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnCheckDbExists_Click(object sender, EventArgs e)
    {
        if (!validateHelper())
            return;
        string dbName = txtCheckDb.Text.Trim();
        if (string.IsNullOrEmpty(dbName))
        {
            MessageBox.Show("Please enter a database name to check.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        bool dbExists = await helper.DatabaseExistsAsync(dbName);
        if (dbExists)
            lblCheckDbResult.Text = $"Database '{dbName}' exists.";
        else
            MessageBox.Show($"Database '{dbName}' does not exist.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private async void BtnCheckTable_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper()) return;
            string tableName = txtCheckTable.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Please enter a table name to check.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lstCheckTableResult.Items.Clear();
            List<string> DbsWithTableName = await helper.GetDBsContainingTableAsync(tableName);
            if (DbsWithTableName.Count > 0)
            {
                lstCheckTableResult.Items.Add("Table found in DB: ");
                foreach (string db in DbsWithTableName)
                {
                    lstCheckTableResult.Items.Add(db);
                }
            }
            else
            {
                MessageBox.Show($"Table '{tableName}' not found in any database.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error checking table existence: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnApplyDbCondition_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper()) return;
            cmbConditionResults.Items.Clear();
            if (listDb.Items.Count == 0)
            {
                MessageBox.Show("No databases available to apply condition.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (var dbItem in listDb.Items)
            {
                string dbName = dbItem.ToString() ?? string.Empty;
                int tableCount = await helper.CountTablesInDBAsync(dbName);

                if (!(dbName.Length % 2 == 0) && (tableCount >= 3 || tableCount == 0))
                {
                    cmbConditionResults.Items.Add($"{dbName} - Tables: {tableCount}");
                }
            }
            if (cmbConditionResults.Items.Count > 0)
                cmbConditionResults.SelectedIndex = 0;
            else
                cmbConditionResults.Items.Add("No databases matched the condition.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error applying DB condition: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnExactTableCount_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper()) return;
            cmbExactTableCountResult.Items.Clear();
            if (listDb.Items.Count == 0)
            {
                MessageBox.Show("No databases available to check table counts.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtExactTableCount.Text.Trim(), out int userTableCount))
            {
                MessageBox.Show("Please enter a valid integer for table count.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<string> matchingDbs = [];
            foreach (var dbItem in listDb.Items)
            {
                string? dbName = dbItem.ToString() ?? string.Empty;
                int tableCount = await helper.CountTablesInDBAsync(dbName);

                if (tableCount == userTableCount)
                {
                    matchingDbs.Add(dbName);
                }
            }
            string resultString = matchingDbs.Count > 0
                ? string.Join(", ", matchingDbs)
                : "No databases found with the exact table count.";
            cmbExactTableCountResult.Items.Add(resultString);
            cmbExactTableCountResult.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error finding DBs with exact table count: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnMinTableName_Click(object sender, EventArgs e)
    {
        try
        {
            if (!validateHelper()) return;
            cmbMinLegthTables.Items.Clear();
            int minLength = int.Parse(txtMinTableLengthInput.Text.Trim());
            // flag for displaying DB names only once
            bool returnDbNamesOnlyOnce = chkDbNameOnly.Checked;
            List<string> dbs = await helper.GetDatabasesAsync();
            foreach (string db in dbs)
            {
                List<string> dbTables = await helper.GetTablesAsync(db);
                foreach (string table in dbTables)
                {
                    if (table.Length > minLength)
                    {
                        if (returnDbNamesOnlyOnce)
                        {
                            {
                                cmbMinLegthTables.Items.Add(db);
                                break;
                            }
                        }
                        else
                            cmbMinLegthTables.Items.Add($"{db} : {table}");
                    }
                }
            }
            if (cmbMinLegthTables.Items.Count > 0)
            {
                cmbMinLegthTables.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No tables found with name length greater than specified minimum.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error finding tables with minimum name length: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnMostTables_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        cmbFilteredDbs.Items.Clear();

        try
        {
            string DbsWithMostTables = await helper.GetDbsWithMostTablesAsync();
            string resultMsg;
            if (DbsWithMostTables == "No database exists in the current cloud account"
                || DbsWithMostTables == "There are no tables in any of the databases")
                cmbFilteredDbs.Items.Add(DbsWithMostTables);
            else
            {
                List<string> dbList = DbsWithMostTables
                    .Split([", "], StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                // check count for first item because all items have same table count
                int tableCount = await helper.CountTablesInDBAsync(dbList[0]);
                resultMsg = $"The databases have {tableCount} TBs:";
                cmbFilteredDbs.Items.Add(resultMsg);
                foreach (string db in dbList)
                    cmbFilteredDbs.Items.Add(db);
            }
            if (cmbFilteredDbs.Items.Count > 0)
                cmbFilteredDbs.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error finding DB(s) with most tables: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async void BtnSaveClient_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string dbName = txtClientDbName.Text.Trim();
        string tableName = txtClientTableName.Text.Trim();
        if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName))
        {
            MessageBox.Show("Please enter both database and table names.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        StudentInfo student = new StudentInfo
        {
            id = Guid.NewGuid().ToString(),
            idNum = txtClientTz.Text.Trim(),
            firstName = txtClientFirstName.Text.Trim(),
            lastName = txtClientLastName.Text.Trim(),
            Courses = []
        };

        try
        {
            await helper.SaveItemToCosmosAsync(dbName, tableName, student);
            MessageBox.Show("Student info saved successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtClientTz.Clear();
            txtClientFirstName.Clear();
            txtClientLastName.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving to Cosmos DB: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async Task LoadDatabasesIntoComboBox()
    {
        if (!validateHelper())
            return;
        List<string> dbs = await helper.GetDatabasesAsync();
        comboDbTables.Items.Clear();
        foreach (string db in dbs)
            comboDbTables.Items.Add(db);

        if (comboDbTables.Items.Count > 0)
        {
            comboDbTables.SelectedIndex = 0;   // select first DB safely

            string? selectedDb = comboDbTables.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDb))
            {
                await RefreshTablesList(selectedDb);
            }
        }
    }
    private async void ComboDbTables_SelectedIndexChanged(object sender, EventArgs e)
    {
        string? selectedDb = comboDbTables.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedDb))
            return;
        await RefreshTablesList(selectedDb);
    }
    private async Task RefreshTablesList(string dbName)
    {
        if (helper == null || string.IsNullOrEmpty(dbName))
            return;
        List<string> tables = await helper.GetTablesAsync(dbName);
        listTables.Items.Clear();
        foreach (string table in tables)
            listTables.Items.Add(table);
        if (listTables.Items.Count > 0)
            listTables.SelectedIndex = 0;
        await UpdateTableCountAsync(dbName);
    }
    private async Task UpdateTableCountAsync(string dbName)
    {
        int count = await helper.CountTablesInDBAsync(dbName);
        lblTableCount.Text = $"Tables / Containers: {count}";
    }
    private async Task ValidateTextboxAsync(
            TextBox textbox,
                Label indicatorLabel,
                    Func<string, Task<bool>> validateFunc)
    {
        string value = textbox.Text.Trim();

        if (string.IsNullOrEmpty(value))
        {
            indicatorLabel.Text = "";
            return;
        }

        bool exists = await validateFunc(value);

        indicatorLabel.Text = exists ? "✔" : "✖";
        indicatorLabel.ForeColor = exists ? Color.Green : Color.Red;
    }
    private async void TxtClientDbName_TextChanged(object sender, EventArgs e)
    {
        await ValidateTextboxAsync(
            txtClientDbName,
                lblDbCheck,
                    helper.DatabaseExistsAsync);
    }
    private async void TxtClientTableName_TextChanged(object sender, EventArgs e)
    {
        string db = txtClientDbName.Text.Trim();
        await ValidateTextboxAsync(txtClientTableName, lblTbCheck,
                table => helper.TableExistsAsync(db, table));
    }
    private async void TxtClientId_TextChanged(object sender, EventArgs e)
    {
        string dbName = txtClientDbName.Text.Trim();
        string tableName = txtClientTableName.Text.Trim();
        string id = txtClientId.Text.Trim();

        if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(id))
        {
            lblIdCheck.Text = "";
            return;
        }

        bool exists = await helper.ItemExistsAsync(dbName, tableName, id);
        lblIdCheck.Text = exists ? "✔" : "✖";
        lblIdCheck.ForeColor = exists ? Color.Green : Color.Red;
    }
    //=======================================================
    //                    JSON Utils
    //=======================================================
    private void BtnLoadJsonFile_Click(object sender, EventArgs e)
    {
        using OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "JSON files|*.json";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            string json = File.ReadAllText(ofd.FileName);
            txtJsonContent.Text = json;
        }
    }
    private async void BtnInsertToCloud_Click(object sender, EventArgs e)
    {
        string dbName = txtClientDbName.Text.Trim();
        string tableName = txtClientTableName.Text.Trim();
        string json = txtJsonContent.Text;
        if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(json))
        {
            lblJsonStatus.Text = "Please fill DB, Table and JSON content.";
            return;
        }
        try
        {
            var item = JObject.Parse(json);
            string? id = item["id"]?.ToString();
            if (string.IsNullOrEmpty(id))
            {
                lblJsonStatus.Text = "JSON must include an 'id' field!";
                lblJsonStatus.ForeColor = Color.Red;
                return;
            }
            var existing = await helper.GetItemFromCosmosAsync(dbName, tableName, id);
            if (existing != null)
            {
                lblJsonStatus.Text = $"Item '{item["name"]}' with id '{id}' already exists!";
                lblJsonStatus.ForeColor = Color.Orange;
                return;
            }
            await helper.SaveJsonItemToCosmosAsync(dbName, tableName, item);
            lblJsonStatus.Text = "Inserted successfully!";
            lblJsonStatus.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            lblJsonStatus.Text = $"Error: {ex.Message}";
            lblJsonStatus.ForeColor = Color.Red;
        }
    }
    private async void BtnUpdateCloud_Click(object sender, EventArgs e)
    {
        try
        {
            string dbName = txtClientDbName.Text.Trim();
            string containerName = txtClientTableName.Text.Trim();
            string id = txtClientId.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                lblJsonStatus.ForeColor = Color.Red;
                lblJsonStatus.Text = "Cannot update: ID field is empty!";
                return;
            }
            var json = JObject.Parse(txtJsonContent.Text);
            json["id"] = id; // force correct ID
            var existing = await helper.GetItemFromCosmosAsync(dbName, containerName, id);
            if (existing == null)
            {
                lblJsonStatus.ForeColor = Color.Orange;
                lblJsonStatus.Text = $"Item with id '{id}' does not exist!";
                return;
            }
            await helper.ReplaceItemInCosmosAsync(dbName, containerName, id, json);
            lblJsonStatus.ForeColor = Color.Green;
            lblJsonStatus.Text = "Updated successfully!";
        }
        catch (Exception ex)
        {
            lblJsonStatus.ForeColor = Color.Red;
            lblJsonStatus.Text = "Error: " + ex.Message;
        }
    }
    private async void BtnDeleteCloud_Click(object sender, EventArgs e)
    {
        try
        {
            string dbName = txtClientDbName.Text.Trim();
            string containerName = txtClientTableName.Text.Trim();
            string id = txtClientId.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                lblJsonStatus.ForeColor = Color.Red;
                lblJsonStatus.Text = "Cannot delete: ID field is empty!";
                return;
            }
            var existing = await helper.GetItemFromCosmosAsync(dbName, containerName, id);
            if (existing == null)
            {
                lblJsonStatus.ForeColor = Color.Orange;
                lblJsonStatus.Text = $"Item with id '{id}' does not exist!";
                return;
            }
            await helper.DeleteItemFromCosmosAsync(dbName, containerName, id);
            lblJsonStatus.ForeColor = Color.Green;
            lblJsonStatus.Text = "Deleted successfully!";
        }
        catch (Exception ex)
        {
            lblJsonStatus.ForeColor = Color.Red;
            lblJsonStatus.Text = "Error: " + ex.Message;
        }
    }
    private async void BtnReadCloud_Click(object sender, EventArgs e)
    {
        try
        {
            string dbName = txtClientDbName.Text.Trim();
            string containerName = txtClientTableName.Text.Trim();
            string id = txtClientId.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                lblJsonStatus.ForeColor = Color.Red;
                lblJsonStatus.Text = "Cannot read: ID field is empty!";
                return;
            }

            var existing = await helper.GetItemFromCosmosAsync(dbName, containerName, id);
            if (existing == null)
            {
                lblJsonStatus.ForeColor = Color.Orange;
                lblJsonStatus.Text = $"Item with id '{id}' does not exist!";
                return;
            }

            txtJsonContent.Text = existing.ToString();
            lblJsonStatus.ForeColor = Color.Green;
            lblJsonStatus.Text = "Read successfully!";
        }
        catch (Exception ex)
        {
            lblJsonStatus.ForeColor = Color.Red;
            lblJsonStatus.Text = "Error: " + ex.Message;
        }
    }
}
