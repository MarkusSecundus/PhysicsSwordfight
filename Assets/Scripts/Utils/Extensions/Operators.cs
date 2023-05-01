using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.Utils
{
    /// <summary>
    /// Static class containing convenience methods to supplement for operators that were not provided by the C# langugage
    /// </summary>
    public static class Op
    {
        /// <summary>
        /// Performs post-assign operation - assigns new value to a variable but returns the old value (simialar as standard post-increment/post-decrement operators).
        /// </summary>
        /// <typeparam name="T">Type of the value to be assigned</typeparam>
        /// <param name="variable">Variable to be assigned new value</param>
        /// <param name="newValue">Value to be assigned to the variable</param>
        /// <returns>Original value of <paramref name="variable"/></returns>
        public static T post_assign<T>(ref T variable, T newValue)
        {
            var ret = variable;
            variable = newValue;
            return ret;
        }
    }

}
