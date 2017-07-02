using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperExtensionsReloaded.Test.Helpers
{
    public class Protected
    {
        private readonly object _obj;

        public Protected(object obj)
        {
            _obj = obj ?? throw new ArgumentException("object cannot be null.", nameof(obj));
        }

        public static Expression IsNull<T>()
        {
            Expression<Func<Type>> expr = () => typeof(T);
            return expr.Body;
        }

        public void RunMethod(string name, params object[] parameters)
        {
            InvokeMethod(name, null, parameters);
        }

        public T RunMethod<T>(string name, params object[] parameters)
        {
            return (T)InvokeMethod(name, null, parameters);
        }

        public void RunGenericMethod(string name, Type[] genericTypes, params object[] parameters)
        {
            InvokeMethod(name, genericTypes, parameters);
        }

        public TResult RunGenericMethod<TResult>(string name, Type[] genericTypes, params object[] parameters)
        {
            return (TResult)InvokeMethod(name, genericTypes, parameters);
        }

        public object InvokeMethod(string name, Type[] genericTypes, object[] parameters)
        {
            var pa = parameters.Select(p =>
            {
                if (p is ConstantExpression)
                {
                    return null;
                }

                return p;
            }).ToArray();
            var method = GetMethod(name, parameters);
            try
            {
                if (genericTypes != null && genericTypes.Any())
                {
                    method = method.MakeGenericMethod(genericTypes);
                }

                return method.Invoke(_obj, pa);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public MethodInfo GetMethod(string name, object[] parameters)
        {
            var types = parameters.Select(p =>
            {
                if (p is ConstantExpression)
                {
                    return (Type)((ConstantExpression)p).Value;
                }

                return p.GetType();
            }).ToArray();


            var methods = _obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(x => x.Name == name)
                .Where(x => x.GetParameters().Length == parameters.Length)
                .ToList();

            // the following check probably doesn't work, but isn't required by any test anyway
            MethodInfo method = null;
            foreach (var m in methods)
            {
                var found = true;
                var arguments = m.GetParameters();
                for (var i = 0; i < types.Length; i++)
                {
                    if (!arguments[i].ParameterType.IsAssignableFrom(types[i]))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    method = m;
                    break;
                }
            }
            
            if (method == null)
            {
                throw new ArgumentException(string.Format("{0} was not found in {1}.", name, _obj.GetType()), name);
            }
            
            return method;
        }
    }
}