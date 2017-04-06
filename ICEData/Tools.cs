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
        /// Populate the geometry km information based on the calculated distance from the first km post.
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
                
        /// <summary>
        /// Linear interpolation to a target point.
        /// </summary>
        /// <param name="targetX">Target invariant location to be interpolated to.</param>
        /// <param name="X0">Lower invariant position to interpolate between.</param>
        /// <param name="X1">Upper invariant position to interpolate between.</param>
        /// <param name="Y0">Lower variant to interpolate between.</param>
        /// <param name="Y1">Upper variant to interpolate between.</param>
        /// <returns>The interpolate variant value at the target invariant location.</returns>
        private double linear(double targetX, double X0, double X1, double Y0, double Y1)
        {
            /* Take the average when the invariant location does not change. */
            if ((X1 - X0) == 0)
                return (Y0 + Y1) / 2;

            return Y0 + (targetX - X0) * (Y1 - Y0) / (X1 - X0);
        
        }

        /// <summary>
        ///  Interpolate the train speed to s specified interval using a linear interpolation.
        /// </summary>
        /// <param name="trains">List of train objects containing the parameters for each train journey.</param>
        /// <param name="startKm">Starting kilometreage for the interpolation.</param>
        /// <param name="endKm">End kilometerage for the interpolation.</param>
        /// <param name="interval">interpolation interval, specified in metres.</param>
        /// <returns>List of train objects with interpolated values at the specified interval.</returns>
        public List<Train> interpolateTrainData(List<Train> trains, double startKm, double endKm, double interval)
        {
            /* Placeholders for the interpolated distance markers. */
            double previousKm = 0;
            double currentKm = 0;
            /* Place holder to calaculte the time for each interpolated value. */
            DateTime time = new DateTime();
            /* Flag to indicate when to collect the next time value. */
            bool timeChange = true;

            /* Index values for the interpolation parameters */
            int index0 = -1;
            int index1 = -1;

            /* Interplation parameters. */
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;


            /* Create a new list of trains for the journies interpolated values. */
            List<Train> newTrainList = new List<Train>();
            /* Create a journey list to store the existing journey details. */
            List<TrainDetails> journey = new List<TrainDetails>();

            /* Cycle through each train to interpolate between points. */
            for (int trainidx = 0; trainidx < trains.Count(); trainidx++)
            {
                /* Create a new journey list of interpolated values. */
                List<InterpolatedTrain> interpolatedTrainList = new List<InterpolatedTrain>();

                journey = trains[trainidx].TrainJourney;

                if (journey[0].trainDirection == direction.increasing)
                {
                    /* Set the start of the interpolation. */
                    currentKm = startKm;

                    while (currentKm < endKm)
                    {
                        /* Find the closest kilometerage markers either side of the current interpoaltion point. */
                        index0 = findClosestLowerKm(currentKm, journey);
                        index1 = findClosestGreaterKm(currentKm, journey);

                        /* If a valid index is found, extract the existing journey parameters and interpolate. */
                        if (index0 >= 0 && index1 >= 0)
                        {
                            X0 = journey[index0].geometryKm;
                            X1 = journey[index1].geometryKm;
                            Y0 = journey[index0].speed;
                            Y1 = journey[index1].speed;
                            if (timeChange)
                            {
                                time = journey[index0].NotificationDateTime;
                                timeChange = false;
                            }

                            /* Perform linear interpolation. */
                            interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                            /* Interpolate the time. */
                            time = time.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                        }
                        else
                        {
                            /* Boundary conditions for interpolating the data prior to and beyond the existing journey points. */
                            time = new DateTime(2000, 1, 1);
                            interpolatedSpeed = 0;

                        }

                        /* Determine if we need to extract the time from the data or interpolate it. */
                        if (index1 >= 0)
                            if (currentKm >= journey[index1].geometryKm)
                                timeChange = true;

                        /* Create the interpolated data object and add it to the list. */
                        InterpolatedTrain item = new InterpolatedTrain(trains[trainidx].TrainJourney[0].TrainID, trains[trainidx].TrainJourney[0].LocoID,
                                                                        time, currentKm, interpolatedSpeed);
                        interpolatedTrainList.Add(item);

                        /* Create a copy of the current km marker and increment. */
                        previousKm = currentKm;
                        currentKm = currentKm + interval / 1000;

                    }

                }
                else if (journey[0].trainDirection == direction.decreasing)
                {
                    /* Set the start of the interpolation. */
                    currentKm = endKm;

                    while (currentKm > startKm)
                    {
                        /* Find the closest kilometerage markers either side of the current interpoaltion point. */
                        index0 = findClosestLowerKm(currentKm, journey);
                        index1 = findClosestGreaterKm(currentKm, journey);

                        /* If a valid index is found, extract the existing journey parameters and interpolate. */
                        if (index0 >= 0 && index1 >= 0)
                        {
                            X0 = journey[index0].geometryKm;
                            X1 = journey[index1].geometryKm;
                            Y0 = journey[index0].speed;
                            Y1 = journey[index1].speed;
                            if (timeChange)
                            {
                                time = journey[index0].NotificationDateTime;
                                timeChange = false;
                            }

                            /* Perform linear interpolation. */
                            interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                            /* Interpolate the time. */
                            time = time.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                        }
                        else
                        {
                            /* Boundary conditions for interpolating the data prior to and beyond the existing journey points. */
                            time = new DateTime(2000, 1, 1);
                            interpolatedSpeed = 0;
                        }

                        /* Determine if we need to extract the time from the data or interpolate it. */
                        if (index0 >= 0)
                            if (currentKm <= journey[index0].geometryKm)
                                timeChange = true;


                        /* Create the interpolated data object and add it to the list. */
                        InterpolatedTrain item = new InterpolatedTrain(trains[trainidx].TrainJourney[0].TrainID, trains[trainidx].TrainJourney[0].LocoID,
                                                                        time, currentKm, interpolatedSpeed);
                        interpolatedTrainList.Add(item);

                        /* Create a copy of the current km marker and increment. */
                        previousKm = currentKm;
                        currentKm = currentKm - interval / 1000;

                    }

                }
                else
                {
                    /* The train direction is not defined. */
                }

                /* Add the interpolated list to the list of new train objects. */
                Train trainItem = new Train(interpolatedTrainList, journey[0].trainDirection, true);
                newTrainList.Add(trainItem);

            }

            /* Return the completed interpolated train data. */
            return newTrainList;
        }
        
        /// <summary>
        /// Calculate the time interval between two locations based on the speed.
        /// </summary>
        /// <param name="startPositon">Starting kilometreage.</param>
        /// <param name="endPosition">Final kilometreage.</param>
        /// <param name="speed">Average speed between locations.</param>
        /// <returns>The time taken to traverse the distance in hours.</returns>
        private double calculateTimeInterval(double startPositon, double endPosition, double speed)
        {
            return Math.Abs(endPosition - startPositon) / speed;    // hours.
        }

        /// <summary>
        /// Find the index of the closest kilometerage that is less than the target point.
        /// </summary>
        /// <param name="target">The target kilometerage.</param>
        /// <param name="journey">The list of train details containig the journey parameters.</param>
        /// <returns>The index of the closest point that is less than the target point. 
        /// Returns -1 if a point does not exist.</returns>
        private int findClosestLowerKm(double target, List<TrainDetails> journey)
        {
            /* set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int i = 0; i < journey.Count(); i++)
            {
                /* Find the difference if the value is lower. */
                if (journey[i].geometryKm < target)
                    difference = Math.Abs(journey[i].geometryKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = i;
                }

            }

            if (difference == double.MaxValue)
                return -1;
            
            return index;
        }

        /// <summary>
        /// Find the index of the closest kilometerage that is larger than the target point.
        /// </summary>
        /// <param name="target">The target kilometerage.</param>
        /// <param name="journey">The list of train details containig the journey parameters.</param>
        /// <returns>The index of the closest point that is larger than the target point. 
        /// Returns -1 if a point does not exist.</returns>
        private int findClosestGreaterKm(double target, List<TrainDetails> journey)
        {
            /* set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int i = 0; i < journey.Count(); i++)
            {
                /* Find the difference if the value is lower. */
                if (journey[i].geometryKm > target)
                    difference = Math.Abs(journey[i].geometryKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = i;
                }

            }

            if (difference == double.MaxValue)
                return -1;

            return index;
        }


    }
}
