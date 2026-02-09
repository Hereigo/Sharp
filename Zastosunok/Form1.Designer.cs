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
            // label1
            // 
            labelHrs.AutoSize = true;
            labelHrs.FlatStyle = FlatStyle.System;
            labelHrs.Font = new Font("Verdana", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelHrs.Location = new Point(9, 9);
            labelHrs.Margin = new Padding(0);
            labelHrs.Name = "label1";
            labelHrs.Size = new Size(50, 31);
            labelHrs.TabIndex = 0;
            labelHrs.Text = "00";
            // 
            // label2
            // 
            labelMin.AutoSize = true;
            labelMin.FlatStyle = FlatStyle.System;
            labelMin.Font = new Font("Verdana", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelMin.Location = new Point(59, 9);
            labelMin.Margin = new Padding(0);
            labelMin.Name = "label2";
            labelMin.Size = new Size(50, 31);
            labelMin.TabIndex = 1;
            labelMin.Text = "00";
            // 
            // label3
            // 
            labelSec.AutoSize = true;
            labelSec.FlatStyle = FlatStyle.System;
            labelSec.Font = new Font("Verdana", 18.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelSec.Location = new Point(109, 9);
            labelSec.Margin = new Padding(0);
            labelSec.Name = "label3";
            labelSec.Size = new Size(50, 31);
            labelSec.TabIndex = 2;
            labelSec.Text = "00";
            // 
            // button1
            // 
            buttonStart.Location = new Point(9, 45);
            buttonStart.Name = "button1";
            buttonStart.Size = new Size(66, 44);
            buttonStart.TabIndex = 3;
            buttonStart.Text = "Start";
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += buttonStart_Click;
            // 
            // button2
            // 
            buttonReset.Location = new Point(80, 45);
            buttonReset.Name = "button2";
            buttonReset.Size = new Size(66, 44);
            buttonReset.TabIndex = 4;
            buttonReset.Text = "Reset";
            buttonReset.UseVisualStyleBackColor = true;
            buttonReset.Click += buttonReset_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(158, 99);
            Controls.Add(buttonReset);
            Controls.Add(buttonStart);
            Controls.Add(labelSec);
            Controls.Add(labelMin);
            Controls.Add(labelHrs);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
