using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICEData
{
    class Tools
    {
        /* Create a processing Class. */
        processing processing = new processing();

        public Tools()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Display a message box with details about an error or information about some properties.
        /// </summary>
        /// <param name="message">Message to display.</param>
        public void messageBox(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Information",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Display a message box with details about an error or information about some properties.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <param name="caption">Caption for the message box.</param>
        public void messageBox(string message, string caption)
        {
            System.Windows.Forms.MessageBox.Show(message, caption,
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Function opens a dialog box to browse and select the data file.
        /// </summary>
        /// <returns>The full filename of the data file.</returns>
        public string selectDataFile()
        {

            /* Declare the filename to return. */
            string filename = null;

            /* Create a fileDialog browser to select the data file. */
            OpenFileDialog fileSelectBrowser = new OpenFileDialog();
            /* Set the browser properties. */
            fileSelectBrowser.Title = "Select a file.";
            fileSelectBrowser.InitialDirectory =
                @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";
            fileSelectBrowser.Filter = //"Text Files|*.csv";
             "All EXCEL FILES (*.xlsx*)|*.xlsx*|All files (*.*)|*.*";
            fileSelectBrowser.FilterIndex = 2;
            fileSelectBrowser.RestoreDirectory = true;
            try
            {
                /* Open the browser and select a file. */
                if (fileSelectBrowser.ShowDialog() == DialogResult.OK)
                {
                    filename = fileSelectBrowser.FileName;
                }
                else
                    return filename;

            }
            catch
            {
                /* If there was a problem with the file, show an error  */
                this.messageBox("Could not Open data file: ", "Failed to open data file.");
                throw;
            }
            return filename;
        }

        /// <summary>
        /// Function opens a dialog box to browse and select the data file.
        /// </summary>
        /// <param name="browseTitle">The title of the file browser window.</param>
        /// <returns>The full filename of the data file.</returns>
        public string selectDataFile(string browseTitle)
        {

            /* Declare the filename to return. */
            string filename = null;

            /* Create a fileDialog browser to select the data file. */
            OpenFileDialog fileSelectBrowser = new OpenFileDialog();
            /* Set the browser properties. */
            fileSelectBrowser.Title = browseTitle;
            fileSelectBrowser.InitialDirectory =
                @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";
            fileSelectBrowser.Filter = //"Text Files|*.csv";
            "All EXCEL FILES (*.xlsx*)|*.xlsx*|All files (*.*)|*.*";
            fileSelectBrowser.FilterIndex = 2;
            fileSelectBrowser.RestoreDirectory = true;
            try
            {
                /* Open the browser and select a file. */
                if (fileSelectBrowser.ShowDialog() == DialogResult.OK)
                {
                    filename = fileSelectBrowser.FileName;
                }
                else
                    return filename;

            }
            catch
            {
                /* If there was a problem with the file, show an error  */
                this.messageBox("Could not Open data file: ", "Failed to open data file.");
                throw;
            }
            return filename;
        }
        
        /// <summary>
        /// A wrapper function to contain the try catch block for selecting a file using the browser.
        /// </summary>
        /// <returns>The full path of the file selected.</returns>
        public string browseFile()
        {
            string filename = null;
            try
            {
                // Open the browser and retrieve the file.
                filename = selectDataFile();
                if (filename == null)
                    return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return filename;
        }
        
        /// <summary>
        /// A wrapper function to contain the try catch block for selecting a file using the browser.
        /// </summary>
        /// <param name="browseTitle">The title of the browser window.</param>
        /// <returns>The full path of the file selected.</returns>
        public string browseFile(string browseTitle)
        {
            string filename = null;
            try
            {
                // Open the browser and retrieve the file.
                filename = selectDataFile(browseTitle);
                if (filename == null)
                    return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return filename;
        }

        /// <summary>
        /// Determine if a file is already open before trying to read the file.
        /// </summary>
        /// <param name="filename">Filename of the file to be opened</param>
        /// <returns>True if the file is already open.</returns>
        public void isFileOpen(string filename)
        {
            /* Can the file be opened and read. */
            try
            {
                string[] l = System.IO.File.ReadAllLines(filename);
            }
            catch (IOException e)
            {
                /* File is already opended and locked for reading. */
                messageBox(e.Message + ":\n\nClose the file and start again");
                Environment.Exit(0);
            }
            
        }
  
        
    } // Class tools
}

