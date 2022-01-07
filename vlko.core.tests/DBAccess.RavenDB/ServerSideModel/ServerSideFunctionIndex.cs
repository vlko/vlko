using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.tests.DBAccess.RavenDB.ServerSideModel
{
    public class ServerSideFunctionIndex : AbstractIndexCreationTask<DataModel, ServerSideResult>
    {
        public ServerSideFunctionIndex()
        {
            Map = items => from item in items
                           let related = LoadDocument<RelatedModel>(item.RelId)
                           select new
                           {
                               Id = item.RelId,
                               Year = item.Year,
                               Name = related.Name ?? null,
                               AssetActual = GetActual(item.Data, "1"),
                               AssetPrev = GetPrev(item.Data, "1"),
                               AssetChange = GetChange(item.Data, "1"),
                           };
            Reduce = results => from result in results
                                group result by result.Id
                                    into g
                                let financials = g.Where(x => x.Year != null).ToArray()
                                    .OrderByDescending(x => x.Year).ToArray()
                                let actualFinancial = financials.FirstOrDefault()
                                select new
                                {
                                    Id = g.Key,
                                    Year = actualFinancial.Year ?? null,
                                    Name = actualFinancial.Name ?? null,
                                    AssetActual = actualFinancial.AssetActual ?? null,
                                    AssetPrev = actualFinancial.AssetPrev ?? null,
                                    AssetChange = actualFinancial.AssetChange ?? null,
                                };

            AdditionalSources = new Dictionary<string, string>
            {
                { "Compute", ComputeFunctions }
            };
        }

        private const string ComputeFunctions =
        @"
            using System;
            namespace finstat.Indexes
            {
                public class ValueSubModel
                {
                    public double? Actual { get; set; }
                    public double? Prev { get; set; }
                }
                public partial class ServerSideFunctionIndex
                {
                    public static double? GetActual(IDictionary<string, ValueSubModel> data, string index)
                    {
                        if (data != null && data.ContainsKey(index))
                        {
                            return data[index].Actual;
                        }
                        return null;
                    }

                    public static double? GetPrev(IDictionary<string, ValueSubModel> data, string index)
                    {
                        if (data != null && data.ContainsKey(index))
                        {
                            return data[index].Prev;
                        }
                        return null;
                    }

                    public static double? GetChange(IDictionary<string, ValueSubModel> data, string index)
                    {
                        if (data != null && data.ContainsKey(index)
                            && data[index].Actual != null && data[index].Prev != null
                            && !double.IsNaN(data[index].Actual) && !double.IsNaN(data[index].Prev)
                            && data[index].Prev != 0)
                        {
                            return ((double)data[index].Actual - (double)data[index].Prev) / Math.Abs((double)data[index].Prev);
                        }
                        return null;
                    }
                }
            }
        ";
        public static double? GetActual(IDictionary<string, ValueSubModel> data, string index)
        {
            if (data != null && data.ContainsKey(index))
            {
                return data[index].Actual;
            }
            return null;
        }

        public static double? GetPrev(IDictionary<string, ValueSubModel> data, string index)
        {
            if (data != null && data.ContainsKey(index))
            {
                return data[index].Prev;
            }
            return null;
        }

        public static double? GetChange(IDictionary<string, ValueSubModel> data, string index)
        {
            if (data != null && data.ContainsKey(index)
                && data[index].Actual != null && data[index].Prev != null
                && !double.IsNaN(data[index].Actual.Value) && !double.IsNaN(data[index].Prev.Value)
                && data[index].Prev.Value != 0)
            {
                return (data[index].Actual - data[index].Prev) / Math.Abs(data[index].Prev.Value);
            }
            return null;
        }
    }

    public class ServerSideResult
    {
        public string Id { get; set; }
        public int? Year { get; set; }
        public string Name { get; set; }
        public double? AssetActual { get; set; }
        public double? AssetPrev { get; set; }
        public double? AssetChange { get; set; }
    }
}
