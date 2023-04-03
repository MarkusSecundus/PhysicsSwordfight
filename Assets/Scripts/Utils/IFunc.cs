using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFunc<TRet>
{
    public TRet Invoke();
}
public interface IFunc<T, TRet>
{
    public TRet Invoke(T a);
}
public interface IFunc<TA, TB, TRet>
{
    public TRet Invoke(TA a, TB b);
}
public interface IFunc<TA, TB, TC, TRet>
{
    public TRet Invoke(TA a, TB b, TC c);
}
