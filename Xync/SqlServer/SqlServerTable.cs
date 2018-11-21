using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xync.Abstracts;
using System.Xml.Serialization;

namespace Xync.SqlServer
{
    [XmlRoot]
    public class SqlServerTable<TDocumentModel> : IRelationalTable<TDocumentModel>
    {
        [XmlIgnore]
        private TDocumentModel _docModel = default(TDocumentModel);
        [XmlIgnore]
        public TDocumentModel DocumentModel {
            get
            {
                return _docModel;
            }
        }
        [XmlAttribute]
        public long ObjectId { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; } = "[DBO]";
        public string DB { get; set; }
        [XmlIgnore]
        public List<IRelationalAttribute> Attributes { get; set; }
        public IRelationalAttribute this[string col]
        {
            get
            {
                if (this.Attributes != null && this.Attributes.Count != 0)
                {
                   return this.Attributes.Where(x => x.Name.Equals(col, StringComparison.OrdinalIgnoreCase)).First();
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

        }
        public Type DocumentModelType
        {
            get
            {
                return typeof(TDocumentModel);
            }
        }
        public void Listen()
        {
            //Register things for getting changes from DB
            Console.WriteLine($"Listening for changes in {Schema}.{Name}");
            Console.WriteLine($"Change detected in {Schema}.{Name}");
            RowChanged();
        }
        public event RowChangedEventHandler OnRowChange;
        public void RowChanged()
        {
            //fetching the changes
            string[] columns = new string[4] { "empId", "DOB","FirstName","LastName" };
            object[] values = new object[4] { 25, DateTime.Now,"Nithin","Chandran" };

            //fetch collection if any
            _docModel = default(TDocumentModel);//get current data from mongo db here
            //set current values in attributes

            for (int i = 0; i < columns.Length; i++)
            {
                IRelationalAttribute attr = this[columns[i]];
                attr.Value = values[i];
                attr.hasChange = true;
            }

            //creating object


            _docModel = this.CreateModel(this);//set newly created, processed model here


            //Trigger user defined events if any
            if (OnRowChange != null)
            {
                OnRowChange(this, new EventArgs());
            }
        }
        public TDocumentModel CreateModel(IRelationalTable<TDocumentModel> tbl)
        {
            TDocumentModel model = (TDocumentModel)Activator.CreateInstance(this.DocumentModelType);
            foreach (var attr in tbl.Attributes)
            {
                if (attr.hasChange)
                {
                    foreach (Map map in attr.Maps)
                    {
                        string key = map.DocumentProperty.Key;
                        object newMappedValue = attr.Value;
                        //run if any logic for manipulating result by value
                        if (map.ManipulateByValue != null)
                        {
                            newMappedValue = map.ManipulateByValue(attr.Value);
                        }
                        //run if any logic for manipulating result by row
                        if (map.ManipulateByRow != null)
                        {
                            newMappedValue = map.ManipulateByRow(this);
                        }
                        model.GetType().GetProperty(key).SetValue(model, newMappedValue);
                    }
                }
                attr.hasChange=false;
            }
            return model;
        }


    }
}
