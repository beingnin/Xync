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
    }
}