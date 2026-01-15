using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CosmosExplorerApp
{
    public class DocumentViewForm : Form
    {
        private RichTextBox rtbContent = null!;
        private RadioButton rbRaw = null!;
        private RadioButton rbFiltered = null!;
        private Button btnClose = null!;
        private Label lblId = null!;
        private JObject? _currentDoc;

        public DocumentViewForm(string docId, JObject? doc)
        {
            _currentDoc = doc;
            InitializeComponent(docId);

            rbRaw.Checked = true;
            UpdateView();
        }

        private void InitializeComponent(string docId)
        {
            this.Text = $"Document Viewer - {docId}";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterParent;

            // Panel
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 50;
            this.Controls.Add(topPanel);

            // Label
            lblId = new Label();
            lblId.Text = $"ID: {docId}";
            lblId.AutoSize = true;
            lblId.Location = new Point(10, 15);
            lblId.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            topPanel.Controls.Add(lblId);

            // Radio Button: Raw
            rbRaw = new RadioButton();
            rbRaw.Text = "Raw JSON";
            rbRaw.Location = new Point(200, 12);
            rbRaw.AutoSize = true;
            rbRaw.CheckedChanged += (s, e) => UpdateView();
            topPanel.Controls.Add(rbRaw);

            // Radio Button: Filtered
            rbFiltered = new RadioButton();
            rbFiltered.Text = "Filtered (Preview)";
            rbFiltered.Location = new Point(300, 12);
            rbFiltered.AutoSize = true;
            rbFiltered.CheckedChanged += (s, e) => UpdateView();
            topPanel.Controls.Add(rbFiltered);

            // Close Button
            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Location = new Point(480, 10);
            btnClose.Click += (s, e) => this.Close();
            topPanel.Controls.Add(btnClose);

            rtbContent = new RichTextBox();
            rtbContent.Location = new Point(10, 60);
            rtbContent.Size = new Size(this.ClientSize.Width - 25, this.ClientSize.Height - 75);
            rtbContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            rtbContent.Font = new Font("Consolas", 10);
            rtbContent.ReadOnly = true;
            rtbContent.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbContent.BackColor = Color.White;

            this.Controls.Add(rtbContent);
        }

        private void UpdateView()
        {
            if (_currentDoc == null)
            {
                rtbContent.Clear();
                rtbContent.Text = "Error: Document data is missing.";
                rtbContent.ForeColor = Color.Red;
                return;
            }

            if (rbRaw.Checked)
            {
                rtbContent.Text = _currentDoc.ToString();
                rtbContent.ForeColor = Color.Black;
            }
            else
            {
                rtbContent.Clear();
                IEnumerable<JProperty> sortedProps = _currentDoc.Properties()
                    .Where(p => p.Name != "id" && !p.Name.StartsWith("_"))
                    .OrderBy(p => p.Value is JValue ? 0 : 1)
                    .ThenBy(p => p.Name);

                List<string> previewProps = sortedProps
                    .Take(30)
                    .Select(p => $" {p.Name}: {LimitLength(p.Value.ToString(), 80)}")
                    .ToList();

                rtbContent.Text = string.Join(Environment.NewLine, previewProps);
                rtbContent.ForeColor = Color.Blue;
            }
        }

        private string LimitLength(string source, int maxLength)
        {
            if (string.IsNullOrEmpty(source)) return "";
            string clean = source.Replace("\r", "").Replace("\n", " ");
            if (clean.Length <= maxLength) return clean;
            return clean.Substring(0, maxLength) + "...";
        }
    }
}