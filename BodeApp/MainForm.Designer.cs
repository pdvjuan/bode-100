namespace BodeApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button startMeasurementButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.TextBox inputTextBox1;
        private System.Windows.Forms.TextBox inputTextBox2;
        private System.Windows.Forms.TextBox inputTextBox3;
        private System.Windows.Forms.TextBox inputTextBox4;
        private System.Windows.Forms.TextBox inputTextBox5;
        private System.Windows.Forms.TextBox inputTextBox6;
        private System.Windows.Forms.TextBox inputTextBox7;
        private System.Windows.Forms.Label inputLabel1;
        private System.Windows.Forms.Label inputLabel2;
        private System.Windows.Forms.Label inputLabel3;
        private System.Windows.Forms.Label inputLabel4;
        private System.Windows.Forms.Label inputLabel5;
        private System.Windows.Forms.Label inputLabel6;
        private System.Windows.Forms.Label inputLabel7;
        private System.Windows.Forms.TextBox dateTimeTextBox;
        private System.Windows.Forms.ListBox resultsListBox;


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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            connectButton = new Button();
            startMeasurementButton = new Button();
            exportButton = new Button();
            inputTextBox1 = new TextBox();
            inputTextBox2 = new TextBox();
            inputTextBox3 = new TextBox();
            inputTextBox4 = new TextBox();
            inputTextBox5 = new TextBox();
            inputTextBox6 = new TextBox();
            inputTextBox7 = new TextBox();
            inputLabel1 = new Label();
            inputLabel2 = new Label();
            inputLabel3 = new Label();
            inputLabel4 = new Label();
            inputLabel5 = new Label();
            inputLabel6 = new Label();
            inputLabel7 = new Label();
            dateTimeTextBox = new TextBox();
            disconnectButton = new Button();
            resultsListBox = new ListBox();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Enabled = false;
            connectButton.Location = new Point(14, 417);
            connectButton.Margin = new Padding(4, 3, 4, 3);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(175, 27);
            connectButton.TabIndex = 15;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // startMeasurementButton
            // 
            startMeasurementButton.Enabled = false;
            startMeasurementButton.Location = new Point(14, 451);
            startMeasurementButton.Margin = new Padding(4, 3, 4, 3);
            startMeasurementButton.Name = "startMeasurementButton";
            startMeasurementButton.Size = new Size(175, 27);
            startMeasurementButton.TabIndex = 16;
            startMeasurementButton.Text = "Start Measurement";
            startMeasurementButton.UseVisualStyleBackColor = true;
            startMeasurementButton.Click += startMeasurementButton_Click;
            // 
            // exportButton
            // 
            exportButton.Enabled = false;
            exportButton.Location = new Point(14, 486);
            exportButton.Margin = new Padding(4, 3, 4, 3);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(175, 27);
            exportButton.TabIndex = 17;
            exportButton.Text = "Export to CSV";
            exportButton.UseVisualStyleBackColor = true;
            exportButton.Click += exportButton_Click;
            // 
            // inputTextBox1
            // 
            inputTextBox1.Location = new Point(14, 63);
            inputTextBox1.Margin = new Padding(4, 3, 4, 3);
            inputTextBox1.Name = "inputTextBox1";
            inputTextBox1.Size = new Size(174, 23);
            inputTextBox1.TabIndex = 2;
            inputTextBox1.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox2
            // 
            inputTextBox2.Location = new Point(14, 117);
            inputTextBox2.Margin = new Padding(4, 3, 4, 3);
            inputTextBox2.Name = "inputTextBox2";
            inputTextBox2.Size = new Size(174, 23);
            inputTextBox2.TabIndex = 4;
            inputTextBox2.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox3
            // 
            inputTextBox3.Location = new Point(14, 170);
            inputTextBox3.Margin = new Padding(4, 3, 4, 3);
            inputTextBox3.Name = "inputTextBox3";
            inputTextBox3.Size = new Size(174, 23);
            inputTextBox3.TabIndex = 6;
            inputTextBox3.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox4
            // 
            inputTextBox4.Location = new Point(14, 223);
            inputTextBox4.Margin = new Padding(4, 3, 4, 3);
            inputTextBox4.Name = "inputTextBox4";
            inputTextBox4.Size = new Size(174, 23);
            inputTextBox4.TabIndex = 8;
            inputTextBox4.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox5
            // 
            inputTextBox5.Location = new Point(14, 276);
            inputTextBox5.Margin = new Padding(4, 3, 4, 3);
            inputTextBox5.Name = "inputTextBox5";
            inputTextBox5.Size = new Size(174, 23);
            inputTextBox5.TabIndex = 10;
            inputTextBox5.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox6
            // 
            inputTextBox6.Location = new Point(14, 329);
            inputTextBox6.Margin = new Padding(4, 3, 4, 3);
            inputTextBox6.Name = "inputTextBox6";
            inputTextBox6.Size = new Size(174, 23);
            inputTextBox6.TabIndex = 12;
            inputTextBox6.TextChanged += inputTextBox_TextChanged;
            // 
            // inputTextBox7
            // 
            inputTextBox7.Location = new Point(14, 382);
            inputTextBox7.Margin = new Padding(4, 3, 4, 3);
            inputTextBox7.Name = "inputTextBox7";
            inputTextBox7.Size = new Size(174, 23);
            inputTextBox7.TabIndex = 14;
            inputTextBox7.TextChanged += inputTextBox_TextChanged;
            // 
            // inputLabel1
            // 
            inputLabel1.AutoSize = true;
            inputLabel1.Location = new Point(14, 45);
            inputLabel1.Margin = new Padding(4, 0, 4, 0);
            inputLabel1.Name = "inputLabel1";
            inputLabel1.Size = new Size(47, 15);
            inputLabel1.TabIndex = 1;
            inputLabel1.Text = "Input 1:";
            // 
            // inputLabel2
            // 
            inputLabel2.AutoSize = true;
            inputLabel2.Location = new Point(14, 98);
            inputLabel2.Margin = new Padding(4, 0, 4, 0);
            inputLabel2.Name = "inputLabel2";
            inputLabel2.Size = new Size(47, 15);
            inputLabel2.TabIndex = 3;
            inputLabel2.Text = "Input 2:";
            // 
            // inputLabel3
            // 
            inputLabel3.AutoSize = true;
            inputLabel3.Location = new Point(14, 151);
            inputLabel3.Margin = new Padding(4, 0, 4, 0);
            inputLabel3.Name = "inputLabel3";
            inputLabel3.Size = new Size(47, 15);
            inputLabel3.TabIndex = 5;
            inputLabel3.Text = "Input 3:";
            // 
            // inputLabel4
            // 
            inputLabel4.AutoSize = true;
            inputLabel4.Location = new Point(14, 204);
            inputLabel4.Margin = new Padding(4, 0, 4, 0);
            inputLabel4.Name = "inputLabel4";
            inputLabel4.Size = new Size(47, 15);
            inputLabel4.TabIndex = 7;
            inputLabel4.Text = "Input 4:";
            // 
            // inputLabel5
            // 
            inputLabel5.AutoSize = true;
            inputLabel5.Location = new Point(14, 257);
            inputLabel5.Margin = new Padding(4, 0, 4, 0);
            inputLabel5.Name = "inputLabel5";
            inputLabel5.Size = new Size(47, 15);
            inputLabel5.TabIndex = 9;
            inputLabel5.Text = "Input 5:";
            // 
            // inputLabel6
            // 
            inputLabel6.AutoSize = true;
            inputLabel6.Location = new Point(14, 310);
            inputLabel6.Margin = new Padding(4, 0, 4, 0);
            inputLabel6.Name = "inputLabel6";
            inputLabel6.Size = new Size(47, 15);
            inputLabel6.TabIndex = 11;
            inputLabel6.Text = "Input 6:";
            // 
            // inputLabel7
            // 
            inputLabel7.AutoSize = true;
            inputLabel7.Location = new Point(14, 363);
            inputLabel7.Margin = new Padding(4, 0, 4, 0);
            inputLabel7.Name = "inputLabel7";
            inputLabel7.Size = new Size(47, 15);
            inputLabel7.TabIndex = 13;
            inputLabel7.Text = "Input 7:";
            // 
            // dateTimeTextBox
            // 
            dateTimeTextBox.Location = new Point(14, 10);
            dateTimeTextBox.Margin = new Padding(4, 3, 4, 3);
            dateTimeTextBox.Name = "dateTimeTextBox";
            dateTimeTextBox.ReadOnly = true;
            dateTimeTextBox.Size = new Size(174, 23);
            dateTimeTextBox.TabIndex = 0;
            // 
            // disconnectButton
            // 
            disconnectButton.Location = new Point(14, 519);
            disconnectButton.Margin = new Padding(4, 3, 4, 3);
            disconnectButton.Name = "disconnectButton";
            disconnectButton.Size = new Size(175, 27);
            disconnectButton.TabIndex = 18;
            disconnectButton.Text = "Disconnect";
            disconnectButton.UseVisualStyleBackColor = true;
            disconnectButton.Visible = false;
            disconnectButton.Click += disconnectButton_Click;
            // 
            // resultsListBox
            // 
            resultsListBox.ItemHeight = 15;
            resultsListBox.Location = new Point(248, 10);
            resultsListBox.Margin = new Padding(4, 3, 4, 3);
            resultsListBox.Name = "resultsListBox";
            resultsListBox.Size = new Size(396, 454);
            resultsListBox.TabIndex = 19;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(677, 565);
            Controls.Add(dateTimeTextBox);
            Controls.Add(inputLabel7);
            Controls.Add(inputTextBox7);
            Controls.Add(inputLabel6);
            Controls.Add(inputTextBox6);
            Controls.Add(inputLabel5);
            Controls.Add(inputTextBox5);
            Controls.Add(inputLabel4);
            Controls.Add(inputTextBox4);
            Controls.Add(inputLabel3);
            Controls.Add(inputTextBox3);
            Controls.Add(inputLabel2);
            Controls.Add(inputTextBox2);
            Controls.Add(inputLabel1);
            Controls.Add(inputTextBox1);
            Controls.Add(startMeasurementButton);
            Controls.Add(connectButton);
            Controls.Add(exportButton);
            Controls.Add(disconnectButton);
            Controls.Add(resultsListBox);
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Bode Automation Interface";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

    }
}
