using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICEData
{
    /// <summary>
    /// Enumerated direction of the train km's.
    /// </summary>
    public enum direction { increasing, decreasing, notSpecified };
    
    
    /// <summary>
    /// A class to hold the train details of each record.
    /// </summary>
    public class TrainDetails
    {
           
        public string TrainID;
        public string LocoID;
        public double powerToWeight;    // *************************************
        public DateTime NotificationDateTime;
        public double latitude;
        public double longitude;
        public double speed;
        public double kmPost;
        public double geometryKm;
        public direction trainDirection;


        /// <summary>
        /// Default trainDetails constructor.
        /// </summary>
        public TrainDetails()
        {
            this.TrainID = "none";
            this.LocoID = "none";
            this.powerToWeight = 1;
            this.NotificationDateTime = new DateTime(2000, 1, 1, 0, 0, 0);
            this.latitude = -33.8519;   //Sydney Harbour Bridge
            this.longitude = 151.2108;
            this.speed = 0;
            this.kmPost = 0;
            this.geometryKm = 0;
            this.trainDirection = direction.notSpecified;
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
        /// <param name="geometryKm">The calcualted distance from the km post of the first point.</param>
        /// <param name="trainDirection">The train bearing.</param>
        public TrainDetails(string TrainID, string locoID, DateTime NotificationDateTime, double latitude, double longitude, 
                            double speed, double kmPost, double geometryKm, direction trainDirection)
        {
            this.TrainID = TrainID;
            this.LocoID = locoID;
            this.powerToWeight = 1; // ************************************
            this.NotificationDateTime = NotificationDateTime;
            this.latitude = latitude;
            this.longitude = longitude;
            this.speed = speed;
            this.kmPost = kmPost;
            this.geometryKm = geometryKm;
            this.trainDirection = trainDirection;

        }
       
    }

    /// <summary>
    /// A Class to hold the train journey details for an individual train.
    /// </summary>
    public class Train
    {
        public List<TrainDetails> TrainJourney;
        public bool include;                        // Include the train in the data.

        /// <summary>
        /// Default constructor
        /// </summary>
        public Train()
        {
            this.TrainJourney = null;
            this.include = false;
        }

        /// <summary>
        /// Train Object constructor.
        /// </summary>
        /// <param name="trainDetails">The list of trainDetails objects containing the details of the train journey.</param>
        public Train(List<TrainDetails> trainDetails)
        {
            this.TrainJourney = trainDetails;
            this.include = true;
        }


        /// <summary>
        /// Train Object constructor.
        /// </summary>
        /// <param name="trainDetails">The list of trainDetails objects containing the details of the train journey.</param>
        /// <param name="include">A flag to determine if the train journey will be included in the data.</param>
        public Train(List<TrainDetails> trainDetails, bool include)
        {
            this.TrainJourney = trainDetails;
            this.include = include;
        }

        /// <summary>
        /// Train Object constructor converting an list of interpolatedTrain objects into a list of trainDetail objects.
        /// </summary>
        /// <param name="trainDetails">A list of interpolatedTrain objects.</param>
        /// <param name="trainDirection">The direction of kilometreage of the train.</param>
        public Train(List<InterpolatedTrain> trainDetails,  direction trainDirection)
        {

            List<TrainDetails> journey = new List<TrainDetails>();

            for (int journeyIdx = 0; journeyIdx < trainDetails.Count(); journeyIdx++)
            {
                /* Convert each interpolatedTrain object to a trainDetail object. */
                TrainDetails newitem = new TrainDetails(trainDetails[journeyIdx].TrainID, trainDetails[journeyIdx].LocoID, trainDetails[journeyIdx].NotificationDateTime, 0, 0, 
                                                        trainDetails[journeyIdx].speed, 0, trainDetails[journeyIdx].geometryKm, trainDirection);
                journey.Add(newitem);

            }
            this.TrainJourney = journey;
            this.include = true;
        }
 
    }

    /* A class to hold the interpolated train details. */
    public class InterpolatedTrain
    {
        public string TrainID;
        public string LocoID;
        public double powerToWeight;    // ********************************************
        public DateTime NotificationDateTime;
        public double speed;
        public double geometryKm;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InterpolatedTrain()
        { 
            this.TrainID = null;
            this.LocoID = null;
            this.powerToWeight = 1;
            this.NotificationDateTime = new DateTime(2000, 1, 1, 0, 0, 0);
            this.speed = 0;
            this.geometryKm = 0;
        }

        /// <summary>
        /// InterpolatedTrain object constructor.
        /// </summary>
        /// <param name="TrainID">The train ID.</param>
        /// <param name="locoID">The Loco ID</param>
        /// <param name="NotificationDateTime">The intiial notification time for the start of the data.</param>
        /// <param name="geometryKm">The calculated actual kilometerage of the train.</param>
        /// <param name="speed">The speed (kph) at each location.</param>
        public InterpolatedTrain(string TrainID, string locoID, DateTime NotificationDateTime, double geometryKm, double speed)
        {
            this.TrainID = TrainID;
            this.LocoID = locoID;
            this.powerToWeight = 1;
            this.NotificationDateTime = NotificationDateTime;
            this.geometryKm = geometryKm;
            this.speed = speed;
        }

        /// <summary>
        /// InterpolatedTrain object constructor to convert a trainDetails object into an interpolatedTrain object.
        /// </summary>
        /// <param name="details">The trainDetails object containing the train journey parameters.</param>
        public InterpolatedTrain(TrainDetails details)
        {
            this.TrainID = details.TrainID;
            this.LocoID = details.LocoID;
            this.powerToWeight = details.powerToWeight;
            this.NotificationDateTime = details.NotificationDateTime;
            this.geometryKm = details.geometryKm;
            this.speed = details.speed;
        }
    }

    /// <summary>
    /// A class describing a geographic location
    /// </summary>
    public class GeoLocation
    {
        /* Latitude and longitude of the location */
        public double latitude;
        public double longitude;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GeoLocation()
        {
            // Default: Sydney Harbour Bridge
            this.latitude = -33.8519;
            this.longitude = 151.2108;
        }

        /// <summary>
        /// Geolocation constructor
        /// </summary>
        /// <param name="lat">latitude of the location.</param>
        /// <param name="lon">longitude of the location.</param>
        public GeoLocation(double lat, double lon)
        {
            this.latitude = lat;
            this.longitude = lon;
        }

        /// <summary>
        /// Geolocation constructor
        /// </summary>
        /// <param name="trainDetails">The trainDetails object containing the latitude and longitude of the location.</param>
        public GeoLocation(TrainDetails trainDetails)
        {
            this.latitude = trainDetails.latitude;
            this.longitude = trainDetails.longitude;
        }


    }



    class ICEData
    {

        /* Create a tools Class. */
        public static Tools tool = new Tools();
        /* Create a processing Class. */
        public static processing processing = new processing();
        /* Create a trackGeometry Class. */
        public static trackGeometry track = new trackGeometry();


        [STAThread]
        static void Main(string[] args)
        {
            /* Artificial input parameters. */
            /* These parameters will be passed into the program. */
            DateTime[] dateRange = new DateTime[2] { new DateTime(2016, 1, 1), new DateTime(2016, 2, 1) };
            bool includeAListOfTrainsToExclude = false;
            double interval = 50;
            /* Corridor dependant parameters. */
            double startKm = 5;
            double endKm = 70;
            double minimumJourneyDistance = 40000;
            double[] latitude = new double[2] { -33.0, -35.0 };
            double[] longitude = new double[2] { 150.0, 152.0 };
            
            /* Ensure there is a empty list of trains to exclude to start. */
            List<string> excludeTrainList = new List<string> { };

            /* Use a browser to select the desired data file. */
            string filename = null;
            string geometryFile = null; 
            string trainList = null;

            /* Select the data file and the trainList file. */
            filename = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\raw data - sample.csv"; 
            //tool.browseFile("Select the data file.");
            geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Macarthur to Botany\Macarthur to Botany Geometry.csv"; 
            //tool.browseFile("Select the geometry file.");

            if (includeAListOfTrainsToExclude)
            {
                trainList = tool.browseFile("Select the train list file.");

                /* Populate the exluded train list. */
                if (trainList != null || !trainList.Equals(""))
                    excludeTrainList = readTrainList(trainList);
            }

            /* Read in the track gemoetry data. */
            List<trackGeometry> trackGeometry = new List<trackGeometry>();
            trackGeometry = track.readGeometryfile(geometryFile);

            /* Read the data. */
            List<TrainDetails> TrainRecords = new List<TrainDetails>();
            TrainRecords = readICEData(filename, latitude, longitude, dateRange, excludeTrainList);

            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainDetails> OrderdTrainRecords = new List<TrainDetails>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.TrainID).ThenBy(t => t.LocoID).ThenBy(t => t.NotificationDateTime).ThenBy(t => t.kmPost).ToList();

            /* Clean data - remove trains with insufficient data. */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> CleanTrainRecords = new List<Train>();
            CleanTrainRecords = CleanData(trackGeometry, OrderdTrainRecords, minimumJourneyDistance);

            /* interpolate data */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> interpolatedRecords = new List<Train>();
            interpolatedRecords = processing.interpolateTrainData(CleanTrainRecords, startKm, endKm, interval);
            List<InterpolatedTrain> unpackedInterpolation = new List<InterpolatedTrain>();
            unpackedInterpolation = unpackInterpolatedData(interpolatedRecords);
            writeTrainData(unpackedInterpolation);

            /* Average the train data for each direction with regard for TSR's and loop locations. */
            
            /* seprate averages for P/W ratio groups, commodity, Operator */

            /* Unpack the records into a single trainDetails object list. */
            List<TrainDetails> unpackedData = new List<TrainDetails>();
            unpackedData = unpackCleanData(CleanTrainRecords);

            /* Write data to an excel file. */
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
        public static List<TrainDetails> readICEData(string filename, double[] latitudeRange, double[] longitudeRange, DateTime[] dateRange, List<string> excludeTrainList)
        {
            /* Read all the lines of the data file. */
            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\t' };

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Initialise the fields of interest. */
            string TrainID = "none";
            string locoID = "none";
            double speed = 0.0;
            double kmPost = 0.0;
            double geometryKm = 0.0;
            double latitude = 0.0;
            double longitude = 0.0;
            DateTime NotificationDateTime = new DateTime(2000, 1, 1);

            bool header = true;
            bool includeTrain = true;

            /* List of all valid train data. */
            List<TrainDetails> IceRecord = new List<TrainDetails>();

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
                    DateTime.TryParse(fields[14], out NotificationDateTime);
                    /* possible TSR information as well*/

                    /* Check if the train is in the exclude list */
                    includeTrain = excludeTrainList.Contains(TrainID);

                    if (latitude < latitudeRange[0] && latitude > latitudeRange[1] &&
                        longitude > longitudeRange[0] && longitude < longitudeRange[1] &&
                        NotificationDateTime >= dateRange[0] && NotificationDateTime < dateRange[1] &&
                        !includeTrain)
                    {
                        TrainDetails record = new TrainDetails(TrainID, locoID, NotificationDateTime, latitude, longitude, speed, kmPost, geometryKm, direction.notSpecified);
                        IceRecord.Add(record);
                    }

                }
            }

            /* Return the list of records. */
            return IceRecord;
        }

        /// <summary>
        /// This function reads the file with the list of trains to exclude from the 
        /// data and stores the list in a managable list object.
        /// The file is assumed to have one train per line or have each train seperated 
        /// by a common delimiter [ , \ " \t \n]
        /// </summary>
        /// <param name="filename">The full path of the file containing the list of trains to exclude.</param>
        /// <returns>The populated list of all trains to exclude.</returns>
        public static List<string> readTrainList(string filename)
        {
            List<string> excludeTrainList = new List<string>();

            /* Read all the lines of the file. */
            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\'', '"', '\t', '\n' };     // not sure of the delimter ??

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            /* Add the trains to the list. */
            foreach (string line in lines)
                excludeTrainList.Add(line);


            return excludeTrainList;
        }

        /// <summary>
        /// Write the train records to an excel file for inspection.
        /// </summary>
        /// <param name="trainRecords">The list of trainDetails object containing all the train records.</param>
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
            string[] headerString = { "Train ID", "loco ID", " Notification Date Time", "Latitude", "Longitude", "Speed", "km Post", "Actual Km", "Train Direction" };

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
            string[,] TrainID = new string[excelPageSize + 10, 1];
            string[,] LocoID = new string[excelPageSize + 10, 1];
            DateTime[,] NotificationTime = new DateTime[excelPageSize + 10, 1];
            double[,] latitude = new double[excelPageSize + 10, 1];
            double[,] longitude = new double[excelPageSize + 10, 1];
            double[,] speed = new double[excelPageSize, 1];
            double[,] kmPost = new double[excelPageSize, 1];
            double[,] geometryKm = new double[excelPageSize, 1];
            string[,] trainDirection = new string[excelPageSize, 1];

            
            /* Loop through the excel pages. */
            for (int excelPage = 0; excelPage < excelPages; excelPage++)
            {
                /* Set the active worksheet. */
                worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.Sheets[excelPage + 1];
                workbook.Sheets[excelPage + 1].Activate();
                worksheet.get_Range("A1", "I1").Value2 = headerString;

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
                        geometryKm[j, 0] = trainRecords[checkIdx].geometryKm;
                        trainDirection[j, 0] = trainRecords[checkIdx].trainDirection.ToString();
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
                        geometryKm[j, 0] = 0.0;
                        trainDirection[j, 0] = direction.notSpecified.ToString();
                    }
                }

                /* Write the data to the active excel workseet. */
                worksheet.get_Range("A" + headerOffset, "A" + (headerOffset + excelPageSize - 1)).Value2 = TrainID;
                worksheet.get_Range("B" + headerOffset, "B" + (headerOffset + excelPageSize - 1)).Value2 = LocoID;
                worksheet.get_Range("C" + headerOffset, "C" + (headerOffset + excelPageSize - 1)).Value2 = NotificationTime;
                worksheet.get_Range("D" + headerOffset, "D" + (headerOffset + excelPageSize - 1)).Value2 = latitude;
                worksheet.get_Range("E" + headerOffset, "E" + (headerOffset + excelPageSize - 1)).Value2 = longitude;
                worksheet.get_Range("F" + headerOffset, "F" + (headerOffset + excelPageSize - 1)).Value2 = speed;
                worksheet.get_Range("G" + headerOffset, "G" + (headerOffset + excelPageSize - 1)).Value2 = kmPost;
                worksheet.get_Range("H" + headerOffset, "H" + (headerOffset + excelPageSize - 1)).Value2 = geometryKm;
                worksheet.get_Range("I" + headerOffset, "I" + (headerOffset + excelPageSize - 1)).Value2 = trainDirection;

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
        /// Write the train records to an excel file for inspection.
        /// </summary>
        /// <param name="trainRecords">The list of interpolatedTrain object containing all the train records.</param>
        public static void writeTrainData(List<InterpolatedTrain> trainRecords)
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
            string[] headerString = { "Train ID", "loco ID", " Notification Date Time", "Actual Km",  "Speed" };

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
            string[,] TrainID = new string[excelPageSize + 10, 1];
            string[,] LocoID = new string[excelPageSize + 10, 1];
            DateTime[,] NotificationTime = new DateTime[excelPageSize + 10, 1];
            double[,] speed = new double[excelPageSize, 1];
            double[,] geometryKm = new double[excelPageSize, 1];
           

            /* Loop through the excel pages. */
            for (int excelPage = 0; excelPage < excelPages; excelPage++)
            {
                /* Set the active worksheet. */
                worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.Sheets[excelPage + 1];
                workbook.Sheets[excelPage + 1].Activate();
                worksheet.get_Range("A1", "E1").Value2 = headerString;

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
                        speed[j, 0] = trainRecords[checkIdx].speed;
                        geometryKm[j, 0] = trainRecords[checkIdx].geometryKm;
                        
                    }
                    else
                    {
                        /* The end of the data has been reached. Populate the remaining elements. */
                        TrainID[j, 0] = "";
                        LocoID[j, 0] = "";
                        NotificationTime[j, 0] = DateTime.MinValue;
                        speed[j, 0] = 0.0;
                        geometryKm[j, 0] = 0.0;
                        
                    }
                }

                /* Write the data to the active excel workseet. */
                worksheet.get_Range("A" + headerOffset, "A" + (headerOffset + excelPageSize - 1)).Value2 = TrainID;
                worksheet.get_Range("B" + headerOffset, "B" + (headerOffset + excelPageSize - 1)).Value2 = LocoID;
                worksheet.get_Range("C" + headerOffset, "C" + (headerOffset + excelPageSize - 1)).Value2 = NotificationTime;
                worksheet.get_Range("D" + headerOffset, "D" + (headerOffset + excelPageSize - 1)).Value2 = geometryKm;
                worksheet.get_Range("E" + headerOffset, "E" + (headerOffset + excelPageSize - 1)).Value2 = speed;
                
            }

            /* Generate the resulting file name and location to save to. */
            string savePath = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis";
            string saveFilename = savePath + @"\ICEData_Interpolated" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

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
        /// <param name="trackGeometry">A lit of track Geometry objects</param>
        /// <param name="OrderdTrainRecords">List of TrainDetail objects</param>
        /// <param name="minimumJourneyDistance">The minimum required distance a train must travel 
        /// to be included in the analysis.</param>
        /// <returns>List of Train objects containign the journey details of each train.</returns>
        public static List<Train> CleanData(List<trackGeometry> trackGeometry, List<TrainDetails> OrderdTrainRecords, double minimumJourneyDistance)
        {
            bool removeTrain = false;
            double distanceThreshold = 4000; // metres
            double distance = 0;
            double journeyDistance = 0;
            double timeThreshold = 10 * 60; // 10 hours between trains with the same Train ID and Loco ID

            GeoLocation point1 = null;
            GeoLocation point2 = null;

            /* Place holder for the train records that are acceptable. */
            List<TrainDetails> newTrainList = new List<TrainDetails>();
            /* List of each Train with its journey details that is acceptable. */
            List<Train> cleanTrainList = new List<Train>();

            /* Add the first record to the list. */
            newTrainList.Add(OrderdTrainRecords[0]);
            GeoLocation trainPoint  = new GeoLocation(OrderdTrainRecords[0]);
            /* Populate the first actual kilometreage point. */
            newTrainList[0].geometryKm = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);


            for (int trainIndex = 1; trainIndex < OrderdTrainRecords.Count(); trainIndex++)
            {
                
                if (OrderdTrainRecords[trainIndex].TrainID.Equals(OrderdTrainRecords[trainIndex - 1].TrainID) &&
                    OrderdTrainRecords[trainIndex].LocoID.Equals(OrderdTrainRecords[trainIndex - 1].LocoID) &&
                    (OrderdTrainRecords[trainIndex].NotificationDateTime - OrderdTrainRecords[trainIndex - 1].NotificationDateTime).TotalMinutes < timeThreshold)
                {
                    /* If the current and previous record represent the same train journey, add it to the list. */
                    newTrainList.Add(OrderdTrainRecords[trainIndex]);

                    point1 = new GeoLocation(OrderdTrainRecords[trainIndex - 1]);
                    point2 = new GeoLocation(OrderdTrainRecords[trainIndex]);

                    distance = processing.calculateDistance(point1, point2);
                    journeyDistance = journeyDistance + distance;
                    
                    if (distance > distanceThreshold)
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
                    if (!removeTrain && journeyDistance > minimumJourneyDistance)
                    {
                        /* If all points are acceptable and the train ravels the minimum distance, 
                         * add the train journey to the cleaned list. 
                         */
                        Train item = new Train();
                        item.TrainJourney = newTrainList.ToList();

                        /* Determine direction and actual km. */
                        tool.populateDirection(item);
                        tool.populateGeometryKm(item);

                        cleanTrainList.Add(item);

                    }

                    /* Reset the parameters for the next train. */
                    removeTrain = false;
                    journeyDistance = 0;
                    newTrainList.Clear();
                    
                    /* Add the first record of the new train journey. */
                    newTrainList.Add(OrderdTrainRecords[trainIndex]);
                    trainPoint = new GeoLocation(OrderdTrainRecords[trainIndex]);
                    newTrainList[0].geometryKm = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
                }

                /* The end of the records have been reached. */
                if (trainIndex == OrderdTrainRecords.Count() - 1 && !removeTrain)
                {
                    /* If all points are aceptable, add the train journey to the cleaned list. */
                    Train item = new Train();
                    item.TrainJourney = newTrainList.ToList();
                    tool.populateDirection(item);
                    tool.populateGeometryKm(item);

                    cleanTrainList.Add(item);

                }

            }

            return cleanTrainList;

        }

        /// <summary>
        /// Unpack the Train data structure into a single list of TrainDetails objects.
        /// </summary>
        /// <param name="OrderdTrainRecords">The Train object containing a list of trains with their journey details.</param>
        /// <returns>A single list of TrainDetail objects.</returns>
        public static List<TrainDetails> unpackCleanData(List<Train> OrderdTrainRecords)
        {
            /* Place holder to store all train records in one list. */
            List<TrainDetails> unpackedData = new List<TrainDetails>();

            /* Cycle through each train. */
            foreach (Train train in OrderdTrainRecords)
            {
                /* Cycle through each record in the train journey. */
                for (int journeyIdx = 0; journeyIdx < train.TrainJourney.Count(); journeyIdx++)
                {
                    /* Add it to the list. */
                    unpackedData.Add(train.TrainJourney[journeyIdx]);
                }
            }
            return unpackedData;
        }

        /// <summary>
        /// Unpack the Train data structure into a single list of interpolatedTrain objects.
        /// </summary>
        /// <param name="OrderdTrainRecords">The Train object containing a list of trains with their journey details.</param>
        /// <returns>A single list of interpolatedTrain objects.</returns>
        public static List<InterpolatedTrain> unpackInterpolatedData(List<Train> OrderdTrainRecords)
        {
            /* Place holder to store all train records in one list. */
            List<InterpolatedTrain> unpackedData = new List<InterpolatedTrain>();

            /* Cycle through each train. */
            foreach (Train train in OrderdTrainRecords)
            {
                /* Cycle through each record in the train journey. */
                for (int journeyIdx = 0; journeyIdx < train.TrainJourney.Count(); journeyIdx++)
                {
                    /* Add it to the list. */
                    unpackedData.Add(new InterpolatedTrain(train.TrainJourney[journeyIdx]));
                }
            }
            return unpackedData;
        }
    

    } // Class ICEData
    
} // namespace

