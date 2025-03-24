using ThermoFisher.CommonCore.Data;

namespace i2i_learn
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            plotWindow1 = new OxyPlot.WindowsForms.PlotView();
            btnLoad = new Button();
            dropdownFilter = new ComboBox();
            btnFolder = new Button();
            textFilePath = new TextBox();
            textNumFiles = new NumericUpDown();
            lblScanFilter = new Label();
            lblFilePath = new Label();
            lblNumOfFiles = new Label();
            btnLoadAll = new Button();
            numPPM = new NumericUpDown();
            txtPPMtol = new Label();
            listBoxTolUnit = new ListBox();
            label1 = new Label();
            textBoxAnalyzer = new TextBox();
            label2 = new Label();
            button1 = new Button();
            progressBarFileLoad = new ProgressBar();
            button2 = new Button();
            numericMzValue = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            numericSpacingBetweenLines = new NumericUpDown();
            numericVelocity = new NumericUpDown();
            button3 = new Button();
            pictureBox1 = new PictureBox();
            button4 = new Button();
            toleranceBindingSource = new BindingSource(components);
            averageButton = new Button();
            buttonROI = new Button();
            ((System.ComponentModel.ISupportInitialize)textNumFiles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPPM).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericMzValue).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericSpacingBetweenLines).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericVelocity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)toleranceBindingSource).BeginInit();
            SuspendLayout();
            // 
            // plotWindow1
            // 
            plotWindow1.BackColor = SystemColors.Window;
            plotWindow1.Location = new Point(27, 30);
            plotWindow1.Name = "plotWindow1";
            plotWindow1.PanCursor = Cursors.Hand;
            plotWindow1.Size = new Size(345, 228);
            plotWindow1.TabIndex = 0;
            plotWindow1.Text = "plotView1";
            plotWindow1.ZoomHorizontalCursor = Cursors.No;
            plotWindow1.ZoomRectangleCursor = Cursors.No;
            plotWindow1.ZoomVerticalCursor = Cursors.No;
            plotWindow1.MouseClick += testROI;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(262, 398);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 1;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += Form1_Load;
            // 
            // dropdownFilter
            // 
            dropdownFilter.FormattingEnabled = true;
            dropdownFilter.Location = new Point(499, 106);
            dropdownFilter.MaxDropDownItems = 20;
            dropdownFilter.Name = "dropdownFilter";
            dropdownFilter.Size = new Size(192, 23);
            dropdownFilter.TabIndex = 2;
            // 
            // btnFolder
            // 
            btnFolder.Location = new Point(499, 12);
            btnFolder.Name = "btnFolder";
            btnFolder.Size = new Size(75, 23);
            btnFolder.TabIndex = 3;
            btnFolder.Text = "Pick Folder";
            btnFolder.UseVisualStyleBackColor = true;
            btnFolder.Click += button1_Click;
            // 
            // textFilePath
            // 
            textFilePath.Location = new Point(499, 62);
            textFilePath.Name = "textFilePath";
            textFilePath.Size = new Size(192, 23);
            textFilePath.TabIndex = 4;
            textFilePath.TextChanged += textFilePath_TextChanged;
            // 
            // textNumFiles
            // 
            textNumFiles.Location = new Point(710, 106);
            textNumFiles.Name = "textNumFiles";
            textNumFiles.Size = new Size(45, 23);
            textNumFiles.TabIndex = 6;
            // 
            // lblScanFilter
            // 
            lblScanFilter.AutoSize = true;
            lblScanFilter.Location = new Point(499, 88);
            lblScanFilter.Name = "lblScanFilter";
            lblScanFilter.Size = new Size(61, 15);
            lblScanFilter.TabIndex = 7;
            lblScanFilter.Text = "Scan Filter";
            lblScanFilter.Click += label1_Click;
            // 
            // lblFilePath
            // 
            lblFilePath.AutoSize = true;
            lblFilePath.Location = new Point(499, 45);
            lblFilePath.Name = "lblFilePath";
            lblFilePath.Size = new Size(55, 15);
            lblFilePath.TabIndex = 8;
            lblFilePath.Text = "Directory";
            // 
            // lblNumOfFiles
            // 
            lblNumOfFiles.AutoSize = true;
            lblNumOfFiles.Location = new Point(710, 88);
            lblNumOfFiles.Name = "lblNumOfFiles";
            lblNumOfFiles.Size = new Size(89, 15);
            lblNumOfFiles.TabIndex = 9;
            lblNumOfFiles.Text = "Number of files";
            // 
            // btnLoadAll
            // 
            btnLoadAll.Location = new Point(569, 398);
            btnLoadAll.Name = "btnLoadAll";
            btnLoadAll.Size = new Size(75, 23);
            btnLoadAll.TabIndex = 10;
            btnLoadAll.Text = "Load All";
            btnLoadAll.UseVisualStyleBackColor = true;
            btnLoadAll.Click += btnLoadAll_Click;
            btnLoadAll.MouseClick += clickLoadAll;
            // 
            // numPPM
            // 
            numPPM.DecimalPlaces = 1;
            numPPM.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numPPM.Location = new Point(600, 154);
            numPPM.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numPPM.Name = "numPPM";
            numPPM.Size = new Size(44, 23);
            numPPM.TabIndex = 11;
            numPPM.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // txtPPMtol
            // 
            txtPPMtol.AutoSize = true;
            txtPPMtol.Location = new Point(600, 136);
            txtPPMtol.Name = "txtPPMtol";
            txtPPMtol.Size = new Size(88, 15);
            txtPPMtol.TabIndex = 12;
            txtPPMtol.Text = "Tolerance value";
            txtPPMtol.Click += label1_Click_1;
            // 
            // listBoxTolUnit
            // 
            listBoxTolUnit.FormattingEnabled = true;
            listBoxTolUnit.ItemHeight = 15;
            listBoxTolUnit.Items.AddRange(new object[] { "mmu", "amu", "ppm" });
            listBoxTolUnit.Location = new Point(496, 154);
            listBoxTolUnit.Name = "listBoxTolUnit";
            listBoxTolUnit.Size = new Size(89, 49);
            listBoxTolUnit.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(496, 136);
            label1.Name = "label1";
            label1.Size = new Size(81, 15);
            label1.TabIndex = 15;
            label1.Text = "Tolerance unit";
            label1.Click += label1_Click_2;
            // 
            // textBoxAnalyzer
            // 
            textBoxAnalyzer.Location = new Point(660, 26);
            textBoxAnalyzer.Name = "textBoxAnalyzer";
            textBoxAnalyzer.Size = new Size(100, 23);
            textBoxAnalyzer.TabIndex = 16;
            textBoxAnalyzer.Text = "N/A";
            textBoxAnalyzer.UseWaitCursor = true;
            textBoxAnalyzer.TextChanged += textBox1_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(660, 8);
            label2.Name = "label2";
            label2.Size = new Size(80, 15);
            label2.TabIndex = 17;
            label2.Text = "Mass analyzer";
            label2.Click += label2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(451, 400);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 19;
            button1.Text = "TestLoadMatrixAll";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // progressBarFileLoad
            // 
            progressBarFileLoad.Location = new Point(499, 251);
            progressBarFileLoad.Name = "progressBarFileLoad";
            progressBarFileLoad.Size = new Size(100, 23);
            progressBarFileLoad.TabIndex = 20;
            // 
            // button2
            // 
            button2.Location = new Point(613, 251);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 22;
            button2.Text = "Load data";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // numericMzValue
            // 
            numericMzValue.DecimalPlaces = 4;
            numericMzValue.Location = new Point(668, 180);
            numericMzValue.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericMzValue.Name = "numericMzValue";
            numericMzValue.Size = new Size(120, 23);
            numericMzValue.TabIndex = 23;
            numericMzValue.Value = new decimal(new int[] { 8664821, 0, 0, 262144 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 382);
            label3.Name = "label3";
            label3.Size = new Size(49, 15);
            label3.TabIndex = 24;
            label3.Text = "Spacing";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(90, 382);
            label4.Name = "label4";
            label4.Size = new Size(96, 15);
            label4.TabIndex = 25;
            label4.Text = "Velocity of probe";
            // 
            // numericSpacingBetweenLines
            // 
            numericSpacingBetweenLines.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numericSpacingBetweenLines.Location = new Point(12, 400);
            numericSpacingBetweenLines.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numericSpacingBetweenLines.Name = "numericSpacingBetweenLines";
            numericSpacingBetweenLines.Size = new Size(60, 23);
            numericSpacingBetweenLines.TabIndex = 26;
            numericSpacingBetweenLines.Value = new decimal(new int[] { 150, 0, 0, 0 });
            // 
            // numericVelocity
            // 
            numericVelocity.ImeMode = ImeMode.On;
            numericVelocity.Location = new Point(90, 400);
            numericVelocity.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numericVelocity.Name = "numericVelocity";
            numericVelocity.Size = new Size(77, 23);
            numericVelocity.TabIndex = 27;
            numericVelocity.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // button3
            // 
            button3.Location = new Point(320, 331);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 28;
            button3.Text = "updatefigure";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(444, 372);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(82, 22);
            pictureBox1.TabIndex = 21;
            pictureBox1.TabStop = false;
            // 
            // button4
            // 
            button4.Location = new Point(569, 359);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 29;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // toleranceBindingSource
            // 
           // toleranceBindingSource.DataSource = typeof(ThermoFisher.CommonCore.Data.Business.Tolerance);
            // 
            // averageButton
            // 
            averageButton.Location = new Point(703, 347);
            averageButton.Name = "averageButton";
            averageButton.Size = new Size(75, 23);
            averageButton.TabIndex = 30;
            averageButton.Text = "Average";
            averageButton.UseVisualStyleBackColor = true;
            averageButton.Click += averageButton_Click;
            // 
            // buttonROI
            // 
            buttonROI.Location = new Point(409, 251);
            buttonROI.Name = "buttonROI";
            buttonROI.Size = new Size(75, 23);
            buttonROI.TabIndex = 31;
            buttonROI.Text = "ROI";
            buttonROI.UseVisualStyleBackColor = true;
            buttonROI.Click += buttonROI_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonROI);
            Controls.Add(averageButton);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(numericVelocity);
            Controls.Add(numericSpacingBetweenLines);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(numericMzValue);
            Controls.Add(button2);
            Controls.Add(pictureBox1);
            Controls.Add(progressBarFileLoad);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(textBoxAnalyzer);
            Controls.Add(label1);
            Controls.Add(listBoxTolUnit);
            Controls.Add(txtPPMtol);
            Controls.Add(numPPM);
            Controls.Add(btnLoadAll);
            Controls.Add(lblNumOfFiles);
            Controls.Add(lblFilePath);
            Controls.Add(lblScanFilter);
            Controls.Add(textNumFiles);
            Controls.Add(textFilePath);
            Controls.Add(btnFolder);
            Controls.Add(dropdownFilter);
            Controls.Add(btnLoad);
            Controls.Add(plotWindow1);
            Name = "Form1";
            Text = "Ion-to-image in C#";
            ((System.ComponentModel.ISupportInitialize)textNumFiles).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPPM).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericMzValue).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericSpacingBetweenLines).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericVelocity).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)toleranceBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OxyPlot.WindowsForms.PlotView plotWindow1;
        private Button btnLoad;
        private Button btnFolder;
        private TextBox textFilePath;
        private NumericUpDown textNumFiles;
        private Label lblScanFilter;
        private Label lblFilePath;
        private Label lblNumOfFiles;
        private Button btnLoadAll;
        private NumericUpDown numPPM;
        private Label txtPPMtol;
        private ListBox listBoxTolUnit;
        private Label label1;
        private TextBox textBoxAnalyzer;
        private Label label2;
        public ComboBox dropdownFilter;
        private Button button1;
        private ProgressBar progressBarFileLoad;
        private Button button2;
        private NumericUpDown numericMzValue;
        private Label label3;
        private Label label4;
        private NumericUpDown numericSpacingBetweenLines;
        private NumericUpDown numericVelocity;
        private Button button3;
        private PictureBox pictureBox1;
        private Button button4;
        private BindingSource toleranceBindingSource;
        private Button averageButton;
        private Button buttonROI;
    }
}
