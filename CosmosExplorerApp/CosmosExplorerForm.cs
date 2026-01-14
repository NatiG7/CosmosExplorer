namespace CosmosExplorerApp;

using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using cloudApp;
using cloudLogger;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using System.Text.Json.Nodes;

public partial class CosmosExplorerForm : Form
{
    private CosmosHelper helper = null!;
    private CosmosClient? _client;
    public CosmosExplorerForm()
    {
        InitializeComponent();
        InitDynamicLayout();

        // Logger Listener
        CosmosLogger.OnLogNotification += (msg) =>
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.txtLogActivity.Text = msg;
            });
        };
        CosmosLogger.Log("System: Logger online.");

        this.FormClosed += (s, e) =>
        {
            CosmosLogger.Log("System: Session Terminated.");
        };
    }
    private void UpdateInputLayout()
    {
        string selectedMode = cmbEntityMode.SelectedItem?.ToString() ?? "Student";

        if (selectedMode == "Student")
        {
            lblClientTz.Text = "Student ID (Tz):";
            lblClientFirstName.Text = "First Name:";
            btnSaveClient.Text = "Save Student Info";
            lblClientLastName.Visible = true;
            txtClientLastName.Visible = true;
            lblCourses.Visible = true;
            txtCourses.Visible = true;
            lblProducts.Visible = false;
            txtProducts.Visible = false;
            lblBranches.Visible = false;
            txtBranches.Visible = false;
        }
        else if (selectedMode == "Business")
        {
            lblClientTz.Text = "Dealer Number:";
            lblClientFirstName.Text = "Business Name:";
            btnSaveClient.Text = "Save Business Info";
            lblClientLastName.Visible = false;
            txtClientLastName.Visible = false;
            lblCourses.Visible = false;
            txtCourses.Visible = false;
            lblProducts.Visible = true;
            txtProducts.Visible = true;
            lblBranches.Visible = true;
            txtBranches.Visible = true;
        }
    }
    private void InitDynamicLayout()
    {
        cmbEntityMode = new ComboBox();
        cmbEntityMode.Location = new Point(460, 16);
        cmbEntityMode.Width = 150;
        cmbEntityMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEntityMode.Items.Add("Student");
        cmbEntityMode.Items.Add("Business");
        cmbEntityMode.SelectedIndexChanged += (s, e) => UpdateInputLayout();
        tabClient.Controls.Add(cmbEntityMode);
        lblCourses = new Label { Text = "Courses (comma sep):", Location = new Point(20, 260), AutoSize = true };
        txtCourses = new TextBox { Location = new Point(220, 260), Width = 200 };
        lblProducts = new Label { Text = "Products (comma sep):", Location = new Point(20, 260), AutoSize = true };
        txtProducts = new TextBox { Location = new Point(220, 260), Width = 200 };
        lblBranches = new Label { Text = "Branches (comma sep):", Location = new Point(20, 300), AutoSize = true };
        txtBranches = new TextBox { Location = new Point(220, 300), Width = 200 };
        tabClient.Controls.Add(lblCourses);
        tabClient.Controls.Add(txtCourses);
        tabClient.Controls.Add(lblProducts);
        tabClient.Controls.Add(txtProducts);
        tabClient.Controls.Add(lblBranches);
        tabClient.Controls.Add(txtBranches);
        this.btnSaveClient.Location = new Point(20, 340);
        this.btnLoadJsonFile.Location = new Point(20, 380);
        this.btnClearJson.Location = new Point(230, 380);
        this.txtJsonContent.Location = new Point(20, 420);
        this.txtJsonContent.Height = 150;
        this.lblJsonStatus.Location = new Point(20, 580);
        this.btnInsertToCloud.Location = new Point(20, 610);
        this.btnUpdateCloud.Location = new Point(150, 610);
        this.btnDeleteCloud.Location = new Point(280, 610);
        this.btnReadCloud.Location = new Point(410, 610);
        cmbEntityMode.SelectedIndex = 0;
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
    private async Task<bool> RefreshDatabasesAsync()
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
                    return false; ;
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
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error refreshing databases: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return false;
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
        try
        {
            string? endpointUri = ConfigurationManager.AppSettings["EndPointUri"];
            string? pKey = ConfigurationManager.AppSettings["PrimaryKey"];
            string port = "";
            endpointTxtBox.Text = endpointUri;
            pkeyTxtBox.Text = pKey;

            // Initialize helper
            helper = new CosmosHelper(
                endpointTxtBox.Text.Trim(),
                pkeyTxtBox.Text.Trim()
            );

            validateHelper();

            _client = helper.GetClient();
            if (endpointUri != null && Uri.TryCreate(endpointUri, UriKind.Absolute, out Uri? uriParseResult))
            {
                port = uriParseResult.Port.ToString();
            }
            else port = "Uknown";

            if (!await RefreshDatabasesAsync()) return;
            await LoadDatabasesIntoComboBox();
            CosmosLogger.Log($"Keys Loaded, DB Connection::{port}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Connection Failed: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            CosmosLogger.Log($"[System] Keys Load Failed: {ex.Message}");
        }
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
        CosmosLogger.Log($"[UI] User requested create DB: '{dbNameTxtBox.Text.Trim()}'");
        try
        {
            (bool created, string status) result = await helper.CreateDatabaseAsync(dbName);
            MessageBox.Show(
                $"Database created? {result.created} (Status: {result.status})",
                "DB Creation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            if (!await RefreshDatabasesAsync()) return;
            await LoadDatabasesIntoComboBox();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error refreshing tables: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
        try
        {
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
        catch (Exception ex)
        {
            MessageBox.Show($"Error refreshing tables: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

            lblTableCount.Text = $"Tables / Containers: {tables.Count}";
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
            CosmosLogger.Log($"[UI] User requested create Container: '{txtContainerName.Text.Trim()}' in '{selectedDb}'");
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
        try
        {
            bool dbExists = await helper.DatabaseExistsAsync(dbName);
            lblCheckDbResult.Text = dbExists ? "✔ Exists" : "✖ Not Found";
            lblCheckDbResult.ForeColor = dbExists ? Color.Green : Color.Red;
        }
        catch (Exception ex)
        {
            lblCheckDbResult.Text = "Error checking DB.";
            lblCheckDbResult.ForeColor = Color.Red;
            MessageBox.Show(ex.Message);
        }
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
            List<string> allDbs = await helper.GetDatabasesAsync();
            if (allDbs.Count == 0)
            {
                MessageBox.Show("No databases available to apply condition.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (string dbName in allDbs)
            {
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
            List<string> allDbs = await helper.GetDatabasesAsync();
            if (allDbs.Count == 0)
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
            foreach (string dbName in allDbs)
            {
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
            if (!int.TryParse(txtMinTableLengthInput.Text.Trim(), out int minLength))
            {
                MessageBox.Show("Please enter a valid number for length.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
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
        string idValue = txtClientTz.Text.Trim();
        string nameVal = txtClientFirstName.Text.Trim();
        if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName))
        {
            MessageBox.Show("Please enter both database and table names.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        try
        {
            JObject payload = new JObject();
            string mode = cmbEntityMode.SelectedItem?.ToString() ?? "Student";
            if (mode == "Student")
            {
                // ctor with jobject
                payload["id"] = idValue;
                payload["firstName"] = nameVal;
                payload["lastName"] = txtClientLastName.Text.Trim();
                payload["adresses"] = new JArray();
                payload["courses"] = TextToJArray(txtCourses.Text);
            }
            else if (mode == "Business")
            {
                payload["id"] = idValue;
                payload["firstName"] = nameVal;
                payload["branches"] = TextToJArray(txtBranches.Text);
                payload["products"] = TextToJArray(txtProducts.Text);
            }
            await helper.SaveJsonItemToCosmosAsync(dbName, tableName, payload);
            MessageBox.Show($"{mode} saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtClientTz.Clear();
            txtClientFirstName.Clear();
            txtClientLastName.Clear();
            txtCourses.Clear();
            txtBranches.Clear();
            txtProducts.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving to Cosmos DB: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private JArray TextToJArray(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return new JArray();
        return new JArray(input.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)));
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
        if (!validateHelper()) return;
        string? selectedDb = comboDbTables.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedDb))
            return;
        try { await RefreshTablesList(selectedDb); }
        catch (Exception ex)
        {
            MessageBox.Show($"Error refreshing tables: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private async Task RefreshTablesList(string dbName)
    {
        if (!validateHelper()) return;
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
        if (!validateHelper()) return;
        int count = await helper.CountTablesInDBAsync(dbName);
        lblTableCount.Text = $"Tables / Containers: {count}";
    }
    private async Task ValidateTextboxAsync(
            TextBox textbox,
                Label indicatorLabel,
                    Func<string, Task<bool>> validateFunc)
    {
        if (!validateHelper()) return;
        string value = textbox.Text.Trim();

        if (string.IsNullOrEmpty(value))
        {
            indicatorLabel.Text = "";
            return;
        }

        try
        {
            bool exists = await validateFunc(value);
            indicatorLabel.Text = exists ? "✔" : "✖";
            indicatorLabel.ForeColor = exists ? Color.Green : Color.Red;
        }
        catch (Exception)
        {
            indicatorLabel.Text = "⚠";
            indicatorLabel.ForeColor = Color.Orange;
        }
    }
    private async void TxtClientDbName_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        await ValidateTextboxAsync(
            txtClientDbName,
                lblDbCheck,
                    helper.DatabaseExistsAsync);
    }
    private async void TxtClientTableName_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtClientDbName.Text.Trim();
        await ValidateTextboxAsync(txtClientTableName, lblTbCheck,
                table => helper.TableExistsAsync(db, table));
    }
    private async void TxtClientId_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string dbName = txtClientDbName.Text.Trim();
        string tableName = txtClientTableName.Text.Trim();
        string id = txtClientId.Text.Trim();

        if (string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(id))
        {
            lblIdCheck.Text = "";
            return;
        }
        try
        {
            bool exists = await helper.ItemExistsAsync(dbName, tableName, id);
            lblIdCheck.Text = exists ? "✔" : "✖";
            lblIdCheck.ForeColor = exists ? Color.Green : Color.Red;
        }
        catch (Exception)
        {
            lblIdCheck.Text = "⚠";
            lblIdCheck.ForeColor = Color.Orange;
        }
    }
    private void BtnLoadJsonFile_Click(object sender, EventArgs e)
    {
        using OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "JSON files|*.json";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string json = File.ReadAllText(ofd.FileName);
                txtJsonContent.Text = json;
            }
            catch (Exception ex)
            { MessageBox.Show($"Could not read file : {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
    private void BtnClearJson_Click(object sender, EventArgs e)
    {
        txtJsonContent.Clear();
        lblJsonStatus.Text = "JSON cleared.";
        lblJsonStatus.ForeColor = Color.Black;
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
            var item = JToken.Parse(json);
            if (item is JObject jsonItem)
            {
                string? id = jsonItem["id"]?.ToString();
                if (string.IsNullOrEmpty(id))
                {
                    lblJsonStatus.Text = "JSON must include an 'id' field!";
                    lblJsonStatus.ForeColor = Color.Red;
                    return;
                }
                CosmosLogger.Log($"[UI] Batch/Insert initiated for target '{dbName}/{tableName}'");
                var existing = await helper.GetItemFromCosmosAsync(dbName, tableName, id);
                if (existing != null)
                {
                    lblJsonStatus.Text = $"Item '{jsonItem["name"]}' with id '{id}' already exists!";
                    lblJsonStatus.ForeColor = Color.Orange;
                    return;
                }
                await helper.SaveJsonItemToCosmosAsync(dbName, tableName, jsonItem);
                lblJsonStatus.Text = "Inserted successfully!";
                lblJsonStatus.ForeColor = Color.Green;
            }
            else if (item is JArray jsonArray)
            {
                int successCount = 0;
                int skippedCount = 0;
                foreach (JObject arrJsonItem in item)
                {
                    string? id = arrJsonItem["id"]?.ToString();
                    if (string.IsNullOrEmpty(id))
                    {
                        skippedCount++;
                        continue;
                    }
                    var existing = await helper.GetItemFromCosmosAsync(dbName, tableName, id);
                    if (existing != null)
                    {
                        skippedCount++;
                        continue;
                    }
                    await helper.SaveJsonItemToCosmosAsync(dbName, tableName, arrJsonItem);
                    successCount++;
                }
                lblJsonStatus.Text = $"Batch Done: {successCount} inserted, {skippedCount} skipped.";
                lblJsonStatus.ForeColor = Color.Blue;
            }
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
            CosmosLogger.Log($"[UI] Update Item '{id}' requested in '{dbName}/{containerName}'");
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
            CosmosLogger.Log($"[UI] Delete Item '{id}' requested in '{dbName}/{containerName}'");
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
    private async void BtnCountAllDocs_Click(object sender, EventArgs e)
    {
        if (!validateHelper())
            return;
        try
        {
            btnCountAllDocs.Enabled = false;
            btnCountAllDocs.Text = "Counting...";
            int itemCount = await helper.CountAllItemsAsync();
            lblDocCountResult.Text = $"Total items in all DBs: {itemCount}";

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCountAllDocs.Enabled = true;
            btnCountAllDocs.Text = "Count ALL Items (Global)";
        }
    }
    private async void BtnCountAllDocsInDb_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        try
        {
            btnCountItemsInDb.Enabled = false;
            btnCountItemsInDb.Text = "Counting...";
            string dbName = txtCountItemsInDb.Text.Trim();
            if (string.IsNullOrEmpty(dbName))
            {
                MessageBox.Show("Please enter a database name first.",
                                                    "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool exists = await helper.DatabaseExistsAsync(dbName);
            if (!exists)
            {
                MessageBox.Show($"Database '{dbName}' was not found.", "Not Found",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            List<string> tableNames = await helper.GetTablesAsync(dbName);
            int totalItemCount = 0;
            foreach (string tableName in tableNames)
            {
                CosmosHelper.TableInfo tableData = new CosmosHelper.TableInfo
                {
                    DbName = dbName,
                    TableName = tableName
                };
                totalItemCount += await helper.CountItemsInTableAsync(tableData);
            }
            lblCountItemsInDbResult.Text = $"Items in {dbName} : {totalItemCount}";
        }
        catch (Exception ex)
        {
            lblCountItemsInDbResult.Text = "Request Failed.";
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCountItemsInDb.Enabled = true;
            btnCountItemsInDb.Text = "Count Items in DB";
        }
    }
    private async void BtnCountAllDocsAllTables_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        try
        {
            btnListObjCounts.Enabled = false;
            List<string> allDocsPerTableList = [];
            cmbListObjCounts.Items.Clear();
            List<string> dbNames = await helper.GetDatabasesAsync();
            foreach (string thisDb in dbNames)
            {
                List<string> tableNames = await helper.GetTablesAsync(thisDb);
                foreach (string thisTable in tableNames)
                {
                    CosmosHelper.TableInfo tableData = new CosmosHelper.TableInfo
                    {
                        DbName = thisDb,
                        TableName = thisTable
                    };
                    int itemCount = await helper.CountItemsInTableAsync(tableData);
                    string countStr = (itemCount == -1) ? "Error" : itemCount.ToString();
                    allDocsPerTableList.Add($"{tableData.DbName} - {tableData.TableName} - {countStr} objects");
                }
            }
            cmbListObjCounts.Items.AddRange([.. allDocsPerTableList]);
            if (cmbListObjCounts.Items.Count > 0) cmbListObjCounts.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            btnListObjCounts.Text = "Request Failed.";
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnListObjCounts.Enabled = true;
            btnListObjCounts.Text = "List Object Counts (All Tables)";
        }
    }
    private async void BtnCountMaxDocuments_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        try
        {
            btnMaxObjCounts.Enabled = false;
            btnMaxObjCounts.Text = "Scanning...";

            int maxCount = -1;
            List<string> winners = [];

            List<string> dbNames = await helper.GetDatabasesAsync();
            foreach (string thisDb in dbNames)
            {
                List<string> tableNames = await helper.GetTablesAsync(thisDb);
                foreach (string thisTable in tableNames)
                {
                    CosmosHelper.TableInfo tableData = new CosmosHelper.TableInfo
                    {
                        DbName = thisDb,
                        TableName = thisTable
                    };
                    int itemCount = await helper.CountItemsInTableAsync(tableData);

                    // King of the Hill Logic
                    if (itemCount > maxCount)
                    {
                        maxCount = itemCount;
                        winners.Clear(); // New record, remove old winners
                        winners.Add($"{tableData.DbName}-{tableData.TableName}");
                    }
                    else if (itemCount == maxCount)
                    {
                        winners.Add($"{tableData.DbName}-{tableData.TableName}"); // It's a tie, add to list
                    }
                }
            }

            // Final Output
            if (winners.Count > 0)
            {
                string allWinners = string.Join(", ", winners);
                txtMaxObjCountsResult.Text = $"Tables: {allWinners} with {maxCount} objects";
            }
            else
            {
                txtMaxObjCountsResult.Text = "No tables found.";
            }
        }
        catch (Exception ex)
        {
            txtMaxObjCountsResult.Text = "Request Failed.";
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnMaxObjCounts.Enabled = true;
            btnMaxObjCounts.Text = "Show Tables w/ Max Objects";
        }
    }
    private async void TxtCountItemsInDb_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        await ValidateTextboxAsync(
            txtCountItemsInDb,
            lblCountItemsDbCheck,
            helper.DatabaseExistsAsync
        );
    }
    private async void TxtCountItemsInTable_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtCountItemsInDb.Text.Trim();
        string table = txtCountItemsInTable.Text.Trim();
        await ValidateTextboxAsync(txtCountItemsInTable, lblCountItemsTableCheck,
                table => helper.TableExistsAsync(db, table));
    }
    private async void BtnCountAllDocsInTable_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        try
        {
            btnCountItemsInTable.Enabled = false;
            btnCountItemsInTable.Text = "Counting...";
            string dbName = txtCountItemsInDb.Text.Trim();
            if (string.IsNullOrEmpty(dbName))
            {
                MessageBox.Show("Please enter a database name first.",
                                                    "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool dbExists = await helper.DatabaseExistsAsync(dbName);
            if (!dbExists)
            {
                MessageBox.Show($"Database '{dbName}' was not found.", "Not Found",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string tableName = txtCountItemsInTable.Text.Trim();
            if (string.IsNullOrEmpty(tableName))
            {
                MessageBox.Show("Please enter a table name first.",
                                                    "Input Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            bool tableExists = await helper.TableExistsAsync(dbName, tableName);
            if (!tableExists)
            {
                MessageBox.Show($"Table '{tableName}' was not found.", "Not Found",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CosmosHelper.TableInfo tableData = new CosmosHelper.TableInfo
            {
                DbName = dbName,
                TableName = tableName
            };
            int totalItemsInTable = await helper.CountItemsInTableAsync(tableData);
            lblCountItemsInTableResult.Text = $"Item count in '{tableName}': {totalItemsInTable}";
        }
        catch (Exception ex)
        {
            lblCountItemsInTableResult.Text = "Request Failed.";
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnCountItemsInTable.Enabled = true;
            btnCountItemsInTable.Text = "Count Items in table";
        }
    }
    private void BtnClearReadItemResult_Click(object sender, EventArgs e)
    {
        rtbReadItemResult.Clear();
        txtEnterDocumentId.Clear();
    }
    private async void TxtEnterDocumentId_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtCountItemsInDb.Text.Trim();
        string table = txtCountItemsInTable.Text.Trim();
        if (string.IsNullOrEmpty(db) || string.IsNullOrEmpty(table))
        {
            lblEnterDocumentIdChk.Text = "";
            return;
        }
        await ValidateTextboxAsync(txtEnterDocumentId, lblEnterDocumentIdChk,
        itemId => helper.ItemExistsAsync(db, table, itemId));
    }
    private async void btnEnterDocumentIdReadItem_Click(object sender, EventArgs e)
    {
        string db = txtCountItemsInDb.Text.Trim();
        string table = txtCountItemsInTable.Text.Trim();
        string itemId = txtEnterDocumentId.Text.Trim();
        if (string.IsNullOrEmpty(db) || string.IsNullOrEmpty(table) || string.IsNullOrEmpty(itemId))
        {
            lblEnterDocumentIdReadItemResults.ForeColor = Color.Orange;
            lblEnterDocumentIdReadItemResults.Text = "Missing inputs!";
            return;
        }
        try
        {
            btnEnterDocumentIdReadItem.Enabled = false;
            lblEnterDocumentIdReadItemResults.Text = "Reading Item...";
            JObject? requestedItem = await helper.GetItemFromCosmosAsync(db, table, itemId);
            if (requestedItem != null)
            {
                rtbReadItemResult.Text = requestedItem?.ToString();
                lblEnterDocumentIdReadItemResults.ForeColor = Color.Green;
                lblEnterDocumentIdReadItemResults.Text = "Request OK";
            }
            else
            {
                rtbReadItemResult.Clear();
                lblEnterDocumentIdReadItemResults.ForeColor = Color.Red;
                lblEnterDocumentIdReadItemResults.Text = "Item not found (404)";
            }
        }
        catch (Exception ex)
        {
            rtbReadItemResult.Clear();
            lblEnterDocumentIdReadItemResults.ForeColor = Color.Red;
            lblEnterDocumentIdReadItemResults.Text = "Request failed.";
            rtbReadItemResult.Text = ex.ToString();
        }
        finally
        {
            btnEnterDocumentIdReadItem.Enabled = true;
        }
    }
    private async void TxtInvDb_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        await ValidateTextboxAsync(
            txtInvDb,
            lblInvDbCheck,
            helper.DatabaseExistsAsync
        );
    }
    private async void TxtInvTable_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtInvDb.Text.Trim();
        if (string.IsNullOrEmpty(db))
        {
            lblInvTableCheck.Text = "";
            return;
        }
        await ValidateTextboxAsync(
            txtInvTable,
            lblInvTableCheck,
            (tableName) => helper.TableExistsAsync(db, tableName)
        );
    }
    private async void TxtInvDocId_TextChanged(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtInvDb.Text.Trim();
        string table = txtInvTable.Text.Trim();
        if (string.IsNullOrEmpty(db) || string.IsNullOrEmpty(table))
        {
            lblInvDocIdCheck.Text = "";
            return;
        }
        await ValidateTextboxAsync(
            txtInvDocId,
            lblInvDocIdCheck,
            (docId) => helper.ItemExistsAsync(db, table, docId)
        );
    }
    private async Task<bool> ValidateInputAndExistsAsync(string val, string context, Func<Task<bool>> isExists)
    {
        {
            if (string.IsNullOrEmpty(val))
            {
                MessageBox.Show($"Please enter a {context} first.", "Input Required",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            bool exists = await isExists();
            if (!exists)
            {
                MessageBox.Show($"{context} '{val}' was not found.", "Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
    private async void BtnInvestigate_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        dgvInvResults.Visible = false;
        rtbInvResult.Visible = true;
        try
        {
            btnInvestigate.Enabled = false;
            btnInvestigate.Text = "Investigating...";
            string db = txtInvDb.Text.Trim();
            string tableName = txtInvTable.Text.Trim();
            string docId = txtInvDocId.Text.Trim();
            if (!await ValidateInputAndExistsAsync(db, "Database", () => helper.DatabaseExistsAsync(db))) return;
            if (!await ValidateInputAndExistsAsync(tableName, "Table", () => helper.TableExistsAsync(db, tableName))) return;
            CosmosHelper.TableInfo tableData = new CosmosHelper.TableInfo
            {
                DbName = db,
                TableName = tableName
            };
            if (!await ValidateInputAndExistsAsync(docId, "Document", () => helper.ItemExistsAsync(tableData.DbName, tableData.TableName, docId))) return;
            JObject? thisDoc = await helper.GetItemFromCosmosAsync(tableData.DbName,
                                                        tableData.TableName, docId);
            if (chkCountCourses.Checked)
            {
                string courseOutput = FetchStudentCourses(thisDoc);
                txtCourseResult.Text = courseOutput;
            }
            else txtCourseResult.Text = "No Courses";
            TextBox[] nameBoxes = { txtInvFieldName1, txtInvFieldName2, txtInvFieldName3 };
            ComboBox[] operatorBoxes = { cmbInvOp1, cmbInvOp2, cmbInvOp3 };
            TextBox[] valueBoxes = { txtInvFieldValue1, txtInvFieldValue2, txtInvFieldValue3 };
            List<string> fieldResults = [];
            List<string> failedFields = [];
            bool isMatch = true;
            if (thisDoc != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    string fName = nameBoxes[i].Text.Trim();
                    string fValue = valueBoxes[i].Text.Trim();
                    string userOperator = operatorBoxes[i].Text.Trim();
                    if (string.IsNullOrEmpty(fName)) continue;

                    JToken? dbToken = thisDoc[fName];
                    if (dbToken != null)
                    {
                        bool passed = CheckRule(dbToken, userOperator, fValue);
                        if (passed) fieldResults.Add($"[PASS] Field '{fName}' met condition: {userOperator} '{fValue}'");
                        else
                        {
                            isMatch = false;
                            fieldResults.Add($"[FAIL] Field '{fName}' is '{dbToken}'. Expected {userOperator} '{fValue}'");
                            failedFields.Add($"{fName} (Condition Failed)");
                        }
                    }
                    else
                    {
                        isMatch = false;
                        fieldResults.Add($"[FAIL] Field '{fName}' does not exist in this document.");
                        failedFields.Add($"{fName} (Missing)");
                    }
                }
                rtbInvResult.Text = string.Join(Environment.NewLine, fieldResults) +
                                    Environment.NewLine +
                                    "------------------------------" +
                                    Environment.NewLine +
                                    thisDoc.ToString();
                List<string> fieldsInvestigate = [txtInvFieldName1.Text.Trim(),
                                                txtInvFieldName2.Text.Trim(),
                                                txtInvFieldName3.Text.Trim()];
                if (isMatch)
                {
                    lblInvResult.Text = "Match Confirmed!";
                    lblInvResult.ForeColor = Color.Green;
                    List<string> validFields = [.. fieldsInvestigate.Where(f => !string.IsNullOrEmpty(f))];
                    string fieldsMsg = validFields.Count > 0 ? string.Join(", ", validFields) : "(ID Check)";
                    CosmosLogger.Log($"[Investigation] MATCH CONFIRMED for ID '{docId}'. Fields verified: {fieldsMsg}");
                }
                else
                {
                    lblInvResult.Text = "Issues Found";
                    lblInvResult.ForeColor = Color.Red;
                    string errorDetails = failedFields.Count > 0 ? string.Join(", ", failedFields) : "Unknown Issue";
                    CosmosLogger.Log($"[Investigation] ISSUES FOUND for ID '{docId}'.\nFailures: {errorDetails}");
                }
            }
        }
        catch (Exception ex)
        {
            lblInvResult.Text = "Request Failed.";
            MessageBox.Show($"Error counting items: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnInvestigate.Enabled = true;
            btnInvestigate.Text = "Investigate Document";
        }
    }
    private bool CheckRule(JToken? dbToken, string userOperator, string userValue, int depth = 0)
    {
        if (dbToken == null) return false;
        if (depth > 10)
        {
            // overflow risk, some bugged data or malicious stuff
            DialogResult result = MessageBox.Show(
                $"Extreme recursion depth detected ({depth}).\n\n" +
                "Continue scanning? (Click No to abort this branch)",
                "Stack Overflow Risk",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No) return false;
        }

        // jsonArray check, rule engine upgrade
        if (dbToken is JArray jsonArr)
        {
            if (userOperator.StartsWith("Length"))
            {
                if (int.TryParse(userValue, out int lenLen))
                {
                    if (userOperator == "Length >") return jsonArr.Count > lenLen;
                    if (userOperator == "Length <") return jsonArr.Count < lenLen;
                    if (userOperator == "Length =") return jsonArr.Count == lenLen;
                }
                return false;
            }
            foreach (JToken item in jsonArr) if (CheckRule(item, userOperator, userValue, depth + 1)) return true;
            return false;
        }
        string dbVal = dbToken.ToString();
        switch (userOperator)
        {
            case "==": return dbVal.Equals(userValue, StringComparison.OrdinalIgnoreCase);
            case "!=": return !dbVal.Equals(userValue, StringComparison.OrdinalIgnoreCase);
            case "Contains": return dbVal.Contains(userValue, StringComparison.OrdinalIgnoreCase);
            case "StartsWith": return dbVal.StartsWith(userValue, StringComparison.OrdinalIgnoreCase);
            case ">":
            case "<":
                if (double.TryParse(dbVal, out double dbNum)
                && double.TryParse(userValue, out double userNum))
                { return userOperator == ">" ? dbNum > userNum : dbNum < userNum; }
                return false;
            case "Length >":
            case "Length <":
            case "Length =":
                if (int.TryParse(userValue, out int lenLen))
                {
                    if (userOperator == "Length >") return dbVal.Length > lenLen;
                    if (userOperator == "Length <") return dbVal.Length < lenLen;
                    if (userOperator == "Length =") return dbVal.Length == lenLen;
                }
                return false;

            default:
                return false;
        }
    }
    private string FetchStudentCourses(JObject? document)
    {
        if (document == null) return "Error reading document.";
        try
        {
            JToken? thisDocCourses = document["courses"];
            if (thisDocCourses is JArray coursesArray)
            {
                int validCount = 0;
                foreach (JToken item in coursesArray)
                {
                    if (item != null && item.Type != JTokenType.Null) validCount++;
                }
                return "Number of coursers: " + validCount.ToString();
            }
        }
        catch (Exception)
        { }
        return "No Courses";
    }
    private void BtnClearInvestigateResult(object sender, EventArgs e)
    {
        rtbInvResult.Clear();
    }
    private async void BtnSearchItems_Click(object sender, EventArgs e)
    {
        if (!validateHelper()) return;
        string db = txtInvDb.Text.Trim();
        string table = txtInvTable.Text.Trim();
        // validate
        if (!await ValidateInputAndExistsAsync(db, "Database", () => helper.DatabaseExistsAsync(db))) return;
        if (!await ValidateInputAndExistsAsync(table, "Table", () => helper.TableExistsAsync(db, table))) return;

        try
        {
            btnSearchItems.Enabled = false;
            btnSearchItems.Text = "Scanning...";
            rtbInvResult.Visible = false;
            dgvInvResults.Visible = true;
            dgvInvResults.DataSource = null;
            List<JObject> allDocs = await helper.GetAllDocumentsAsync(db, table);
            TextBox[] nameBoxes = { txtInvFieldName1, txtInvFieldName2, txtInvFieldName3 };
            ComboBox[] opBoxes = { cmbInvOp1, cmbInvOp2, cmbInvOp3 };
            TextBox[] valueBoxes = { txtInvFieldValue1, txtInvFieldValue2, txtInvFieldValue3 };
            var matchedDocs = new List<dynamic>();
            List<string> activeRules = [];
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(nameBoxes[i].Text))
                    activeRules.Add($"{nameBoxes[i].Text} {opBoxes[i].Text} {valueBoxes[i].Text}");
            }
            string ruleSummary = activeRules.Count > 0 ? string.Join(" AND ", activeRules) : "No Constraints";
            CosmosLogger.Log($"[Search] START scanning '{table}'. Criteria: {ruleSummary}");

            foreach (JObject doc in allDocs)
            {
                bool isMatch = true;
                string matchSummary = "";
                for (int i = 0; i < 3; i++)
                {
                    string fName = nameBoxes[i].Text.Trim();
                    string op = opBoxes[i].Text;
                    string fVal = valueBoxes[i].Text.Trim();
                    if (string.IsNullOrEmpty(fName)) continue;
                    List<JToken> docTokens = doc.SelectTokens(fName).ToList();
                    JToken? docToken = docTokens.Count > 1 ? new JArray(docTokens) : docTokens.FirstOrDefault();
                    bool passed = CheckRule(docToken, op, fVal);
                    if (!passed)
                    {
                        isMatch = false;
                        break;
                    }
                    matchSummary += $"{fName}={docToken} | ";
                }
                if (isMatch)
                {
                    var previewProps = doc.Properties()
                    .Where(p => p.Name != "id" && !p.Name.StartsWith("_"))
                    .Take(5)
                    .Select(p => $"{p.Name}:{LimitLength(p.Value.ToString(), 30)}")
                    .ToList();

                    string previewStr = string.Join(" | ", previewProps);
                    matchedDocs.Add(new
                    {
                        ID = doc["id"]?.ToString(),
                        Preview = previewStr,
                        MatchRules = matchSummary.TrimEnd(' ', '|')
                    });
                }
            }
            if (matchedDocs.Count > 0)
            {
                dgvInvResults.DataSource = matchedDocs;
                lblInvResult.Text = $"Found {matchedDocs.Count} documents.";
                lblInvResult.ForeColor = Color.Green;
                CosmosLogger.Log($"[Search] Filter found {matchedDocs.Count} matches in '{table}'");
            }
            else
            {
                lblInvResult.Text = "No documents matched.";
                lblInvResult.ForeColor = Color.Red;
                CosmosLogger.Log("[Search] FINISHED. No documents matched.");
                MessageBox.Show("No documents matched your criteria.",
                "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            CosmosLogger.Log($"[Search] ERROR: {ex.Message}");
            MessageBox.Show($"Error searching: {ex.Message}", "Error",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSearchItems.Enabled = true;
            btnSearchItems.Text = "Search / Filter";
        }
    }
    private string LimitLength(string source, int maxLength)
    {
        if (string.IsNullOrEmpty(source)) return "";
        source = source.Replace("\r", "").Replace("\n", " ");
        if (source.Length <= maxLength) return source;
        return source.Substring(0, maxLength) + "...";
    }
}
