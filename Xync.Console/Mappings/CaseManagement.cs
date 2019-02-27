using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Console.MDO.EGATE;
using Xync.Mongo;
using Xync.SqlServer;
namespace Xync.Console.Mappings
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
                                Key="Id",
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
                                Key="Name",
                                Name="Name"
                            }
                        },
                       new Map
                       {
                           DocumentProperty=new MongoDocumentProperty
                           {
                               DbType=typeof(string),
                               Key="QualifiedName",
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
                                Key="CaseId"
                            }
                        }
                    }
                },
                new SqlServerColumn
                {
                    Name="CaseId",
                    DbType=typeof(long),
                    Maps=new List<Map>
                    {
                        new Map
                        {
                            DocumentProperty=new MongoDocumentProperty
                            {
                                Name="CaseId",
                                DbType=typeof(long),
                                Key="CaseId"
                            }
                        }
                    }
                }
            }
        };
    }
}