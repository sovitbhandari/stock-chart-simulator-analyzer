using System; // Provides basic system types and Math functions

namespace Project3 // Defines the Project_2 namespace
{
    /// <summary>  
    /// Extends CandleStick with numeric helpers for range, body, shadows, etc.  
    /// </summary>  
    public class SmartCandleStick : CandleStick // Inherits from base CandleStick class
    {
        public decimal Range => High - Low;
        // Calculates full height of the candle: high price minus low price

        public decimal Body => Math.Abs(Open - Close);
        // Calculates the body size: absolute difference between open and close prices

        public decimal UpperShadow => High - Math.Max(Open, Close);
        // Calculates upper wick (shadow): high price minus the higher of open/close

        public decimal LowerShadow => Math.Min(Open, Close) - Low;
        // Calculates lower wick (shadow): lower of open/close minus low price

        /// <summary>  
        /// Construct from CSV directly.  
        /// </summary>  
        public SmartCandleStick(string csvLine) : base(csvLine) { }
        // Calls base constructor to parse a CSV line into CandleStick fields

        /// <summary>  
        /// Copy ctor: build from existing CandleStick fields.  
        /// </summary>  
        public SmartCandleStick(CandleStick cs) : base() // Calls default base constructor
        {
            Date = cs.Date;        // Copies the date value
            Open = cs.Open;        // Copies the open price
            High = cs.High;        // Copies the high price
            Low = cs.Low;          // Copies the low price
            Close = cs.Close;      // Copies the close price
            Volume = cs.Volume;    // Copies the trading volume
        }
    }
}
