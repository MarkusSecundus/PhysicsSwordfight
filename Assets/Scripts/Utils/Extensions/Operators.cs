using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.Utils
{
    public static class Op
    {
        public static T post_assign<T>(ref T variable, T newValue)
        {
            var ret = variable;
            variable = newValue;
            return ret;
        }
    }

}
