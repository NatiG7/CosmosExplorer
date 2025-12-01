using Microsoft.Azure.Cosmos;

namespace CosmosExplorerApp;

partial class CosmosExplorerForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabDatabase;
    private System.Windows.Forms.TabPage tabContainers;
    private System.Windows.Forms.TabPage tabDocuments;
    private System.Windows.Forms.Label lblEndpoint;
    private System.Windows.Forms.TextBox endpointTxtBox;
    private System.Windows.Forms.Label lblPkey;
    private System.Windows.Forms.TextBox pkeyTxtBox;
    private System.Windows.Forms.Button btnLoadKeys;
    private System.Windows.Forms.Button btnCreateDb;
    private System.Windows.Forms.ListBox listDb;
    private System.Windows.Forms.TextBox dbNameTxtBox;
    private System.Windows.Forms.Label lblDbName;
    private System.Windows.Forms.Label lblListDb;
    private System.Windows.Forms.Label lblSelectDbTables;
    private System.Windows.Forms.ComboBox comboDbTables;
    private System.Windows.Forms.Label lblTablesList;
    private System.Windows.Forms.ListBox listTables;
    private System.Windows.Forms.Label lblContainerName;
    private System.Windows.Forms.TextBox txtContainerName;
    private System.Windows.Forms.Button btnCreateContainer;
    private System.Windows.Forms.Button btnRefreshTables;
    private System.Windows.Forms.Label lblTableCount;
    private System.Windows.Forms.Button btnCountDatabases;


    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // ================================
        // ========== FORM SETUP ==========
        // ================================
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "Cosmos Explorer App";

        // ================================
        // ========= TAB CONTROL ==========
        // ================================
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabDatabase = new System.Windows.Forms.TabPage();
        this.tabContainers = new System.Windows.Forms.TabPage();
        this.tabDocuments = new System.Windows.Forms.TabPage();

        this.tabControl1.Location = new System.Drawing.Point(10, 10);
        this.tabControl1.Size = new System.Drawing.Size(760, 500);

        this.tabDatabase.Text = "Databases";
        this.tabContainers.Text = "Containers";
        this.tabDocuments.Text = "Documents";

        this.tabControl1.Controls.Add(this.tabDatabase);
        this.tabControl1.Controls.Add(this.tabContainers);
        this.tabControl1.Controls.Add(this.tabDocuments);

        this.Controls.Add(this.tabControl1);

        #region DatabaseTab
        // ===========================================
        // ========== DATABASE TAB CONTROLS ==========
        // ===========================================

        // Endpoint label
        this.lblEndpoint = new Label();
        this.lblEndpoint.Text = "Endpoint URI:";
        this.lblEndpoint.Location = new System.Drawing.Point(20, 15);
        this.lblEndpoint.AutoSize = true;
        this.tabDatabase.Controls.Add(lblEndpoint);

        // Endpoint textbox
        this.endpointTxtBox = new TextBox();
        this.endpointTxtBox.Location = new System.Drawing.Point(20, 40);
        this.endpointTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(endpointTxtBox);

        // Primary Key label
        this.lblPkey = new Label();
        this.lblPkey.Text = "Primary Key:";
        this.lblPkey.Location = new System.Drawing.Point(20, 75);
        this.lblPkey.AutoSize = true;
        this.tabDatabase.Controls.Add(lblPkey);

        // Primary key textbox
        this.pkeyTxtBox = new TextBox();
        this.pkeyTxtBox.Location = new System.Drawing.Point(20, 100);
        this.pkeyTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(pkeyTxtBox);

        // Load keys button
        this.btnLoadKeys = new Button();
        this.btnLoadKeys.Text = "Load Keys";
        this.btnLoadKeys.Location = new System.Drawing.Point(525, 100);
        this.btnLoadKeys.Size = new System.Drawing.Size(120, 35);
        this.btnLoadKeys.Click += BtnLoadKeys_Click;
        this.tabDatabase.Controls.Add(btnLoadKeys);

        // Database name label
        this.lblDbName = new Label();
        this.lblDbName.Text = "Database Name:";
        this.lblDbName.Location = new System.Drawing.Point(20, 150);
        this.lblDbName.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblDbName);

        // DB name textbox
        this.dbNameTxtBox = new TextBox();
        this.dbNameTxtBox.Location = new System.Drawing.Point(20, 175);
        this.dbNameTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(this.dbNameTxtBox);

        // Create DB button
        this.btnCreateDb = new Button();
        this.btnCreateDb.Text = "Create Database";
        this.btnCreateDb.Location = new System.Drawing.Point(525, 175);
        this.btnCreateDb.Size = new System.Drawing.Size(200, 35);
        this.btnCreateDb.Click += BtnCreateDb_Click;
        this.tabDatabase.Controls.Add(btnCreateDb);

        // Label for DB list
        this.lblListDb = new Label();
        this.lblListDb.Text = "Databases:";
        this.lblListDb.Location = new System.Drawing.Point(20, 245);
        this.lblListDb.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblListDb);

        // Database listbox
        this.listDb = new ListBox();
        this.listDb.Location = new System.Drawing.Point(20, 270);
        this.listDb.Size = new System.Drawing.Size(500, 200);
        this.tabDatabase.Controls.Add(listDb);

        // Count db button
        this.btnCountDatabases = new Button();
        this.btnCountDatabases.Text = "Count Databases";
        this.btnCountDatabases.Location = new System.Drawing.Point(540, 270); // next to listDb
        this.btnCountDatabases.Size = new System.Drawing.Size(150, 35);
        this.btnCountDatabases.Click += BtnCountDatabases_Click;
        this.tabDatabase.Controls.Add(this.btnCountDatabases);

        #endregion

        #region ContainersTab
        // ===========================================
        // ========== CONTAINERS / TABLES TAB =========
        // ===========================================

        // Select DB label
        this.lblSelectDbTables = new Label();
        this.lblSelectDbTables.Text = "Select Database:";
        this.lblSelectDbTables.Location = new System.Drawing.Point(20, 20);
        this.lblSelectDbTables.AutoSize = true;
        this.tabContainers.Controls.Add(this.lblSelectDbTables);

        // DB ComboBox
        this.comboDbTables = new ComboBox();
        this.comboDbTables.Location = new System.Drawing.Point(20, 45);
        this.comboDbTables.Width = 300;
        this.comboDbTables.DropDownStyle = ComboBoxStyle.DropDownList;
        this.comboDbTables.SelectedIndexChanged += ComboDbTables_SelectedIndexChanged;
        this.tabContainers.Controls.Add(this.comboDbTables);

        // Tables label
        this.lblTablesList = new Label();
        this.lblTablesList.Text = "Tables:";
        this.lblTablesList.Location = new System.Drawing.Point(20, 90);
        this.lblTablesList.AutoSize = true;
        this.tabContainers.Controls.Add(this.lblTablesList);


        // Tables listbox
        this.listTables = new ListBox();
        this.listTables.Location = new System.Drawing.Point(20, 115);
        this.listTables.Size = new System.Drawing.Size(350, 250);
        this.tabContainers.Controls.Add(this.listTables);

        // Label table count
        this.lblTableCount = new Label();
        this.lblTableCount.Text = "Tables: 0";
        this.lblTableCount.Location = new System.Drawing.Point(20, 400);
        this.lblTableCount.AutoSize = true;
        this.tabContainers.Controls.Add(this.lblTableCount);

        // Container name label
        this.lblContainerName = new Label();
        this.lblContainerName.Text = "New Container Name:";
        this.lblContainerName.Location = new System.Drawing.Point(400, 20);
        this.lblContainerName.AutoSize = true;
        this.tabContainers.Controls.Add(this.lblContainerName);

        // Container name textbox
        this.txtContainerName = new TextBox();
        this.txtContainerName.Location = new System.Drawing.Point(400, 45);
        this.txtContainerName.Width = 300;
        this.tabContainers.Controls.Add(this.txtContainerName);

        // Create container button
        this.btnCreateContainer = new Button();
        this.btnCreateContainer.Text = "Create Container";
        this.btnCreateContainer.Location = new System.Drawing.Point(400, 85);
        this.btnCreateContainer.Size = new System.Drawing.Size(180, 35);
        this.btnCreateContainer.Click += BtnCreateContainer_Click;
        this.tabContainers.Controls.Add(this.btnCreateContainer);

        // Refresh tables button
        this.btnRefreshTables = new Button();
        this.btnRefreshTables.Text = "Refresh Tables";
        this.btnRefreshTables.Location = new System.Drawing.Point(400, 135);
        this.btnRefreshTables.Size = new System.Drawing.Size(180, 35);
        this.btnRefreshTables.Click += BtnRefreshTables_Click;
        this.tabContainers.Controls.Add(this.btnRefreshTables);

        #endregion

        #region DocumentsTab
        // ===========================================
        // ========== DOCUMENTS TAB (EMPTY) ===========
        // ===========================================

        // Reserved area for future:

        #endregion
    }


    #endregion
}
