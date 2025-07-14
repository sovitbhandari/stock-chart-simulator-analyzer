using System;                       // Provides fundamental classes and base types
using System.Windows.Forms;         // Provides classes for creating Windows-based applications

namespace Project3               // Defines the Project_2 namespace
{
    /// <summary>  
    /// Application entry point.  
    /// </summary>  
    internal static class Program    // Static class containing the Main method
    {
        [STAThread]                // Marks the COM threading model for the application as single-threaded apartment
        static void Main()          // Main entry point method for the application
        {
            Application.EnableVisualStyles();                // Enables visual styles for controls to match OS theme
            Application.SetCompatibleTextRenderingDefault(false); // Uses GDI+ text rendering for controls by default
            Application.Run(new Form1());                    // Creates and runs the main application form (Form1)
        }
    }
}
