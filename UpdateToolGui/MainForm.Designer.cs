namespace UpdateToolGui
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.prg_bar = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog3 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chk_compress = new System.Windows.Forms.CheckBox();
            this.chk_createchanged = new System.Windows.Forms.CheckBox();
            this.chk_checkdeleted = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_newdbpath = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.txt_updatefolder = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_olddbpath = new System.Windows.Forms.TextBox();
            this.txt_sourcefolder = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_run = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.txt_status = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // prg_bar
            // 
            this.prg_bar.Location = new System.Drawing.Point(12, 294);
            this.prg_bar.Name = "prg_bar";
            this.prg_bar.Size = new System.Drawing.Size(513, 16);
            this.prg_bar.TabIndex = 21;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(7);
            this.label3.Size = new System.Drawing.Size(537, 67);
            this.label3.TabIndex = 19;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chk_compress);
            this.panel1.Controls.Add(this.chk_createchanged);
            this.panel1.Controls.Add(this.chk_checkdeleted);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txt_newdbpath);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.txt_updatefolder);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txt_olddbpath);
            this.panel1.Controls.Add(this.txt_sourcefolder);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(3, 70);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 189);
            this.panel1.TabIndex = 26;
            this.panel1.EnabledChanged += new System.EventHandler(this.panel1_EnabledChanged);
            // 
            // chk_compress
            // 
            this.chk_compress.AutoSize = true;
            this.chk_compress.Location = new System.Drawing.Point(9, 162);
            this.chk_compress.Name = "chk_compress";
            this.chk_compress.Size = new System.Drawing.Size(121, 17);
            this.chk_compress.TabIndex = 41;
            this.chk_compress.Text = "Compress database";
            this.chk_compress.UseVisualStyleBackColor = true;
            // 
            // chk_createchanged
            // 
            this.chk_createchanged.AutoSize = true;
            this.chk_createchanged.Enabled = false;
            this.chk_createchanged.Location = new System.Drawing.Point(9, 139);
            this.chk_createchanged.Name = "chk_createchanged";
            this.chk_createchanged.Size = new System.Drawing.Size(271, 17);
            this.chk_createchanged.TabIndex = 40;
            this.chk_createchanged.Text = "Create a list with changed files in the update folder";
            this.chk_createchanged.UseVisualStyleBackColor = true;
            // 
            // chk_checkdeleted
            // 
            this.chk_checkdeleted.AutoSize = true;
            this.chk_checkdeleted.Checked = true;
            this.chk_checkdeleted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_checkdeleted.Enabled = false;
            this.chk_checkdeleted.Location = new System.Drawing.Point(9, 116);
            this.chk_checkdeleted.Name = "chk_checkdeleted";
            this.chk_checkdeleted.Size = new System.Drawing.Size(311, 17);
            this.chk_checkdeleted.TabIndex = 39;
            this.chk_checkdeleted.Text = "Create a list with deleted files in the update folder (default)";
            this.chk_checkdeleted.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(122, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "New database file path:";
            // 
            // txt_newdbpath
            // 
            this.txt_newdbpath.Location = new System.Drawing.Point(210, 59);
            this.txt_newdbpath.Name = "txt_newdbpath";
            this.txt_newdbpath.Size = new System.Drawing.Size(271, 21);
            this.txt_newdbpath.TabIndex = 37;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(487, 59);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(35, 20);
            this.button5.TabIndex = 36;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // txt_updatefolder
            // 
            this.txt_updatefolder.Enabled = false;
            this.txt_updatefolder.Location = new System.Drawing.Point(210, 85);
            this.txt_updatefolder.Name = "txt_updatefolder";
            this.txt_updatefolder.Size = new System.Drawing.Size(271, 21);
            this.txt_updatefolder.TabIndex = 34;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(487, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(35, 20);
            this.button3.TabIndex = 33;
            this.button3.Text = "...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 87);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(184, 17);
            this.checkBox1.TabIndex = 32;
            this.checkBox1.Text = "Copy updated files to this folder:";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Old database file path (optional):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Source folder:";
            // 
            // txt_olddbpath
            // 
            this.txt_olddbpath.Location = new System.Drawing.Point(210, 33);
            this.txt_olddbpath.Name = "txt_olddbpath";
            this.txt_olddbpath.Size = new System.Drawing.Size(271, 21);
            this.txt_olddbpath.TabIndex = 29;
            // 
            // txt_sourcefolder
            // 
            this.txt_sourcefolder.Location = new System.Drawing.Point(210, 7);
            this.txt_sourcefolder.Name = "txt_sourcefolder";
            this.txt_sourcefolder.Size = new System.Drawing.Size(271, 21);
            this.txt_sourcefolder.TabIndex = 28;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(487, 33);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(35, 20);
            this.button2.TabIndex = 27;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(487, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 20);
            this.button1.TabIndex = 26;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_run
            // 
            this.btn_run.Location = new System.Drawing.Point(450, 265);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(75, 23);
            this.btn_run.TabIndex = 35;
            this.btn_run.Text = "OK";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.button4_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // txt_status
            // 
            this.txt_status.Location = new System.Drawing.Point(8, 318);
            this.txt_status.Name = "txt_status";
            this.txt_status.Size = new System.Drawing.Size(517, 13);
            this.txt_status.TabIndex = 28;
            this.txt_status.Text = "Ready";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "db";
            this.saveFileDialog1.FileName = "files.db";
            this.saveFileDialog1.Filter = "Database files|*.db|All files|*";
            this.saveFileDialog1.Title = "New database file";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "db";
            this.openFileDialog1.FileName = "files.db";
            this.openFileDialog1.Filter = "Database files|*.db|All files|*";
            this.openFileDialog1.ReadOnlyChecked = true;
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            this.openFileDialog1.Title = "Old database file";
            // 
            // btn_cancel
            // 
            this.btn_cancel.Enabled = false;
            this.btn_cancel.Location = new System.Drawing.Point(450, 265);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 36;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Visible = false;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 340);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.txt_status);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.prg_bar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_run);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update Tool by Icedream";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar prg_bar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.TextBox txt_updatefolder;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_olddbpath;
        private System.Windows.Forms.TextBox txt_sourcefolder;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label txt_status;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_newdbpath;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chk_checkdeleted;
        private System.Windows.Forms.CheckBox chk_createchanged;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.CheckBox chk_compress;
    }
}

