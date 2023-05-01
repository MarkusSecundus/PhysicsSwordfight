using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.Utils
{
    /// <summary>
    /// Provider of a pure function.
    /// 
    /// <para>
    /// To be implemented by structs used for implementing simple templated compile-time policy pattern ala C++. 
    /// (For examples see e.g. <see cref="MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy"/>)
    /// </para>
    /// </summary>
    public interface IFunc<TRet>
    {
        public TRet Invoke();
    }
    /// <summary>
    /// Provider of a pure function.
    /// 
    /// <para>
    /// To be implemented by structs used for implementing simple templated compile-time policy pattern ala C++. 
    /// (For examples see e.g. <see cref="MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy"/>)
    /// </para>
    /// </summary>
    public interface IFunc<T, TRet>
    {
        public TRet Invoke(T a);
    }
    /// <summary>
    /// Provider of a pure function.
    /// 
    /// <para>
    /// To be implemented by structs used for implementing simple templated compile-time policy pattern ala C++. 
    /// (For examples see e.g. <see cref="MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy"/>)
    /// </para>
    /// </summary>
    public interface IFunc<TA, TB, TRet>
    {
        public TRet Invoke(TA a, TB b);
    }
    /// <summary>
    /// Provider of a pure function.
    /// 
    /// <para>
    /// To be implemented by structs used for implementing simple templated compile-time policy pattern ala C++. 
    /// (For examples see e.g. <see cref="MarkusSecundus.PhysicsSwordfight.Utils.Interpolation.RetargetableInterpolator.VectorInterpolationPolicy"/>)
    /// </para>
    /// </summary>
    public interface IFunc<TA, TB, TC, TRet>
    {
        public TRet Invoke(TA a, TB b, TC c);
    }

}