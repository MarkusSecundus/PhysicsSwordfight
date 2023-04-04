using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject Root;
    public bool IsPaused { get=>Root.activeSelf; }

    float lastTimeScale;
    private void Awake()
    {
        lastTimeScale = Time.timeScale;
        Root.SetActive(false);
    }

    public void DoPause()
    {
        if (IsPaused) return;
        lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Root.SetActive(true);
    }

    public void DoUnpause()
    {
        if (!IsPaused) return;
        Time.timeScale = lastTimeScale;
        Root.SetActive(false);
    }

    public void TogglePause()
    {
        if (IsPaused) DoUnpause();
        else DoPause();
    }
}
