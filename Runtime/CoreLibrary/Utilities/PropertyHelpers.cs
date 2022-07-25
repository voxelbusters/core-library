using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public static class PropertyHelpers
    {
        #region Static methods

        public static string GetValueOrDefault(string value, string defaultValue = default)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;

            return value;
        }

        public static string GetValueOrDefault<TInstance, U>(TInstance instance, Expression<Func<TInstance, U>> fieldAccess, string value)
        {
            if (!string.IsNullOrEmpty(value)) return value;

            // find default value using reflection
            var		fieldName	= ReflectionUtility.GetFieldName(fieldAccess);
			var		fieldInfo	= ReflectionUtility.GetField(typeof(TInstance), fieldName);
			var		attribute	= ReflectionUtility.GetAttribute<DefaultValueAttribute>(fieldInfo);
            return attribute.StringValue;
        }

        #endregion
    }
}