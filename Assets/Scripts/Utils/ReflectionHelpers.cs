using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ReflectionHelpers
{
    public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static void InitFields<TBase>(this object self, object[] args, BindingFlags flags = DefaultBindingFlags)
    {
        foreach(var f in self.GetType().GetFieldsOfType<TBase>(flags))
                f.SetValue(self, Activator.CreateInstance(f.FieldType, args));
    }

    public static IEnumerable<FieldInfo> GetFieldsOfType<TBase>(this Type self, BindingFlags flags = DefaultBindingFlags)
        => self.GetFields(flags).Where(f => typeof(TBase).IsAssignableFrom(f.FieldType));
}
