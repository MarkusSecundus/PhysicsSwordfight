using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class ScriptSubmodulesContainer<TKey, TSubmodule, TScript> : SerializableDictionary<TKey, TSubmodule, ScriptSubmodulesContainer<TKey, TSubmodule, TScript>.Entry> where TSubmodule: IScriptSubmodule<TScript>
{

    [System.Serializable]
    public struct Entry : SerializableDictionary.IEntry<TKey, TSubmodule>
    {
        [SerializeField] public TKey Key;
        [SerializeField][SerializeReference][Subclass] public TSubmodule Value;
        TKey SerializableDictionary.IEntry<TKey, TSubmodule>.Key { get => this.Key; init => this.Key = value; }
        TSubmodule SerializableDictionary.IEntry<TKey, TSubmodule>.Value { get => this.Value; init => this.Value = value; }
    }

    [SerializeField, SerializeReference, Subclass] public TSubmodule Default;

    protected override void FillDictionaryValues(Dictionary<TKey, TSubmodule> dictionary)
    {
        base.FillDictionaryValues(dictionary);
        dictionary[default] = Default;
    }
}

public class ScriptSubmoduleListManager<TKey, TSubmodule, TScript> : IScriptSubmodule<TScript> where TSubmodule : IScriptSubmodule<TScript>
{
    public ScriptSubmodulesContainer<TKey, TSubmodule, TScript> Modes { get; init; }
    public Func<TKey, bool> ActivityPredicate { protected get; init; }

    TSubmodule activeMode;
    protected override void OnStart()
    {
        foreach (var mode in Modes.Values.Values) mode.Init(Script);
    }

    public override void OnUpdate(float delta)
    {
        MakeSureRightModeIsActive();
        activeMode?.OnUpdate(Time.deltaTime);
    }
    public override void OnFixedUpdate(float delta)
    {
        MakeSureRightModeIsActive();
        activeMode?.OnFixedUpdate(delta);
    }

    public override void OnDrawGizmos() => activeMode?.OnDrawGizmos();
    void MakeSureRightModeIsActive()
    {
        var mode = Modes.Values[Modes.Values.Keys.FirstOrDefault(ActivityPredicate)];
        if (activeMode != mode)
        {
            activeMode?.OnDeactivated();
            (activeMode = mode)?.OnActivated();
        }
    }
}
