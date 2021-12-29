using System;
using System.Collections.Generic;
using System.Text;
using vlko.core.DBAccess;

namespace vlko.core.RavenDB.DBAccess
{
    public class RavenSessionOptions : ISessionOptions
    {
        public bool ClusterWide { get; set; }
        public bool NoTracking { get; set; }
        public bool NoCaching { get; set; }
    }

    public class UseClusterSession : RavenSessionOptions
    {
        public UseClusterSession()
        {
            ClusterWide = true;
        }
    }

    public class UseNoTrackingSession : RavenSessionOptions
    {
        public UseNoTrackingSession()
        {
            NoTracking = true;
        }
    }
}
