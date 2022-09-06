using Addon.Models;
using System.Windows.Forms;

namespace Addon.Forms
{
    public partial class MainCircuitBreakerDialog : Form
    {
        private Button _cancelBbutton;
        private ComboBox _currentComboBox;
        private Label _currentLabel;
        private Label _aicLabel;
        private CheckBox _existsCheckBox;
        private Label _voltageLabel;
        private ComboBox _voltageComboBox;
        private Label _poleTypeLabel;
        private ComboBox _poleTypeComboBox;
        private ComboBox _aicComboBox;
        private ComboBox _nemaComboBox;
        private Label _nemaLabel;
        private ComboBox _materialComboBox;
        private Label _materialLabel;
        private ComboBox _protectionTypeComboBox;
        private Label _protectionTypeLabel;
        private Label _tagNumberLabel;
        private NumericUpDown _tagNumberNumericUpDown;
        private Button _acceptButton;

        private void InitializeComponent()
        {
            this._acceptButton = new System.Windows.Forms.Button();
            this._cancelBbutton = new System.Windows.Forms.Button();
            this._currentComboBox = new System.Windows.Forms.ComboBox();
            this._currentLabel = new System.Windows.Forms.Label();
            this._aicLabel = new System.Windows.Forms.Label();
            this._existsCheckBox = new System.Windows.Forms.CheckBox();
            this._voltageLabel = new System.Windows.Forms.Label();
            this._voltageComboBox = new System.Windows.Forms.ComboBox();
            this._poleTypeLabel = new System.Windows.Forms.Label();
            this._poleTypeComboBox = new System.Windows.Forms.ComboBox();
            this._aicComboBox = new System.Windows.Forms.ComboBox();
            this._nemaComboBox = new System.Windows.Forms.ComboBox();
            this._nemaLabel = new System.Windows.Forms.Label();
            this._materialComboBox = new System.Windows.Forms.ComboBox();
            this._materialLabel = new System.Windows.Forms.Label();
            this._protectionTypeComboBox = new System.Windows.Forms.ComboBox();
            this._protectionTypeLabel = new System.Windows.Forms.Label();
            this._tagNumberLabel = new System.Windows.Forms.Label();
            this._tagNumberNumericUpDown = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this._tagNumberNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // _acceptButton
            // 
            this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._acceptButton.Location = new System.Drawing.Point(88, 269);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 9;
            this._acceptButton.Text = "OK";
            this._acceptButton.UseVisualStyleBackColor = true;
            // 
            // _cancelBbutton
            // 
            this._cancelBbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelBbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBbutton.Location = new System.Drawing.Point(169, 269);
            this._cancelBbutton.Name = "_cancelBbutton";
            this._cancelBbutton.Size = new System.Drawing.Size(75, 23);
            this._cancelBbutton.TabIndex = 10;
            this._cancelBbutton.Text = "Cancel";
            this._cancelBbutton.UseVisualStyleBackColor = true;
            // 
            // _currentComboBox
            // 
            this._currentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._currentComboBox.FormattingEnabled = true;
            this._currentComboBox.Location = new System.Drawing.Point(108, 91);
            this._currentComboBox.Name = "_currentComboBox";
            this._currentComboBox.Size = new System.Drawing.Size(121, 21);
            this._currentComboBox.TabIndex = 3;
            // 
            // _currentLabel
            // 
            this._currentLabel.AutoSize = true;
            this._currentLabel.Location = new System.Drawing.Point(39, 94);
            this._currentLabel.Name = "_currentLabel";
            this._currentLabel.Size = new System.Drawing.Size(54, 13);
            this._currentLabel.TabIndex = 3;
            this._currentLabel.Text = "Current, A";
            // 
            // _aicLabel
            // 
            this._aicLabel.AutoSize = true;
            this._aicLabel.Location = new System.Drawing.Point(69, 121);
            this._aicLabel.Name = "_aicLabel";
            this._aicLabel.Size = new System.Drawing.Size(24, 13);
            this._aicLabel.TabIndex = 4;
            this._aicLabel.Text = "AIC";
            // 
            // _existsCheckBox
            // 
            this._existsCheckBox.AutoSize = true;
            this._existsCheckBox.Location = new System.Drawing.Point(108, 14);
            this._existsCheckBox.Name = "_existsCheckBox";
            this._existsCheckBox.Size = new System.Drawing.Size(53, 17);
            this._existsCheckBox.TabIndex = 0;
            this._existsCheckBox.Text = "Exists";
            this._existsCheckBox.UseVisualStyleBackColor = true;
            // 
            // _voltageLabel
            // 
            this._voltageLabel.AutoSize = true;
            this._voltageLabel.Location = new System.Drawing.Point(37, 40);
            this._voltageLabel.Name = "_voltageLabel";
            this._voltageLabel.Size = new System.Drawing.Size(56, 13);
            this._voltageLabel.TabIndex = 6;
            this._voltageLabel.Text = "Voltage, V";
            // 
            // _voltageComboBox
            // 
            this._voltageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._voltageComboBox.FormattingEnabled = true;
            this._voltageComboBox.Location = new System.Drawing.Point(108, 37);
            this._voltageComboBox.Name = "_voltageComboBox";
            this._voltageComboBox.Size = new System.Drawing.Size(121, 21);
            this._voltageComboBox.TabIndex = 1;
            // 
            // _poleTypeLabel
            // 
            this._poleTypeLabel.AutoSize = true;
            this._poleTypeLabel.Location = new System.Drawing.Point(39, 67);
            this._poleTypeLabel.Name = "_poleTypeLabel";
            this._poleTypeLabel.Size = new System.Drawing.Size(55, 13);
            this._poleTypeLabel.TabIndex = 8;
            this._poleTypeLabel.Text = "Pole Type";
            // 
            // _poleTypeComboBox
            // 
            this._poleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._poleTypeComboBox.FormattingEnabled = true;
            this._poleTypeComboBox.Location = new System.Drawing.Point(108, 64);
            this._poleTypeComboBox.Name = "_poleTypeComboBox";
            this._poleTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this._poleTypeComboBox.TabIndex = 2;
            // 
            // _aicComboBox
            // 
            this._aicComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._aicComboBox.FormattingEnabled = true;
            this._aicComboBox.Location = new System.Drawing.Point(108, 118);
            this._aicComboBox.Name = "_aicComboBox";
            this._aicComboBox.Size = new System.Drawing.Size(121, 21);
            this._aicComboBox.TabIndex = 4;
            // 
            // _nemaComboBox
            // 
            this._nemaComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._nemaComboBox.FormattingEnabled = true;
            this._nemaComboBox.Location = new System.Drawing.Point(108, 145);
            this._nemaComboBox.Name = "_nemaComboBox";
            this._nemaComboBox.Size = new System.Drawing.Size(121, 21);
            this._nemaComboBox.TabIndex = 5;
            // 
            // _nemaLabel
            // 
            this._nemaLabel.AutoSize = true;
            this._nemaLabel.Location = new System.Drawing.Point(55, 148);
            this._nemaLabel.Name = "_nemaLabel";
            this._nemaLabel.Size = new System.Drawing.Size(38, 13);
            this._nemaLabel.TabIndex = 11;
            this._nemaLabel.Text = "NEMA";
            // 
            // _materialComboBox
            // 
            this._materialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._materialComboBox.FormattingEnabled = true;
            this._materialComboBox.Location = new System.Drawing.Point(108, 172);
            this._materialComboBox.Name = "_materialComboBox";
            this._materialComboBox.Size = new System.Drawing.Size(121, 21);
            this._materialComboBox.TabIndex = 6;
            // 
            // _materialLabel
            // 
            this._materialLabel.AutoSize = true;
            this._materialLabel.Location = new System.Drawing.Point(55, 175);
            this._materialLabel.Name = "_materialLabel";
            this._materialLabel.Size = new System.Drawing.Size(44, 13);
            this._materialLabel.TabIndex = 13;
            this._materialLabel.Text = "Material";
            // 
            // _protectionTypeComboBox
            // 
            this._protectionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._protectionTypeComboBox.FormattingEnabled = true;
            this._protectionTypeComboBox.Location = new System.Drawing.Point(108, 199);
            this._protectionTypeComboBox.Name = "_protectionTypeComboBox";
            this._protectionTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this._protectionTypeComboBox.TabIndex = 7;
            // 
            // _protectionTypeLabel
            // 
            this._protectionTypeLabel.AutoSize = true;
            this._protectionTypeLabel.Location = new System.Drawing.Point(12, 202);
            this._protectionTypeLabel.Name = "_protectionTypeLabel";
            this._protectionTypeLabel.Size = new System.Drawing.Size(82, 13);
            this._protectionTypeLabel.TabIndex = 15;
            this._protectionTypeLabel.Text = "Protection Type";
            // 
            // _tagNumberLabel
            // 
            this._tagNumberLabel.AutoSize = true;
            this._tagNumberLabel.Location = new System.Drawing.Point(28, 228);
            this._tagNumberLabel.Name = "_tagNumberLabel";
            this._tagNumberLabel.Size = new System.Drawing.Size(66, 13);
            this._tagNumberLabel.TabIndex = 17;
            this._tagNumberLabel.Text = "Tag Number";
            // 
            // _tagNumberNumericUpDown
            // 
            this._tagNumberNumericUpDown.Location = new System.Drawing.Point(108, 226);
            this._tagNumberNumericUpDown.Name = "_tagNumberNumericUpDown";
            this._tagNumberNumericUpDown.Size = new System.Drawing.Size(121, 20);
            this._tagNumberNumericUpDown.TabIndex = 8;
            this._tagNumberNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MainCircuitBreakerDialog
            // 
            this.AcceptButton = this._acceptButton;
            this.CancelButton = this._cancelBbutton;
            this.ClientSize = new System.Drawing.Size(256, 304);
            this.Controls.Add(this._tagNumberNumericUpDown);
            this.Controls.Add(this._tagNumberLabel);
            this.Controls.Add(this._protectionTypeComboBox);
            this.Controls.Add(this._protectionTypeLabel);
            this.Controls.Add(this._materialComboBox);
            this.Controls.Add(this._materialLabel);
            this.Controls.Add(this._nemaComboBox);
            this.Controls.Add(this._nemaLabel);
            this.Controls.Add(this._aicComboBox);
            this.Controls.Add(this._poleTypeComboBox);
            this.Controls.Add(this._poleTypeLabel);
            this.Controls.Add(this._voltageComboBox);
            this.Controls.Add(this._voltageLabel);
            this.Controls.Add(this._existsCheckBox);
            this.Controls.Add(this._aicLabel);
            this.Controls.Add(this._currentLabel);
            this.Controls.Add(this._currentComboBox);
            this.Controls.Add(this._cancelBbutton);
            this.Controls.Add(this._acceptButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainCircuitBreakerDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Main Circuit Breaker Options";
            ((System.ComponentModel.ISupportInitialize)(this._tagNumberNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public bool Existing
        {
            get => _existsCheckBox.Checked;
            set => _existsCheckBox.Checked = value;
        }

        public int? Voltage
        {
            get
            {
                if (int.TryParse(_voltageComboBox.Text, out int val))
                {
                    return val;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _voltageComboBox.Text = value.HasValue.ToString();
                }
                else
                {
                    _voltageComboBox.SelectedIndex = 0;
                }
            }
        }

        public string PoleType
        {
            get => _poleTypeComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _poleTypeComboBox.Text = value;
                }
                else
                {
                    _poleTypeComboBox.SelectedIndex = 0;
                }
            }
        }

        public int? Current
        {
            get
            {
                if (int.TryParse(_currentComboBox.Text, out int val))
                {
                    return val;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _currentComboBox.Text = value.HasValue.ToString();
                }
                else
                {
                    _currentComboBox.SelectedIndex = 0;
                }
            }
        }

        public string Aic
        {
            get => _aicComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _aicComboBox.Text = value;
                }
                else
                {
                    _aicComboBox.SelectedIndex = 0;
                }
            }
        }

        public string Nema
        {
            get => _nemaComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _nemaComboBox.Text = value;
                }
                else
                {
                    _nemaComboBox.SelectedIndex = 0;
                }
            }
        }

        public string Material
        {
            get => _materialComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _materialComboBox.Text = value;
                }
                else
                {
                    _materialComboBox.SelectedIndex = 0;
                }
            }
        }

        public string ProtectionType
        {
            get => _protectionTypeComboBox.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _protectionTypeComboBox.Text = value;
                }
                else
                {
                    _protectionTypeComboBox.SelectedIndex = 0;
                }
            }
        }

        public decimal TagNumber
        {
            get => _tagNumberNumericUpDown.Value;
            set
            {
                if (value > 0)
                {
                    _tagNumberNumericUpDown.Value = value;
                }
                else
                {
                    _tagNumberNumericUpDown.Value = 1;
                }
            }
        }

        public MainCircuitBreakerDialog()
        {
            InitializeComponent();

            _voltageComboBox.DataSource = new int[]
            {
                120,
                240,
                277,
                480,
            };
            _poleTypeComboBox.DataSource = new string[]
            {
                "1P",
                "2P",
                "3P",
                "4P",
            };
            _currentComboBox.DataSource = new int[]
            {
                20,
                25,
                30,
                35,
                40,
                45,
                50,
                75,
                100,
                150,
                200,
                250,
            };
            _aicComboBox.DataSource = new string[]
            {
                "10k",
                "22k",
                "42k",
                "65k",
            };
            _nemaComboBox.DataSource = NemaType.Values;
            _materialComboBox.DataSource = new string[]
            {
                "Stainless Steel",
                "Cuprum",
            };
            _protectionTypeComboBox.DataSource = new string[]
            {
                "Thermal Magnetic",
                "Something else",
            };
        }
    }
}
