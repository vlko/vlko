using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace vlko.core.tests.DBAccess.RavenDB.DynamicModel
{
    public class DynamicModelIndex : AbstractMultiMapIndexCreationTask<DynamicModelResult>
    {
        public DynamicModelIndex()
        {
            AddMap<DataModel>(models => from model in models
                                        select new
                                        {
                                            Id = model.Id,
                                            MapType = "model",
                                            Name = model.Name,
                                            Year = (int?)null,
                                            Temp = new Dictionary<string, object>
                                            {
                                                { "Created", model.Created },
                                                { "Cancelled", model.Cancelled }
                                            },
                                            _ = "ignored"
                                        });
            AddMap<HistoryDataModel>(models => from model in models
                                               select new
                                               {
                                                   Id = model.RelId,
                                                   MapType = "history",
                                                   Name = (string)null,
                                                   Year = (int?)null,
                                                   Temp = new Dictionary<string, object>
                                            {
                                                { "Subjects", model.Subjects },
                                            },
                                                   _ = "ignored"
                                               });
            AddMap<FinancialDataModel>(models => from model in models
                                                 select new
                                                 {
                                                     Id = model.RelId,
                                                     MapType = "financial",
                                                     Name = (string)null,
                                                     Year = model.Year,
                                                     Temp = new Dictionary<string, object>
                                            {
                                                { "Assets", model.Data["1"] },
                                                { "Analysis", model.Data["A1"] },
                                            },
                                                     _ = "ignored"
                                                 });
            AddMap<ORDateDataModel>(models => from model in models
                                              select new
                                              {
                                                  Id = model.RelId,
                                                  MapType = "ordate",
                                                  Name = (string)null,
                                                  Year = (int?)null,
                                                  Temp = new Dictionary<string, object>
                                            {
                                                { "RelatedDate", model.Date },
                                            },
                                                  _ = "ignored"
                                              });

            Reduce = results => from result in results
                                group result by result.Id into g
                                let model = g.FirstOrDefault(x => x.MapType == "model")
                                let history = g.FirstOrDefault(x => x.MapType == "history")
                                let financial = g.Where(x => x.Year != null).OrderByDescending(x => x.Year).FirstOrDefault(x => x.MapType == "financial")
                                let ordates = g.Where(x => x.MapType == "ordate").ToArray()
                                select new
                                {
                                    Id = g.Key,
                                    Name = model.Name ?? null,
                                    MapType = (string)null,
                                    Year = financial.Year ?? null,
                                    Temp = (string)null,
                                    _ = new string[] { "Created", "Cancelled" }
                                            .Select(x => CreateField(x, model.Temp[x] ?? null, true, true))
                                    .Concat(new string[] { "Assets", "Analysis" }
                                        .Select(x => CreateField(x, financial.Temp[x] ?? null, true, true)))
                                    .Concat(new string[] { "Subjects" }
                                        .Select(x => CreateField(x, history.Temp[x] ?? null, true, true)))
                                    .Concat(new string[] { "RelatedDate" }
                                        .Select(x => CreateField(x, ordates.Select(o => o.Temp[x]) ?? null, true, true)))
                                };
        }
    }

    public class DynamicModelResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MapType { get; set; }
        public int? Year { get; set; }
        public IDictionary<string, object> Temp { get; set; }
    }

    public class DynamicModelProjection
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Cancelled { get; set; }
        public string[] Subjects { get; set; }

        public double? Assets { get; set; }
        public double? Analysis { get; set; }
        public DateTime[] RelatedDate { get; set; }
    }
}
