using Raven.Client.Documents.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vlko.core.RavenDB.DBAccess;

namespace vlko.core.tests.DBAccess.Model
{
    public class TestTransformer : AbstractTransformer<IndexItem, TransformResult>
    {
        public TestTransformer()
        {
            TransformResult = data => from item in data
                                      select new TransformResult
                                      {
                                          Id = item.Id,
                                          Computed = "this is generated on fly",
                                          Composited = $"{item.Text}|{item.Integer}"
                                      };
        }
    }

    public class TestLoadTransformer : AbstractTransformer<IndexItem, TransformResult>
    {
        public TestLoadTransformer()
        {
            TransformResult = data => from item in data
                                      let document = RavenQuery.Load<SimpleItem>("transform-load")
                                      select new TransformResult
                                      {
                                          Id = item.Id,
                                          Computed = document.Text,
                                          Composited = $"{item.Text}|{item.Integer}"
                                      };
        }
    }

    public class TestParamTransformer : AbstractTransformer<IndexItem, TransformResult>
    {
        public TestParamTransformer()
        {
            PrepareDefaultParameters = parameters =>
            {
                SetValueIfNotExists(parameters, "ident", string.Empty);
            };
            TransformResultWithParameters = (data, parameters) =>
                    from item in data
                    let document = RavenQuery.Load<SimpleItem>(parameters["ident"])
                    select new TransformResult
                    {
                        Id = item.Id,
                        Computed = document.Text,
                        Composited = $"{item.Text}|{item.Integer}"
                    };
        }
    }

    public class TransformResult
    {
        public string Id { get; set; }
        public string Computed { get; set; }
        public string Composited { get; set; }
    }
}
