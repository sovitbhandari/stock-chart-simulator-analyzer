using System;                       // Provides fundamental .NET types and base functionality
using System.Globalization;         // Provides culture-specific formatting and parsing

namespace Project3                 // Defines Project_2 namespace to group related classes
{
    /// <summary>
    /// Represents one trading day’s OHLC and volume data for a candlestick.
    /// </summary>
    public class CandleStick       // Base class for a single candlestick record
    {
        public DateTime Date { get; protected set; }   // Trading date
        public decimal Open { get; protected set; }    // Opening price
        public decimal High { get; protected set; }    // Highest price of the day
        public decimal Low { get; protected set; }     // Lowest price of the day
        public decimal Close { get; protected set; }   // Closing price
        public long Volume { get; protected set; }     // Number of shares traded

        /// <summary>
        /// Initializes a CandleStick by parsing a CSV line in the format:
        /// Date,Open,High,Low,Close,Volume
        /// </summary>
        public CandleStick(string csvLine)              // Constructor that parses CSV input
        {
            if (string.IsNullOrWhiteSpace(csvLine))     // Check for null or whitespace
                throw new FormatException("Empty or null CSV line."); // Throw if invalid

            csvLine = csvLine.Replace("\"", "");        // Remove any double-quote characters
            string[] fields = csvLine.Split(',');       // Split line into fields by comma
            if (fields.Length != 6)                     // Ensure exactly six fields are present
                throw new FormatException(
                    $"Line not valid. Expected 6 fields: {csvLine}" // Error if wrong format
                );

            try
            {
                // Parse date string "yyyy-MM-dd" exactly using invariant culture
                Date = DateTime.ParseExact(fields[0].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                Open = decimal.Parse(fields[1], CultureInfo.InvariantCulture);   // Parse open price
                High = decimal.Parse(fields[2], CultureInfo.InvariantCulture);   // Parse high price
                Low = decimal.Parse(fields[3], CultureInfo.InvariantCulture);    // Parse low price
                Close = decimal.Parse(fields[4], CultureInfo.InvariantCulture);  // Parse close price
                Volume = long.Parse(fields[5], CultureInfo.InvariantCulture);    // Parse volume as long
            }
            catch (Exception ex)                         // Catch any parsing exceptions
            {
                throw new FormatException($"Parse fail: '{csvLine}' {ex.Message}", ex); // Wrap and rethrow
            }
        }

        /// <summary>
        /// Protected default constructor for use by derived classes or serializers.
        /// </summary>
        protected CandleStick()                        // Default constructor with no parameters
        {
            Date = DateTime.MinValue;                 // Initialize date to minimum value
        }
    }
}
