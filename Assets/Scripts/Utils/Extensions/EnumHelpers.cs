using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.Utils
{
    /// <summary>
    /// Static class containing convenience extensions methods for operating on <see cref="System.Enum"/>s
    /// </summary>
    public static class EnumHelpers
    {
        /// <summary>
        /// Parse enum value from string. Strongly typed version of <see cref="System.Enum.Parse(System.Type, string)"/>
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="name">Name of the enum value, to be parsed</param>
        /// <returns>Parsed value of <paramref name="name"/></returns>
        public static TEnum Parse<TEnum>(string name) => (TEnum)System.Enum.Parse(typeof(TEnum), name);

        /// <summary>
        /// Iterate through all declared values of an enum type.
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>Generateor that iterates through all values that are declared in given enum type</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : System.Enum => (TEnum[])System.Enum.GetValues(typeof(TEnum));
    }

}
