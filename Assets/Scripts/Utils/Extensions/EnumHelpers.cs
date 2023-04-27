using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.Utils
{
    public static class EnumHelpers
    {
        public static TEnum Parse<TEnum>(string name) => (TEnum)System.Enum.Parse(typeof(TEnum), name);

        public static TEnum[] GetValues<TEnum>() where TEnum : System.Enum => (TEnum[])System.Enum.GetValues(typeof(TEnum));
    }

}
