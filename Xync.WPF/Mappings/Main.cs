using System;
using System.Collections.Generic;
using System.Text;
using Xync.Mongo;
using Xync.SqlServer;
using Xync.Abstracts;
using Xync.WPF.POCOs;

namespace Xync.WPF.Mappings
{
    public class Main
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
        public static IRelationalTable<Document> Documents = new SqlServerTable<Document>
        {
            Schema = "CM",
            Name = "Attachments",
            Attributes = new List<IRelationalAttribute>
            {
                new SqlServerColumn()
                {
                    DbType=typeof(int),
                    Name="AttachmentID",
                    Key=true,
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Id",
                                DbType=typeof(int),
                            }

                        }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(long),
                    Name="CaseID",
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Case.CaseId",
                            }
                        }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(bool),
                    Name="IsUserDefined",
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="AttachmentPath",
                },
                new SqlServerColumn()
                {
                    DbType=typeof(int),
                    Name="DocumentTypeId",
                },
                new SqlServerColumn()
                {
                    DbType=typeof(bool),
                    Name="IsOutput",
                },
                new SqlServerColumn()
                {
                    DbType=typeof(int),
                    Name="FolderID",
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="Descriptions",
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="Description",
                            }
                        }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="AttachmentName",
                     Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="FileName",
                            }
                        }
                    }
                },
                new SqlServerColumn()
                {
                    DbType=typeof(string),
                    Name="Extension",
                }
            }
        };
    }
}
