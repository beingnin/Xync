﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.MDO;
using Xync.Web.MDO.EGATE;
using Xync.Mongo;
using Xync.SqlServer;

namespace Xync.Web.Mappings
{
    public static class Main
    {
        #region employee-mapping
        public static IRelationalTable<EmployeesMDO> employees = new SqlServerTable<EmployeesMDO>()
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
                                }
                            }
                        }
                    },
                      new SqlServerColumn()
                    {
                        DbType=typeof(long),
                        Name="LocationId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(long),
                                    Name="Department.Branch.Location.LocationId",
                                }
                            }
                        }
                    },
                      new SqlServerColumn()
                    {
                        DbType=typeof(long),
                        Name="Name",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(long),
                                    Name="Department.Branch.Location.Name",
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
                                },
                                ManipulateByValue=x=>(((DateTime)x).AddYears(1)).ToShortDateString(),

                            },
                            new Map()
                            {
                                DocumentProperty=new    MongoDocumentProperty()
                                {
                                    DbType=typeof(DateTime),
                                    Name="DOB",
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
                                }
                            }
                        }
                    }
                },

        };
        #endregion employee-mapping

        #region product-mapping
        public static IRelationalTable<Product> Products = new SqlServerTable<Product>
        {
            DB = "Xynx",
            Name = "Employees",
            Attributes = new List<IRelationalAttribute>
            {
                new SqlServerColumn()
                    {
                        DbType=typeof(int),
                        Name="ProductId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(int),
                                }
                            }
                        }
                    },
                                new SqlServerColumn()
                    {
                        DbType=typeof(string),
                        Name="ProductName",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                }
                            }
                        }
                    },
                                                new SqlServerColumn()
                    {
                        DbType=typeof(decimal),
                        Name="Price",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(decimal),
                                }
                            }
                        }
                    },
                                                                new SqlServerColumn()
                    {
                        DbType=typeof(int),
                        Name="CustomerId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(int),
                                }
                            }
                        }
                    },
                                                                                new SqlServerColumn()
                    {
                        DbType=typeof(string),
                        Name="CustomerName",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(string),
                                }
                            }
                        }
                    },
                                                                                                new SqlServerColumn()
                    {
                        DbType=typeof(int),
                        Name="TaxId",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(int),
                                }
                            }
                        }
                    },
                                                                                                                new SqlServerColumn()
                    {
                        DbType=typeof(decimal),
                        Name="TaxRate",
                        Maps=new List<Map>()
                        {
                            new Map()
                            {
                                DocumentProperty=new MongoDocumentProperty()
                                {
                                    DbType=typeof(decimal),
                                }
                            }
                        }
                    },
            }
        };
        #endregion product-mapping
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
            Name="Attachments",
            Attributes=new List<IRelationalAttribute>
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
