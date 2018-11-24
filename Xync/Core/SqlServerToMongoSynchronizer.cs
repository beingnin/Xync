using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Abstracts.Core;

namespace Xync.Core
{
    public class SqlServerToMongoSynchronizer<TMongoModel> : ISynchronizer<TMongoModel> where TMongoModel : class
    {
        public SqlServerToMongoSynchronizer()
        {

        }

        private TMongoModel _docModel = null;
        public TMongoModel DocumentModel
        {
            get
            {
                return _docModel;
            }
            set
            {
                _docModel = value;
            }
        }

        public TMongoModel CreateDocumentModel(IRelationalTable<TMongoModel> table)
        {
            throw new NotImplementedException();
        }

        public TMongoModel GetFromDocumentDb<Tkey, Tvalue>(Tkey key, Tvalue value)
        {
            throw new NotImplementedException();
        }
    }
}
