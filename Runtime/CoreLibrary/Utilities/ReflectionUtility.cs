using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public static class ReflectionUtility
    {
        #region Constants

        public  const   string  kCSharpFirstPassAssembly    = "Assembly-CSharp-firstpass";

        public  const   string  kCSharpAssembly             = "Assembly-CSharp";

        #endregion

        #region Static methods

        public static Type GetTypeFromCSharpAssembly(string typeName)
        {
            return GetType(kCSharpAssembly, typeName);
        }

        public static Type GetTypeFromCSharpFirstPassAssembly(string typeName)
        {
            return GetType(kCSharpFirstPassAssembly, typeName);
        }

        public static Type GetType(string assemblyName, string typeName)
        {
            var targetAssembly  = FindAssemblyWithName(assemblyName);
            if (targetAssembly != null)
            {
                return targetAssembly.GetType(typeName, false);
            }

            return null;
        }

        public static Assembly FindAssemblyWithName(string assemblyName)
        {
            return Array.Find(AppDomain.CurrentDomain.GetAssemblies(), (item) =>
            {
                return string.Equals(item.GetName().Name, assemblyName);
            });
        }

        public static T InvokeStaticMethod<T>(this Type type, string method, params object[] parameters)
        {
            return (T)type.GetMethod(method, BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);
        }

        #endregion

        #region Constraints methods

		// Credits: Thomas Hourdel
		public static string GetFieldName<T, U>(Expression<Func<T, U>> fieldAccess)
		{
			var		memberExpression	= fieldAccess.Body as MemberExpression;
			if (memberExpression != null)
			{
				return memberExpression.Member.Name;
			}
			throw new InvalidOperationException("Member expression expected");
		}

		public static FieldInfo GetField(Type type, string fieldName)
		{
			return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
		}

		public static A GetAttribute<A>(FieldInfo field) where A : Attribute
		{
			return (A)Attribute.GetCustomAttribute(field, typeof(A));
		}

		// MinAttribute
		public static void ConstrainMin<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, float value)
		{
			var		fieldName	= GetFieldName(fieldAccess);
			var		fieldInfo	= GetField(typeof(TInstance), fieldName);
			fieldInfo.SetValue(instance, Mathf.Max(value, GetAttribute<MinAttribute>(fieldInfo).min));
		}

		public static void ConstrainMin<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, int value)
		{
			var		fieldName	= GetFieldName(fieldAccess);
			var		fieldInfo	= GetField(typeof(TInstance), fieldName);
			fieldInfo.SetValue(instance, (int)Mathf.Max(value, GetAttribute<MinAttribute>(fieldInfo).min));
		}

		// RangeAttribute
		public static void ConstrainRange<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, float value)
		{
			var		fieldName	= GetFieldName(fieldAccess);
			var		fieldInfo	= GetField(typeof(TInstance), fieldName);
			var		attribute	= GetAttribute<RangeAttribute>(fieldInfo);
			fieldInfo.SetValue(instance, Mathf.Clamp(value, attribute.min, attribute.max));
		}

		public static void ConstrainRange<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, int value)
		{
			var		fieldName	= GetFieldName(fieldAccess);
			var		fieldInfo	= GetField(typeof(TInstance), fieldName);
			var		attribute	= GetAttribute<RangeAttribute>(fieldInfo);
			fieldInfo.SetValue(instance, (int)Mathf.Clamp(value, attribute.min, attribute.max));
		}

		public static void ConstrainDefault<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, Func<bool> condition = null)
		{
			if ((condition == null) || !condition()) return;

			var		fieldName	= GetFieldName(fieldAccess);
			var		fieldInfo	= GetField(typeof(TInstance), fieldName);
			var		attribute	= GetAttribute<DefaultValueAttribute>(fieldInfo);
			fieldInfo.SetValue(instance, attribute.GetValue(fieldInfo.FieldType));
		}

		public static void ConstrainDefault<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, string value)
		{
			if (!string.IsNullOrEmpty(value)) return;

			ConstrainDefault(instance, fieldAccess);
		}

        #endregion
    }
}