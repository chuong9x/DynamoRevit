﻿using System;
using Autodesk.Revit.DB;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using RevitServices.Transactions;
using System.Collections.Generic;

namespace Revit.Filter
{
    /// <summary>
    /// Revit Filter Rule
    /// </summary>
    public class FilterRule
    {

        #region Internal Properties

        /// <summary>
        /// Internal reference to the Revit Element
        /// </summary>
        internal Autodesk.Revit.DB.FilterRule InternalFilterRule
        { 
            get; set;
        }

        /// <summary>
        /// Reference to the Element
        /// </summary>
        /// <param name="internalFilterRule"></param>
        internal FilterRule(Autodesk.Revit.DB.FilterRule internalFilterRule)
        {
            this.InternalFilterRule = internalFilterRule;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// FilterType Enumeration
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public enum FilterType
        {
            BeginsWith,
            Contains,
            EndsWith,
            Equals,
            Greater,
            Less,
            GeaterOrEqual,
            LessOrEqual,
            NotBeginsWith,
            NotContains,
            NotEndsWith,
            NotEquals,
        }

        #endregion

        #region Public static constructors

        /// <summary>
        /// Prefix for reflection call of Create Rule
        /// </summary>
        const string CreatePrefix = "Create";

        /// <summary>
        /// Suffix for reflection call of Create Rule 
        /// </summary>
        const string RuleSuffix = "Rule";

        /// <summary>
        /// Create a new Filter Rule
        /// </summary>
        /// <param name="type">Filter Rule Type</param>
        /// <param name="value">Value to check</param>
        /// <param name="parameter">Parameter to filter</param>
        /// <returns></returns>
        public static FilterRule ByRuleType(string typename, object value, Elements.Parameter parameter)
        {

            FilterType type = FilterType.Contains;

            if (!Enum.TryParse<FilterType>(typename, out type))
            {
                throw new Exception(Properties.Resources.InvalidFilterType);
            }

            ElementId parameterId = parameter.InternalParameter.Id;

            // assemble the method name to construct a new filter using the FilterType
            string methodname = string.Format("{0}{1}{2}", new object[] { CreatePrefix , type, RuleSuffix});
    
            // all of the following FilterType construcotrs are handled the same
            if (
                type == FilterType.Equals ||
                type == FilterType.NotEquals ||
                type == FilterType.Greater ||
                type == FilterType.Less ||
                type == FilterType.GeaterOrEqual ||
                type == FilterType.LessOrEqual
                )
            {
                if (value.GetType() == typeof(int))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(int) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (int)value }));
                    }
                }
                else if (value.GetType() == typeof(double))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(double), typeof(double) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (double)value, 0 }));
                    }
                }
                else if (value.GetType() == typeof(string))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(string), typeof(bool) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (string)value, true }));
                    }
                }
            }
            else
            {
                if (value.GetType() == typeof(string))
                {
                    System.Reflection.MethodInfo methodInfo = typeof(ParameterFilterRuleFactory).GetMethod(methodname, new[] { typeof(ElementId), typeof(string), typeof(bool) });
                    if (methodInfo != null)
                    {
                        return new FilterRule((Autodesk.Revit.DB.FilterRule)methodInfo.Invoke(null, new object[] { parameterId, (string)value, true }));
                    }
                }
            }


            return null;
        }

        #endregion

    }



}
