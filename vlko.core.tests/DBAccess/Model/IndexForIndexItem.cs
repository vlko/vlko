using Raven.Client.Documents.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vlko.core.tests.DBAccess.Model
{
    public class IndexForIndexItem : AbstractIndexCreationTask<IndexItem, ProjectionItem>
    {
        public IndexForIndexItem()
        {
            Map = items => from item in items
                           select new {
                               item.Id,
                               item.Text,
                               item.Integer,
                               item.Date,
                               item.Value,
                               item.NullDate,
                               item.NullValue,
                               Computed = item.Id + "|" + item.Text
                           };
            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.Text, FieldStorage.Yes);
            Store(x => x.Integer, FieldStorage.Yes);
            Store(x => x.Date, FieldStorage.Yes);
            Store(x => x.Value, FieldStorage.Yes);
            Store(x => x.NullDate, FieldStorage.Yes);
            Store(x => x.NullValue, FieldStorage.Yes);
            Store(x => x.Computed, FieldStorage.Yes);
        }
    }
}
