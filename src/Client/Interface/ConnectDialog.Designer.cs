namespace Squirrel.Client.Interface
{
    partial class ConnectDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.OptionaLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.LabelIP = new System.Windows.Forms.Label();
            this.LabelPort = new System.Windows.Forms.Label();
            this.LabelNickname = new System.Windows.Forms.Label();
            this.TextBoxIP = new System.Windows.Forms.TextBox();
            this.TextBoxPort = new System.Windows.Forms.TextBox();
            this.TextBoxNickname = new System.Windows.Forms.TextBox();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.MainTableLayout.SuspendLayout();
            this.OptionaLayoutTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTableLayout
            // 
            this.MainTableLayout.ColumnCount = 1;
            this.MainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTableLayout.Controls.Add(this.OptionaLayoutTable, 0, 0);
            this.MainTableLayout.Controls.Add(this.ButtonConnect, 0, 1);
            this.MainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTableLayout.Location = new System.Drawing.Point(3, 3);
            this.MainTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this.MainTableLayout.Name = "MainTableLayout";
            this.MainTableLayout.RowCount = 2;
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.MainTableLayout.Size = new System.Drawing.Size(378, 106);
            this.MainTableLayout.TabIndex = 0;
            // 
            // OptionaLayoutTable
            // 
            this.OptionaLayoutTable.ColumnCount = 3;
            this.OptionaLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.OptionaLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.OptionaLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.OptionaLayoutTable.Controls.Add(this.TextBoxNickname, 2, 1);
            this.OptionaLayoutTable.Controls.Add(this.TextBoxPort, 1, 1);
            this.OptionaLayoutTable.Controls.Add(this.LabelNickname, 2, 0);
            this.OptionaLayoutTable.Controls.Add(this.LabelPort, 1, 0);
            this.OptionaLayoutTable.Controls.Add(this.LabelIP, 0, 0);
            this.OptionaLayoutTable.Controls.Add(this.TextBoxIP, 0, 1);
            this.OptionaLayoutTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OptionaLayoutTable.Location = new System.Drawing.Point(0, 0);
            this.OptionaLayoutTable.Margin = new System.Windows.Forms.Padding(0);
            this.OptionaLayoutTable.Name = "OptionaLayoutTable";
            this.OptionaLayoutTable.RowCount = 2;
            this.OptionaLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.OptionaLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.OptionaLayoutTable.Size = new System.Drawing.Size(378, 53);
            this.OptionaLayoutTable.TabIndex = 0;
            // 
            // LabelIP
            // 
            this.LabelIP.AutoSize = true;
            this.LabelIP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelIP.Location = new System.Drawing.Point(3, 0);
            this.LabelIP.Name = "LabelIP";
            this.LabelIP.Size = new System.Drawing.Size(119, 26);
            this.LabelIP.TabIndex = 0;
            this.LabelIP.Text = "IP";
            this.LabelIP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelPort
            // 
            this.LabelPort.AutoSize = true;
            this.LabelPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelPort.Location = new System.Drawing.Point(128, 0);
            this.LabelPort.Name = "LabelPort";
            this.LabelPort.Size = new System.Drawing.Size(120, 26);
            this.LabelPort.TabIndex = 1;
            this.LabelPort.Text = "Port";
            this.LabelPort.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelNickname
            // 
            this.LabelNickname.AutoSize = true;
            this.LabelNickname.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LabelNickname.Location = new System.Drawing.Point(254, 0);
            this.LabelNickname.Name = "LabelNickname";
            this.LabelNickname.Size = new System.Drawing.Size(121, 26);
            this.LabelNickname.TabIndex = 2;
            this.LabelNickname.Text = "Nickname";
            this.LabelNickname.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextBoxIP
            // 
            this.TextBoxIP.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TextBoxIP.Location = new System.Drawing.Point(12, 29);
            this.TextBoxIP.Name = "TextBoxIP";
            this.TextBoxIP.Size = new System.Drawing.Size(100, 20);
            this.TextBoxIP.TabIndex = 3;
            this.TextBoxIP.Text = "127.0.0.1";
            // 
            // TextBoxPort
            // 
            this.TextBoxPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TextBoxPort.Location = new System.Drawing.Point(138, 29);
            this.TextBoxPort.Name = "TextBoxPort";
            this.TextBoxPort.Size = new System.Drawing.Size(100, 20);
            this.TextBoxPort.TabIndex = 4;
            this.TextBoxPort.Text = "37500";
            // 
            // TextBoxNickname
            // 
            this.TextBoxNickname.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TextBoxNickname.Location = new System.Drawing.Point(264, 29);
            this.TextBoxNickname.Name = "TextBoxNickname";
            this.TextBoxNickname.Size = new System.Drawing.Size(100, 20);
            this.TextBoxNickname.TabIndex = 5;
            this.TextBoxNickname.Text = "Player";
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonConnect.Location = new System.Drawing.Point(3, 56);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(372, 47);
            this.ButtonConnect.TabIndex = 1;
            this.ButtonConnect.Text = "Connect";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // ConnectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 112);
            this.Controls.Add(this.MainTableLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ConnectDialog";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect To Server";
            this.MainTableLayout.ResumeLayout(false);
            this.OptionaLayoutTable.ResumeLayout(false);
            this.OptionaLayoutTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainTableLayout;
        private System.Windows.Forms.TableLayoutPanel OptionaLayoutTable;
        private System.Windows.Forms.TextBox TextBoxNickname;
        private System.Windows.Forms.TextBox TextBoxPort;
        private System.Windows.Forms.Label LabelNickname;
        private System.Windows.Forms.Label LabelPort;
        private System.Windows.Forms.Label LabelIP;
        private System.Windows.Forms.TextBox TextBoxIP;
        private System.Windows.Forms.Button ButtonConnect;
    }
}