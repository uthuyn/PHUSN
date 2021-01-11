partial class FrmMain
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.numMaxAvg = new System.Windows.Forms.NumericUpDown();
            this.numMinAvg = new System.Windows.Forms.NumericUpDown();
            this.numMaxPer = new System.Windows.Forms.NumericUpDown();
            this.numMinPer = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnminPer = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.txtMaxLength = new System.Windows.Forms.TextBox();
            this.btnShowData = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtminUntil = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOutputFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPHUSN = new System.Windows.Forms.Button();
            this.btnOutput = new System.Windows.Forms.Button();
            this.listTreeHUSP = new System.Windows.Forms.TreeView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAvg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinAvg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPer)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.numMaxAvg);
            this.panel1.Controls.Add(this.numMinAvg);
            this.panel1.Controls.Add(this.numMaxPer);
            this.panel1.Controls.Add(this.numMinPer);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnminPer);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.txtMaxLength);
            this.panel1.Controls.Add(this.btnShowData);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtminUntil);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.txtInputFile);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtOutputFile);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnPHUSN);
            this.panel1.Controls.Add(this.btnOutput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(722, 184);
            this.panel1.TabIndex = 22;
            // 
            // numMaxAvg
            // 
            this.numMaxAvg.DecimalPlaces = 2;
            this.numMaxAvg.Location = new System.Drawing.Point(619, 151);
            this.numMaxAvg.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numMaxAvg.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxAvg.Name = "numMaxAvg";
            this.numMaxAvg.Size = new System.Drawing.Size(93, 20);
            this.numMaxAvg.TabIndex = 51;
            this.numMaxAvg.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numMinAvg
            // 
            this.numMinAvg.DecimalPlaces = 2;
            this.numMinAvg.Location = new System.Drawing.Point(444, 151);
            this.numMinAvg.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMinAvg.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinAvg.Name = "numMinAvg";
            this.numMinAvg.Size = new System.Drawing.Size(93, 20);
            this.numMinAvg.TabIndex = 50;
            this.numMinAvg.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numMaxPer
            // 
            this.numMaxPer.DecimalPlaces = 2;
            this.numMaxPer.Location = new System.Drawing.Point(265, 151);
            this.numMaxPer.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numMaxPer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxPer.Name = "numMaxPer";
            this.numMaxPer.Size = new System.Drawing.Size(93, 20);
            this.numMaxPer.TabIndex = 49;
            this.numMaxPer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numMinPer
            // 
            this.numMinPer.DecimalPlaces = 2;
            this.numMinPer.Location = new System.Drawing.Point(91, 151);
            this.numMinPer.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMinPer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinPer.Name = "numMinPer";
            this.numMinPer.Size = new System.Drawing.Size(93, 20);
            this.numMinPer.TabIndex = 48;
            this.numMinPer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(556, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 17);
            this.label7.TabIndex = 47;
            this.label7.Text = "maxAvg";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(384, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 17);
            this.label6.TabIndex = 46;
            this.label6.Text = "minAvg";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(204, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 17);
            this.label5.TabIndex = 45;
            this.label5.Text = "maxPer";
            // 
            // btnminPer
            // 
            this.btnminPer.AutoSize = true;
            this.btnminPer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnminPer.Location = new System.Drawing.Point(33, 154);
            this.btnminPer.Name = "btnminPer";
            this.btnminPer.Size = new System.Drawing.Size(52, 17);
            this.btnminPer.TabIndex = 44;
            this.btnminPer.Text = "minPer";
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.Location = new System.Drawing.Point(463, 75);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(93, 23);
            this.btnExit.TabIndex = 30;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtMaxLength
            // 
            this.txtMaxLength.Location = new System.Drawing.Point(182, 112);
            this.txtMaxLength.Name = "txtMaxLength";
            this.txtMaxLength.Size = new System.Drawing.Size(126, 20);
            this.txtMaxLength.TabIndex = 28;
            // 
            // btnShowData
            // 
            this.btnShowData.AutoSize = true;
            this.btnShowData.Location = new System.Drawing.Point(568, 10);
            this.btnShowData.Name = "btnShowData";
            this.btnShowData.Size = new System.Drawing.Size(70, 23);
            this.btnShowData.TabIndex = 26;
            this.btnShowData.Text = "Show Data";
            this.btnShowData.UseVisualStyleBackColor = true;
            this.btnShowData.Click += new System.EventHandler(this.btnShowData_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(33, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 23;
            this.label4.Text = "Max Length";
            // 
            // txtminUntil
            // 
            this.txtminUntil.Location = new System.Drawing.Point(182, 77);
            this.txtminUntil.Name = "txtminUntil";
            this.txtminUntil.Size = new System.Drawing.Size(126, 20);
            this.txtminUntil.TabIndex = 21;
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.Location = new System.Drawing.Point(490, 9);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(66, 23);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Browse";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(182, 12);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(300, 20);
            this.txtInputFile.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choose Input Data";
            // 
            // txtOutputFile
            // 
            this.txtOutputFile.Enabled = false;
            this.txtOutputFile.Location = new System.Drawing.Point(182, 43);
            this.txtOutputFile.Name = "txtOutputFile";
            this.txtOutputFile.Size = new System.Drawing.Size(300, 20);
            this.txtOutputFile.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(33, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Set Output Data";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(33, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Min Utility";
            // 
            // btnPHUSN
            // 
            this.btnPHUSN.AutoSize = true;
            this.btnPHUSN.Location = new System.Drawing.Point(333, 75);
            this.btnPHUSN.Name = "btnPHUSN";
            this.btnPHUSN.Size = new System.Drawing.Size(93, 23);
            this.btnPHUSN.TabIndex = 13;
            this.btnPHUSN.Text = "PHUSN";
            this.btnPHUSN.UseVisualStyleBackColor = true;
            this.btnPHUSN.Click += new System.EventHandler(this.btnPHUSN_Click);
            // 
            // btnOutput
            // 
            this.btnOutput.AutoSize = true;
            this.btnOutput.Location = new System.Drawing.Point(490, 40);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(66, 23);
            this.btnOutput.TabIndex = 8;
            this.btnOutput.Text = "Open File";
            this.btnOutput.UseVisualStyleBackColor = true;
            this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
            // 
            // listTreeHUSP
            // 
            this.listTreeHUSP.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listTreeHUSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listTreeHUSP.Location = new System.Drawing.Point(0, 190);
            this.listTreeHUSP.Name = "listTreeHUSP";
            this.listTreeHUSP.Size = new System.Drawing.Size(722, 471);
            this.listTreeHUSP.TabIndex = 23;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(722, 661);
            this.Controls.Add(this.listTreeHUSP);
            this.Controls.Add(this.panel1);
            this.Name = "FrmMain";
            this.Text = "PHUSN";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxAvg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinAvg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPer)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnShowData;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtminUntil;
    private System.Windows.Forms.Button btnOpen;
    private System.Windows.Forms.TextBox txtInputFile;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtOutputFile;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnPHUSN;
    private System.Windows.Forms.Button btnOutput;
    private System.Windows.Forms.TreeView listTreeHUSP;
    private System.Windows.Forms.TextBox txtMaxLength;
    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.NumericUpDown numMaxAvg;
    private System.Windows.Forms.NumericUpDown numMinAvg;
    private System.Windows.Forms.NumericUpDown numMaxPer;
    private System.Windows.Forms.NumericUpDown numMinPer;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label btnminPer;
}