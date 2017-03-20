using System;
using System.IO;
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

    public class Train
    {
        public List<TrainDetails> TrainJourney;
        public bool include;

        public Train()
        {
            this.TrainJourney = null;
            this.include = false;
        }

        public Train(List<TrainDetails> trainDetails)
        {
            this.TrainJourney = trainDetails;
            this.include = true;
        }
        
        public Train(List<TrainDetails> trainDetails, bool include)
        {
            this.TrainJourney = trainDetails;
            this.include = include;
        }
    }

    class ICEData
    {

        /* Create a tools Object. */
        public static Tools tool = new Tools();

        [STAThread]
        static void Main(string[] args)
        {
            /* Artificial input parameters. */
            /* These parameters will be passed into the program. */
            double[] latitude = new double[2] { -33.0, -35.0 };
            double[] longitude = new double[2] { 150.0, 152.0};
            DateTime[] dateRange = new DateTime[2] { new DateTime(2016, 1, 1), new DateTime(2016, 2, 1) };
            //string[] trainList = new string[] {"1,","1PS6" };
            //bool excludeTrainList = true;

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
            List<TrainDetails> TrainRecords = new List<TrainDetails>();
            TrainRecords = readICEData(filename, latitude, longitude, dateRange);

            // Sort the date by [trainID, locoID, Date, Time, kmPost]
            List<TrainDetails> OrderdTrainRecords = new List<TrainDetails>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.TrainID).ThenBy(t => t.LocoID).ThenBy(t => t.NotificationDateTime).ThenBy(t => t.kmPost).ToList();


            // Clean data - remove trains with insufficient data
            List<Train> CleanTrainRecords = new List<Train>();
            CleanTrainRecords = CleanData(OrderdTrainRecords);

            List<TrainDetails> unpackedData = new List<TrainDetails>();
            unpackedData = unpackCleanData(CleanTrainRecords);

            // Write data to an excel file
            writeTrainData(unpackedData);

            tool.messageBox("Program Complete.");
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
        public static List<TrainDetails> readICEData(string filename, double[] latitudeRange, double[] longitudeRange, DateTime[] dateRange)
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
                    /* Ignore the header line. */
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
                    DateTime.TryParse(fields[14], out NotificationDateTime);

                    if (latitude < latitudeRange[0] & latitude > latitudeRange[1] &
                        longitude > longitudeRange[0] & longitude < longitudeRange[1] &
                        NotificationDateTime >= dateRange[0] & NotificationDateTime < dateRange[1])
                    {
                        TrainDetails record = new TrainDetails(TrainID, locoID, NotificationDateTime, latitude, longitude, speed, kmPost, trainDirection);
                        IceRecord.Add(record);
                    }

                }
            }

            // Return the list of records.
            return IceRecord;
        }


        /// <summary>
        /// Write the train records to an excel file.
        /// </summary>
        /// <param name="trainRecords">The list of train details object containing all the train records.</param>
        public static void writeTrainData(List<TrainDetails> trainRecords)
        {
            
            /* Create the microsfot excel references. */
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel._Workbook workbook;
            Microsoft.Office.Interop.Excel._Worksheet worksheet;

            /* Start Excel and get Application object. */
            excel = new Microsoft.Office.Interop.Excel.Application();

            /* Get the reference to the new workbook. */
            workbook = (Microsoft.Office.Interop.Excel._Workbook)(excel.Workbooks.Add(""));

            /* Create the header details. */
            string[] headerString = { "Train ID", "loco ID", " Notification Date Time", "Latitude", "Longitude", "Speed", "km Post", "Train Direction" };

            /* Pagenate the data for writing to excel. */
            int excelPageSize = 1000000;        /* Page size of the excel worksheet. */
            int excelPages = 1;                 /* Number of Excel pages to write. */
            int headerOffset = 2;

            /* Adjust the excel page size or the number of pages to write. */
            if (trainRecords.Count() < excelPageSize)
                excelPageSize = trainRecords.Count();
            else
                excelPages = (int)Math.Round((double)trainRecords.Count() / excelPageSize + 0.5);
            

            /* Deconstruct the train details into excel columns. */
            string[,] TrainID = new string[excelPageSize+10, 1];
            string[,] LocoID = new string[excelPageSize+10, 1];
            DateTime[,] NotificationTime = new DateTime[excelPageSize+10, 1];
            double[,] latitude = new double[excelPageSize+10, 1];
            double[,] longitude = new double[excelPageSize+10, 1];
            double[,] speed = new double[excelPageSize, 1];
            double[,] kmPost = new double[excelPageSize, 1];
            double[,] direction = new double[excelPageSize, 1];

            int a;
            /* Loop through the excel pages. */
            for (int excelPage = 0; excelPage < excelPages; excelPage++)
            {
                /* Set the active worksheet. */
                worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.Sheets[excelPage + 1];
                workbook.Sheets[excelPage + 1].Activate();
                worksheet.get_Range("A1", "H1").Value2 = headerString;

                /* Loop through the data for each excel page. */
                for (int j = 0; j < excelPageSize; j++)
                {
                    
                    /* Check we dont try to read more data than there really is. */
                    int checkIdx = j + excelPage * excelPageSize;
                    if (checkIdx < trainRecords.Count())
                    {
                        TrainID[j, 0] = trainRecords[checkIdx].TrainID;
                        LocoID[j, 0] = trainRecords[checkIdx].LocoID;
                        NotificationTime[j, 0] = trainRecords[checkIdx].NotificationDateTime;
                        latitude[j, 0] = trainRecords[checkIdx].latitude;
                        longitude[j, 0] = trainRecords[checkIdx].longitude;
                        speed[j, 0] = trainRecords[checkIdx].speed;
                        kmPost[j, 0] = trainRecords[checkIdx].kmPost;
                        direction[j, 0] = trainRecords[checkIdx].trainDirection;
                    }
                    else
                    {
                        /* The end of the data has been reached. Populate the remaining elements. */
                        TrainID[j, 0] = "";
                        LocoID[j, 0] = "";
                        NotificationTime[j, 0] = DateTime.MinValue;
                        latitude[j, 0] = 0.0;
                        longitude[j, 0] = 0.0;
                        speed[j, 0] = 0.0;
                        kmPost[j, 0] = 0;
                        direction[j, 0] = 0.0;
                    }
                }

                /* Write the data to the active excel workseet. */
                worksheet.get_Range("A" + headerOffset, "A" + (headerOffset + excelPageSize-1)).Value2 = TrainID;
                worksheet.get_Range("B" + headerOffset, "B" + (headerOffset + excelPageSize-1)).Value2 = LocoID;
                worksheet.get_Range("C" + headerOffset, "C" + (headerOffset + excelPageSize-1)).Value2 = NotificationTime;
                worksheet.get_Range("D" + headerOffset, "D" + (headerOffset + excelPageSize-1)).Value2 = latitude;
                worksheet.get_Range("E" + headerOffset, "E" + (headerOffset + excelPageSize-1)).Value2 = longitude;
                worksheet.get_Range("F" + headerOffset, "F" + (headerOffset + excelPageSize-1)).Value2 = speed;
                worksheet.get_Range("G" + headerOffset, "G" + (headerOffset + excelPageSize-1)).Value2 = kmPost;
                worksheet.get_Range("H" + headerOffset, "H" + (headerOffset + excelPageSize-1)).Value2 = direction;

            }

            /* Generate the resulting file name and location to save to. */
            string savePath = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";
            string saveFilename = savePath + @"\ICEData_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            /* Check the file does not exist yet. */
            if (File.Exists(saveFilename))
                File.Delete(saveFilename);

            /* Save the excel file. */
            excel.UserControl = false;
            workbook.SaveAs(saveFilename, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            workbook.Close();

            return;
        }

        /// <summary>
        /// Remove the whole train journey that does not contain successive points that conform to 
        /// the minimum distance threshold.
        /// </summary>
        /// <param name="OrderdTrainRecords">List of TrainDetail objects</param>
        /// <returns>List of Train objects containign the journey details of each train.</returns>
        public static List<Train> CleanData(List<TrainDetails> OrderdTrainRecords)
        {
            bool removeTrain = false;
            double distanceThreshold = 4000; // metres

            /* Place holder for the train records that are acceptable. */
            List<TrainDetails> newTrainList = new List<TrainDetails>();
            /* List of each Train with its journey details that is acceptable. */            
            List<Train> cleanTrainList = new List<Train>();

            /* Add the first record to the list. */
            newTrainList.Add(OrderdTrainRecords[0]);

            for (int trainIndex = 1; trainIndex < OrderdTrainRecords.Count(); trainIndex++ )
            {
                if (OrderdTrainRecords[trainIndex].TrainID.Equals(OrderdTrainRecords[trainIndex - 1].TrainID) &
                    OrderdTrainRecords[trainIndex].LocoID.Equals(OrderdTrainRecords[trainIndex - 1].LocoID)  &
                    (OrderdTrainRecords[trainIndex].NotificationDateTime - OrderdTrainRecords[trainIndex - 1].NotificationDateTime).TotalMinutes < 1440 )
                {
                    /* If the current and previous record represent the same train journey, add it to the list */
                    newTrainList.Add(OrderdTrainRecords[trainIndex]);
                    
                    if (Math.Abs(OrderdTrainRecords[trainIndex].kmPost - OrderdTrainRecords[trainIndex-1].kmPost) > distanceThreshold)
                    {
                        /* If the distance between successive km points is greater than the
                         * threshold then we want to remove this train from the data. 
                         */
                        removeTrain = true;
                    }

                }
                else 
                {
                    /* The end of the train journey had been reached. */
                    if (!removeTrain)
                    {
                        /* If all points are aceptable, add the train journey to the cleaned list. */
                        Train item = new Train();
                        item.TrainJourney = newTrainList.ToList();

                        cleanTrainList.Add(item);
                        
                    }
                    
                    /* Reset the parameters for teh next train. */
                    removeTrain = false;
                    newTrainList.Clear();
                    /* Add the first record of the new train journey. */
                    newTrainList.Add(OrderdTrainRecords[trainIndex]);
                }

                /* The end of the records have been reached. */
                if (trainIndex == OrderdTrainRecords.Count() -1 & !removeTrain)
                {
                    /* If all points are aceptable, add the train journey to the cleaned list. */
                    Train item = new Train();
                    item.TrainJourney = newTrainList.ToList();

                    cleanTrainList.Add(item);
                    
                }
                
            }
            
            return cleanTrainList;
            
        }

        /// <summary>
        /// Unpack the Train data structure into a single list of TrainDetails objects.
        /// </summary>
        /// <param name="OrderdTrainRecords">The Train object containing a list of trains with there journey details.</param>
        /// <returns>A single list of TrainDetail objects.</returns>
        public static List<TrainDetails> unpackCleanData(List<Train> OrderdTrainRecords)
        {
            /* Place holder to store all train records in one list. */
            List<TrainDetails> unpackedData = new List<TrainDetails>();
            
            /* Cycle through each train. */
            foreach (Train train in OrderdTrainRecords)
            {
                /* Cycle through each record in the train journey. */
                for (int i = 0; i < train.TrainJourney.Count(); i++)
                {                    
                    /* Add it to the list. */
                    unpackedData.Add(train.TrainJourney[i]);
                }
            }
            return unpackedData;
        }

    }
}
