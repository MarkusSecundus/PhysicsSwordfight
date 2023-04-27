using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MarkusSecundus.PhysicsSwordfight.Utils.Extensions
{
    public static class GameObjectHelpers
    {
        public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, float delay)
        {
            self.StartCoroutine(impl());

            IEnumerator<YieldInstruction> impl()
            {
                yield return new WaitForSeconds(delay);
                toPerform();
            }
        }


        public static GameObject InstantiateWithTransform(this GameObject o, bool copyPosition = true, bool copyRotation = true, bool copyScale = true, bool copyParent = true)
        {
            var ret = GameObject.Instantiate(o);

            if (copyPosition) ret.transform.position = o.transform.position;
            if (copyRotation) ret.transform.rotation = o.transform.rotation;
            if (copyScale) ret.transform.localScale = o.transform.localScale;
            if (copyParent) ret.transform.parent = o.transform.parent;
            ret.SetActive(true);

            return ret;
        }

        public static GameObject FindTagInAncestors(this GameObject o, string tag)
        {
            for (; o != null; o = o?.transform?.parent?.gameObject)
                if (o.CompareTag(tag))
                    return o;
            return null;
        }

        //IsChildOf is a terrible name for what it is actually checking
        public static bool IsDescendantOf(this Transform descendant, Transform parent) => descendant.IsChildOf(parent);

        private static readonly string UtilGameObjectRootTag = "UtilGameObjectRoot";
        public static Transform GetUtilObjectParent()
        {
            var ret = GameObject.FindWithTag(UtilGameObjectRootTag);
            if (ret == null)
            {
                ret = new GameObject("UtilityObjects");
                ret.tag = UtilGameObjectRootTag;
            }
            return ret.transform;
        }


        public static GameObject InstantiateUtilObject(string name, params System.Type[] componentsToAdd)
            => GetUtilObjectParent().CreateChild(name, componentsToAdd);

        public static T GetUtilComponent<T>() where T : UnityEngine.Component
        {
            var parent = GetUtilObjectParent();
            if (parent.GetComponentInChildren<T>() is not null and var ret)
                return ret;
            else
                return parent.CreateChild(typeof(T).Name).AddComponent<T>();
        }

        public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, object toYield)
        {
            self.StartCoroutine(impl());
            IEnumerator impl()
            {
                yield return toYield;
                toPerform();
            }
        }

        public static GameObject CreateChild(this Transform father, string name, params System.Type[] componentsToAdd)
        {
            var ret = new GameObject(name, componentsToAdd);
            ret.transform.SetParent(father);
            ret.transform.ResetLocalPositionRotationScale();
            return ret;
        }

        public static void ResetLocalPositionRotationScale(this Transform t)
        {
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        public static bool IsOrHasAncestor(this Transform self, Transform ancestor)
        {
            if (self == ancestor) return true;
            for (; self != null; self = self.parent) if (self == ancestor) return true;
            return false;
        }

        public static T IfNil<T>(this T self, T defaultValue) => self.IsNil() ? defaultValue : self;
        public static bool IsNotNil(this object self) => !self.IsNil();
        public static bool IsNil(this object self) => self == null || self.Equals(null);


        public struct IndirectMessage
        {
            static readonly Regex Format = new Regex(@"^(?<Callee>[a-zA-Z][a-zA-Z0-9]*)\.(?<MessageName>[a-zA-Z][a-zA-Z0-9]*)$");

            public string CalleeName { get; init; }
            public string Message { get; init; }

            public object Invoke(object callee, System.Action<string> logError = null)
            {
                var methodToInvoke = callee.GetType().GetMethod(Message);
                if (methodToInvoke == null)
                    logError?.Invoke($"Called message '{Message}' does not exist on object '{callee}'({CalleeName})");
                else
                    return methodToInvoke.Invoke(callee, System.Array.Empty<object>());
                return null;
            }
            public static IndirectMessage? Make(string calleeAndMessage, System.Action<string> logError = null)
            {
                var parsed = Format.Match(calleeAndMessage);
                if (!parsed.Success)
                {
                    logError?.Invoke($"Invalid message declaration: '{calleeAndMessage}'");
                    return null;
                }
                return new IndirectMessage
                {
                    CalleeName = parsed.Groups["Callee"].Value,
                    Message = parsed.Groups["MessageName"].Value,
                };
            }
        }
    }

}