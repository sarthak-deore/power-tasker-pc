using System.Windows.Forms;
using System;

namespace PowerTaskerPC
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Panel pnlHome;
        private Label lblRelayStatus;
        private Label lblPublicKey;
        private Button btnGenerateKeys;
        private TextBox txtRelayUrl;
        private Button btnCheckConnection;
        private RadioButton radioEnabled;
        private RadioButton radioDisabled;
        private Label lblStartup;

        private Panel pnlKeyPair;
        private TextBox txtPrivateKey;
        private TextBox txtPublicKey;
        private Button btnGenerateKeyPair;
        private Button btnCopyPrivateKey;
        private Button btnCopyPublicKey;
        private CheckBox chkKeysSetup;
        private Button btnFinishSetup;
        private Label lblPrivateKey;
        private Label lblPublicKeyKeyPair;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (trayIcon != null)
                {
                    trayIcon.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHome = new System.Windows.Forms.Panel();
            this.lblRelayStatus = new System.Windows.Forms.Label();
            this.lblPublicKey = new System.Windows.Forms.Label();
            this.btnGenerateKeys = new System.Windows.Forms.Button();
            this.txtRelayUrl = new System.Windows.Forms.TextBox();
            this.btnCheckConnection = new System.Windows.Forms.Button();
            this.lblStartup = new System.Windows.Forms.Label();
            this.radioEnabled = new System.Windows.Forms.RadioButton();
            this.radioDisabled = new System.Windows.Forms.RadioButton();
            this.pnlKeyPair = new System.Windows.Forms.Panel();
            this.txtPrivateKey = new System.Windows.Forms.TextBox();
            this.txtPublicKey = new System.Windows.Forms.TextBox();
            this.btnGenerateKeyPair = new System.Windows.Forms.Button();
            this.btnCopyPrivateKey = new System.Windows.Forms.Button();
            this.btnCopyPublicKey = new System.Windows.Forms.Button();
            this.chkKeysSetup = new System.Windows.Forms.CheckBox();
            this.btnFinishSetup = new System.Windows.Forms.Button();
            this.lblPrivateKey = new System.Windows.Forms.Label();
            this.lblPublicKeyKeyPair = new System.Windows.Forms.Label();
            this.pnlHome.SuspendLayout();
            this.pnlKeyPair.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHome
            // 
            this.pnlHome.Controls.Add(this.lblRelayStatus);
            this.pnlHome.Controls.Add(this.lblPublicKey);
            this.pnlHome.Controls.Add(this.btnGenerateKeys);
            this.pnlHome.Controls.Add(this.txtRelayUrl);
            this.pnlHome.Controls.Add(this.btnCheckConnection);
            this.pnlHome.Controls.Add(this.lblStartup);
            this.pnlHome.Controls.Add(this.radioEnabled);
            this.pnlHome.Controls.Add(this.radioDisabled);
            this.pnlHome.Location = new System.Drawing.Point(0, 0);
            this.pnlHome.Name = "pnlHome";
            this.pnlHome.Size = new System.Drawing.Size(600, 400);
            this.pnlHome.TabIndex = 0;
            // 
            // lblRelayStatus
            // 
            this.lblRelayStatus.Location = new System.Drawing.Point(23, 17);
            this.lblRelayStatus.Name = "lblRelayStatus";
            this.lblRelayStatus.Size = new System.Drawing.Size(200, 25);
            this.lblRelayStatus.TabIndex = 0;
            this.lblRelayStatus.Text = "Relay: Not Connected";
            // 
            // lblPublicKey
            // 
            this.lblPublicKey.Location = new System.Drawing.Point(23, 134);
            this.lblPublicKey.Name = "lblPublicKey";
            this.lblPublicKey.Size = new System.Drawing.Size(550, 22);
            this.lblPublicKey.TabIndex = 1;
            this.lblPublicKey.Text = "No Public Key Set";
            // 
            // btnGenerateKeys
            // 
            this.btnGenerateKeys.Location = new System.Drawing.Point(26, 159);
            this.btnGenerateKeys.Name = "btnGenerateKeys";
            this.btnGenerateKeys.Size = new System.Drawing.Size(160, 35);
            this.btnGenerateKeys.TabIndex = 2;
            this.btnGenerateKeys.Text = "Generate Key Pair";
            this.btnGenerateKeys.Click += new System.EventHandler(this.BtnGenerateKeys_Click);
            // 
            // txtRelayUrl
            // 
            this.txtRelayUrl.Location = new System.Drawing.Point(26, 45);
            this.txtRelayUrl.Name = "txtRelayUrl";
            this.txtRelayUrl.Size = new System.Drawing.Size(300, 22);
            this.txtRelayUrl.TabIndex = 3;
            this.txtRelayUrl.Text = "";
            this.txtRelayUrl.Leave += new System.EventHandler(this.TxtRelayUrl_Leave);
            // 
            // btnCheckConnection
            // 
            this.btnCheckConnection.Location = new System.Drawing.Point(26, 73);
            this.btnCheckConnection.Name = "btnCheckConnection";
            this.btnCheckConnection.Size = new System.Drawing.Size(104, 28);
            this.btnCheckConnection.TabIndex = 4;
            this.btnCheckConnection.Text = "Test and Save Relay";
            this.btnCheckConnection.Click += new System.EventHandler(this.BtnCheckConnection_Click);
            // 
            // lblStartup
            // 
            this.lblStartup.AutoSize = true;
            this.lblStartup.Location = new System.Drawing.Point(23, 250);
            this.lblStartup.Name = "lblStartup";
            this.lblStartup.Size = new System.Drawing.Size(248, 16);
            this.lblStartup.TabIndex = 5;
            this.lblStartup.Text = "Run PowerTasker When Windows Starts";
            // 
            // radioEnabled
            // 
            this.radioEnabled.AutoSize = true;
            this.radioEnabled.Location = new System.Drawing.Point(28, 274);
            this.radioEnabled.Name = "radioEnabled";
            this.radioEnabled.Size = new System.Drawing.Size(80, 20);
            this.radioEnabled.TabIndex = 6;
            this.radioEnabled.Text = "Enabled";
            this.radioEnabled.CheckedChanged += new System.EventHandler(this.RadioEnabled_CheckedChanged);
            // 
            // radioDisabled
            // 
            this.radioDisabled.AutoSize = true;
            this.radioDisabled.Checked = true;
            this.radioDisabled.Location = new System.Drawing.Point(128, 274);
            this.radioDisabled.Name = "radioDisabled";
            this.radioDisabled.Size = new System.Drawing.Size(80, 20);
            this.radioDisabled.TabIndex = 7;
            this.radioDisabled.TabStop = true;
            this.radioDisabled.Text = "Disabled";
            this.radioDisabled.CheckedChanged += new System.EventHandler(this.RadioDisabled_CheckedChanged);
            // 
            // pnlKeyPair
            // 
            this.pnlKeyPair.Controls.Add(this.txtPrivateKey);
            this.pnlKeyPair.Controls.Add(this.txtPublicKey);
            this.pnlKeyPair.Controls.Add(this.btnGenerateKeyPair);
            this.pnlKeyPair.Controls.Add(this.btnCopyPrivateKey);
            this.pnlKeyPair.Controls.Add(this.btnCopyPublicKey);
            this.pnlKeyPair.Controls.Add(this.chkKeysSetup);
            this.pnlKeyPair.Controls.Add(this.btnFinishSetup);
            this.pnlKeyPair.Controls.Add(this.lblPrivateKey);
            this.pnlKeyPair.Controls.Add(this.lblPublicKeyKeyPair);
            this.pnlKeyPair.Location = new System.Drawing.Point(0, 0);
            this.pnlKeyPair.Name = "pnlKeyPair";
            this.pnlKeyPair.Size = new System.Drawing.Size(600, 400);
            this.pnlKeyPair.TabIndex = 1;
            this.pnlKeyPair.Visible = false;
            // 
            // txtPrivateKey
            // 
            this.txtPrivateKey.Location = new System.Drawing.Point(20, 50);
            this.txtPrivateKey.Name = "txtPrivateKey";
            this.txtPrivateKey.ReadOnly = true;
            this.txtPrivateKey.Size = new System.Drawing.Size(400, 22);
            this.txtPrivateKey.TabIndex = 0;
            // 
            // txtPublicKey
            // 
            this.txtPublicKey.Location = new System.Drawing.Point(20, 150);
            this.txtPublicKey.Name = "txtPublicKey";
            this.txtPublicKey.ReadOnly = true;
            this.txtPublicKey.Size = new System.Drawing.Size(400, 22);
            this.txtPublicKey.TabIndex = 1;
            // 
            // btnGenerateKeyPair
            // 
            this.btnGenerateKeyPair.Location = new System.Drawing.Point(20, 200);
            this.btnGenerateKeyPair.Name = "btnGenerateKeyPair";
            this.btnGenerateKeyPair.Size = new System.Drawing.Size(150, 40);
            this.btnGenerateKeyPair.TabIndex = 2;
            this.btnGenerateKeyPair.Text = "Generate New Keys";
            this.btnGenerateKeyPair.Click += new System.EventHandler(this.BtnGenerateKeyPair_Click);
            // 
            // btnCopyPrivateKey
            // 
            this.btnCopyPrivateKey.Location = new System.Drawing.Point(430, 45);
            this.btnCopyPrivateKey.Name = "btnCopyPrivateKey";
            this.btnCopyPrivateKey.Size = new System.Drawing.Size(150, 32);
            this.btnCopyPrivateKey.TabIndex = 3;
            this.btnCopyPrivateKey.Text = "Copy Private Key";
            this.btnCopyPrivateKey.Click += new System.EventHandler(this.BtnCopyPrivateKey_Click);
            // 
            // btnCopyPublicKey
            // 
            this.btnCopyPublicKey.Location = new System.Drawing.Point(430, 145);
            this.btnCopyPublicKey.Name = "btnCopyPublicKey";
            this.btnCopyPublicKey.Size = new System.Drawing.Size(150, 32);
            this.btnCopyPublicKey.TabIndex = 4;
            this.btnCopyPublicKey.Text = "Copy Public Key";
            this.btnCopyPublicKey.Click += new System.EventHandler(this.BtnCopyPublicKey_Click);
            // 
            // chkKeysSetup
            // 
            this.chkKeysSetup.Location = new System.Drawing.Point(20, 275);
            this.chkKeysSetup.Name = "chkKeysSetup";
            this.chkKeysSetup.Size = new System.Drawing.Size(400, 30);
            this.chkKeysSetup.TabIndex = 5;
            this.chkKeysSetup.Text = "Added keys to Relay and Mobile App";
            this.chkKeysSetup.CheckedChanged += new System.EventHandler(this.ChkKeysSetup_CheckedChanged);
            // 
            // btnFinishSetup
            // 
            this.btnFinishSetup.Enabled = false;
            this.btnFinishSetup.Location = new System.Drawing.Point(20, 305);
            this.btnFinishSetup.Name = "btnFinishSetup";
            this.btnFinishSetup.Size = new System.Drawing.Size(150, 40);
            this.btnFinishSetup.TabIndex = 6;
            this.btnFinishSetup.Text = "Finish Setup";
            this.btnFinishSetup.Click += new System.EventHandler(this.BtnFinishSetup_Click);
            // 
            // lblPrivateKey
            // 
            this.lblPrivateKey.Location = new System.Drawing.Point(20, 20);
            this.lblPrivateKey.Name = "lblPrivateKey";
            this.lblPrivateKey.Size = new System.Drawing.Size(200, 30);
            this.lblPrivateKey.TabIndex = 7;
            this.lblPrivateKey.Text = "Current Private Key:";
            // 
            // lblPublicKeyKeyPair
            // 
            this.lblPublicKeyKeyPair.Location = new System.Drawing.Point(20, 120);
            this.lblPublicKeyKeyPair.Name = "lblPublicKeyKeyPair";
            this.lblPublicKeyKeyPair.Size = new System.Drawing.Size(200, 30);
            this.lblPublicKeyKeyPair.TabIndex = 8;
            this.lblPublicKeyKeyPair.Text = "Current Public Key:";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.pnlHome);
            this.Controls.Add(this.pnlKeyPair);
            this.Name = "Form1";
            this.Text = "Power Tasker Setup";
            this.pnlHome.ResumeLayout(false);
            this.pnlHome.PerformLayout();
            this.pnlKeyPair.ResumeLayout(false);
            this.pnlKeyPair.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}