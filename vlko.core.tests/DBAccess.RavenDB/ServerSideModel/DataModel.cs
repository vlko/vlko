using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.tests.DBAccess.RavenDB.ServerSideModel
{
    public class DataModel
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public string RelId { get; set; }
        public IDictionary<string, ValueSubModel> Data { get; set; }
    }

    public class ValueSubModel
    {
        public ValueSubModel() { }
        public ValueSubModel(double? actual, double? prev)
        {
            Actual = actual;
            Prev = prev;
        }

        public double? Actual { get; set; }
        public double? Prev { get; set; }
    }
}
