using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICEData
{
    /// <summary>
    /// A class to hold the train details ofeach record.
    /// </summary>
    public class TrainDetails
    {

        public string TrainID;
        public string LocoID;
        public DateTime NotificationDateTime;
        public double latitude;
        public double longitude;
        public double speed;
        public double kmPost;
        public double trainDirection;


        /// <summary>
        /// Default trainDetails constructor.
        /// </summary>
        public TrainDetails()
        {
            this.TrainID = "none";
            this.LocoID = "none";
            this.NotificationDateTime = new DateTime(2000,1,1,0,0,0);
            this.latitude = -33.8519;   //Sydney Harbour Bridge
            this.longitude = 151.2108;
            this.speed = 0;
            this.kmPost = 0;
            this.trainDirection = 0;
        }

        /// <summary>
        /// TrainDetails constructor
        /// </summary>
        /// <param name="TrainID">The Train ID.</param>
        /// <param name="locoID">The Locomotive ID.</param>
        /// <param name="NotificationDateTime">The date and time of the record.</param>
        /// <param name="latitude">The latitude of the train.</param>
        /// <param name="longitude">The longitude of the train.</param>
        /// <param name="speed">The instantaneous speed of the train.</param>
        /// <param name="kmPost">The closest km marker to the train at the time of recording</param>
        /// <param name="trainDirection">The train bearing.</param>
        public TrainDetails(string TrainID, string locoID, DateTime NotificationDateTime, double latitude, double longitude, double speed, double kmPost, double trainDirection)
        {
            this.TrainID = TrainID;
            this.LocoID = locoID;
            this.NotificationDateTime = NotificationDateTime;
            this.latitude = latitude;
            this.longitude = longitude;
            this.speed = speed;
            this.kmPost = kmPost;
            this.trainDirection = trainDirection;

        }




    }



    class ICEData
    {

        /* Create a tools Object. */
        public static Tools tool = new Tools();

        [STAThread]
        static void Main(string[] args)
        {


            /* Use a browser to select the desired data file. */
            string filename = null;
            try
            {
                filename = tool.selectDataFile();
                if (filename == null)
                    return;
            }
            catch
            {
                return;
            }

            
            // Read the data
            readICEData(filename);

            // Write data to file


        }

        /// <summary>
        /// Read the ICE data file.
        /// The file is assumed to be in a specific format.
        /// 
        /// 1       Alarms
        /// 2       Exclude
        /// 3       TrainID - Exclude
        /// 4       Train Shortlist
        /// 5       TrainCount
        /// 6       Direction
        /// 7       Extract Date Time
        /// 8       Faults
        /// 9       Insert Date Time
        /// 10      Journey ID	
        /// 11      KM Post	
        /// 12      Latitude	
        /// 13      Loco ID	
        /// 14      Longitude	
        /// 15      Notification Date Time	
        /// 16      Notification Date	
        /// 17      Notification Time	
        /// 18      Notification Type	
        /// 19      Number of Records	
        /// 20      Source System	
        /// 21      Speed	
        /// 22      Track Number	
        /// 23      Train ID	
        /// 24      Update Date Time
        /// 25      Version ID
        /// </summary>
        /// <param name="filename">The filename of the ICE data</param>
        /// <returns>The list of trainDetails objects containnig each valid record.</returns>
        public static List<TrainDetails> readICEData(string filename)
        {
            /* Read all the lines of the data file. */
            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the field of interest. */
            string TrainID = "none";    
            string locoID = "none";     
            double speed = 0.0;     
            double kmPost = 0.0;    
            double latitude = 0.0;  
            double longitude = 0.0; 
            double trainDirection = 0.0;    
            DateTime NotificationDateTime = new DateTime(2000, 1, 1);   
            
            bool header = true;

            List<TrainDetails> IceRecord = new List<TrainDetails>();            // List of all valid train data.
            //List<TrainDetails> TrainRecords = new List<TrainDetails>();         // list of data for individual trains (only 1 Train Id and loco ID)
            //List<List<TrainDetails>> train = new List<List<TrainDetails>>();    // list of individual trains
            
            foreach (string line in lines)
            {
                if (header)
                    /* Ignore the field headers. */
                    header = false;
                else
                {
                    /* Seperate each record into each field */
                    fields = line.Split(delimeters);

                    TrainID = fields[22];
                    locoID = fields[12];
                    double.TryParse(fields[20], out speed);
                    double.TryParse(fields[10], out kmPost);
                    double.TryParse(fields[11], out latitude);
                    double.TryParse(fields[13], out longitude);
                    double.TryParse(fields[5], out trainDirection);
                    DateTime.TryParseExact(fields[14], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.AssumeLocal, out NotificationDateTime);

                    // if record within range limits include in recordlist
                    TrainDetails record = new TrainDetails(TrainID, locoID, NotificationDateTime, latitude, longitude, speed, kmPost, trainDirection);
                    IceRecord.Add(record);

                
                
                }
            }

            // Return the list of records.
            return IceRecord;
        }



    }
}
