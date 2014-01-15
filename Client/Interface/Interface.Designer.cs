namespace Squirrel.Client.Interface
{
    partial class Interface
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
            this.MainLayoutGrid = new System.Windows.Forms.TableLayoutPanel();
            this.GameLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.BottomInfoLabel = new System.Windows.Forms.Label();
            this.ChatLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ChatBarLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SendChatButton = new System.Windows.Forms.Button();
            this.ChatOutgoingTextBox = new System.Windows.Forms.TextBox();
            this.ChatIncomingTextBox = new System.Windows.Forms.TextBox();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.MenuButtonFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButtonConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButtonDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuButtonExit = new System.Windows.Forms.ToolStripMenuItem();
            this.GameWindow = new Squirrel.Client.Interface.GameWindow();
            this.MainLayoutGrid.SuspendLayout();
            this.GameLayoutPanel.SuspendLayout();
            this.ChatLayoutPanel.SuspendLayout();
            this.ChatBarLayoutPanel.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayoutGrid
            // 
            this.MainLayoutGrid.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MainLayoutGrid.ColumnCount = 2;
            this.MainLayoutGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.MainLayoutGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.MainLayoutGrid.Controls.Add(this.GameLayoutPanel, 0, 0);
            this.MainLayoutGrid.Controls.Add(this.ChatLayoutPanel, 1, 0);
            this.MainLayoutGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainLayoutGrid.Location = new System.Drawing.Point(0, 24);
            this.MainLayoutGrid.Margin = new System.Windows.Forms.Padding(0);
            this.MainLayoutGrid.Name = "MainLayoutGrid";
            this.MainLayoutGrid.RowCount = 1;
            this.MainLayoutGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayoutGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 738F));
            this.MainLayoutGrid.Size = new System.Drawing.Size(1264, 738);
            this.MainLayoutGrid.TabIndex = 0;
            // 
            // GameLayoutPanel
            // 
            this.GameLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.GameLayoutPanel.ColumnCount = 1;
            this.GameLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.GameLayoutPanel.Controls.Add(this.GameWindow, 0, 0);
            this.GameLayoutPanel.Controls.Add(this.BottomInfoLabel, 0, 1);
            this.GameLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.GameLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.GameLayoutPanel.Name = "GameLayoutPanel";
            this.GameLayoutPanel.RowCount = 2;
            this.GameLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.GameLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.GameLayoutPanel.Size = new System.Drawing.Size(1005, 735);
            this.GameLayoutPanel.TabIndex = 1;
            // 
            // BottomInfoLabel
            // 
            this.BottomInfoLabel.AutoSize = true;
            this.BottomInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomInfoLabel.Location = new System.Drawing.Point(0, 705);
            this.BottomInfoLabel.Margin = new System.Windows.Forms.Padding(0);
            this.BottomInfoLabel.Name = "BottomInfoLabel";
            this.BottomInfoLabel.Size = new System.Drawing.Size(1005, 30);
            this.BottomInfoLabel.TabIndex = 1;
            this.BottomInfoLabel.Text = "DISCONNECTED";
            this.BottomInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChatLayoutPanel
            // 
            this.ChatLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChatLayoutPanel.ColumnCount = 1;
            this.ChatLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChatLayoutPanel.Controls.Add(this.ChatBarLayoutPanel, 0, 1);
            this.ChatLayoutPanel.Controls.Add(this.ChatIncomingTextBox, 0, 0);
            this.ChatLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatLayoutPanel.Location = new System.Drawing.Point(1011, 0);
            this.ChatLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ChatLayoutPanel.Name = "ChatLayoutPanel";
            this.ChatLayoutPanel.Padding = new System.Windows.Forms.Padding(3);
            this.ChatLayoutPanel.RowCount = 2;
            this.ChatLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChatLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.ChatLayoutPanel.Size = new System.Drawing.Size(253, 738);
            this.ChatLayoutPanel.TabIndex = 0;
            // 
            // ChatBarLayoutPanel
            // 
            this.ChatBarLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ChatBarLayoutPanel.ColumnCount = 2;
            this.ChatBarLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChatBarLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.ChatBarLayoutPanel.Controls.Add(this.SendChatButton, 1, 0);
            this.ChatBarLayoutPanel.Controls.Add(this.ChatOutgoingTextBox, 0, 0);
            this.ChatBarLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatBarLayoutPanel.Location = new System.Drawing.Point(3, 710);
            this.ChatBarLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ChatBarLayoutPanel.Name = "ChatBarLayoutPanel";
            this.ChatBarLayoutPanel.RowCount = 1;
            this.ChatBarLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ChatBarLayoutPanel.Size = new System.Drawing.Size(247, 25);
            this.ChatBarLayoutPanel.TabIndex = 0;
            // 
            // SendChatButton
            // 
            this.SendChatButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.SendChatButton.Enabled = false;
            this.SendChatButton.Location = new System.Drawing.Point(197, 2);
            this.SendChatButton.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.SendChatButton.Name = "SendChatButton";
            this.SendChatButton.Size = new System.Drawing.Size(50, 21);
            this.SendChatButton.TabIndex = 0;
            this.SendChatButton.Text = "Send";
            this.SendChatButton.UseVisualStyleBackColor = true;
            this.SendChatButton.Click += new System.EventHandler(this.SendChatButton_Click);
            // 
            // ChatOutgoingTextBox
            // 
            this.ChatOutgoingTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ChatOutgoingTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ChatOutgoingTextBox.Location = new System.Drawing.Point(3, 3);
            this.ChatOutgoingTextBox.MaxLength = 100;
            this.ChatOutgoingTextBox.Name = "ChatOutgoingTextBox";
            this.ChatOutgoingTextBox.Size = new System.Drawing.Size(191, 20);
            this.ChatOutgoingTextBox.TabIndex = 1;
            this.ChatOutgoingTextBox.Text = "Hello, I\'m new to the server";
            this.ChatOutgoingTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChatOutgoingTextBox_KeyDown);
            // 
            // ChatIncomingTextBox
            // 
            this.ChatIncomingTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ChatIncomingTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatIncomingTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChatIncomingTextBox.Location = new System.Drawing.Point(6, 6);
            this.ChatIncomingTextBox.Multiline = true;
            this.ChatIncomingTextBox.Name = "ChatIncomingTextBox";
            this.ChatIncomingTextBox.ReadOnly = true;
            this.ChatIncomingTextBox.Size = new System.Drawing.Size(241, 701);
            this.ChatIncomingTextBox.TabIndex = 1;
            // 
            // MainMenu
            // 
            this.MainMenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuButtonFile});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.MainMenu.Size = new System.Drawing.Size(1264, 24);
            this.MainMenu.TabIndex = 1;
            this.MainMenu.Text = "menuStrip1";
            // 
            // MenuButtonFile
            // 
            this.MenuButtonFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuButtonConnect,
            this.MenuButtonDisconnect,
            this.MenuButtonExit});
            this.MenuButtonFile.Name = "MenuButtonFile";
            this.MenuButtonFile.Size = new System.Drawing.Size(37, 20);
            this.MenuButtonFile.Text = "File";
            // 
            // MenuButtonConnect
            // 
            this.MenuButtonConnect.Name = "MenuButtonConnect";
            this.MenuButtonConnect.Size = new System.Drawing.Size(133, 22);
            this.MenuButtonConnect.Text = "Connect";
            this.MenuButtonConnect.Click += new System.EventHandler(this.MenuButtonConnect_Click);
            // 
            // MenuButtonDisconnect
            // 
            this.MenuButtonDisconnect.Enabled = false;
            this.MenuButtonDisconnect.Name = "MenuButtonDisconnect";
            this.MenuButtonDisconnect.Size = new System.Drawing.Size(133, 22);
            this.MenuButtonDisconnect.Text = "Disconnect";
            this.MenuButtonDisconnect.Click += new System.EventHandler(this.MenuButtonDisconnect_Click);
            // 
            // MenuButtonExit
            // 
            this.MenuButtonExit.Name = "MenuButtonExit";
            this.MenuButtonExit.Size = new System.Drawing.Size(133, 22);
            this.MenuButtonExit.Text = "Exit";
            this.MenuButtonExit.Click += new System.EventHandler(this.MenuButtonExit_Click);
            // 
            // GameWindow
            // 
            this.GameWindow.BackColor = System.Drawing.SystemColors.ControlLight;
            this.GameWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameWindow.Location = new System.Drawing.Point(3, 3);
            this.GameWindow.Name = "GameWindow";
            this.GameWindow.Size = new System.Drawing.Size(999, 699);
            this.GameWindow.TabIndex = 0;
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 762);
            this.Controls.Add(this.MainLayoutGrid);
            this.Controls.Add(this.MainMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimumSize = new System.Drawing.Size(1280, 800);
            this.Name = "Interface";
            this.Text = "Project Squirrel Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Interface_FormClosing);
            this.Load += new System.EventHandler(this.Interface_Load);
            this.MainLayoutGrid.ResumeLayout(false);
            this.GameLayoutPanel.ResumeLayout(false);
            this.GameLayoutPanel.PerformLayout();
            this.ChatLayoutPanel.ResumeLayout(false);
            this.ChatLayoutPanel.PerformLayout();
            this.ChatBarLayoutPanel.ResumeLayout(false);
            this.ChatBarLayoutPanel.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayoutGrid;
        private System.Windows.Forms.TableLayoutPanel ChatLayoutPanel;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.TableLayoutPanel GameLayoutPanel;
        private System.Windows.Forms.ToolStripMenuItem MenuButtonFile;
        private System.Windows.Forms.ToolStripMenuItem MenuButtonConnect;
        private System.Windows.Forms.ToolStripMenuItem MenuButtonDisconnect;
        private System.Windows.Forms.ToolStripMenuItem MenuButtonExit;
        private System.Windows.Forms.TableLayoutPanel ChatBarLayoutPanel;
        private System.Windows.Forms.Button SendChatButton;
        private System.Windows.Forms.TextBox ChatOutgoingTextBox;
        private System.Windows.Forms.TextBox ChatIncomingTextBox;
        private GameWindow GameWindow;
        private System.Windows.Forms.Label BottomInfoLabel;

    }
}

