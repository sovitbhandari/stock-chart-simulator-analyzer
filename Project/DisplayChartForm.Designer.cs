namespace Project3 // Defines the namespace for this project
{
    partial class ChartDisplayForm // Partial class for the form definition
    {
        private System.ComponentModel.IContainer components = null; // Container for holding components

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) // Override Dispose to free resources
        {
            if (disposing && (components != null)) components.Dispose(); // Dispose components if disposing
            base.Dispose(disposing); // Call base class Dispose
        }

        #region Windows Form Designer generated code // Marks region for designer-generated code

        /// <summary>
        /// Required method for Designer support
        /// </summary>
        private void InitializeComponent() // Method to initialize UI components
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea(); // Create first chart area
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea(); // Create second chart area
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series(); // Create first series
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series(); // Create second series
            this.dateTimePickerStartDate = new System.Windows.Forms.DateTimePicker(); // Instantiate start date picker
            this.dateTimePickerEndDate = new System.Windows.Forms.DateTimePicker(); // Instantiate end date picker
            this.chart_stockData = new System.Windows.Forms.DataVisualization.Charting.Chart(); // Instantiate chart control
            this.button_updateStockData = new System.Windows.Forms.Button(); // Instantiate update button
            this.label_updateStartDate = new System.Windows.Forms.Label(); // Instantiate start date label
            this.label_updateEndDate = new System.Windows.Forms.Label(); // Instantiate end date label
            this.hScrollBar_margin = new System.Windows.Forms.HScrollBar(); // Instantiate margin scrollbar
            this.label_margin = new System.Windows.Forms.Label(); // Instantiate margin label
            this.comboBox_upWaves = new System.Windows.Forms.ComboBox(); // Instantiate up waves combo box
            this.comboBox_downWaves = new System.Windows.Forms.ComboBox(); // Instantiate down waves combo box
            this.label_upWaves = new System.Windows.Forms.Label(); // Instantiate up waves label
            this.label_downWaves = new System.Windows.Forms.Label(); // Instantiate down waves label
            this.label1 = new System.Windows.Forms.Label(); // Instantiate generic label
            ((System.ComponentModel.ISupportInitialize)(this.chart_stockData)).BeginInit(); // Begin chart initialization block
            this.SuspendLayout(); // Temporarily suspend layout logic
            // 
            // dateTimePickerStartDate // Configuration for the start date picker
            // 
            this.dateTimePickerStartDate.Location = new System.Drawing.Point(949, 620); // Position the control
            this.dateTimePickerStartDate.Name = "dateTimePickerStartDate"; // Set control name
            this.dateTimePickerStartDate.Size = new System.Drawing.Size(252, 20); // Set control size
            this.dateTimePickerStartDate.TabIndex = 4; // Set tab order index
            this.dateTimePickerStartDate.ValueChanged += new System.EventHandler(this.DateTimePickerStartDate_ValueChanged); // Attach value changed event handler
            // 
            // dateTimePickerEndDate // Configuration for the end date picker
            // 
            this.dateTimePickerEndDate.Location = new System.Drawing.Point(949, 653); // Position the control
            this.dateTimePickerEndDate.Name = "dateTimePickerEndDate"; // Set control name
            this.dateTimePickerEndDate.Size = new System.Drawing.Size(252, 20); // Set control size
            this.dateTimePickerEndDate.TabIndex = 5; // Set tab order index
            this.dateTimePickerEndDate.ValueChanged += new System.EventHandler(this.dateTimePickerEndDate_ValueChanged); // Attach value changed event handler
            // 
            // chart_stockData // Configuration for the chart control
            // 
            chartArea1.Name = "area_OHLC"; // Name first chart area
            chartArea1.Position.Auto = false; // Disable automatic positioning
            chartArea1.Position.Height = 70F; // Set height percentage
            chartArea1.Position.Width = 100F; // Set width percentage
            chartArea2.Name = "area_Volume"; // Name second chart area
            chartArea2.Position.Auto = false; // Disable automatic positioning
            chartArea2.Position.Height = 30F; // Set height percentage
            chartArea2.Position.Width = 100F; // Set width percentage
            chartArea2.Position.Y = 70F; // Set vertical offset
            this.chart_stockData.ChartAreas.Add(chartArea1); // Add first area to chart
            this.chart_stockData.ChartAreas.Add(chartArea2); // Add second area to chart
            this.chart_stockData.Location = new System.Drawing.Point(63, 176); // Position the chart
            this.chart_stockData.Name = "chart_stockData"; // Set chart name
            series1.ChartArea = "area_OHLC"; // Assign series1 to first chart area
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick; // Set chart type to candlestick
            series1.CustomProperties = "PriceUpColor=Green, PriceDownColor=Red"; // Set color properties
            series1.IsXValueIndexed = true; // Enable indexed X values
            series1.Name = "series_OHLC"; // Name the series
            series1.YValuesPerPoint = 4; // OHLC requires 4 Y values per point
            series2.ChartArea = "area_Volume"; // Assign series2 to volume area
            series2.IsXValueIndexed = true; // Enable indexed X values
            series2.Name = "series_Volume"; // Name the series
            this.chart_stockData.Series.Add(series1); // Add series1 to chart
            this.chart_stockData.Series.Add(series2); // Add series2 to chart
            this.chart_stockData.Size = new System.Drawing.Size(1399, 424); // Set chart size
            this.chart_stockData.TabIndex = 9; // Set tab order index
            this.chart_stockData.Text = "chart1"; // Set chart text property
            this.chart_stockData.Click += new System.EventHandler(this.chart_stockData_Click); // Attach click event handler
            // 
            // button_updateStockData // Configuration for the refresh button
            // 
            this.button_updateStockData.Location = new System.Drawing.Point(213, 620); // Position the button
            this.button_updateStockData.Name = "button_updateStockData"; // Set button name
            this.button_updateStockData.Size = new System.Drawing.Size(179, 61); // Set button size
            this.button_updateStockData.TabIndex = 10; // Set tab order index
            this.button_updateStockData.Text = "Refresh Stock Data"; // Set button text
            this.button_updateStockData.Click += new System.EventHandler(this.button_updateStockData_Click); // Attach click event handler
            // 
            // label_updateStartDate // Configuration for start date label
            // 
            this.label_updateStartDate.AutoSize = true; // Enable auto sizing
            this.label_updateStartDate.Location = new System.Drawing.Point(828, 625); // Position the label
            this.label_updateStartDate.Name = "label_updateStartDate"; // Set label name
            this.label_updateStartDate.Size = new System.Drawing.Size(89, 13); // Set label size
            this.label_updateStartDate.TabIndex = 13; // Set tab order index
            this.label_updateStartDate.Text = "Update Starts On"; // Set label text
            this.label_updateStartDate.Click += new System.EventHandler(this.label_updateStartDate_Click); // Attach click event handler
            // 
            // label_updateEndDate // Configuration for end date label
            // 
            this.label_updateEndDate.AutoSize = true; // Enable auto sizing
            this.label_updateEndDate.Location = new System.Drawing.Point(828, 659); // Position the label
            this.label_updateEndDate.Name = "label_updateEndDate"; // Set label name
            this.label_updateEndDate.Size = new System.Drawing.Size(86, 13); // Set label size
            this.label_updateEndDate.TabIndex = 14; // Set tab order index
            this.label_updateEndDate.Text = "Update Ends On"; // Set label text
            this.label_updateEndDate.Click += new System.EventHandler(this.label_updateEndDate_Click); // Attach click event handler
            // 
            // hScrollBar_margin // Configuration for margin scrollbar
            // 
            this.hScrollBar_margin.LargeChange = 1; // Set large change increment
            this.hScrollBar_margin.Location = new System.Drawing.Point(1334, 638); // Position the scrollbar
            this.hScrollBar_margin.Maximum = 4; // Set maximum value
            this.hScrollBar_margin.Minimum = 1; // Set minimum value
            this.hScrollBar_margin.Name = "hScrollBar_margin"; // Set scrollbar name
            this.hScrollBar_margin.Size = new System.Drawing.Size(55, 17); // Set scrollbar size
            this.hScrollBar_margin.TabIndex = 5; // Set tab order index
            this.hScrollBar_margin.Value = 1; // Set initial value
            this.hScrollBar_margin.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBarMargin_Scroll); // Attach scroll event handler
            // 
            // label_margin // Configuration for margin label
            // 
            this.label_margin.AutoSize = true; // Enable auto sizing
            this.label_margin.Location = new System.Drawing.Point(1256, 638); // Position the label
            this.label_margin.Name = "label_margin"; // Set label name
            this.label_margin.Size = new System.Drawing.Size(51, 13); // Set label size
            this.label_margin.TabIndex = 16; // Set tab order index
            this.label_margin.Text = "Margin: 1"; // Set label text
            this.label_margin.Click += new System.EventHandler(this.label_margin_Click); // Attach click event handler
            // 
            // comboBox_upWaves // Configuration for up waves combo box
            // 
            this.comboBox_upWaves.Location = new System.Drawing.Point(548, 618); // Position the combo box
            this.comboBox_upWaves.Name = "comboBox_upWaves"; // Set name
            this.comboBox_upWaves.Size = new System.Drawing.Size(242, 21); // Set size
            this.comboBox_upWaves.TabIndex = 17; // Set tab order index
            this.comboBox_upWaves.SelectedIndexChanged += new System.EventHandler(this.comboBox_upWaves_SelectedIndexChanged); // Attach selection changed handler
            // 
            // comboBox_downWaves // Configuration for down waves combo box
            // 
            this.comboBox_downWaves.Location = new System.Drawing.Point(548, 651); // Position the combo box
            this.comboBox_downWaves.Name = "comboBox_downWaves"; // Set name
            this.comboBox_downWaves.Size = new System.Drawing.Size(242, 21); // Set size
            this.comboBox_downWaves.TabIndex = 18; // Set tab order index
            this.comboBox_downWaves.SelectedIndexChanged += new System.EventHandler(this.comboBox_downWaves_SelectedIndexChanged); // Attach selection changed handler
            // 
            // label_upWaves // Configuration for up waves label
            // 
            this.label_upWaves.AutoSize = true; // Enable auto sizing
            this.label_upWaves.Location = new System.Drawing.Point(468, 625); // Position the label
            this.label_upWaves.Name = "label_upWaves"; // Set name
            this.label_upWaves.Size = new System.Drawing.Size(61, 13); // Set size
            this.label_upWaves.TabIndex = 19; // Set tab order index
            this.label_upWaves.Text = "Up Waves:"; // Set label text
            this.label_upWaves.Click += new System.EventHandler(this.label_upWaves_Click); // Attach click event handler
            // 
            // label_downWaves // Configuration for down waves label
            // 
            this.label_downWaves.AutoSize = true; // Enable auto sizing
            this.label_downWaves.Location = new System.Drawing.Point(452, 654); // Position the label
            this.label_downWaves.Name = "label_downWaves"; // Set name
            this.label_downWaves.Size = new System.Drawing.Size(75, 13); // Set size
            this.label_downWaves.TabIndex = 20; // Set tab order index
            this.label_downWaves.Text = "Down Waves:"; // Set label text
            this.label_downWaves.Click += new System.EventHandler(this.label_downWaves_Click); // Attach click event handler
            // 
            // label1 // Configuration for validations label
            // 
            this.label1.AutoSize = true; // Enable auto sizing
            this.label1.Location = new System.Drawing.Point(77, 655); // Position the label
            this.label1.Name = "label1"; // Set name
            this.label1.Size = new System.Drawing.Size(58, 13); // Set size
            this.label1.TabIndex = 21; // Set tab order index
            this.label1.Text = "Validations"; // Set label text
            this.label1.Click += new System.EventHandler(this.label1_Click_1); // Attach click event handler
            // 
            // ChartDisplayForm // Configuration for the form itself
            // 
            this.ClientSize = new System.Drawing.Size(1546, 822); // Set form size
            this.Controls.Add(this.label1); // Add validations label to form
            this.Controls.Add(this.label_downWaves); // Add down waves label to form
            this.Controls.Add(this.label_upWaves); // Add up waves label to form
            this.Controls.Add(this.comboBox_downWaves); // Add down waves combo box to form
            this.Controls.Add(this.comboBox_upWaves); // Add up waves combo box to form
            this.Controls.Add(this.label_margin); // Add margin label to form
            this.Controls.Add(this.hScrollBar_margin); // Add margin scrollbar to form
            this.Controls.Add(this.label_updateEndDate); // Add end date label to form
            this.Controls.Add(this.label_updateStartDate); // Add start date label to form
            this.Controls.Add(this.button_updateStockData); // Add update button to form
            this.Controls.Add(this.chart_stockData); // Add chart to form
            this.Controls.Add(this.dateTimePickerEndDate); // Add end date picker to form
            this.Controls.Add(this.dateTimePickerStartDate); // Add start date picker to form
            this.Name = "ChartDisplayForm"; // Set form name
            this.Text = "Chart Data"; // Set form title text
            this.Load += new System.EventHandler(this.ChartDisplayForm_Load); // Attach form load event handler
            ((System.ComponentModel.ISupportInitialize)(this.chart_stockData)).EndInit(); // End chart initialization block
            this.ResumeLayout(false); // Resume layout logic without performing a layout immediately
            this.PerformLayout(); // Apply pending layout logic

        }

        #endregion // End of designer-generated code region

        private System.Windows.Forms.DateTimePicker dateTimePickerStartDate; // Field for start date picker
        private System.Windows.Forms.DateTimePicker dateTimePickerEndDate; // Field for end date picker
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_stockData; // Field for chart control
        private System.Windows.Forms.Button button_updateStockData; // Field for update button
        private System.Windows.Forms.Label label_updateStartDate; // Field for start date label
        private System.Windows.Forms.Label label_updateEndDate; // Field for end date label
        private System.Windows.Forms.HScrollBar hScrollBar_margin; // Field for margin scrollbar
        private System.Windows.Forms.Label label_margin; // Field for margin label
        private System.Windows.Forms.ComboBox comboBox_upWaves; // Field for up waves combo box
        private System.Windows.Forms.ComboBox comboBox_downWaves; // Field for down waves combo box
        private System.Windows.Forms.Label label_upWaves; // Field for up waves label
        private System.Windows.Forms.Label label_downWaves; // Field for down waves label
        private System.Windows.Forms.Label label1; // Field for generic label
    }
}
