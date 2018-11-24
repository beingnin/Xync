using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Core;
using Xync.Mongo;
using System.Runtime.Serialization;
using Xync.SqlServer;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Xync
{
    class Program
    {
        static void Main(string[] args)
        {
            #region
            IRelationalTable<EmployeesMDO> employees = new SqlServerTable<EmployeesMDO>()
            {
                DB = "Xynx",
                Name = "Employees",
                Attributes = new List<IRelationalAttribute>()
                {
                     new SqlServerColumn()
                    {
                        DbType=typeof(long),
                        Name="BranchId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(long),
                                    Name="Department.Branch.BranchId",
                                    Key="Department.Branch.BranchId"
                                }
                            }
                        }
                    },
                                        new SqlServerColumn()
                    {
                        DbType=typeof(long),
                        Name="DepId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(long),
                                    Name="Department.DepId",
                                    Key="Department.DepId"
                                }
                            }
                        }
                    },
                                       
                    new SqlServerColumn()
                    {
                        DbType=typeof(long),
                        Name="EmpId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new    MongoDocumentProperty()
                                {
                                    DbType=typeof(long),
                                    Name="EmpId",
                                    Key="EmpId"
                                }
                            }
                        }
                    },

                    new SqlServerColumn()
                    {
                        DbType=typeof(string),
                        Name="FirstName",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                    Name="Name",
                                    Key="Name"
                                },
                                ManipulateByRow=x=>
                                {

                                   var tbl= (SqlServerTable<EmployeesMDO>) x;
                                    return tbl["FirstName"].Value+" "+tbl["LastName"].Value;
                                    }
                            },
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                    Name="FirstName",
                                    Key="FirstName"
                                }

                            }
                        }
                    },
                    new SqlServerColumn()
                    {
                        DbType=typeof(string),
                        Name="LastName",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                    Name="Name",
                                    Key="Name"
                                },
                                ManipulateByRow=x=>
                                {

                                   var tbl= (SqlServerTable<EmployeesMDO>) x;
                                    return tbl["FirstName"].Value+" "+tbl["LastName"].Value;
                                    }
                            },
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                    Name="LastName",
                                    Key="LastName"
                                }

                            }
                        }
                    },
                    new SqlServerColumn()
                    {
                        DbType=typeof(DateTime),
                        Name="DOB",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new    MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                    Name="DOBString",
                                    Key="DOBString"
                                },
                                ManipulateByValue=x=>(((DateTime)x).AddYears(1)).ToShortDateString(),

                            },
                            new Map()
                            {
                                DocumentProperty=new    MongoDocumentProperty()
                                {
                                    DbType=typeof(DateTime),
                                    Name="DOB",
                                    Key="DOB"
                                },
                                ManipulateByValue=x=>(((DateTime)x).AddYears(1)),

                            }
                        }
                    },
                    new SqlServerColumn()
                    {
                        DbType=typeof(short),
                        Name="DesignationId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(short),
                                    Name="Designation",
                                    Key="Designation"
                                }
                            }
                        }
                    }
                },

            };

            #endregion

            var serializer = new JavaScriptSerializer();
            employees.Listen();
            Console.WriteLine(serializer.Serialize(employees.DocumentModel));

            var xmlserializer = new XmlSerializer(typeof(SqlServerTable<EmployeesMDO>));
            XmlDocument xmlDoc = new XmlDocument();
            using (MemoryStream str = new MemoryStream())
            {
                xmlserializer.Serialize(str, employees);
                str.Position = 0;
                xmlDoc.Load(str);
                xmlDoc.Save(@"C:\Users\Public\mdo.xml");
            }

            Console.ReadKey();







        }
    }

    [XmlRoot]
    public class EmployeesMDO
    {
        [XmlAttribute]
        public long EmpId { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string LastName { get; set; }
        [XmlAttribute]
        public string FirstName { get; set; }
        [XmlAttribute]
        public DateTime DOB { get; set; }
        [XmlAttribute]
        public string DOBString { get; set; }
        [XmlAttribute]
        public short Designation { get; set; }
        public Department Department { get; set; }
    }
    public class Department
    {
        public int DepId { get; set; }
        public string DepName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Branch Branch { get; set; }

    }
    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
    }
}
