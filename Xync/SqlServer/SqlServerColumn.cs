﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Helpers;

namespace Xync.SqlServer
{
    public class SqlServerColumn : IRelationalAttribute
    {
        public long ObjectId { get; set; }
        public bool Key { get; set; }
        public string Name { get; set; }
        public Type DbType { get; set; }
        public List<Map> Maps { get; set; }
        public object Value { get; set; }
        public bool HasChange { get; set; }
        public override string ToString()
        {
            return this.Name.Embrace();
        }
    }
}
