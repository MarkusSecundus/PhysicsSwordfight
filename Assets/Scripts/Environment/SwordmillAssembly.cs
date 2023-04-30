using MarkusSecundus.PhysicsSwordfight.PhysicsUtils;
using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using MarkusSecundus.PhysicsSwordfight.Utils.Geometry;
using MarkusSecundus.PhysicsSwordfight.Utils.Primitives;
using MarkusSecundus.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static MarkusSecundus.Utils.Op;

namespace MarkusSecundus.PhysicsSwordfight.Environment
{
    /// <summary>
    /// Component responsible for assembling a swordmill - sphere ("rotor") on a stick that has swords attached to it and rotates.
    /// </summary>
    public class SwordmillAssembly : MonoBehaviour
    {
        /// <summary>
        /// The central rotating piece that the swords are connected to
        /// </summary>
        public ConfigurableJoint Rotor;
        /// <summary>
        /// Prototype of the sword
        /// </summary>
        public ConfigurableJoint SwordPrototype;
        /// <summary>
        /// How many swords should be spawned
        /// </summary>
        public int SwordsCount = 1;

        void Start()
        {
            MakeSwords(SwordPrototype, SwordsCount);
        }


        IReadOnlyList<ConfigurableJoint> MakeSwords(ConfigurableJoint proto, int count)
        {
            proto.gameObject.SetActive(false);
            List<ConfigurableJoint> ret = new List<ConfigurableJoint> { };

            var angleToAdd = NumericConstants.MaxDegree / count;
            var angleAccumulator = 0f;
            proto.autoConfigureConnectedAnchor = false;
            foreach (var v in SphereGeometryHelpers.PointsOnCircle(count, proto.connectedAnchor, Vector3.up, includeBegin: true))
            {
                var s = proto.gameObject.InstantiateWithTransform().GetComponent<ConfigurableJoint>();
                var rotationHelper = s.MakeRotationHelper();
                ret.Add(s);

                s.connectedAnchor = v;
                s.transform.position = s.connectedBody.transform.LocalToGlobal(s.connectedAnchor);
                var angle = s.targetRotation * Quaternion.AngleAxis(post_assign(ref angleAccumulator, angleAccumulator + angleToAdd), Vector3.up);
                rotationHelper.SetTargetRotation(angle);
                s.transform.rotation = angle;

                s.gameObject.SetActive(true);
            }

            return ret;
        }
    }
}