using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAssembly : MonoBehaviour
{
    public SwordsmanMovement Player;
    public SwordMovement Sword;
    public CameraFollowPoint Camera;
    public DamageHUD DamageReport;

    [System.Serializable] public struct InstructionsOnDeath
    {
        public float SwordDestroyDelay;
    }
    public InstructionsOnDeath DeathInstructions;

    void Start()
    {
        if (Camera.IsNotNil() && Player.CameraToUse.IsNotNil())
        {
            Camera.Target = Player.CameraToUse;
        }
        if (DamageReport.IsNotNil())
        {
            DamageReport.Target = Player.GetComponent<Damageable>();
        }
    }

    public void DestroyTheSwordsman()
    {
        if (DeathInstructions.SwordDestroyDelay.IsNormalNumber()) Destroy(Sword.gameObject, DeathInstructions.SwordDestroyDelay);
        Sword.DropTheSword();
        Destroy(gameObject);
    }
}