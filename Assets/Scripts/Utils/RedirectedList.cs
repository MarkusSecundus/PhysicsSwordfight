using System;
using System.Collections;
using System.Collections.Generic;

public class RedirectedList<TInput, TResult> : IReadOnlyList<TResult>
{
    readonly IReadOnlyList<TInput> Base;
    readonly Func<TInput, TResult> Supplier;
    public RedirectedList(IReadOnlyList<TInput> baseList, Func<TInput, TResult> supplier) => (Base, Supplier) = (baseList, supplier);

    public TResult this[int index] => Supplier(Base[index]);

    public int Count => Base.Count;

    public IEnumerator<TResult> GetEnumerator()
    {
        var it = Base.GetEnumerator();
        IEnumerator<TResult> impl() 
        {
            while (it.MoveNext()) yield return Supplier(it.Current);
        }
        return impl();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
