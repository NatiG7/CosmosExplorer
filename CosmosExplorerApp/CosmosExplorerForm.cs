namespace CosmosExplorerApp;

using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using cloudApp;
using System.Data.Common;

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

        if (helper == null)
        {
            helper = new CosmosHelper(endpoint, key);
            _client = helper.GetClient();
        }

        var result = await helper.CreateDatabaseAsync(dbName);
        MessageBox.Show(
            $"Database created? {result.created} (Status: {result.status})",
            "DB Creation",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        await RefreshDatabasesAsync();
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
        List<string> filteredDbs = await helper.GetDBsStartingWithAsync(prefix);
        cmbFilteredDbs.Items.Clear();
        foreach (string db in filteredDbs)
            cmbFilteredDbs.Items.Add(db);
        // ➜ Auto-select first item
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

            BtnRefreshTables_Click(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error creating table: {ex.Message}",
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
            MessageBox.Show($"Database '{dbName}' exists.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        else
            MessageBox.Show($"Database '{dbName}' does not exist.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    private async Task LoadDatabasesIntoComboBox()
    {
        if (!validateHelper())
            return;
        // call the helper method to get the list of databases
        List<string> dbs = await helper.GetDatabasesAsync();
        comboDbTables.Items.Clear();
        foreach (string db in dbs)
        // iterate through the list and add each database to the combo box
        {
            comboDbTables.Items.Add(db);
        }
        if (comboDbTables.Items.Count > 0)
        {
            comboDbTables.SelectedIndex = 0; // Select the first item by default
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
    }
}
