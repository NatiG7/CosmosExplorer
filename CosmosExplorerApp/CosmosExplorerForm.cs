namespace CosmosExplorerApp;

using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using cloudApp;

public partial class CosmosExplorerForm : Form
{
    private CosmosHelper? helper;
    private CosmosClient? _client;

    public CosmosExplorerForm()
    {
        InitializeComponent();
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

            // Update form title with count
            int count = await helper.CountDatabasesAsync();
            this.Text = $"Cosmos Explorer App - Total Databases: {count}";
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

        _client = helper.GetClient();

        await RefreshDatabasesAsync();
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
}
