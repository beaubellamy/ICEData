using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICEData;

namespace GlobalSettings
{
    public static class Settings
    {
        /* Data boudnaries */
        public static DateTime[] dateRange;                 /* Date range of data to include. */
        public static double[] latitude;                    /* Latitude boundaries of included data. */
        public static double[] longitude;                   /* Longitude boundaries of included data. */
        public static bool includeAListOfTrainsToExclude;   /* Is a list of trains that are to be excluded available. */

        /* Corridor dependant parameters */
        public static double startKm;                       /* Start km for interpoaltion data. */
        public static double endKm;                         /* End km for interpolation data. */
        public static double interval;                      /* Interpolation interval (metres). */
        public static double minimumJourneyDistance;        /* Minimum distance of a train journey to be considered valid. */

        /* Processing parameters */
        public static double loopSpeedThreshold;            /* Cuttoff for the simulation speed, when comparing the train to the simualted train. */
        public static double loopBoundaryThreshold;         /* Distance either side of the loop to be considered within the loop boundary (km). */
        public static double TSRwindowBounday;              /* Distance either side of the TSR location to be considered within the TSR boundary (km). */
        public static double timeThreshold;                 /* Minimum time between data points to be considered a seperate train. */
        public static double distanceThreshold;             /* Minimum ditance between successive data points. */
        

    } // Class Settings

}

