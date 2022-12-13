using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[RequireComponent(typeof(PhysicsDamager))]
public class SwordDamageManager : MonoBehaviour
{
    public float NonIdleTravelSpeed = 1f;
    public float DamageWhenActive = 10f, DamageWhenIdle = 1f;
    public Transform swordTip;
    private Vector3 lastBladetipPosition;
    private PhysicsDamager physicsDamager;
    // Start is called before the first frame update
    void Start()
    {
        physicsDamager = GetComponent<PhysicsDamager>();
        lastBladetipPosition = swordTip.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetSwordDamageBasedOnSpeed(Time.fixedDeltaTime);
    }

    void SetSwordDamageBasedOnSpeed(float delta)
    {
        var tipPosition = swordTip.transform.position;

        var pathTraveled = lastBladetipPosition - tipPosition;
        var travelSpeed = pathTraveled.magnitude / delta;

        this.physicsDamager.DamageMultiplier = travelSpeed >= NonIdleTravelSpeed ? DamageWhenActive : DamageWhenIdle;

        lastBladetipPosition = tipPosition;
    }

}
