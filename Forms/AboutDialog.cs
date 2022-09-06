using System.Windows.Forms;

namespace Addon.Forms
{
    internal class AboutDialog : Form
    {
        private Label _titleLabel;
        private Label _versionLabel;
        private Button _acceptButton;

        public AboutDialog()
        {
            InitializeComponent();

            _versionLabel.Text += $" {Infrastructure.Addon.AssemblyFileVersion}";
        }

        private void InitializeComponent()
        {
            this._acceptButton = new System.Windows.Forms.Button();
            this._titleLabel = new System.Windows.Forms.Label();
            this._versionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _acceptButton
            // 
            this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._acceptButton.Location = new System.Drawing.Point(217, 74);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "OK";
            this._acceptButton.UseVisualStyleBackColor = true;
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Location = new System.Drawing.Point(12, 18);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(201, 13);
            this._titleLabel.TabIndex = 1;
            this._titleLabel.Text = "EDEC Inc. plugin for Autodesk AutoCAD ";
            // 
            // _versionLabel
            // 
            this._versionLabel.AutoSize = true;
            this._versionLabel.Location = new System.Drawing.Point(12, 41);
            this._versionLabel.Name = "_versionLabel";
            this._versionLabel.Size = new System.Drawing.Size(45, 13);
            this._versionLabel.TabIndex = 2;
            this._versionLabel.Text = "Version ";
            // 
            // AboutDialog
            // 
            this.AcceptButton = this._acceptButton;
            this.ClientSize = new System.Drawing.Size(304, 109);
            this.Controls.Add(this._versionLabel);
            this.Controls.Add(this._titleLabel);
            this.Controls.Add(this._acceptButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
