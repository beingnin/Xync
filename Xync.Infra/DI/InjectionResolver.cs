using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts.Core;
using Xync.Core;

namespace Xync.Infra.DI
{
    public enum ImplementationType
    {
        CDC=0, PureTriggers=1
    }
    public static class InjectionResolver
    {
        private static IDictionary<string, Type[]> _implementations = new Dictionary<string, Type[]>
        {
            {
                typeof(ISynchronizer).FullName,
                new Type[2]
                {
                    typeof(SqlServerToMongoSynchronizer),
                    typeof(SqlServerToMongoSynchronizerWithTriggers)
                }
            },
            {
                typeof(ISetup).FullName,
                new Type[2]
                {
                    typeof(Setup),
                    typeof(SetupWithTriggers)
                }
            },
        };
        public static T Resolve<T>(ImplementationType type)
        {
            if (_implementations.TryGetValue(typeof(T).FullName,out Type[] cls))
            {
                return (T)Activator.CreateInstance(cls[(int)type]);
            }
            return default(T);
        }
    }
}
