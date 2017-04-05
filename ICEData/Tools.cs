using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICEData
{
    class Tools
    {
        // Mean radius of the Earth
        private const double EarthRadius = 6371000.0;   // metres

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
        /// Convert degrees in to radians
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        private double degress2radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        
        /// <summary>
        /// Calculates the distance between two points using the haversine formula.
        /// </summary>
        /// <param name="latitude1">Latitude of location 1.</param>
        /// <param name="longitude1">Longitude of location 1.</param>
        /// <param name="latitude2">Latitude of location 2.</param>
        /// <param name="longitude2">Longitude of location 2.</param>
        /// <returns>The Distance between the two points in metres.</returns>
        public double calculateDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {

            double arcsine = Math.Sin(degress2radians(latitude2 - latitude1)) * Math.Sin(degress2radians(latitude2 - latitude1)) +
                Math.Cos(degress2radians(latitude1)) * Math.Cos(degress2radians(latitude2)) *
                Math.Sin(degress2radians(longitude2 - longitude1))*Math.Sin(degress2radians(longitude2-longitude1));
            double arclength = 2*Math.Atan2(Math.Sqrt(arcsine),Math.Sqrt(1-arcsine));
            
            return EarthRadius * arclength;
            
        }

        /// <summary>
        /// Calculates the great circle distance between two points on a sphere 
        /// given there latitudes and longitudes.
        /// </summary>
        /// <param name="point1">Geographic location containinig the latitude and longitude of the reference location.</param>
        /// <param name="point2">Geographic location containinig the latitude and longitude of the destination location.</param>
        /// <returns>The Distance between the two points in metres.</returns>
        public double calculateDistance(GeoLocation point1, GeoLocation point2)
        {

            double arcsine = Math.Sin(degress2radians((point2.latitude - point1.latitude)/2)) * Math.Sin(degress2radians((point2.latitude - point1.latitude)/2)) +
                Math.Cos(degress2radians(point1.latitude)) * Math.Cos(degress2radians(point2.latitude)) *
                Math.Sin(degress2radians((point2.longitude - point1.longitude)/2)) * Math.Sin(degress2radians((point2.longitude - point1.longitude)/2));
            double arclength = 2 * Math.Atan2(Math.Sqrt(arcsine), Math.Sqrt(1 - arcsine));

            return EarthRadius * arclength;
            
        }

        /// <summary>
        /// Function determines the direction of the train using the first and last km posts.
        /// </summary>
        /// <param name="train">A train object containing kmPost information</param>
        /// <returns>Enumerated direction of the train km's.</returns>
        private direction determineTrainDirection(Train train)
        {
            /* Determine the distance and sign from the first point to the last point */
            double journeyDistance = train.TrainJourney[train.TrainJourney.Count - 1].kmPost - train.TrainJourney[0].kmPost;
            
            if (journeyDistance > 0)
                return direction.increasing;
            else
                return direction.decreasing;

        }
        
        /// <summary>
        /// Function populates the direction parameter for the train.
        /// </summary>
        /// <param name="train">The train object</param>
        public void populateDirection(Train train)
        {
            /* Determine the direction of the train */
            direction direction = determineTrainDirection(train);

            /* Populate the direction parameter. */
            foreach (TrainDetails trainPoint in train.TrainJourney)
            {
                trainPoint.trainDirection = direction;
            }

        }

        /// <summary>
        /// Function populates the geometry km information based on the calculated distance from the first km post.
        /// </summary>
        /// <param name="train">A train object.</param>
        public void populateGeometryKm(Train train)
        {
            /* Determine the direction of the km's the train is travelling. */
            direction direction = determineTrainDirection(train);
            double point2PointDistance = 0;

            /* Thie first km point is populated by the parent function ICEData.CleanData(). */
            
            for (int i = 1; i < train.TrainJourney.Count(); i++)
            {
                /* Calculate the distance between successive points. */
                GeoLocation point1 = new GeoLocation(train.TrainJourney[i - 1]);
                GeoLocation point2 = new GeoLocation(train.TrainJourney[i]);
                point2PointDistance = calculateDistance(point1, point2);

                /* Determine the cumulative actual geometry km based on the direction. */
                if (direction.Equals(direction.increasing))
                    train.TrainJourney[i].geometryKm = train.TrainJourney[i - 1].geometryKm + point2PointDistance/1000;
                
                else if (direction.Equals(direction.decreasing))
                    train.TrainJourney[i].geometryKm = train.TrainJourney[i - 1].geometryKm - point2PointDistance/1000;
            
                else
                    train.TrainJourney[i].geometryKm = train.TrainJourney[i].kmPost;
            }
                       
        }


        public List<Train> interpolateTrainData(List<Train> trains)
        {

            /* Consider making this an input parameter */
            double interval = 50.0;     // metres
            double startKm = 5;
            double endKm = 70;
            double previousKm = 0;
            double currentKm = 0;
            double currentTrainKm = 0;

            DateTime time = new DateTime();
            direction trainDirection = direction.notSpecified;
            bool timeChange = false;

            //double gradient = 0;
            //double intercept = 0;
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;

            /* New train list */
            List<Train> newTrainList = new List<Train>();
            List<TrainDetails> journey = new List<TrainDetails>();

            for (int trainidx = 0; trainidx < trains.Count(); trainidx++)
            {
                List<InterpolatedTrain> interpolatedTrainList = new List<InterpolatedTrain>();
            
                journey = trains[trainidx].TrainJourney;
                
                int journeyIdx = 0;
                //timeChange = false;

                if (journey[journeyIdx].trainDirection == direction.increasing)
                {
                    currentKm = startKm;
                    currentTrainKm = journey[journeyIdx].geometryKm;

                    while (currentKm < endKm)
                    {

                        if (journeyIdx > 0 && journeyIdx < journey.Count())   // maybe count-1
                        {
                            X0 = journey[journeyIdx - 1].geometryKm;
                            X1 = journey[journeyIdx].geometryKm;
                            Y0 = journey[journeyIdx - 1].speed;
                            Y1 = journey[journeyIdx].speed;
                            if (timeChange)
                            {
                                time = journey[journeyIdx - 1].NotificationDateTime;
                                timeChange = false;
                            }
                        }
                        else
                        {
                            X0 = currentKm;
                            X1 = X0;
                            Y0 = 0.0;
                            Y1 = Y0;
                            time = new DateTime();
                        }

                        if (currentKm <= X0) // <=
                            interpolatedSpeed = 0;
                        else// if (currentKm > Y0)
                            interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                        //else
                        //   interpolatedSpeed = 0; // linear(currentKm, X0, X1, Y0, Y1);

                        previousKm = currentKm;
                        currentKm = currentKm + interval / 1000;

                        //calculateTimeInterval(previousKm, currentKm, interpolatedSpeed);
                        if (interpolatedSpeed > 0)
                            time = time.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                        InterpolatedTrain item = new InterpolatedTrain(trains[trainidx].TrainJourney[0].TrainID, trains[trainidx].TrainJourney[0].LocoID, 
                                                                        time, previousKm, interpolatedSpeed);
                        interpolatedTrainList.Add(item);


                        if (journeyIdx < journey.Count)
                            if (currentKm >= journey[journeyIdx].geometryKm)
                            {
                                journeyIdx++;
                                timeChange = true;
                            }

                    }
                    //interval = 50;

                }
                else if (journey[journeyIdx].trainDirection == direction.decreasing)
                {
                    currentKm = endKm;
                    currentTrainKm = journey[journeyIdx].geometryKm;

                    while (currentKm > startKm)
                    {
                        if (currentKm < 14.1)
                            interval = 50;


                        if (journeyIdx > 0 && journeyIdx < journey.Count())   // maybe count-1
                        {
                            X0 = journey[journeyIdx - 1].geometryKm;
                            X1 = journey[journeyIdx].geometryKm;
                            Y0 = journey[journeyIdx - 1].speed;
                            Y1 = journey[journeyIdx].speed;
                            if (timeChange)
                            {
                                time = journey[journeyIdx - 1].NotificationDateTime;
                                timeChange = false;
                            }
                        }
                        else
                        {
                            X0 = currentKm;
                            X1 = X0;
                            Y0 = 0.0;
                            Y1 = Y0;
                            time = new DateTime();
                        }

                        if (currentKm < X0) 
                            interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                        else //if (currentKm >= Y0)
                            interpolatedSpeed = 0;
                        //else
                        //    interpolatedSpeed = 0; // linear(currentKm, X0, X1, Y0, Y1);

                        previousKm = currentKm;
                        currentKm = currentKm - interval / 1000;

                        if (interpolatedSpeed > 0)
                            time = time.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                        InterpolatedTrain item = new InterpolatedTrain(trains[trainidx].TrainJourney[0].TrainID, trains[trainidx].TrainJourney[0].LocoID,
                                                                        time, previousKm, interpolatedSpeed);
                        interpolatedTrainList.Add(item);

                        if (journeyIdx < journey.Count)
                            if (currentKm <= journey[journeyIdx].geometryKm)
                            {
                                journeyIdx++;
                                timeChange = true;
                            }


                    }

                    //interval = 50;

                }
                else 
                { 
                    /* direction is not defined. There is an error. */                
                }


                Train trainItem = new Train(interpolatedTrainList, journey[0].trainDirection, true);
                newTrainList.Add(trainItem);
                


            }

            return newTrainList;
        }

        public void interpolateIncreasingTrainData(List<Train> trains)
        {

            /* Consider making this an input parameter */
            double interval = 50.0;     // metres
            double startKm = 5;
            double endKm = 70;

            //double gradient = 0;
            //double intercept = 0;
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;

            /* New train list */
            List<Train> newTrainList = new List<Train>();
            List<TrainDetails> journey = new List<TrainDetails>();

            for (int trainidx = 0; trainidx < trains.Count(); trainidx++)
            {
                journey = trains[trainidx].TrainJourney;
                int journeyIdx = 0;
                double currentKm = startKm;
                double currentTrainKm = journey[journeyIdx].geometryKm;

                while (currentKm < endKm)
                {
                    X0 = journey[journeyIdx].speed;
                    X1 = journey[journeyIdx + 1].speed;
                    Y0 = journey[journeyIdx].geometryKm;
                    Y1 = journey[journeyIdx + 1].geometryKm;

                    interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);

                    currentKm = currentKm + interval / 1000;

                    if (currentKm >= Y1)
                        journeyIdx++;


                }


            }


        }

        public void interpolateDecreasingTrainData(List<Train> trains)
        {

            /* Consider making this an input parameter */
            double interval = 50.0;     // metres
            double startKm = 70;
            double endKm = 5;

            //double gradient = 0;
            //double intercept = 0;
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;

            /* New train list */
            List<Train> newTrainList = new List<Train>();
            List<TrainDetails> journey = new List<TrainDetails>();

            for (int trainidx = 0; trainidx < trains.Count(); trainidx++)
            {
                journey = trains[trainidx].TrainJourney;
                int journeyIdx = 0;
                double currentKm = startKm;
                double currentTrainKm = journey[journeyIdx].geometryKm;

                while (currentKm > endKm)
                {
                    X0 = journey[journeyIdx].speed;
                    X1 = journey[journeyIdx + 1].speed;
                    Y0 = journey[journeyIdx].geometryKm;
                    Y1 = journey[journeyIdx + 1].geometryKm;

                    interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);

                    currentKm = currentKm - interval / 1000;

                    if (currentKm >= Y1)
                        journeyIdx++;


                }


            }


        }

        private double linear(double targetX, double X0, double X1, double Y0, double Y1)
        {
            if ((X1 - X0) == 0)
                return (Y0 + Y1) / 2;

            return Y0 + (targetX - X0) * (Y1 - Y0) / (X1 - X0);
        
        }
        
        private double calculateTimeInterval(double startPositon, double endPosition, double speed)
        {
            return Math.Abs(endPosition - startPositon) / speed;    // hours.

        }

    }
}
