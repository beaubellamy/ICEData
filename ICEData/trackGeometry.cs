using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICEData
{
    class trackGeometry
    {
        int corridorNumber;
        string corridorName;
        GeoLocation point = new GeoLocation();
        double elevation;
        double kilometreage;
        double virtualKilometreage;
        bool isLoopHere;

        Tools tool = new Tools();

        /* Constructors */

        /// <summary>
        /// Default track geometry constructor
        /// </summary>
        public trackGeometry()
        {
            this.corridorNumber = 0;
            this.corridorName = "not specified";
            this.point.latitude = -33.8519;   //Sydney Harbour Bridge
            this.point.longitude = 151.2108;
            this.elevation = 0;
            this.kilometreage = 0;
            this.virtualKilometreage = 0;
            this.isLoopHere = false;
        }

        /// <summary>
        /// Track Geometry Constructor
        /// </summary>
        /// <param name="corridorNumber">Integer value representing the corridor</param>
        /// <param name="corridorName">The corridor name</param>
        /// <param name="latitude">The geographic latitude of the track</param>
        /// <param name="longitude">The geographic longitude of the track.</param>
        /// <param name="elevation">The elevation of the track.</param>
        /// <param name="kilometreage">The kilometreage of the track.</param>
        /// <param name="virtualKilometreage">The cummulative kilometreage for a single corridor track.</param>
        /// <param name="loop">A flag indicating if a loop is located at this location.</param>
        public trackGeometry(int corridorNumber, string corridorName, double latitude, double longitude, double elevation, double kilometreage, double virtualKilometreage, bool loop)
        {
            this.corridorNumber = corridorNumber;
            this.corridorName = corridorName;
            this.point.latitude = latitude;
            this.point.longitude = longitude;
            this.elevation = elevation;
            this.kilometreage = kilometreage;
            this.virtualKilometreage = virtualKilometreage;
            this.isLoopHere = loop;
        }

        /// <summary>
        /// Function reads in the track geometry data from file.
        /// </summary>
        /// <param name="filename">Full filename of teh geometry file.</param>
        /// <returns>A list of track Geometry objects describing the track geometry.</returns>
        public List<trackGeometry> readGeometryfile(string filename)
        {
            /* Create the list of track geometry objects. */
            List<trackGeometry> trackGeometry = new List<trackGeometry>();
           
            bool header = true;
            
            /* Read all the lines of the file. */
            string[] lines = System.IO.File.ReadAllLines(filename);
            char[] delimeters = { ',', '\'', '"', '\t', '\n' };     // not sure of the delimter ??

            /* Seperate the fields. */
            string[] fields = lines[0].Split(delimeters);

            int firstPoint = 0;

            /* Define the track geomerty parameters. */
            string geometryName = null;
            double latitude = 0.0;
            double longitude = 0.0;
            double elevation = 0.0;
            double kilometreage = 0.0;
            double virtualKilometreage = 0.0;
            bool isLoopHere = false;

            /* Define some additional helper parameters. */
            double distance = 0;
            direction direction = direction.notSpecified;
            double previousLat = 0;
            double previousLong = 0;
            double previouskm = 0;
            string loop;

            /* Add the trains to the list. */
            foreach (string line in lines)
            {
                if (header)
                    /* Ignore the header line. */
                    header = false;
                else
                {

                    /* Seperate each record into each field */
                    fields = line.Split(delimeters);
                    geometryName = fields[0];
                    double.TryParse(fields[1], out latitude);
                    double.TryParse(fields[2], out longitude);
                    double.TryParse(fields[3], out elevation);
                    double.TryParse(fields[4], out kilometreage);
                    loop = fields[6];

                    if (loop.Equals("loop", StringComparison.OrdinalIgnoreCase) || loop.Equals("true", StringComparison.OrdinalIgnoreCase))
                        isLoopHere = true;
                    else
                        isLoopHere = false;

                    /* The virtual kilometreage starts at the first kilometreage of the track. */
                    if (firstPoint == 0)
                    {
                        virtualKilometreage = kilometreage;
                        /* Set the 'pervious' parameters. */
                        previousLat = latitude;
                        previousLong = longitude;
                        previouskm = kilometreage;
                        firstPoint++;
                    }
                    else
                    {
                        /* Determine the direction the track kilometreage. */
                        if (firstPoint == 1)
                        {
                            if (kilometreage - previouskm > 0)
                                direction = direction.increasing;
                            else
                                direction = direction.decreasing;

                            firstPoint++;
                        }

                        /* Calcualte the distance between succesive points and increment the virtual kilometreage. */
                        distance = tool.calculateDistance(previousLat, previousLong, latitude, longitude);
                        if (direction == direction.increasing)
                            virtualKilometreage = virtualKilometreage + distance/1000;
                        
                        else
                            virtualKilometreage = virtualKilometreage - distance/1000;
                        
                        /* Set the 'previous' parameters. */
                        previousLat = latitude;
                        previousLong = longitude;
                        
                    }
              
                /* Add the geometry point to the list. */
                trackGeometry geometry = new trackGeometry(0, geometryName, latitude, longitude, elevation, kilometreage, virtualKilometreage, isLoopHere);
                trackGeometry.Add(geometry);
                }
            }


            return trackGeometry;
        }

        /// <summary>
        /// Function finds the kilometreage point on the track that is closest to the supplied point (latitude, longitude)
        /// </summary>
        /// <param name="trackGeometry">A list of trackGeometry objects.</param>
        /// <param name="Latitude">Latitude of the point supplied</param>
        /// <param name="Longitude">Longitude of the point supplied.</param>
        /// <returns>The kilometreage of the closest point to the track geometry.</returns>
        public double findClosestTrackGeometryPoint(List<trackGeometry> trackGeometry, double Latitude, double Longitude)
        {
            /* Set up initial values. */
            int minimumIndex = 0;
            double minimumDistance = double.MaxValue;
            double distance = 0;
            GeoLocation trackPoint = new GeoLocation();


            for (int trackIdx = 0; trackIdx < trackGeometry.Count(); trackIdx++)
            {
                /* Set the current track geometry point. */
                trackPoint = trackGeometry[trackIdx].point;
                /* Calcualte the distance between the current track point and the location supplied. */
                distance = tool.calculateDistance(trackPoint.latitude, trackPoint.longitude, Latitude, Longitude);

                /* Determine when the minimum distance is reached. */
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    minimumIndex = trackIdx;
                }

            }

            /* Return the kilometreage of the point that is closest to the location supplied. */
            return trackGeometry[minimumIndex].kilometreage;
        }

        /// <summary>
        /// Function finds the kilometreage point on the track that is closest to the supplied geographic location (latitude, longitude)
        /// </summary>
        /// <param name="trackGeometry">List of trackGeometry objects.</param>
        /// <param name="Location">Geographic location with latitude and longitude.</param>
        /// <returns>The kilometreage of the closest point to the track geometry.</returns>
        public double findClosestTrackGeometryPoint(List<trackGeometry> trackGeometry, GeoLocation Location)
        {
            /* Set up initial values. */
            int minimumIndex = 0;
            double minimumDistance = double.MaxValue;
            double distance = 0;
            GeoLocation trackPoint = new GeoLocation();


            for (int trackIdx = 0; trackIdx < trackGeometry.Count(); trackIdx++)
            {
                /* Set the current track geometry point. */
                trackPoint = trackGeometry[trackIdx].point;
                /* Calcualte the distance between the current track point and the location supplied. */
                distance = tool.calculateDistance(trackPoint, Location);
                
                /* Determine when the minimum distance is reached. */
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    minimumIndex = trackIdx;
                }

            }

            /* Return the kilometreage of the point that is closest to the location supplied. */
            return trackGeometry[minimumIndex].kilometreage;
        }


    }

    
}
