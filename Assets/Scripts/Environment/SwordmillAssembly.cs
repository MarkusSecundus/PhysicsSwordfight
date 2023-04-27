using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Environment
{

    public class SwordmillAssembly : MonoBehaviour
    {
        public ConfigurableJoint Rotor;
        public ConfigurableJoint SwordPrototype;
        public int SwordsCount = 1;

        void Start()
        {
            MakeSwords(SwordPrototype, SwordsCount);
        }


        IReadOnlyList<ConfigurableJoint> MakeSwords(ConfigurableJoint proto, int count)
        {
            proto.gameObject.SetActive(false);
            List<ConfigurableJoint> ret = new List<ConfigurableJoint> { };

            var angleToAdd = RotationUtil.MaxDegree / count;
            var angleAccumulator = 0f;
            proto.autoConfigureConnectedAnchor = false;
            foreach (var v in GeometryUtils.PointsOnCircle(count, proto.connectedAnchor, Vector3.up, includeBegin: true))
            {
                var s = proto.gameObject.InstantiateWithTransform().GetComponent<ConfigurableJoint>();
                var rotationHelper = s.MakeRotationHelper();
                ret.Add(s);

                s.connectedAnchor = v;
                s.transform.position = s.connectedBody.transform.LocalToGlobal(s.connectedAnchor);
                var angle = s.targetRotation * Quaternion.AngleAxis(Op.post_assign(ref angleAccumulator, angleAccumulator + angleToAdd), Vector3.up);
                rotationHelper.SetTargetRotation(angle);
                s.transform.rotation = angle;

                s.gameObject.SetActive(true);
            }

            return ret;
        }
    }
}