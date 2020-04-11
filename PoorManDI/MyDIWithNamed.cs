using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoorManDI
{
    class MyDIWithNamed
    {
        private volatile object _syncObj = new object();
        readonly Dictionary<Type, Dictionary<string, Type>> _maps = new Dictionary<Type, Dictionary<string, Type>>();
        public void Register<TAbstractType, TConcreteType>(string name="")
        {
            var concreteType = typeof(TConcreteType);
            var abstractType = typeof(TAbstractType);
            var hasNamedDictTypeMapping = _maps.TryGetValue(abstractType, out var nameDict);
            if (!hasNamedDictTypeMapping)
            {
                lock(_syncObj) // 執行續安全性鎖定
                {
                    if (!hasNamedDictTypeMapping)  // 鎖定後的二次檢查，詳細請參閱 Lazy Pattern
                    {
                        nameDict = new Dictionary<string, Type>();
                        _maps[typeof(TAbstractType)] = nameDict;
                    }
                }
            }

            nameDict[name] = concreteType;

        }

        // 解析繼承關係
        private Type ResolveRegistryType(Type abstractType, string name)
        {
            var hasRegistried = _maps.TryGetValue(abstractType, out var fooConcreteTypeDict);

            if (hasRegistried)
            {
                return fooConcreteTypeDict[name];
            }
            else
            {
                if (abstractType.BaseType != null)
                {
                    return ResolveRegistryType(abstractType.BaseType, name);
                }
                else
                {
                    return null;
                }
            }
        }

        public object Resolve(Type type, string name)
        {
            var fooConcreteType = ResolveRegistryType(type, name);
            if (fooConcreteType == null)
            {
                return null;
            }

            var fooTypeConstructorInfos = fooConcreteType.GetConstructors();

            var paramMaxLength = fooTypeConstructorInfos.Max(f => f.GetParameters().Length);

            var someOfConstructor = fooTypeConstructorInfos
                                    // 建構式篩選條件
                                    // 這邊採用 取建構式參數最多的做舉例
                                    .Where(x => x.GetParameters().Length == paramMaxLength)
                                    .FirstOrDefault();
            var paramInfos = someOfConstructor.GetParameters();
            var paramterInstances = paramInfos
                                    .Select(p => {
                                        var paramInstance = Resolve(p.ParameterType, name);
                                        return paramInstance ?? p.DefaultValue; // 支援預設參數的建構式
                                    }).ToArray(); // 這邊有使用到 遞迴
            object instance = Activator.CreateInstance(fooConcreteType, paramterInstances);
            return instance;
        }

        public TAbstractType Resolve<TAbstractType>(string name = "")
        {
            return (TAbstractType)Resolve(typeof(TAbstractType), name);
        }
    }
}
