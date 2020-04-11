using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PoorManDI_MultiArgsConstructorSupports
{
    public class MyDI
    {
        readonly Dictionary<Type, Type> _maps = new Dictionary<Type, Type>();
        public  void Register<TAbstractType, TConcreteType>()
        {
            _maps[typeof(TAbstractType)] = typeof(TConcreteType);
        }

        // 解析繼承關係
        private Type ResolveRegistryType(Type type)
        {
            var hasRegistried = _maps.TryGetValue(type, out var fooConcreteType);

            if (hasRegistried)
            {
                return fooConcreteType;
            }
            else
            {
                if (type.BaseType != null)
                {
                    return ResolveRegistryType(type.BaseType);
                }
                else
                {
                    return null;
                }
            }
        }

        public  object Resolve(Type type)
        {
            var fooConcreteType = ResolveRegistryType(type);
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
                                        var paramInstance = Resolve(p.ParameterType);
                                        return paramInstance ?? p.DefaultValue; // 支援預設參數的建構式
                                     }).ToArray(); // 這邊有使用到 遞迴
            object instance = Activator.CreateInstance(fooConcreteType, paramterInstances);
            return instance;
        }

        public  TAbstractType Resolve<TAbstractType>()
        {
            return (TAbstractType)Resolve(typeof(TAbstractType));
        }
    }
}
