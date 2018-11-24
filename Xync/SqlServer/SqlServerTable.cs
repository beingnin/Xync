using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xync.Abstracts;
using System.Xml.Serialization;
using Xync.Utils;

namespace Xync.SqlServer
{
    [XmlRoot]
    public class SqlServerTable<TDocumentModel> : IRelationalTable<TDocumentModel>
    {
        [XmlIgnore]
        private TDocumentModel _docModel = default(TDocumentModel);
        [XmlIgnore]
        public TDocumentModel DocumentModel
        {
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
            string[] columns = new string[7] { "DepId", "BranchId", "empId", "DOB", "FirstName", "LastName", "LocationId",};
            object[] values = new object[7] { 33, 99 ,36, DateTime.Now, "Nithin", "Chandran",25,};

            //fetch collection if any
            _docModel = GetFromMongo(null);
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
            TDocumentModel model = this._docModel;
            foreach (var attr in tbl.Attributes)
            {
                if (attr.hasChange)
                {
                    foreach (Map map in attr.Maps)
                    {
                        string[] keys = map.DocumentProperty.Key.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        int pendingAttr = keys.Length;
                        StringBuilder concatanatedProp = new StringBuilder(string.Empty);
                        for (int i = 0; i < keys.Length; i++)
                        {
                            string k = keys[i];
                            if (i != 0)
                            {
                                concatanatedProp.Append(".").Append(k);
                            }
                            else
                            {
                                concatanatedProp.Append(k);

                            }
                            if (pendingAttr > 1)
                            {

                                //Type propType = this.DocumentModelType.GetProperty(concatanatedProp.ToString()).PropertyType;
                                Type propType = model.GetNestedType(concatanatedProp.ToString());
                                //var currentValue = this.DocumentModelType.GetProperty(concatanatedProp.ToString()).GetValue(model);

                                object propInstance = model.GetNestedValue(concatanatedProp.ToString());
                                if (propInstance == null)
                                {
                                    propInstance = Activator.CreateInstance(propType);

                                }
                                //model.GetNestedProperty(concatanatedProp.ToString()).SetValue(model,propInstance);


                                model.SetNestedValue(concatanatedProp.ToString(), propInstance);
                                //this.DocumentModelType.GetProperty(concatanatedProp.ToString()).SetValue(model, propInstance);
                            }
                            else
                            {
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
                                model.SetNestedValue(concatanatedProp.ToString(),newMappedValue);
                                //this.DocumentModelType.GetProperty(concatanatedProp.ToString()).SetValue(model, newMappedValue);
                            }
                            --pendingAttr;
                        }

                        //old logic
                        //string key = map.DocumentProperty.Key;
                        //object newMappedValue = attr.Value;
                        ////run if any logic for manipulating result by value
                        //if (map.ManipulateByValue != null)
                        //{
                        //    newMappedValue = map.ManipulateByValue(attr.Value);
                        //}
                        ////run if any logic for manipulating result by row
                        //if (map.ManipulateByRow != null)
                        //{
                        //    newMappedValue = map.ManipulateByRow(this);
                        //}
                        //model.GetType().GetProperty(key).SetValue(model, newMappedValue);
                    }
                }
                attr.hasChange = false;
            }
            return model;
        }
        public TDocumentModel GetFromMongo(object identifier)
        {
            //get doc from collection

            var ser =new System.Web.Script.Serialization.JavaScriptSerializer();
            TDocumentModel doc = ser.Deserialize<TDocumentModel>("{\"Department\":{\"Branch\":{\"BranchId\":0,\"BranchName\":null,\"Location\":{\"LocationId\":0,\"Name\":\"America\"}},\"DepId\":33,\"DepName\":null,\"CreatedDate\":\"\\/Date(-62135596800000)\\/\"},\"EmpId\":25,\"Name\":\"Nithin Chandran\",\"FirstName\":\"Nithin\",\"LastName\":\"Chandran\",\"DOBString\":\"24-11-2019\",\"DOB\":\"\\/Date(1574611014553)\\/\",\"Designation\":0}");
            return doc;
        }


    }
}
