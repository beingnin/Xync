using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Web.MDO.EGATE;
using Xync.Mongo;
using Xync.SqlServer;
namespace Xync.Web.Mappings
{
    public class CaseManagement
    {
        public static IRelationalTable<Folder> Folders = new SqlServerTable<Folder>
        {
            
            Schema = "CM",
            Name = "Folders",
            Attributes = new List<IRelationalAttribute>
            {
                new SqlServerColumn()
                {
                    DbType=typeof(int),
                    Name="FolderId",
                    Key=true,
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                DbType=typeof(int),
                                Name="Id"
                            }
                        }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(int),
                    Name="ParentId"
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="FolderName",
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                DbType=typeof(string),
                                Name="Name"
                            }
                        },
                       new Map
                       {
                           DocumentProperty=new MongoDocumentProperty
                           {
                               DbType=typeof(string),
                               Name="QualifiedName",
                           },
                           ManipulateByRow=(item)=>{
                               var table=(SqlServerTable<Folder>)item;
                               return ((SqlServerColumn)table["FolderId"]).Value+":"+((SqlServerColumn)table["FolderName"]).Value;
                           },
                           //ManipulateByValue=(val)=>$"[{val}]"
                       }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(long),
                    Name="CaseId"
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="FolderPath"
                }
            }
        };
        public static IRelationalTable<Case> Cases = new SqlServerTable<Case>
        {
            Name = "Cases",
            Collection="CM_Cases",
            Schema = "CM",
            Attributes = new List<IRelationalAttribute>
            {
                new SqlServerColumn
                {
                    Name="CaseId",
                    Key=true,
                    DbType=typeof(long),
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="CaseId",
                                DbType=typeof(long),
                            }
                        },
                         new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Id",
                                DbType=typeof(long),
                            },

                        }
                    }
                },
               new SqlServerColumn
               {
                   Name="ProcessId",
                   DbType=typeof(Guid),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="ProcessId",
                           DbType=typeof(Guid),
                       }
                      }
                   }
               },
               new SqlServerColumn
               {
                   Name="CaseType",
                   DbType=typeof(Int16),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Type.Id",
                           DbType=typeof(Int16),
                       }
                      }
                   }
               },
               new SqlServerColumn
               {
                   Name="CaseName",
                   DbType=typeof(Int16),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="CaseName",
                           DbType=typeof(string),
                       }
                      }
                   }
               },
               new SqlServerColumn
               {
                   Name="CaseNumber",
                   DbType=typeof(string),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Number",
                           DbType=typeof(string),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="CreatedDateUTC",
                   DbType=typeof(DateTime),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="CreatedUTC",
                           DbType=typeof(DateTime),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="DueDate",
                   DbType=typeof(DateTime),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="DueDate",
                           DbType=typeof(DateTime),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="Status",
                   DbType=typeof(DateTime),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Status",
                           DbType=typeof(DateTime),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="Description",
                   DbType=typeof(string),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Description",
                           DbType=typeof(string),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="Priority",
                   DbType=typeof(Int16),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Priority",
                           DbType=typeof(Int16),
                       }
                      }
                   }
               },
                new SqlServerColumn
               {
                   Name="ParentID",
                   DbType=typeof(long),
                   Maps=new List<Map>
                   {
                      new Map
                      {
                        DocumentProperty= new MongoDocumentProperty
                       {
                           Name="Parent.CaseId",
                           DbType=typeof(long),
                       }
                      }
                   }
               }
            }
        };
        public static IRelationalTable<object> Police = new SqlServerTable<object>
        {
            Schema = "HRMS",
            Name = "mstUser",
            Collection = "Police",

            Attributes = new List<IRelationalAttribute>
            {
                new SqlServerColumn
                {
                    Name="UserID",
                    DbType=typeof(int),
                    Key=true,
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="_id"

                            },
                            ManipulateByValue=x=>Convert.ToString(x)
                        },
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="PoliceId"

                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="UserGUID",
                    DbType=typeof(Guid),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="UserGuid",
                                DbType=typeof(Guid)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="UserName",
                    DbType=typeof(string),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="MilitaryNumber",
                                DbType=typeof(string)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="EmployeeID",
                    DbType=typeof(int),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Employee.EmployeeID",
                                DbType=typeof(int)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="Password",
                    DbType=typeof(string),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Password",
                                DbType=typeof(string)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="ActiveFrom",
                    DbType=typeof(DateTime),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="ActiveFrom",
                                DbType=typeof(DateTime)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="ActiveTo",
                    DbType=typeof(DateTime),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="ActiveTo",
                                DbType=typeof(DateTime)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="Active",
                    DbType=typeof(bool),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Active",
                                DbType=typeof(bool)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="CreatedBy",
                    DbType=typeof(int),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="CreatedBy.PoliceId",
                                DbType=typeof(int)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="CreatedOn",
                    DbType=typeof(DateTime),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="CreatedOn",
                                DbType=typeof(DateTime)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="ModifiedBy",
                    DbType=typeof(int),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="ModifiedBy.PoliceId",
                                DbType=typeof(int)
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="ModifiedOn",
                    DbType=typeof(DateTime),
                    Maps= new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="ModifiedOn",
                                DbType=typeof(DateTime)
                            }
                        }
                    }
                }
            }

        };
    }
}