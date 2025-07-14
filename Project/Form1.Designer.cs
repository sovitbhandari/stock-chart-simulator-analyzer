using System; // Import basic system namespace
using System.Windows.Forms; // Import Windows Forms namespace

namespace Project3
{
    partial class Form1 // Partial class for the main form
    {
        private System.ComponentModel.IContainer components = null; // Container for form components

        /// <summary>
        /// Disposes resources used by Form1.
        /// </summary>
        /// <param name="disposing">True to dispose managed resources.</param>
        protected override void Dispose(bool disposing) // Override Dispose to clean up resources
        {
            if (disposing && (components != null)) // Check if disposing and components exist
            {
                components.Dispose(); // Dispose managed components
            }
            base.Dispose(disposing); // Call base class Dispose
        }

        #region Windows Form Designer generated code // Region for designer-generated layout code

        /// <summary>
        /// Initializes the form's components and layout.
        /// </summary>
        private void InitializeComponent() // Method to set up controls
        {
            this.button_loadStockData = new System.Windows.Forms.Button(); // Instantiate Load Data button
            this.dateTimePicker_endDate = new System.Windows.Forms.DateTimePicker(); // Instantiate end date picker
            this.dateTimePicker_startDate = new System.Windows.Forms.DateTimePicker(); // Instantiate start date picker
            this.label_startDate = new System.Windows.Forms.Label(); // Instantiate start date label
            this.label_endDate = new System.Windows.Forms.Label(); // Instantiate end date label
            this.openFileDialog_load = new System.Windows.Forms.OpenFileDialog(); // Instantiate file open dialog
            this.SuspendLayout(); // Suspend layout logic
            // 
            // button_loadStockData
            // 
            this.button_loadStockData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left))); // Anchor to bottom-left
            this.button_loadStockData.BackColor = System.Drawing.SystemColors.ActiveCaption; // Set background color
            this.button_loadStockData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F); // Set button font
            this.button_loadStockData.Location = new System.Drawing.Point(44, 97); // Position on form
            this.button_loadStockData.Name = "button_loadStockData"; // Control name
            this.button_loadStockData.Size = new System.Drawing.Size(226, 46); // Control size
            this.button_loadStockData.TabIndex = 11; // Tab order index
            this.button_loadStockData.Text = "Load Data"; // Button text
            this.button_loadStockData.UseVisualStyleBackColor = false; // Do not use default visual style background
            this.button_loadStockData.Click += new System.EventHandler(this.button_loadStockData_Click); // Attach click event handler
            // 
            // dateTimePicker_endDate
            // 
            this.dateTimePicker_endDate.Anchor = System.Windows.Forms.AnchorStyles.Right; // Anchor to right
            this.dateTimePicker_endDate.Location = new System.Drawing.Point(211, 53); // Position on form
            this.dateTimePicker_endDate.Name = "dateTimePicker_endDate"; // Control name
            this.dateTimePicker_endDate.Size = new System.Drawing.Size(254, 20); // Control size
            this.dateTimePicker_endDate.TabIndex = 12; // Tab order index
            this.dateTimePicker_endDate.ValueChanged += new System.EventHandler(this.dateTimePicker_endDate_ValueChanged); // Attach value changed event handler
            // 
            // dateTimePicker_startDate
            // 
            this.dateTimePicker_startDate.Anchor = System.Windows.Forms.AnchorStyles.Left; // Anchor to left
            this.dateTimePicker_startDate.Location = new System.Drawing.Point(211, 14); // Position on form
            this.dateTimePicker_startDate.Name = "dateTimePicker_startDate"; // Control name
            this.dateTimePicker_startDate.Size = new System.Drawing.Size(252, 20); // Control size
            this.dateTimePicker_startDate.TabIndex = 13; // Tab order index
            // 
            // label_startDate
            // 
            this.label_startDate.Anchor = System.Windows.Forms.AnchorStyles.Left; // Anchor to left
            this.label_startDate.AutoSize = true; // Enable auto-resizing
            this.label_startDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Underline); // Set font and underline style
            this.label_startDate.Location = new System.Drawing.Point(41, 14); // Position on form
            this.label_startDate.Name = "label_startDate"; // Control name
            this.label_startDate.Size = new System.Drawing.Size(72, 17); // Control size
            this.label_startDate.TabIndex = 14; // Tab order index
            this.label_startDate.Text = "Start Date"; // Label text
            this.label_startDate.Click += new System.EventHandler(this.label_startDate_Click); // Attach click event handler
            // 
            // label_endDate
            // 
            this.label_endDate.Anchor = System.Windows.Forms.AnchorStyles.Right; // Anchor to right
            this.label_endDate.AutoSize = true; // Enable auto-resizing
            this.label_endDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Underline); // Set font and underline style
            this.label_endDate.Location = new System.Drawing.Point(41, 55); // Position on form
            this.label_endDate.Name = "label_endDate"; // Control name
            this.label_endDate.Size = new System.Drawing.Size(67, 17); // Control size
            this.label_endDate.TabIndex = 15; // Tab order index
            this.label_endDate.Text = "End Date"; // Label text
            // 
            // openFileDialog_load
            // 
            this.openFileDialog_load.FileName = "ABBV-Day"; // Default file name
            this.openFileDialog_load.Filter = "All Stock files|*.csv|Daily Stocks|*-Day.csv|Weekly Stocks|*-Week.csv|Monthly Stocks|*-Month.csv"; // File filter options
            this.openFileDialog_load.Multiselect = true; // Allow multiple file selection
            this.openFileDialog_load.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_load_FileOk); // Attach FileOk event handler
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(591, 155); // Set form size
            this.Controls.Add(this.label_endDate); // Add end date label to form
            this.Controls.Add(this.label_startDate); // Add start date label to form
            this.Controls.Add(this.dateTimePicker_startDate); // Add start date picker to form
            this.Controls.Add(this.dateTimePicker_endDate); // Add end date picker to form
            this.Controls.Add(this.button_loadStockData); // Add load data button to form
            this.Name = "Form1"; // Form name
            this.Text = "Stock Data/Chart Analysis"; // Form title text
            this.Load += new System.EventHandler(this.Form1_Load); // Attach form load event handler
            this.ResumeLayout(false); // Resume layout logic
            this.PerformLayout(); // Apply pending layout logic

        }

        #endregion // End of designer-generated code region

        private System.Windows.Forms.Button button_loadStockData; // Field for load data button
        private System.Windows.Forms.DateTimePicker dateTimePicker_endDate; // Field for end date picker
        private System.Windows.Forms.DateTimePicker dateTimePicker_startDate; // Field for start date picker
        private System.Windows.Forms.Label label_startDate; // Field for start date label
        private System.Windows.Forms.Label label_endDate; // Field for end date label
        private System.Windows.Forms.OpenFileDialog openFileDialog_load; // Field for open file dialog
    }
}
