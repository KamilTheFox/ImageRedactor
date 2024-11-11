namespace ImageRedactor
{
    partial class MainMenu
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
            buttonOpenImage = new Button();
            pictureBox1 = new PictureBox();
            procentCompress = new TextBox();
            buttonCompress = new Button();
            saveImage = new Button();
            progressBar1 = new ProgressBar();
            buttonNegative = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // buttonOpenImage
            // 
            buttonOpenImage.Location = new Point(9, 13);
            buttonOpenImage.Name = "buttonOpenImage";
            buttonOpenImage.Size = new Size(158, 23);
            buttonOpenImage.TabIndex = 0;
            buttonOpenImage.Text = "Открыть изображение";
            buttonOpenImage.UseVisualStyleBackColor = true;
            buttonOpenImage.Click += buttonOpenImage_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(173, 11);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(766, 670);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // procentCompress
            // 
            procentCompress.Location = new Point(9, 43);
            procentCompress.Name = "procentCompress";
            procentCompress.Size = new Size(158, 23);
            procentCompress.TabIndex = 2;
            // 
            // buttonCompress
            // 
            buttonCompress.Location = new Point(9, 72);
            buttonCompress.Name = "buttonCompress";
            buttonCompress.Size = new Size(158, 23);
            buttonCompress.TabIndex = 3;
            buttonCompress.Text = "Компресс";
            buttonCompress.UseVisualStyleBackColor = true;
            buttonCompress.Click += buttonCompress_Click;
            // 
            // saveImage
            // 
            saveImage.Location = new Point(9, 101);
            saveImage.Name = "saveImage";
            saveImage.Size = new Size(161, 23);
            saveImage.TabIndex = 4;
            saveImage.Text = "Сохранить";
            saveImage.UseVisualStyleBackColor = true;
            saveImage.Click += saveImage_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(9, 128);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(158, 23);
            progressBar1.TabIndex = 5;
            // 
            // buttonNegative
            // 
            buttonNegative.Location = new Point(9, 157);
            buttonNegative.Name = "buttonNegative";
            buttonNegative.Size = new Size(158, 23);
            buttonNegative.TabIndex = 6;
            buttonNegative.Text = "Негатив";
            buttonNegative.UseVisualStyleBackColor = true;
            buttonNegative.Click += buttonNegative_Click;
            // 
            // MainMenu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            ClientSize = new Size(951, 693);
            Controls.Add(buttonNegative);
            Controls.Add(progressBar1);
            Controls.Add(saveImage);
            Controls.Add(buttonCompress);
            Controls.Add(procentCompress);
            Controls.Add(pictureBox1);
            Controls.Add(buttonOpenImage);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainMenu";
            Text = "ImageCompressor";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonOpenImage;
        private PictureBox pictureBox1;
        private TextBox procentCompress;
        private Button buttonCompress;
        private Button saveImage;
        private ProgressBar progressBar1;
        private Button buttonNegative;
    }
}