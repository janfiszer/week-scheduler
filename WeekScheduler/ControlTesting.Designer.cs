namespace WeekScheduler
{
    partial class ControlTesting
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
            this.weekPlaner1 = new WeekScheduler.weekPlaner();
            this.SuspendLayout();
            // 
            // weekPlaner1
            // 
            this.weekPlaner1.Location = new System.Drawing.Point(13, 13);
            this.weekPlaner1.MaxWeeksFoward = 20;
            this.weekPlaner1.Name = "weekPlaner1";
            this.weekPlaner1.Size = new System.Drawing.Size(1107, 562);
            this.weekPlaner1.TabIndex = 0;
            // 
            // ControlTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 615);
            this.Controls.Add(this.weekPlaner1);
            this.Name = "ControlTesting";
            this.Text = "ControlTesting";
            this.ResumeLayout(false);

        }

        #endregion

        private weekPlaner weekPlaner1;
    }
}