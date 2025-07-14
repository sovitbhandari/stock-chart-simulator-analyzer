using System; // Import basic system utilities
using System.Collections.Generic; // Import generic collections (List, etc.)
using System.IO; // Import IO functionality for file operations
using System.Linq; // Import LINQ for data querying and sorting
using System.Windows.Forms; // Import Windows Forms UI components

namespace Project3 // Define the namespace for the project
{
    /// <summary>
    /// The main form that loads stock data and initiates chart display.
    /// </summary>
    public partial class Form1 : Form // Form1 inherits from Windows Forms Form
    {
        /// <summary>
        /// Constructor that initializes Form1 with default settings.
        /// </summary>
        public Form1() // Form1 constructor
        {
            InitializeComponent(); // Initialize UI components from designer file
            dateTimePicker_endDate.Value = DateTime.Now; // Set end date picker to today's date
            dateTimePicker_startDate.Value = DateTime.Now.AddMonths(-1); // Set start date picker to one month ago
        }

        /// <summary>
        /// Handles the click event for the load stock data button, opening a file dialog and creating chart forms.
        /// </summary>
        /// <param name="sender">The object that triggered the event (button).</param>
        /// <param name="sender">The event arguments (unused here).</param>
        private void button_loadStockData_Click(object sender, EventArgs e) // Event handler for Load Data button click
        {
            DialogResult result = openFileDialog_load.ShowDialog(); // Show file dialog and store result
            if (result == DialogResult.OK) // If user clicked OK
            {
                foreach (string file in openFileDialog_load.FileNames) // For each selected file
                {
                    try // Try to read and parse the file
                    {
                        string[] lines = File.ReadAllLines(file); // Read all lines from the CSV file
                        if (lines.Length <= 1) // If file is empty or only header
                        {
                            MessageBox.Show($"File {Path.GetFileName(file)} is empty or only a header."); // Warn user
                            continue; // Skip to next file
                        }

                        List<CandleStick> candlesticks = new List<CandleStick>(); // Create list to hold parsed data
                        for (int i = 1; i < lines.Length; i++) // Loop over lines, skipping header
                        {
                            try // Try parsing each line
                            {
                                var cs = new CandleStick(lines[i]); // Parse CSV line into CandleStick object
                                candlesticks.Add(cs); // Add to list
                            }
                            catch (FormatException ex) // Catch format errors
                            {
                                MessageBox.Show($"Parsing issue on line {i + 1}:\n{ex.Message}"); // Show parse error
                                continue; // Skip invalid line
                            }
                        }

                        candlesticks = candlesticks.OrderBy(cs => cs.Date).ToList(); // Sort candlesticks by date
                        if (candlesticks.Count == 0) // If no valid data parsed
                        {
                            MessageBox.Show($"No valid data in {Path.GetFileName(file)}."); // Warn user
                            continue; // Skip to next file
                        }

                        // Instantiate and show the chart display form
                        ChartDisplayForm cdf = new ChartDisplayForm(
                            file, // Pass file path
                            candlesticks, // Pass parsed data
                            dateTimePicker_startDate, // Pass start date picker control
                            dateTimePicker_endDate // Pass end date picker control
                        );
                        cdf.Show(); // Display the chart form
                    }
                    catch (Exception ex) // Catch any file loading errors
                    {
                        MessageBox.Show($"File load failed: {ex.Message}"); // Show error message
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for when the file dialog confirms selection (not used).
        /// </summary>
        /// <param name="sender">The object that triggered the event (dialog).</param>
        /// <param name="e">The cancel event arguments (unused here).</param>
        private void openFileDialog_load_FileOk(object sender, System.ComponentModel.CancelEventArgs e) // Handler for FileOk event
        {
            // Stub: No action needed
        }

        /// <summary>
        /// Event handler for clicking the start date label (not used).
        /// </summary>
        /// <param name="sender">The object that triggered the event (label).</param>
        /// <param name="e">The event arguments (unused here).</param>
        private void label_startDate_Click(object sender, EventArgs e) // Handler for label click
        {
            // Stub: No action needed
        }

        /// <summary>
        /// Event handler for when the form loads (not used).
        /// </summary>
        /// <param name="sender">The object that triggered the event (form).</param>
        /// <param name="e">The event arguments (unused here).</param>
        private void Form1_Load(object sender, EventArgs e) // Handler for form load event
        {
            // Set start and end dates to February 2021
            dateTimePicker_startDate.Value = new DateTime(2021, 2, 1); // Set start date picker value
            dateTimePicker_endDate.Value = new DateTime(2021, 2, 28); // Set end date picker value

            string defaultFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ABBV-Day.csv"); // Determine default CSV file path

            if (!File.Exists(defaultFile)) // If default file does not exist
            {
                MessageBox.Show("Default file ABBV.csv not found in application directory."); // Inform user
                return; // Exit handler
            }

            try // Try loading default file data
            {
                string[] lines = File.ReadAllLines(defaultFile); // Read all lines from default file
                if (lines.Length <= 1) // If file is empty or only has header
                {
                    MessageBox.Show("ABBV.csv is empty or only contains a header."); // Warn user
                    return; // Exit handler
                }

                List<CandleStick> candlesticks = new List<CandleStick>(); // Create list for parsed data
                for (int i = 1; i < lines.Length; i++) // Loop over lines skipping header
                {
                    try // Try parsing each line
                    {
                        var cs = new CandleStick(lines[i]); // Parse line into CandleStick object
                        candlesticks.Add(cs); // Add to list
                    }
                    catch (FormatException ex) // Catch format errors
                    {
                        MessageBox.Show($"Parsing issue on line {i + 1}:\n{ex.Message}"); // Show parse error
                        continue; // Skip invalid line
                    }
                }

                candlesticks = candlesticks.OrderBy(cs => cs.Date).ToList(); // Sort by date
                if (candlesticks.Count == 0) // If no valid data
                {
                    MessageBox.Show("No valid data in ABBV.csv."); // Warn user
                    return; // Exit handler
                }

                ChartDisplayForm cdf = new ChartDisplayForm(
                    defaultFile, // Pass default file path
                    candlesticks, // Pass parsed data
                    dateTimePicker_startDate, // Pass start date picker
                    dateTimePicker_endDate // Pass end date picker
                );
                cdf.Show(); // Display chart form
            }
            catch (Exception ex) // Catch loading errors
            {
                MessageBox.Show($"Failed to load ABBV.csv: {ex.Message}"); // Show error message
            }
        }

        /// <summary>
        /// Event handler for when the end date picker value changes (not used).
        /// </summary>
        /// <param name="sender">The object that triggered the event (picker).</param>
        /// <param name="e">The event arguments (unused here).</param>
        private void dateTimePicker_endDate_ValueChanged(object sender, EventArgs e) // Handler for end date change
        {
            // Stub: No action needed
        }
    }
}
