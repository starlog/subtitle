namespace LogAnalysis
{
    partial class Form1
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
            this.textBox_Data = new System.Windows.Forms.TextBox();
            this.button_ReadLog = new System.Windows.Forms.Button();
            this.button_analysis = new System.Windows.Forms.Button();
            this.textBox_result = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button_save = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_Data
            // 
            this.textBox_Data.Location = new System.Drawing.Point(12, 12);
            this.textBox_Data.Multiline = true;
            this.textBox_Data.Name = "textBox_Data";
            this.textBox_Data.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Data.Size = new System.Drawing.Size(688, 217);
            this.textBox_Data.TabIndex = 0;
            this.textBox_Data.WordWrap = false;
            // 
            // button_ReadLog
            // 
            this.button_ReadLog.Location = new System.Drawing.Point(12, 236);
            this.button_ReadLog.Name = "button_ReadLog";
            this.button_ReadLog.Size = new System.Drawing.Size(102, 23);
            this.button_ReadLog.TabIndex = 1;
            this.button_ReadLog.Text = "로그파일 읽기";
            this.button_ReadLog.UseVisualStyleBackColor = true;
            this.button_ReadLog.Click += new System.EventHandler(this.button_ReadLog_Click);
            // 
            // button_analysis
            // 
            this.button_analysis.Location = new System.Drawing.Point(131, 235);
            this.button_analysis.Name = "button_analysis";
            this.button_analysis.Size = new System.Drawing.Size(75, 23);
            this.button_analysis.TabIndex = 2;
            this.button_analysis.Text = "로그찾기";
            this.button_analysis.UseVisualStyleBackColor = true;
            this.button_analysis.Click += new System.EventHandler(this.button_analysis_Click);
            // 
            // textBox_result
            // 
            this.textBox_result.Location = new System.Drawing.Point(13, 266);
            this.textBox_result.Multiline = true;
            this.textBox_result.Name = "textBox_result";
            this.textBox_result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_result.Size = new System.Drawing.Size(687, 225);
            this.textBox_result.TabIndex = 3;
            this.textBox_result.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 502);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(712, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(222, 235);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 5;
            this.button_save.Text = "저장하기";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 524);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBox_result);
            this.Controls.Add(this.button_analysis);
            this.Controls.Add(this.button_ReadLog);
            this.Controls.Add(this.textBox_Data);
            this.Name = "Form1";
            this.Text = "Log analyser";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Data;
        private System.Windows.Forms.Button button_ReadLog;
        private System.Windows.Forms.Button button_analysis;
        private System.Windows.Forms.TextBox textBox_result;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button_save;
    }
}

