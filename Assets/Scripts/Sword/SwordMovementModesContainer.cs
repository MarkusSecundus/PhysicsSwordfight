using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Submodule = SwordMovement.Submodule;

[System.Serializable]
public class SwordMovementModesContainer : SerializableDictionary<KeyCode, Submodule, SwordMovementModesContainer.Entry>
{

    [System.Serializable]
    public struct Entry : SerializableDictionary.IEntry<KeyCode, Submodule>
    {
        [SerializeField] public KeyCode Key;
        [SerializeField][SerializeReference][Subclass] public Submodule Value;
        KeyCode SerializableDictionary.IEntry<KeyCode, Submodule>.Key { get => this.Key; init => this.Key = value; }
        Submodule SerializableDictionary.IEntry<KeyCode, Submodule>.Value { get => this.Value; init => this.Value = value; }
    }

    [SerializeField, SerializeReference, Subclass] public Submodule Default;

    protected override void FillDictionaryValues(Dictionary<KeyCode, Submodule> dictionary)
    {
        base.FillDictionaryValues(dictionary);
        dictionary[default] = Default;
    }
}