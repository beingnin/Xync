using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Desktop.MDO.EGATE;
using Xync.Mongo;
using Xync.SqlServer;

namespace Xync.Console.Mappings
{
    public static class Main
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
    }
}
