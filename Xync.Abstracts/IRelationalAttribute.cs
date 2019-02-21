using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface IRelationalAttribute
    {
        long ObjectId { get; set; }
        string Name { get; set; }
        object Value { get; set; }
        bool Key { get; set; }
        bool HasChange { get; set; }
        Type DbType { get; set; }
        List<Map> Maps { get; set; }
    }
    public enum RelationType
    {
        OneToOne=0,OneToMany=1,ManyToMany=2
    }
}
