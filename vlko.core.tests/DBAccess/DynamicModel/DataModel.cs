using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.tests.DBAccess.DynamicModel
{
    public class DataModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Cancelled { get; set; }
    }

    public class HistoryDataModel
    {
        public string Id { get; set; }
        public string RelId { get; set; }
        public string[] Subjects { get; set; }
    }

    public class FinancialDataModel
    {
        public string Id { get; set; }
        public string RelId { get; set; }
        public int Year { get; set; }
        public IDictionary<string, double?> Data { get; set; }
    }

    public class ORDateDataModel
    {
        public string Id { get; set; }
        public string RelId { get; set; }
        public DateTime Date { get; set; }
    }
}
