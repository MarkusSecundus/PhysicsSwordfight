using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class KeyPicker : MonoBehaviour
{
    [System.Serializable] public class KeySetEvent : UnityEvent<KeyCode> { }

    [SerializeField] string Format = "K: {0}", Prompt = "<Press a key>";

    TMP_Text text;

    public KeySetEvent OnKeySet;
    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        UpdateText("");
    }
    public void PickButton()
    {
        UpdateText(Prompt);
        StartCoroutine(waitForKeyPress());
        IEnumerator waitForKeyPress()
        {
            for(; ; )
            {
                if (Input.anyKeyDown)
                {
                    foreach (var v in EnumUtil.GetValues<KeyCode>())
                        if (Input.GetKeyDown(v))
                        {
                            UpdateText(v.ToString());
                            OnKeySet.Invoke(v);
                            yield break;
                        }
                }
                yield return null;
            }
        }
    }

    void UpdateText(string keyName) => text.text = string.Format(Format, keyName);
}
