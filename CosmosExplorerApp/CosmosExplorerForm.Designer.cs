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
        // Form Setup
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Text = "Cosmos Explorer App";

        // TabControl
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabDatabase = new System.Windows.Forms.TabPage();
        this.tabContainers = new System.Windows.Forms.TabPage();
        this.tabDocuments = new System.Windows.Forms.TabPage();

        // TabControl settings
        this.tabControl1.Location = new System.Drawing.Point(10, 10);
        this.tabControl1.Size = new System.Drawing.Size(760, 500);

        // TabPages
        this.tabDatabase.Text = "Databases";
        this.tabContainers.Text = "Containers";
        this.tabDocuments.Text = "Documents";

        // Add Tabs to TabControl
        this.tabControl1.Controls.Add(this.tabDatabase);
        this.tabControl1.Controls.Add(this.tabContainers);
        this.tabControl1.Controls.Add(this.tabDocuments);

        // Add TabControl to Form
        this.Controls.Add(this.tabControl1);

        // Label for Endpoint
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

        // Label for PKey
        this.lblPkey = new Label();
        this.lblPkey.Text = "Primary Key:";
        this.lblPkey.Location = new System.Drawing.Point(20, 75);
        this.lblPkey.AutoSize = true;
        this.tabDatabase.Controls.Add(lblPkey);

        // PrimaryKey textbox
        this.pkeyTxtBox = new TextBox();
        this.pkeyTxtBox.Location = new System.Drawing.Point(20, 100);
        this.pkeyTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(pkeyTxtBox);

        // Load URI and Pkey button
        this.btnLoadKeys = new Button();
        this.btnLoadKeys.Text = "Load Keys";
        this.btnLoadKeys.Location = new System.Drawing.Point(525, 100);
        this.btnLoadKeys.Size = new System.Drawing.Size(120, 35);
        this.btnLoadKeys.Click += BtnLoadKeys_Click;
        this.tabDatabase.Controls.Add(btnLoadKeys);

        // Label for Database Name
        this.lblDbName = new Label();
        this.lblDbName.Text = "Database Name:";
        this.lblDbName.Location = new System.Drawing.Point(20, 150);
        this.lblDbName.AutoSize = true;
        this.tabDatabase.Controls.Add(this.lblDbName);

        // TextBox for Database Name
        this.dbNameTxtBox = new TextBox();
        this.dbNameTxtBox.Location = new System.Drawing.Point(20, 175);
        this.dbNameTxtBox.Width = 500;
        this.tabDatabase.Controls.Add(this.dbNameTxtBox);

        // Create database button
        this.btnCreateDb = new Button();
        this.btnCreateDb.Text = "Create Database";
        this.btnCreateDb.Location = new System.Drawing.Point(525, 175);
        this.btnCreateDb.Size = new System.Drawing.Size(200, 35);
        this.btnCreateDb.Click += BtnCreateDb_Click;
        this.tabDatabase.Controls.Add(btnCreateDb);
        
        // Label for DbList
        this.lblListDb = new Label();
        this.lblListDb.Text = "Databases:";
        this.lblListDb.Location = new System.Drawing.Point(20, 245);
        this.tabDatabase.Controls.Add(this.lblListDb);

        // ListBox
        this.listDb = new ListBox();
        listDb.Location = new System.Drawing.Point(20, 270);
        listDb.Size = new System.Drawing.Size(500, 200);
        this.tabDatabase.Controls.Add(listDb);
    }

    #endregion
}
