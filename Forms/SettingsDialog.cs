using System.Windows.Forms;

namespace Addon.Forms
{
    internal class SettingsDialog : Form
    {
        private Button _cancelBbutton;
        private Button _acceptButton;
        private CheckBox _checkUpdateOnStart;

        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this._checkUpdateOnStart = new System.Windows.Forms.CheckBox();
            this._cancelBbutton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _checkUpdateOnStart
            // 
            this._checkUpdateOnStart.AutoSize = true;
            this._checkUpdateOnStart.Location = new System.Drawing.Point(23, 25);
            this._checkUpdateOnStart.Name = "_checkUpdateOnStart";
            this._checkUpdateOnStart.Size = new System.Drawing.Size(178, 17);
            this._checkUpdateOnStart.TabIndex = 0;
            this._checkUpdateOnStart.Text = "Check update on start AutoCAD";
            this._checkUpdateOnStart.UseVisualStyleBackColor = true;
            // 
            // _cancelBbutton
            // 
            this._cancelBbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelBbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBbutton.Location = new System.Drawing.Point(283, 114);
            this._cancelBbutton.Name = "_cancelBbutton";
            this._cancelBbutton.Size = new System.Drawing.Size(75, 23);
            this._cancelBbutton.TabIndex = 12;
            this._cancelBbutton.Text = "Cancel";
            this._cancelBbutton.UseVisualStyleBackColor = true;
            // 
            // _acceptButton
            // 
            this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._acceptButton.Location = new System.Drawing.Point(202, 114);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 11;
            this._acceptButton.Text = "OK";
            this._acceptButton.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this._acceptButton;
            this.CancelButton = this._cancelBbutton;
            this.ClientSize = new System.Drawing.Size(370, 149);
            this.Controls.Add(this._cancelBbutton);
            this.Controls.Add(this._acceptButton);
            this.Controls.Add(this._checkUpdateOnStart);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
