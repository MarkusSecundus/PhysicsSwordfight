using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SwordmillBehavior : MonoBehaviour
{
    public ConfigurableJoint Rotor;
    public ConfigurableJoint SwordPrototype;
    public int SwordsCount = 1;
    public Vector3 positionOffset = Vector3.zero;   

    public float SwordsAngle = 0f;

    private IEnumerable<ConfigurableJoint> swords;
    private Vector3 originalConnectedAnchor;
    void Start()
    {
        swords = MakeSwords(SwordPrototype, SwordsCount);
        originalConnectedAnchor = Rotor.connectedAnchor;
        Rotor.autoConfigureConnectedAnchor = false;
    }



    void Update()
    {
        //foreach (var s in swords) s.targetRotation = Quaternion.Euler(s.targetRotation.eulerAngles.With(x: SwordsAngle));

        Rotor.connectedAnchor = originalConnectedAnchor + positionOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision: {gameObject.name} - {collision.gameObject.name}");
    }



    IEnumerable<ConfigurableJoint> MakeSwords(ConfigurableJoint proto, int count)
    {
        proto.gameObject.SetActive(false);
        List<ConfigurableJoint> ret = new List<ConfigurableJoint> {};

        var angleToAdd = RotationUtil.MaxDegree / count;
        var angleAccumulator = 0f;
        proto.autoConfigureConnectedAnchor = false;
        foreach (var v in GeometryUtils.PointsOnCircle(count, proto.connectedAnchor, Vector3.up, includeBegin:true))
        {
            var s = proto.gameObject.InstantiateWithTransform().GetComponent<ConfigurableJoint>();
            s.gameObject.SetActive(true);
            ret.Add(s);
            s.connectedAnchor = v;
            s.targetRotation = s.targetRotation * Quaternion.AngleAxis(Op.post_assign(ref angleAccumulator, angleAccumulator + angleToAdd), Vector3.up);
        }

        return ret;
    }
}
