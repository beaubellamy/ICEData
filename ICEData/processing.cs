using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalSettings;

namespace ICEData
{
    class processing
    {
        /* Mean radius of the Earth */
        private const double EarthRadius = 6371000.0;   // metres
        /* Constant time factors. */
        private const double secPerHour = 3600;
        private const double secPerDay = 86400;
        private const double hoursPerDay = 24;
        private const double minutesPerHour = 60;
        private const double secPerMinute = 60;

        
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

            double arcsine = Math.Sin(degress2radians((latitude2 - latitude1)/2)) * Math.Sin(degress2radians((latitude2 - latitude1)/2)) +
                Math.Cos(degress2radians(latitude1)) * Math.Cos(degress2radians(latitude2)) *
                Math.Sin(degress2radians((longitude2 - longitude1)/2)) * Math.Sin(degress2radians((longitude2 - longitude1)/2));
            double arclength = 2 * Math.Atan2(Math.Sqrt(arcsine), Math.Sqrt(1 - arcsine));

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

            double arcsine = Math.Sin(degress2radians((point2.latitude - point1.latitude) / 2)) * Math.Sin(degress2radians((point2.latitude - point1.latitude) / 2)) +
                Math.Cos(degress2radians(point1.latitude)) * Math.Cos(degress2radians(point2.latitude)) *
                Math.Sin(degress2radians((point2.longitude - point1.longitude) / 2)) * Math.Sin(degress2radians((point2.longitude - point1.longitude) / 2));
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
            for (int journeyIdx = 1; journeyIdx < train.TrainJourney.Count(); journeyIdx++)
            {
                /* Calculate the distance between successive points. */
                GeoLocation point1 = new GeoLocation(train.TrainJourney[journeyIdx - 1]);
                GeoLocation point2 = new GeoLocation(train.TrainJourney[journeyIdx]);
                point2PointDistance = calculateDistance(point1, point2);

                /* Determine the cumulative actual geometry km based on the direction. */
                if (direction.Equals(direction.increasing))
                    train.TrainJourney[journeyIdx].geometryKm = train.TrainJourney[journeyIdx - 1].geometryKm + point2PointDistance / 1000;

                else if (direction.Equals(direction.decreasing))
                    train.TrainJourney[journeyIdx].geometryKm = train.TrainJourney[journeyIdx - 1].geometryKm - point2PointDistance / 1000;

                else
                    train.TrainJourney[journeyIdx].geometryKm = train.TrainJourney[journeyIdx].kmPost;
            }

        }

        /// <summary>
        /// Populate the loop location information for each point in the train journey.
        /// </summary>
        /// <param name="train">A train object containing the journey details.</param>
        /// <param name="trackGeometry">The track geometry object indicating the location of the loops.</param>
        public void populateLoopLocations(Train train, List<trackGeometry> trackGeometry)
        {
            /* Create a track geometry object. */
            trackGeometry track = new trackGeometry();
            int index = 0;
            double trainPoint = 0;

            /* Cycle through the train journey. */
            foreach (TrainDetails journey in train.TrainJourney)
            {
                trainPoint = journey.geometryKm;
                /* Find the index of the closest point on the track to the train. */
                index = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
                /* Populate the loop */
                journey.isLoopHere = trackGeometry[index].isLoopHere;

            }
                    
        }

        /// <summary>
        /// populate the temporary speed rrestrcition information for each train journey.
        /// </summary>
        /// <param name="train">A train object containing teh journey details.</param>
        /// <param name="trackGeometry">teh track Geometry object indicating the TSR information at each location.</param>
        public void populateTemporarySpeedRestrictions(Train train, List<trackGeometry> trackGeometry)
        {
            /* Create a track geometry object. */
            trackGeometry track = new trackGeometry();
            int index = 0;
            double trainPoint = 0;

            /* Cycle through the train journey. */
            foreach (TrainDetails journey in train.TrainJourney)
            {
                trainPoint = journey.geometryKm;
                /* Find the index of the closest point on the track to the train. */
                index = track.findClosestTrackGeometryPoint(trackGeometry, trainPoint);
                /* Populate the loop */
                journey.isTSRHere = trackGeometry[index].isTSRHere;
                journey.TSRspeed = trackGeometry[index].temporarySpeedRestriction;
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
        ///  Interpolate the train speed to a specified interval using a linear interpolation.
        /// </summary>
        /// <param name="trains">List of train objects containing the parameters for each train journey.</param>
        /// <param name="trackGeometry">The list of Track geometry data to align the train location.</param>
        /// <param name="startKm">Starting kilometreage for the interpolation.</param>
        /// <param name="endKm">End kilometerage for the interpolation.</param>
        /// <param name="interval">interpolation interval, specified in metres.</param>
        /// <returns>List of train objects with interpolated values at the specified interval.</returns>
        public List<Train> interpolateTrainData(List<Train> trains, List<trackGeometry> trackGeometry)
        {
            /* Placeholders for the interpolated distance markers. */
            double previousKm = 0;
            double currentKm = 0;
            /* Place holder to calaculte the time for each interpolated value. */
            DateTime time = new DateTime();
            /* Flag to indicate when to collect the next time value. */
            bool timeChange = true;

            /* Additional loop and TSR details. */
            int geometryIdx = 0;
            bool loop = false;
            bool TSR = false;
            double TSRspeed = 0;
                
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
            for (int trainIdx = 0; trainIdx < trains.Count(); trainIdx++)
            {
                /* Create a new journey list of interpolated values. */
                List<InterpolatedTrain> interpolatedTrainList = new List<InterpolatedTrain>();

                journey = trains[trainIdx].TrainJourney;

                if (journey[0].trainDirection == direction.increasing)
                {
                    /* Set the start of the interpolation. */
                    currentKm = Settings.startKm;

                    while (currentKm < Settings.endKm)
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

                        geometryIdx = trackGeometry[0].findClosestTrackGeometryPoint(trackGeometry, currentKm);

                        if (geometryIdx >= 0)
                        {
                            /* Check if there is a loop at this location. */
                            loop = trackGeometry[geometryIdx].isLoopHere;

                            /* Check if there is a TSR at this location. */
                            TSR = trackGeometry[geometryIdx].isTSRHere;
                            TSRspeed = trackGeometry[geometryIdx].temporarySpeedRestriction;
                        }

                        /* Create the interpolated data object and add it to the list. */
                        InterpolatedTrain item = new InterpolatedTrain(trains[trainIdx].TrainJourney[0].TrainID, trains[trainIdx].TrainJourney[0].LocoID,
                                                                        time, currentKm, interpolatedSpeed, loop, TSR, TSRspeed);
                        interpolatedTrainList.Add(item);

                        /* Create a copy of the current km marker and increment. */
                        previousKm = currentKm;
                        currentKm = currentKm + Settings.interval / 1000;

                    }

                }
                else if (journey[0].trainDirection == direction.decreasing)
                {
                    /* Set the start of the interpolation. */
                    currentKm = Settings.endKm;

                    while (currentKm > Settings.startKm)
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

                        geometryIdx = trackGeometry[0].findClosestTrackGeometryPoint(trackGeometry, currentKm);

                        if (geometryIdx >= 0)
                        {
                            /* Check if there is a loop at this location. */
                            loop = trackGeometry[geometryIdx].isLoopHere;

                            /* Check if there is a TSR at this location. */
                            TSR = trackGeometry[geometryIdx].isTSRHere;
                            TSRspeed = trackGeometry[geometryIdx].temporarySpeedRestriction;
                        }
                        
                        /* Create the interpolated data object and add it to the list. */
                        InterpolatedTrain item = new InterpolatedTrain(trains[trainIdx].TrainJourney[0].TrainID, trains[trainIdx].TrainJourney[0].LocoID,
                                                                        time, currentKm, interpolatedSpeed, loop, TSR, TSRspeed);
                        interpolatedTrainList.Add(item);

                        /* Create a copy of the current km marker and increment. */
                        previousKm = currentKm;
                        currentKm = currentKm - Settings.interval / 1000;

                    }

                }
                else
                {
                    /* The train direction is not defined. */
                }

                /* Add the interpolated list to the list of new train objects. */
                Train trainItem = new Train(interpolatedTrainList, journey[0].trainDirection);
                newTrainList.Add(trainItem);

            }

            /* Return the completed interpolated train data. */
            return newTrainList;
        }

        /// <summary>
        /// Interpolate the simultion data to a specified interval using a linear interpolation.
        /// </summary>
        /// <param name="simulatedTrain">The list of the simulated train details.</param>
        /// <param name="trackGeometry">The list of Track geometry data to align the train location.</param>
        /// <param name="startKm">Starting kilometreage for the interpolation.</param>
        /// <param name="endKm">End kilometerage for the interpolation.</param>
        /// <param name="interval">interpolation interval, specified in metres.</param>
        /// <returns>List of interpolated values for the simulation.</returns>
        public List<InterpolatedTrain> interpolateSimulationData(List<simulatedTrain> simulatedTrain, List<trackGeometry> trackGeometry)
        {
            /* Placeholders for the interpolated distance markers. */
            double previousKm = 0;
            double currentKm = 0;
            /* Place holder to calaculte the time for each interpolated value. */
            DateTime simTime = new DateTime();
            double time = 0;
            double days, hours, minutes, seconds;
            /* Flag to indicate when to collect the next time value. */
            bool timeChange = true;

            /* Additional loop and TSR details. */
            int geometryIdx = 0;
            bool loop = false;
            bool TSR = false;
            double TSRspeed = 0;

            /* Index values for the interpolation parameters */
            int index0 = -1;
            int index1 = -1;

            /* Interplation parameters. */
            double interpolatedSpeed = 0;
            double X0, X1, Y0, Y1;

            List<InterpolatedTrain> simulatedInterpolation = new List<InterpolatedTrain>();

            if (simulatedTrain[1].singleLineKm - simulatedTrain[0].singleLineKm > 0)    // Increasing Km
            {
                /* Set the start of the interpolation. */
                currentKm = Settings.startKm;

                while (currentKm < Settings.endKm)
                {
                    /* Find the closest kilometerage markers either side of the current interpoaltion point. */
                    index0 = findClosestLowerKm(currentKm, simulatedTrain);
                    index1 = findClosestGreaterKm(currentKm, simulatedTrain);

                    /* If a valid index is found, extract the existing journey parameters and interpolate. */
                    if (index0 >= 0 && index1 >= 0)
                    {
                        X0 = simulatedTrain[index0].singleLineKm;
                        X1 = simulatedTrain[index1].singleLineKm;
                        Y0 = simulatedTrain[index0].velocity;
                        Y1 = simulatedTrain[index1].velocity;
                        if (timeChange)
                        {
                            time = simulatedTrain[index0].time;
                            days = (time / secPerDay);
                            hours = (days - Math.Truncate(days)) * hoursPerDay;
                            minutes = (hours - Math.Truncate(hours)) * minutesPerHour;
                            seconds = (minutes - Math.Truncate(minutes)) * secPerMinute;

                            simTime = new DateTime(2000, 1, (int)days+1, (int)hours, (int)minutes, (int)seconds);
                            timeChange = false;
                        }

                        /* Perform linear interpolation. */
                        interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                        /* Interpolate the time. */
                        simTime = simTime.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                    }
                    else
                    {
                        /* Boundary conditions for interpolating the data prior to and beyond the existing journey points. */
                        time = 0;
                        interpolatedSpeed = 0;

                    }

                    /* Determine if we need to extract the time from the data or interpolate it. */
                    if (index1 >= 0)
                        if (currentKm >= simulatedTrain[index1].singleLineKm)
                            timeChange = true;

                    geometryIdx = trackGeometry[0].findClosestTrackGeometryPoint(trackGeometry, currentKm);

                    if (geometryIdx >= 0)
                    {
                        /* Check if there is a loop at this location. */
                        loop = trackGeometry[geometryIdx].isLoopHere;

                        /* Check if there is a TSR at this location. */
                        TSR = trackGeometry[geometryIdx].isTSRHere;
                        TSRspeed = trackGeometry[geometryIdx].temporarySpeedRestriction;
                    }

                    /* Create the interpolated data object and add it to the list. */
                    InterpolatedTrain item = new InterpolatedTrain("Simulated Train", "Simulated Loco", simTime, currentKm, interpolatedSpeed, loop, TSR, TSRspeed);
                    simulatedInterpolation.Add(item);

                    /* Create a copy of the current km marker and increment. */
                    previousKm = currentKm;
                    currentKm = currentKm + Settings.interval / 1000;

                }

            }
            else              // Decreasing km.            
            {
                /* Set the start of the interpolation. */
                currentKm = Settings.endKm;

                while (currentKm > Settings.startKm)
                {
                    /* Find the closest kilometerage markers either side of the current interpoaltion point. */
                    index0 = findClosestLowerKm(currentKm, simulatedTrain);
                    index1 = findClosestGreaterKm(currentKm, simulatedTrain);

                    /* If a valid index is found, extract the existing journey parameters and interpolate. */
                    if (index0 >= 0 && index1 >= 0)
                    {
                        X0 = simulatedTrain[index0].singleLineKm;
                        X1 = simulatedTrain[index1].singleLineKm;
                        Y0 = simulatedTrain[index0].velocity;
                        Y1 = simulatedTrain[index1].velocity;
                        if (timeChange)
                        {
                            time = simulatedTrain[index0].time;
                            days = (time / secPerDay);
                            hours = (days - Math.Truncate(days)) * hoursPerDay;
                            minutes = (hours - Math.Truncate(hours)) * minutesPerHour;
                            seconds = (minutes - Math.Truncate(minutes)) * secPerMinute;

                            simTime = new DateTime(2000, 1, (int)days+1, (int)hours, (int)minutes, (int)seconds);
                            timeChange = false;
                        }

                        /* Perform linear interpolation. */
                        interpolatedSpeed = linear(currentKm, X0, X1, Y0, Y1);
                        /* Interpolate the time. */
                        simTime = simTime.AddHours(calculateTimeInterval(previousKm, currentKm, interpolatedSpeed));

                    }
                    else
                    {
                        /* Boundary conditions for interpolating the data prior to and beyond the existing journey points. */
                        time = 0;
                        interpolatedSpeed = 0;
                    }

                    /* Determine if we need to extract the time from the data or interpolate it. */
                    if (index0 >= 0)
                        if (currentKm <= simulatedTrain[index0].singleLineKm)
                            timeChange = true;

                    geometryIdx = trackGeometry[0].findClosestTrackGeometryPoint(trackGeometry, currentKm);

                    if (geometryIdx >= 0)
                    {
                        /* Check if there is a loop at this location. */
                        loop = trackGeometry[geometryIdx].isLoopHere;

                        /* Check if there is a TSR at this location. */
                        TSR = trackGeometry[geometryIdx].isTSRHere;
                        TSRspeed = trackGeometry[geometryIdx].temporarySpeedRestriction;
                    }

                    /* Create the interpolated data object and add it to the list. */
                    InterpolatedTrain item = new InterpolatedTrain("Simualted Train","Simulated Loco",simTime, currentKm, interpolatedSpeed, loop, TSR, TSRspeed);
                    simulatedInterpolation.Add(item);

                    /* Create a copy of the current km marker and increment. */
                    previousKm = currentKm;
                    currentKm = currentKm - Settings.interval / 1000;
                }
            }

            return simulatedInterpolation;
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
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].geometryKm < target)
                    difference = Math.Abs(journey[journeyIdx].geometryKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = journeyIdx;
                }

            }

            if (difference == double.MaxValue)
                return -1;

            return index;
        }

        /// <summary>
        /// Find the index of the closest kilometerage that is less than the target point.
        /// </summary>
        /// <param name="target">The target kilometerage.</param>
        /// <param name="journey">The list of train details containig the journey parameters.</param>
        /// <returns>The index of the closest point that is less than the target point. 
        /// Returns -1 if a point does not exist.</returns>
        private int findClosestLowerKm(double target, List<simulatedTrain> journey)
        {
            /* set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].singleLineKm < target)
                    difference = Math.Abs(journey[journeyIdx].singleLineKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = journeyIdx;
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
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].geometryKm > target)
                    difference = Math.Abs(journey[journeyIdx].geometryKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = journeyIdx;
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
        /// <param name="journey">The list of simulated train data containig the journey parameters.</param>
        /// <returns>The index of the closest point that is larger than the target point. 
        /// Returns -1 if a point does not exist.</returns>        
        private int findClosestGreaterKm(double target, List<simulatedTrain> journey)
        {
            /* set the initial values. */
            double minimum = double.MaxValue;
            double difference = double.MaxValue;
            int index = 0;

            /* Cycle through the journey parameters. */
            for (int journeyIdx = 0; journeyIdx < journey.Count(); journeyIdx++)
            {
                /* Find the difference if the value is lower. */
                if (journey[journeyIdx].singleLineKm > target)
                    difference = Math.Abs(journey[journeyIdx].singleLineKm - target);

                /* Find the minimum difference. */
                if (difference < minimum)
                {
                    minimum = difference;
                    index = journeyIdx;
                }
            }

            if (difference == double.MaxValue)
                return -1;

            return index;
        }

        /// <summary>
        /// Determine the average speed of all trains in a specified direction given a power to weight range.
        /// </summary>
        /// <param name="trains">All train data containig each train journey details.</param>
        /// <param name="simulation">The journey of the simulated train.</param>
        /// <param name="lowerBound">The lower bound of the acceptable power to weight ratio.</param>
        /// <param name="upperBound">The upper bound of the acceptable power to weight ratio.</param>
        /// <param name="direction">The direction of the km of the train journey.</param>
        /// <returns>A list for the average speed of trains within the power to weight range.</returns>
        public List<double> powerToWeightAverageSpeed(List<Train> trains, List<InterpolatedTrain> simulation, double lowerBound, double upperBound, direction direction)
        {
            
            int size = (int)((Settings.endKm - Settings.startKm) / (Settings.interval / 1000));
            double sum = 0;
            
            /* Place holders for the included speeds and teh resulting average speed at each location. */
            List<double> speed = new List<double>();
            List<double> averageSpeed = new List<double>();

            TrainDetails journey = new TrainDetails();
            
            /* Loop through each interpoalted location. */
            for (int journeyIdx = 0; journeyIdx <= size; journeyIdx++)
            {
                sum = 0;
                speed.Clear();
                
                /* Loop through each train. */
                foreach (Train train in trains)
                {
                    journey = train.TrainJourney[journeyIdx];

                    if (journey.trainDirection == direction)
                    {
                            
                        /* Is there a TSR that applies */                        
                        if (!isTrainInTSRBoundary())
                        {
                            /* Check train is not within the loop boundaries */
                            if (!isTrainInLoopBoundary(train, Settings.loopBoundaryThreshold, journey.geometryKm))
                            {
                                if (journey.powerToWeight > lowerBound && journey.powerToWeight < upperBound)
                                {
                                    speed.Add(journey.speed);
                                    sum = sum + journey.speed;
                                }
                            }
                            else 
                            {
                                /* Train is within the loop boundaries */
                                if (journey.speed > (simulation[journeyIdx].speed * Settings.loopSpeedThreshold))
                                {
                                    speed.Add(journey.speed);
                                    sum = sum + journey.speed;
                                }
                            }
                        }
                        else 
                        {
                            /* A TSR applies to the current position on the train. */
                        }
                    }                    
                }
            
                if (speed.Count == 0)
                    averageSpeed.Add(0);       // This might have to be the simulated speed
                else
                {
                    
                    if (sum == 0)
                    {
                        averageSpeed.Add(0);
                    }
                    else
                    {
                        averageSpeed.Add(speed.Where(x => x > 0.0).Average(x => x));
                    }
                }

            }
            
            return averageSpeed;
        }

       /// <summary>
       /// Determine if the train is approaching, leaving or within a loop.
       /// </summary>
       /// <param name="train">The train object containing the journey details.</param>
       /// <param name="loopThreshold">The window either side of the loop to consider the train within the loop boundary.</param>
       /// <param name="targetLocation">The specific location being considered.</param>
       /// <returns>True, if the train is within the boundaries of teh loop window.</returns>
        public bool isTrainInLoopBoundary(Train train, double loopThreshold, double targetLocation)
        {
            
            /* Find the indecies of the boundaries of the loop. */
            double lookBack = targetLocation - loopThreshold;
            double lookForward = targetLocation + loopThreshold;
            int lookBackIdx = train.indexOfgeometryKm(train.TrainJourney, lookBack);
            int lookForwardIdx = train.indexOfgeometryKm(train.TrainJourney, lookForward);

            /* Check the indecies are valid */
            if (lookBack < Settings.startKm && lookBackIdx == -1)
                lookBackIdx = 0;
            if (lookForward > Settings.endKm && lookForwardIdx == -1)
                lookForwardIdx = train.TrainJourney.Count() - 1;

            /* Determine if a loop is within the loop window of the current position. */
            if (lookBackIdx >= 0 && lookForwardIdx >= 0)
            {
                for (int journeyIdx = lookBackIdx; journeyIdx < lookForwardIdx; journeyIdx++)
                {
                    TrainDetails journey = train.TrainJourney[journeyIdx];

                    if (journey.isLoopHere)
                        return true;

                }
            }
            return false;
        }

        /* Dummy TSR boundary function. */
        public bool isTrainInTSRBoundary()
        {

            return false;
        }



    } // Class processing


} // namespace

