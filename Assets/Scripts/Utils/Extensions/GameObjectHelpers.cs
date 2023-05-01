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
    /// <summary>
    /// Static class containing convenience extensions methods for <see cref="GameObject"/> and related things.
    /// </summary>
    public static class GameObjectHelpers
    {
        /// <summary>
        /// Perform given action with given delay, defined as a coroutine yield value
        /// </summary>
        /// <param name="self">Component responsible for performing the action</param>
        /// <param name="toPerform">Action to perform</param>
        /// <param name="toYield">Yield-object defining the delay</param>
        public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, object toYield)
        {
            self.StartCoroutine(impl());
            IEnumerator impl()
            {
                yield return toYield;
                toPerform();
            }
        }
        /// <summary>
        /// Perform given action with given delay in seconds.
        /// </summary>
        /// <param name="self">Component responsible for performing the action</param>
        /// <param name="toPerform">Action to perform</param>
        /// <param name="delay">Delay in seconds until the action is performed</param>
        public static void PerformWithDelay(this MonoBehaviour self, System.Action toPerform, float delay)
            => self.PerformWithDelay(toPerform, new WaitForSeconds(delay));

        /// <summary>
        /// Create a new instance of given gameobject making sure its transform is preserved
        /// </summary>
        /// <param name="self">GameObject to clone</param>
        /// <param name="copyPosition">if <c>true</c>, make sure position is preserved</param>
        /// <param name="copyRotation">if <c>true</c>, make sure rotation is preserved</param>
        /// <param name="copyScale">if <c>true</c>, make sure localScale is preserved</param>
        /// <param name="copyParent">if <c>true</c>, make sure parent in transform hierarchy is preserved</param>
        /// <returns></returns>
        public static GameObject InstantiateWithTransform(this GameObject self, bool copyPosition = true, bool copyRotation = true, bool copyScale = true, bool copyParent = true)
        {
            var ret = GameObject.Instantiate(self);

            if (copyPosition) ret.transform.position = self.transform.position;
            if (copyRotation) ret.transform.rotation = self.transform.rotation;
            if (copyScale) ret.transform.localScale = self.transform.localScale;
            if (copyParent) ret.transform.parent = self.transform.parent;
            ret.SetActive(true);

            return ret;
        }


        /// <summary>
        /// Same as <see cref="Transform.IsChildOf(Transform)"/> but with name that actually describes what it does.
        /// </summary>
        /// <param name="descendant">Transform to be checked for being a member of the <paramref name="parent"/>s object hierarchy</param>
        /// <param name="parent">Transform to check for being an ancestor of <paramref name="descendant"/></param>
        /// <returns><c>true</c> IFF <paramref name="parent"/> is an ancestor if <paramref name="descendant"/></returns>
        public static bool IsDescendantOf(this Transform descendant, Transform parent) => descendant.IsChildOf(parent);

        private static readonly string UtilGameObjectRootTag = "UtilGameObjectRoot";
        /// <summary>
        /// Get root of utility objects section in the current scene
        /// </summary>
        /// <returns>Root of utility objects section in the current scene</returns>
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

        /// <summary>
        /// Create an object in the utility section of current scene
        /// </summary>
        /// <param name="name">Name for the instantiated object</param>
        /// <param name="componentsToAdd">Components to create on the object</param>
        /// <returns>The newly created utility object</returns>
        public static GameObject InstantiateUtilObject(string name, params System.Type[] componentsToAdd)
            => GetUtilObjectParent().CreateChild(name, componentsToAdd);

        /// <summary>
        /// Get a component in the utility section of current scene. Create one if it doesn't yet exist.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <returns>Found of newly created utility component of given type</returns>
        public static T GetUtilComponent<T>() where T : UnityEngine.Component
        {
            var parent = GetUtilObjectParent();
            if (parent.GetComponentInChildren<T>() is not null and var ret)
                return ret;
            else
                return parent.CreateChild(typeof(T).Name).AddComponent<T>();
        }

        /// <summary>
        /// Create a new object as child of another object
        /// </summary>
        /// <param name="father">Parent of the newly created object</param>
        /// <param name="name">Name for the newly created gameobject</param>
        /// <param name="componentsToAdd">Components the new gameobject should contain</param>
        /// <returns>Newly created gameobject</returns>
        public static GameObject CreateChild(this Transform father, string name, params System.Type[] componentsToAdd)
        {
            var ret = new GameObject(name, componentsToAdd);
            ret.transform.SetParent(father);
            ret.transform.ResetLocalPositionRotationScale();
            return ret;
        }
        /// <summary>
        /// Reset local position, scale and rotation of given transform to default values.
        /// </summary>
        /// <param name="t">Transform to reset</param>
        public static void ResetLocalPositionRotationScale(this Transform t)
        {
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Check if provided value is <c>null</c> - if it is, replace it with default value
        /// </summary>
        /// <typeparam name="T">Type of the values</typeparam>
        /// <param name="self">Value to be checked for <c>null</c></param>
        /// <param name="defaultValue">Value to be used if <paramref name="self"/> is <c>null</c></param>
        /// <returns><paramref name="defaultValue"/> if it is not <c>null</c>, otherwise <paramref name="defaultValue"/></returns>
        public static T IfNil<T>(this T self, T defaultValue) => self.IsNil() ? defaultValue : self;
        /// <summary>
        /// Check if given value is not <c>null</c>. Works for descendants of <see cref="UnityEngine.Object"/>.
        /// </summary>
        /// <param name="self">Object to be checked for <c>null</c></param>
        /// <returns><c>true</c> IFF given object is not <c>null</c></returns>
        public static bool IsNotNil(this object self) => !self.IsNil();
        /// <summary>
        /// Check if given value is <c>null</c>. Works for descendants of <see cref="UnityEngine.Object"/>.
        /// </summary>
        /// <param name="self">Object to be checked for <c>null</c></param>
        /// <returns><c>true</c> IFF given object is <c>null</c></returns>
        public static bool IsNil(this object self) => self == null || self.Equals(null);

        /// <summary>
        /// Utility for calling invoking methods indirectly on an object instance via reflection.
        /// 
        /// <para>
        /// Can parse simple message requests in format "CalleeName.MethodName"
        /// </para>
        /// </summary>
        public struct IndirectMessage
        {
            static readonly Regex Format = new Regex(@"^(?<Callee>[a-zA-Z][a-zA-Z0-9]*)\.(?<MessageName>[a-zA-Z][a-zA-Z0-9]*)$");

            /// <summary>
            /// Name of the object to be messaged
            /// </summary>
            public string CalleeName { get; set; }
            /// <summary>
            /// Name of the method to be invoked
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Argument to be passed as argument to the method. No argument will be passed if <c>null</c>
            /// </summary>
            public object Argument { get; set; }
            /// <summary>
            /// Call the message on provided object instance, which the user found by himself according to <see cref="CalleeName"/>.
            /// </summary>
            /// <param name="callee">Instance on which to invoke the method</param>
            /// <param name="logError">Callback for reporting errors</param>
            /// <returns>Result of the invocation</returns>
            public object Invoke(object callee, System.Action<string> logError = null)
            {
                var methodToInvoke = callee.GetType().GetMethod(Message);
                if (methodToInvoke == null)
                    logError?.Invoke($"Called message '{Message}' does not exist on object '{callee}'({CalleeName})");
                else if(Argument == null)
                    return methodToInvoke.Invoke(callee, System.Array.Empty<object>());
                else
                    return methodToInvoke.Invoke(callee, new object[] {Argument});
                return null;
            }
            /// <summary>
            /// Parse message from format "CalleeName.MethodName"
            /// </summary>
            /// <param name="calleeAndMessage">String describing the callee and method name - in format "CalleeName.MethodsName"</param>
            /// <param name="logError">Callback for reporting errors</param>
            /// <returns>Parsed incomplete message declaration</returns>
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