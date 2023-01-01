using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace JworkzNeosMod.Utility
{
    internal static class DelegateCreator
    {
        public static TDelegate CreateAccessor<TDelegate>(params string[] memberNames) where TDelegate : Delegate
        {
            var invokeMethod = typeof(TDelegate).GetMethod("Invoke");
            if (invokeMethod == null)
            {
                throw new InvalidOperationException($"{typeof(TDelegate)} signature could not be determined.");
            }

            var delegateParameters = invokeMethod.GetParameters();
            if (delegateParameters.Length != 1)
            {
                throw new InvalidOperationException("Delegate must have a single parameter.");
            }

            var paramType = delegateParameters[0].ParameterType;

            var objParam = Expression.Parameter(paramType, "obj");
            Expression memberExpr = objParam;
            foreach (var memberName in memberNames)
            {
                memberExpr = Expression.PropertyOrField(memberExpr, memberName);
            }
            Expression returnExpr = invokeMethod.ReturnType == memberExpr.Type
                ? memberExpr
                : Expression.ConvertChecked(memberExpr, invokeMethod.ReturnType);

            var lambda = Expression.Lambda<TDelegate>(returnExpr, $"Access{paramType.Name}_{memberNames.Join(delimiter: "_")}", new[] { objParam });
            return lambda.Compile();
        }
    }
}
