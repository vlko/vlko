using System;
using System.Collections.Generic;
using System.Text;

namespace vlko.core.tests.DBAccess.Model
{
    public class IndexItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string NotIndexed { get; set; }
        public int Integer { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double? NullValue { get; set; }
        public DateTime? NullDate { get; set; }
    }

    public class ProjectionItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Computed { get; set; }
        public int Integer { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double? NullValue { get; set; }
        public DateTime? NullDate { get; set; }
    }
}
