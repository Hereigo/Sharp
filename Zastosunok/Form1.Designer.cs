namespace Zastosunok
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        ///  Required method for Designer support - DO NOT MODIFY !!!
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            labelHrs = new Label();
            labelMin = new Label();
            labelSec = new Label();
            buttonStart = new Button();
            buttonReset = new Button();
            SuspendLayout();
            // 
            // labelHrs
            // 
            labelHrs.AutoSize = true;
            labelHrs.FlatStyle = FlatStyle.System;
            labelHrs.Font = new Font("Verdana", 18F);
            labelHrs.Location = new Point(1, 1);
            labelHrs.Margin = new Padding(0);
            labelHrs.Name = "labelHrs";
            labelHrs.Size = new Size(43, 29);
            labelHrs.TabIndex = 0;
            labelHrs.Text = "00";
            labelHrs.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelMin
            // 
            labelMin.AutoSize = true;
            labelMin.FlatStyle = FlatStyle.System;
            labelMin.Font = new Font("Verdana", 18F);
            labelMin.Location = new Point(43, 1);
            labelMin.Margin = new Padding(0);
            labelMin.Name = "labelMin";
            labelMin.Size = new Size(43, 29);
            labelMin.TabIndex = 1;
            labelMin.Text = "00";
            labelMin.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelSec
            // 
            labelSec.AutoSize = true;
            labelSec.FlatStyle = FlatStyle.System;
            labelSec.Font = new Font("Verdana", 18F);
            labelSec.Location = new Point(84, 1);
            labelSec.Margin = new Padding(0);
            labelSec.Name = "labelSec";
            labelSec.Size = new Size(43, 29);
            labelSec.TabIndex = 2;
            labelSec.Text = "00";
            labelSec.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(5, 33);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(60, 34);
            buttonStart.TabIndex = 3;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += ButtonStart_Click;
            // 
            // buttonReset
            // 
            buttonReset.BackColor = SystemColors.ButtonHighlight;
            buttonReset.Location = new Point(65, 33);
            buttonReset.Name = "buttonReset";
            buttonReset.Size = new Size(60, 34);
            buttonReset.TabIndex = 4;
            buttonReset.Text = "Reset";
            buttonReset.UseVisualStyleBackColor = false;
            buttonReset.Click += ButtonReset_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(129, 71);
            Controls.Add(buttonReset);
            Controls.Add(buttonStart);
            Controls.Add(labelSec);
            Controls.Add(labelMin);
            Controls.Add(labelHrs);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Tool";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelHrs;
        private Label labelMin;
        private Label labelSec;
        private Button buttonStart;
        private Button buttonReset;
    }
}
