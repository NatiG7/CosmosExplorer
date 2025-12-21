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
    private System.Windows.Forms.TabPage tabClient;
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
    private System.Windows.Forms.ComboBox listTables;
    private System.Windows.Forms.Label lblContainerName;
    private System.Windows.Forms.TextBox txtContainerName;
    private System.Windows.Forms.Button btnCreateContainer;
    private System.Windows.Forms.Button btnRefreshTables;
    private System.Windows.Forms.Label lblTableCount;
    private System.Windows.Forms.Button btnCountDatabases;
    private System.Windows.Forms.Label lblFilterDb;
    private System.Windows.Forms.TextBox txtDbPrefix;
    private System.Windows.Forms.Button btnFilterDb;
    private System.Windows.Forms.Label lblFilterResults;
    private System.Windows.Forms.ComboBox cmbFilteredDbs;
    private System.Windows.Forms.Button btnCountAllTables;
    private System.Windows.Forms.Label lblTotalTables;
    private System.Windows.Forms.Label lblCheckDb;
    private System.Windows.Forms.TextBox txtCheckDb;
    private System.Windows.Forms.Button btnCheckDb;
    private System.Windows.Forms.Label lblCheckDbResult;
    private System.Windows.Forms.Button btnListDbsWithTables;
    private System.Windows.Forms.Label lblCheckTable;
    private System.Windows.Forms.TextBox txtCheckTable;
    private System.Windows.Forms.Button btnCheckTable;
    private System.Windows.Forms.ListBox lstCheckTableResult;
    private System.Windows.Forms.Label lblDbConditionDesc;
    private System.Windows.Forms.Button btnApplyConditionFilter;
    private System.Windows.Forms.ComboBox cmbConditionResults;
    private System.Windows.Forms.Label lblExactTableCount;
    private System.Windows.Forms.TextBox txtExactTableCount;
    private System.Windows.Forms.Button btnExactTableCount;
    private System.Windows.Forms.ComboBox cmbExactTableCountResult;
    private System.Windows.Forms.Label lblMinTableLength;
    private System.Windows.Forms.TextBox txtMinTableLengthInput;
    private System.Windows.Forms.Button btnMinTableName;
    private System.Windows.Forms.ComboBox cmbMinLegthTables;
    private System.Windows.Forms.CheckBox chkDbNameOnly;
    private System.Windows.Forms.CheckBox chkMatchTables;
    private System.Windows.Forms.CheckBox chkLongestNames;
    private System.Windows.Forms.Button btnMostTables;
    private System.Windows.Forms.Label lblClientDbName;
    private System.Windows.Forms.Label lblClientTableName;
    private System.Windows.Forms.Label lblClientTz;
    private System.Windows.Forms.Label lblClientFirstName;
    private System.Windows.Forms.Label lblClientLastName;
    private System.Windows.Forms.TextBox txtClientDbName;
    private System.Windows.Forms.Label lblDbCheck;
    private System.Windows.Forms.TextBox txtClientTableName;
    private System.Windows.Forms.TextBox txtClientTz;
    private System.Windows.Forms.TextBox txtClientFirstName;
    private System.Windows.Forms.TextBox txtClientLastName;
    private System.Windows.Forms.Button btnSaveClient;
    private System.Windows.Forms.Label lblTbCheck;
    private System.Windows.Forms.Button btnLoadJsonFile;
    private System.Windows.Forms.Button btnInsertToCloud;
    private System.Windows.Forms.Button btnUpdateCloud;
    private System.Windows.Forms.Button btnDeleteCloud;
    private System.Windows.Forms.Button btnReadCloud;
    private System.Windows.Forms.RichTextBox txtJsonContent;
    private System.Windows.Forms.Label lblJsonStatus;
    private System.Windows.Forms.Label lblClientId;
    private System.Windows.Forms.TextBox txtClientId;
    private System.Windows.Forms.Label lblIdCheck;
    private System.Windows.Forms.Button btnClearJson;
    private System.Windows.Forms.ComboBox cmbEntityMode;
    private System.Windows.Forms.Label lblCourses;
    private System.Windows.Forms.TextBox txtCourses;
    private System.Windows.Forms.Label lblProducts;
    private System.Windows.Forms.TextBox txtProducts;
    private System.Windows.Forms.Label lblBranches;
    private System.Windows.Forms.TextBox txtBranches;
    private System.Windows.Forms.Button btnCountAllDocs;
    private System.Windows.Forms.Label lblDocCountResult;
    private System.Windows.Forms.TextBox txtCountItemsInDb;
    private System.Windows.Forms.Label lblCountItemsDbCheck;
    private System.Windows.Forms.Button btnCountItemsInDb;
    private System.Windows.Forms.Label lblCountItemsInDb;
    private System.Windows.Forms.Label lblCountItemsInDbResult;



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
        this.ClientSize = new System.Drawing.Size(870, 900);
        this.Text = "Cosmos Explorer App";

        // ================================
        // ========= TAB CONTROL ==========
        // ================================
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabDatabase = new System.Windows.Forms.TabPage();
        this.tabContainers = new System.Windows.Forms.TabPage();
        this.tabDocuments = new System.Windows.Forms.TabPage();
        this.tabClient = new System.Windows.Forms.TabPage();

        this.tabControl1.Location = new System.Drawing.Point(10, 10);
        this.tabControl1.Size = new System.Drawing.Size(850, 870);

        this.tabDatabase.Text = "Databases";
        this.tabContainers.Text = "Containers";
        this.tabDocuments.Text = "Documents";
        this.tabClient.Text = "Client / Student Info";

        this.tabControl1.Controls.Add(this.tabDatabase);
        this.tabControl1.Controls.Add(this.tabContainers);
        this.tabControl1.Controls.Add(this.tabDocuments);
        this.tabControl1.Controls.Add(this.tabClient);

        this.Controls.Add(this.tabControl1);

        #region DatabaseTab
        // ===========================================
        // ========== DATABASE TAB CONTROLS ==========
        // ===========================================

        // Endpoint label
        this.lblEndpoint = new Label();
        this.lblEndpoint.Text = "Endpoint URI:";
        this.lblEndpoint.Location = new System.Drawing.Point(20, 5);
        this.lblEndpoint.AutoSize = true;
        this.tabDatabase.Controls.Add(lblEndpoint);

        // Endpoint textbox
        this.endpointTxtBox = new TextBox();
        this.endpointTxtBox.Location = new System.Drawing.Point(20, 30);
        this.endpointTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(endpointTxtBox);

        // Primary Key label
        this.lblPkey = new Label();
        this.lblPkey.Text = "Primary Key:";
        this.lblPkey.Location = new System.Drawing.Point(20, 65);
        this.lblPkey.AutoSize = true;
        this.tabDatabase.Controls.Add(lblPkey);

        // Primary key textbox
        this.pkeyTxtBox = new TextBox();
        this.pkeyTxtBox.Location = new System.Drawing.Point(20, 90);
        this.pkeyTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(pkeyTxtBox);

        // Load keys button
        this.btnLoadKeys = new Button();
        this.btnLoadKeys.Text = "Load Keys";
        this.btnLoadKeys.Location = new System.Drawing.Point(525, 90);
        this.btnLoadKeys.Size = new System.Drawing.Size(120, 35);
        this.btnLoadKeys.Click += BtnLoadKeys_Click;
        this.tabDatabase.Controls.Add(btnLoadKeys);

        // Database name label
        this.lblDbName = new Label();
        this.lblDbName.Text = "Database Name:";
        this.lblDbName.Location = new System.Drawing.Point(20, 130);
        this.lblDbName.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblDbName);

        // DB name textbox
        this.dbNameTxtBox = new TextBox();
        this.dbNameTxtBox.Location = new System.Drawing.Point(20, 155);
        this.dbNameTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(this.dbNameTxtBox);

        // Create DB button
        this.btnCreateDb = new Button();
        this.btnCreateDb.Text = "Create Database";
        this.btnCreateDb.Location = new System.Drawing.Point(525, 155);
        this.btnCreateDb.Size = new System.Drawing.Size(200, 35);
        this.btnCreateDb.Click += BtnCreateDb_Click;
        this.tabDatabase.Controls.Add(btnCreateDb);

        // Label for DB list
        this.lblListDb = new Label();
        this.lblListDb.Text = "Databases:";
        this.lblListDb.Location = new System.Drawing.Point(20, 195);
        this.lblListDb.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblListDb);

        // Database listbox
        this.listDb = new ListBox();
        this.listDb.Location = new System.Drawing.Point(20, 220);
        this.listDb.Size = new System.Drawing.Size(500, 180);
        this.tabDatabase.Controls.Add(listDb);

        // Count db button
        this.btnCountDatabases = new Button();
        this.btnCountDatabases.Text = "Count Databases";
        this.btnCountDatabases.Location = new System.Drawing.Point(525, 220);
        this.btnCountDatabases.AutoSize = true;
        this.btnCountDatabases.Click += BtnCountDatabases_Click;
        this.tabDatabase.Controls.Add(this.btnCountDatabases);

        // Button: List DBs with table counts
        this.btnListDbsWithTables = new Button();
        this.btnListDbsWithTables.Text = "Count Tables per DB";
        this.btnListDbsWithTables.Location = new System.Drawing.Point(525, 260);
        this.btnListDbsWithTables.Size = new System.Drawing.Size(200, 35);
        this.btnListDbsWithTables.Click += BtnListDbsWithTableCount_Click;
        this.tabDatabase.Controls.Add(this.btnListDbsWithTables);

        #region Database Filter UI

        // Label: Filter DBs
        this.lblFilterDb = new System.Windows.Forms.Label();
        this.lblFilterDb.Text = "Filter DBs starting with:";
        this.lblFilterDb.Location = new System.Drawing.Point(20, 410);
        this.lblFilterDb.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblFilterDb);

        // TextBox: Filter prefix
        this.txtDbPrefix = new System.Windows.Forms.TextBox();
        this.txtDbPrefix.Location = new System.Drawing.Point(210, 405);
        this.txtDbPrefix.Width = 200;
        this.tabDatabase.Controls.Add(this.txtDbPrefix);

        // Button: Filter DBs
        this.btnFilterDb = new System.Windows.Forms.Button();
        this.btnFilterDb.Text = "Filter";
        this.btnFilterDb.Location = new System.Drawing.Point(420, 405);
        this.btnFilterDb.Size = new System.Drawing.Size(100, 30);
        this.btnFilterDb.Click += BtnFilterDb_Click;
        this.tabDatabase.Controls.Add(this.btnFilterDb);
        // Checkbox: Filter DBs that contain tables starting with prefix
        this.chkMatchTables = new System.Windows.Forms.CheckBox();
        this.chkMatchTables.Text = "Only DBs containing matching tables";
        this.chkMatchTables.AutoSize = true;
        this.chkMatchTables.Location = new System.Drawing.Point(20, 440);
        this.tabDatabase.Controls.Add(this.chkMatchTables);

        // Checkbox: Show only longest DB name(s)
        this.chkLongestNames = new System.Windows.Forms.CheckBox();
        this.chkLongestNames.Text = "Show only longest DB name(s)";
        this.chkLongestNames.AutoSize = true;
        this.chkLongestNames.Location = new System.Drawing.Point(20, 470);
        this.tabDatabase.Controls.Add(this.chkLongestNames);

        // Button: Show DB(s) with most tables
        this.btnMostTables = new System.Windows.Forms.Button();
        this.btnMostTables.Text = "Show DB(s) with Most Tables";
        this.btnMostTables.Location = new System.Drawing.Point(20, 500);
        this.btnMostTables.Size = new System.Drawing.Size(250, 30);
        this.btnMostTables.Click += BtnMostTables_Click;
        this.tabDatabase.Controls.Add(this.btnMostTables);

        // Label: Filter Results
        this.lblFilterResults = new System.Windows.Forms.Label();
        this.lblFilterResults.Text = "Filtered Databases:";
        this.lblFilterResults.AutoSize = true;
        this.lblFilterResults.Location = new System.Drawing.Point(20, 535);
        this.tabDatabase.Controls.Add(this.lblFilterResults);

        // ComboBox: Filter results
        this.cmbFilteredDbs = new System.Windows.Forms.ComboBox();
        this.cmbFilteredDbs.Location = new System.Drawing.Point(200, 535);
        this.cmbFilteredDbs.Width = 320;
        this.cmbFilteredDbs.DropDownStyle = ComboBoxStyle.DropDownList;
        this.tabDatabase.Controls.Add(this.cmbFilteredDbs);

        // Label: Describe DB filter condition
        this.lblDbConditionDesc = new Label();
        this.lblDbConditionDesc.Text = "Condition: Odd-length names with >=3 tables or 0 tables";
        this.lblDbConditionDesc.AutoSize = true;
        this.lblDbConditionDesc.Location = new System.Drawing.Point(20, 590);
        this.tabDatabase.Controls.Add(this.lblDbConditionDesc);

        // Button: Apply condition filter
        this.btnApplyConditionFilter = new Button();
        this.btnApplyConditionFilter.Text = "Show Condition Result";
        this.btnApplyConditionFilter.Location = new System.Drawing.Point(525, 610);
        this.btnApplyConditionFilter.AutoSize = true;
        this.btnApplyConditionFilter.Click += BtnApplyDbCondition_Click;
        this.tabDatabase.Controls.Add(this.btnApplyConditionFilter);

        // ComboBox: Display condition results
        this.cmbConditionResults = new ComboBox();
        this.cmbConditionResults.Location = new System.Drawing.Point(20, 615);
        this.cmbConditionResults.Width = lblDbConditionDesc.Width;
        this.cmbConditionResults.DropDownStyle = ComboBoxStyle.DropDownList;
        this.tabDatabase.Controls.Add(this.cmbConditionResults);

        // Button: Count All Tables
        this.btnCountAllTables = new System.Windows.Forms.Button();
        this.btnCountAllTables.Text = "Count All Tables in DBs";
        this.btnCountAllTables.Location = new System.Drawing.Point(20, 660);
        this.btnCountAllTables.Size = new System.Drawing.Size(200, 35);
        this.btnCountAllTables.Click += BtnCountAllTables_Click;
        this.tabDatabase.Controls.Add(this.btnCountAllTables);

        // Label: Total Tables Result
        this.lblTotalTables = new System.Windows.Forms.Label();
        this.lblTotalTables.Text = "Total tables in all DBs: ";
        this.lblTotalTables.AutoSize = true;
        this.lblTotalTables.Location = new System.Drawing.Point(250, 660);
        this.tabDatabase.Controls.Add(this.lblTotalTables);

        // Label: Enter table count
        this.lblExactTableCount = new Label();
        this.lblExactTableCount.Text = "Enter exact table count:";
        this.lblExactTableCount.Location = new System.Drawing.Point(20, 710);
        this.lblExactTableCount.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblExactTableCount);

        // TextBox: User input for table count
        this.txtExactTableCount = new TextBox();
        this.txtExactTableCount.Location = new System.Drawing.Point(220, 705);
        this.txtExactTableCount.Width = 100;
        this.tabDatabase.Controls.Add(this.txtExactTableCount);

        // Button: Show DBs with exact table count
        this.btnExactTableCount = new Button();
        this.btnExactTableCount.Text = "Show DBs";
        this.btnExactTableCount.Location = new System.Drawing.Point(330, 703);
        this.btnExactTableCount.Size = new System.Drawing.Size(120, 30);
        this.btnExactTableCount.Click += BtnExactTableCount_Click;
        this.tabDatabase.Controls.Add(this.btnExactTableCount);

        // ComboBox: Display result
        this.cmbExactTableCountResult = new ComboBox();
        this.cmbExactTableCountResult.Location = new System.Drawing.Point(20, 740);
        this.cmbExactTableCountResult.Width = lblDbConditionDesc.Width;
        this.cmbExactTableCountResult.DropDownStyle = ComboBoxStyle.DropDownList;
        this.tabDatabase.Controls.Add(this.cmbExactTableCountResult);

        #region Find Database UI

        // Label
        this.lblCheckDb = new System.Windows.Forms.Label();
        this.lblCheckDb.Text = "Check if DB Exists:";
        this.lblCheckDb.AutoSize = true;
        this.lblCheckDb.Location = new System.Drawing.Point(20, 790);
        this.tabDatabase.Controls.Add(this.lblCheckDb);

        // TextBox
        this.txtCheckDb = new System.Windows.Forms.TextBox();
        this.txtCheckDb.Location = new System.Drawing.Point(200, 785);
        this.txtCheckDb.Width = 200;
        this.tabDatabase.Controls.Add(this.txtCheckDb);

        // Button
        this.btnCheckDb = new System.Windows.Forms.Button();
        this.btnCheckDb.Text = "Check";
        this.btnCheckDb.Location = new System.Drawing.Point(420, 782);
        this.btnCheckDb.Size = new System.Drawing.Size(100, 30);
        this.btnCheckDb.Click += BtnCheckDbExists_Click;
        this.tabDatabase.Controls.Add(this.btnCheckDb);

        // Result Label
        this.lblCheckDbResult = new System.Windows.Forms.Label();
        this.lblCheckDbResult.AutoSize = true;
        this.lblCheckDbResult.Location = new System.Drawing.Point(530, 787);
        this.lblCheckDbResult.Text = "";
        this.tabDatabase.Controls.Add(this.lblCheckDbResult);

        #endregion

        #endregion

        #endregion

        #region ContainersTab
        // ===========================================
        // ========== CONTAINERS / TABLES TAB ========
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


        // Tables comboBox
        this.listTables = new ComboBox();
        this.listTables.Location = new System.Drawing.Point(20, 115);
        this.listTables.Width = 300;
        this.listTables.DropDownStyle = ComboBoxStyle.DropDownList;
        this.tabContainers.Controls.Add(this.listTables);

        // Label table count
        this.lblTableCount = new Label();
        this.lblTableCount.Text = "Tables: 0";
        this.lblTableCount.Location = new System.Drawing.Point(20, 150);
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

        // ================================
        // Check Table Existence in DBs
        // ================================

        // Label: Table Name
        this.lblCheckTable = new Label();
        this.lblCheckTable.Text = "Table Name to Check:";
        this.lblCheckTable.AutoSize = true;
        this.lblCheckTable.Location = new System.Drawing.Point(400, 200);
        this.tabContainers.Controls.Add(this.lblCheckTable);

        // TextBox: Table Name Input
        this.txtCheckTable = new TextBox();
        this.txtCheckTable.Location = new System.Drawing.Point(400, 225);
        this.txtCheckTable.Width = 250;
        this.tabContainers.Controls.Add(this.txtCheckTable);

        // Button: Check Table
        this.btnCheckTable = new Button();
        this.btnCheckTable.Text = "Check Table";
        this.btnCheckTable.Location = new System.Drawing.Point(660, 223);
        this.btnCheckTable.Size = new System.Drawing.Size(120, 30);
        this.btnCheckTable.Click += BtnCheckTable_Click;
        this.tabContainers.Controls.Add(this.btnCheckTable);

        // ListBox: Result DBs
        this.lstCheckTableResult = new ListBox();
        this.lstCheckTableResult.Location = new System.Drawing.Point(400, 265);
        this.lstCheckTableResult.Size = new System.Drawing.Size(380, 150);
        this.tabContainers.Controls.Add(this.lstCheckTableResult);

        // Label: Enter table name length
        this.lblMinTableLength = new Label();
        this.lblMinTableLength.Text = "Table Name Min Length:";
        this.lblMinTableLength.Location = new System.Drawing.Point(20, 180);
        this.lblMinTableLength.AutoSize = true;
        this.tabContainers.Controls.Add(this.lblMinTableLength);

        // TextBox: User input for table length
        this.txtMinTableLengthInput = new TextBox();
        this.txtMinTableLengthInput.Location = new System.Drawing.Point(220, 175);
        this.txtMinTableLengthInput.Width = 100;
        this.tabContainers.Controls.Add(this.txtMinTableLengthInput);

        // Button: Show tables names longer than input length
        this.btnMinTableName = new Button();
        this.btnMinTableName.Text = "Filter Table names (non-inclusive)";
        this.btnMinTableName.Location = new System.Drawing.Point(20, 210);
        this.btnMinTableName.AutoSize = true;
        this.btnMinTableName.Click += BtnMinTableName_Click;
        this.tabContainers.Controls.Add(this.btnMinTableName);

        // ComboBox: Display result
        this.cmbMinLegthTables = new ComboBox();
        this.cmbMinLegthTables.Location = new System.Drawing.Point(20, 290);
        this.cmbMinLegthTables.Width = comboDbTables.Width;
        this.cmbMinLegthTables.DropDownStyle = ComboBoxStyle.DropDownList;
        this.tabContainers.Controls.Add(this.cmbMinLegthTables);

        // Checkbox: Show DB names only once
        this.chkDbNameOnly = new CheckBox();
        this.chkDbNameOnly.Text = "Return DB names without duplicate entries";
        this.chkDbNameOnly.Location = new System.Drawing.Point(20, 250);
        this.chkDbNameOnly.AutoSize = true;
        this.tabContainers.Controls.Add(this.chkDbNameOnly);

        #endregion

        #region DocumentsTab
        // ===========================================
        // ============= DOCUMENTS TAB ===============
        // ===========================================

        // Button: Count ALL
        this.btnCountAllDocs = new Button();
        this.btnCountAllDocs.Location = new System.Drawing.Point(20, 20);
        this.btnCountAllDocs.Name = "btnCountAllDocs";
        this.btnCountAllDocs.Size = new System.Drawing.Size(180, 35);
        this.btnCountAllDocs.Text = "Count ALL Items (Global)";
        this.btnCountAllDocs.UseVisualStyleBackColor = true;
        this.btnCountAllDocs.UseVisualStyleBackColor = true;
        this.btnCountAllDocs.Click += new System.EventHandler(this.BtnCountAllDocs_Click);

        // Label: Global Result
        this.lblDocCountResult = new Label();
        this.lblDocCountResult.AutoSize = true;
        this.lblDocCountResult.Location = new System.Drawing.Point(220, 28);
        this.lblDocCountResult.Name = "lblDocCountResult";
        this.lblDocCountResult.Text = "Total Items: -";

        // Label: Instruction
        this.lblCountItemsInDb = new Label();
        this.lblCountItemsInDb.AutoSize = true;
        this.lblCountItemsInDb.Location = new System.Drawing.Point(20, 60);
        this.lblCountItemsInDb.Name = "lblCountItemsInDb";
        this.lblCountItemsInDb.Text = "Count Items in specific DB:";

        // TextBox: Input
        this.txtCountItemsInDb = new TextBox();
        this.txtCountItemsInDb.Location = new System.Drawing.Point(20, 85);
        this.txtCountItemsInDb.Width = 200;
        this.txtCountItemsInDb.TextChanged += TxtCountItemsInDb_TextChanged;

        // Label: Green Checkmark
        this.lblCountItemsDbCheck = new Label();
        this.lblCountItemsDbCheck.AutoSize = true;
        this.lblCountItemsDbCheck.Location = new System.Drawing.Point(230, 88);
        this.lblCountItemsDbCheck.Name = "lblCountItemsDbCheck";
        this.lblCountItemsDbCheck.Text = "";
        this.lblCountItemsDbCheck.ForeColor = Color.Green;

        // Button: Action
        this.btnCountItemsInDb = new Button();
        this.btnCountItemsInDb.Location = new System.Drawing.Point(270, 83);
        this.btnCountItemsInDb.Size = new System.Drawing.Size(180, 35);
        this.btnCountItemsInDb.Text = "Count Items in DB";
        this.btnCountItemsInDb.Click += this.BtnCountAllDocsInDb_Click;

        // Label: Specific Result
        this.lblCountItemsInDbResult = new Label();
        this.lblCountItemsInDbResult.AutoSize = true;
        this.lblCountItemsInDbResult.Location = new System.Drawing.Point(520, 88);
        this.lblCountItemsInDbResult.Text = "Result: -";

        // -------------------------------------------------------
        // Add to TabPage
        // -------------------------------------------------------
        this.tabDocuments.Controls.Add(this.btnCountAllDocs);
        this.tabDocuments.Controls.Add(this.lblDocCountResult);

        this.tabDocuments.Controls.Add(this.lblCountItemsInDb);
        this.tabDocuments.Controls.Add(this.txtCountItemsInDb);
        this.tabDocuments.Controls.Add(this.lblCountItemsDbCheck);
        this.tabDocuments.Controls.Add(this.btnCountItemsInDb);
        this.tabDocuments.Controls.Add(this.lblCountItemsInDbResult);

        #endregion

        #region Client/Student Info
        // Database name
        this.lblClientDbName = new Label();
        this.lblClientDbName.Text = "Database Name:";
        this.lblClientDbName.Location = new System.Drawing.Point(20, 20);
        this.lblClientDbName.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientDbName);

        this.txtClientDbName = new TextBox();
        this.txtClientDbName.Location = new System.Drawing.Point(180, 20);
        this.txtClientDbName.Width = 200;
        this.txtClientDbName.TextChanged += TxtClientDbName_TextChanged;
        this.tabClient.Controls.Add(this.txtClientDbName);

        // DB Exists indicator
        this.lblDbCheck = new Label();
        this.lblDbCheck.Text = "";
        this.lblDbCheck.ForeColor = Color.Green;
        this.lblDbCheck.Location = new System.Drawing.Point(390, 20);
        this.lblDbCheck.AutoSize = true;
        this.tabClient.Controls.Add(this.lblDbCheck);

        // Table name
        this.lblClientTableName = new Label();
        this.lblClientTableName.Text = "Table Name:";
        this.lblClientTableName.Location = new System.Drawing.Point(20, 60);
        this.lblClientTableName.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientTableName);

        this.txtClientTableName = new TextBox();
        this.txtClientTableName.Location = new System.Drawing.Point(180, 60);
        this.txtClientTableName.Width = 200;
        this.txtClientTableName.TextChanged += TxtClientTableName_TextChanged;
        this.tabClient.Controls.Add(this.txtClientTableName);

        // Table exists indicator
        this.lblTbCheck = new Label();
        this.lblTbCheck.Text = "";
        this.lblTbCheck.ForeColor = Color.Green;
        this.lblTbCheck.Location = new System.Drawing.Point(390, 60);
        this.lblTbCheck.AutoSize = true;
        this.tabClient.Controls.Add(this.lblTbCheck);

        /// Document ID (Cosmos PK)
        this.lblClientId = new Label();
        this.lblClientId.Text = "Document ID:";
        this.lblClientId.Location = new System.Drawing.Point(20, 100);
        this.lblClientId.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientId);

        // Document exists indicator
        this.lblIdCheck = new Label();
        this.lblIdCheck.Text = "";
        this.lblIdCheck.ForeColor = Color.Green;
        this.lblIdCheck.Location = new System.Drawing.Point(390, 100);
        this.lblIdCheck.AutoSize = true;
        this.tabClient.Controls.Add(this.lblIdCheck);

        this.txtClientId = new TextBox();
        this.txtClientId.Location = new System.Drawing.Point(180, 100);
        this.txtClientId.Width = 200;
        this.txtClientId.TextChanged += TxtClientId_TextChanged;
        this.tabClient.Controls.Add(this.txtClientId);

        // student ID (Tz)
        this.lblClientTz = new Label();
        this.lblClientTz.Text = "Student ID (Tz):";
        this.lblClientTz.Location = new System.Drawing.Point(20, 140);
        this.lblClientTz.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientTz);

        this.txtClientTz = new TextBox();
        this.txtClientTz.Location = new System.Drawing.Point(180, 140);
        this.txtClientTz.Width = 200;
        this.tabClient.Controls.Add(this.txtClientTz);

        // First Name
        this.lblClientFirstName = new Label();
        this.lblClientFirstName.Text = "First Name:";
        this.lblClientFirstName.Location = new System.Drawing.Point(20, 180);
        this.lblClientFirstName.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientFirstName);

        this.txtClientFirstName = new TextBox();
        this.txtClientFirstName.Location = new System.Drawing.Point(180, 180);
        this.txtClientFirstName.Width = 200;
        this.tabClient.Controls.Add(this.txtClientFirstName);

        // Last Name
        this.lblClientLastName = new Label();
        this.lblClientLastName.Text = "Last Name:";
        this.lblClientLastName.Location = new System.Drawing.Point(20, 220);
        this.lblClientLastName.AutoSize = true;
        this.tabClient.Controls.Add(this.lblClientLastName);

        this.txtClientLastName = new TextBox();
        this.txtClientLastName.Location = new System.Drawing.Point(180, 220);
        this.txtClientLastName.Width = 200;
        this.tabClient.Controls.Add(this.txtClientLastName);

        // Save button
        this.btnSaveClient = new Button();
        this.btnSaveClient.Text = "Save Student Info";
        this.btnSaveClient.Location = new System.Drawing.Point(20, 260);
        this.btnSaveClient.AutoSize = true;
        this.btnSaveClient.Click += BtnSaveClient_Click;
        this.tabClient.Controls.Add(this.btnSaveClient);

        #region JSON / Cloud Insert Section (Client Tab)

        // Button: Load JSON from file
        this.btnLoadJsonFile = new Button();
        this.btnLoadJsonFile.Text = "File from JSON Load";
        this.btnLoadJsonFile.Location = new System.Drawing.Point(20, 300);
        this.btnLoadJsonFile.Size = new System.Drawing.Size(200, 30);
        this.btnLoadJsonFile.Click += BtnLoadJsonFile_Click;
        this.tabClient.Controls.Add(this.btnLoadJsonFile);

        // RichTextBox: Display JSON content
        this.txtJsonContent = new RichTextBox();
        this.txtJsonContent.Location = new System.Drawing.Point(20, 335);
        this.txtJsonContent.Size = new System.Drawing.Size(560, 190);
        this.tabClient.Controls.Add(this.txtJsonContent);

        // Button: Insert to Cloud
        this.btnInsertToCloud = new Button();
        this.btnInsertToCloud.Text = "Insert to Cloud";
        this.btnInsertToCloud.Location = new System.Drawing.Point(20, 545);
        this.btnInsertToCloud.Size = new System.Drawing.Size(120, 35);
        this.btnInsertToCloud.Click += BtnInsertToCloud_Click;
        this.tabClient.Controls.Add(this.btnInsertToCloud);

        // Button: Update in Cloud
        this.btnUpdateCloud = new Button();
        this.btnUpdateCloud.Text = "Update in Cloud";
        this.btnUpdateCloud.Location = new System.Drawing.Point(150, 545);
        this.btnUpdateCloud.Size = new System.Drawing.Size(120, 35);
        this.btnUpdateCloud.Click += BtnUpdateCloud_Click;
        this.tabClient.Controls.Add(this.btnUpdateCloud);

        // Button: Delete from Cloud
        this.btnDeleteCloud = new Button();
        this.btnDeleteCloud.Text = "Delete from Cloud";
        this.btnDeleteCloud.Location = new System.Drawing.Point(280, 545);
        this.btnDeleteCloud.Size = new System.Drawing.Size(120, 35);
        this.btnDeleteCloud.Click += BtnDeleteCloud_Click;
        this.tabClient.Controls.Add(this.btnDeleteCloud);

        // Button: Read / Fetch from Cloud
        this.btnReadCloud = new Button();
        this.btnReadCloud.Text = "Read from Cloud";
        this.btnReadCloud.Location = new System.Drawing.Point(410, 545);
        this.btnReadCloud.Size = new System.Drawing.Size(120, 35);
        this.btnReadCloud.Click += BtnReadCloud_Click;
        this.tabClient.Controls.Add(this.btnReadCloud);

        // Button: Clear JSON
        this.btnClearJson = new Button();
        this.btnClearJson.Text = "Clear JSON";
        this.btnClearJson.Location = new System.Drawing.Point(230, 300);
        this.btnClearJson.Size = new System.Drawing.Size(100, 30);
        this.btnClearJson.UseVisualStyleBackColor = true;
        this.btnClearJson.Click += new System.EventHandler(this.BtnClearJson_Click);
        this.tabClient.Controls.Add(this.btnClearJson);

        // Label: JSON operation status
        this.lblJsonStatus = new Label();
        this.lblJsonStatus.Text = "";
        this.lblJsonStatus.AutoSize = true;
        this.lblJsonStatus.Location = new System.Drawing.Point(20, 580);
        this.tabClient.Controls.Add(this.lblJsonStatus);

        #endregion

        #endregion
    }
    #endregion
}
