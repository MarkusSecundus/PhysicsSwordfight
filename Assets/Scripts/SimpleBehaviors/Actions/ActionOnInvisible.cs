using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    public class ActionOnInvisible : MonoBehaviour
    {
        public UnityEvent ToInvoke;

        public Vector3 min = new Vector3(-999, -999, -999), max = new Vector3(999, 999, 999);

        public bool relative = true;

        private void Start()
        {
            if (relative)
            {
                var currPos = transform.position;
                min += currPos;
                max += currPos;
            }
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (transform.position.Any(min, (a, b) => a < b) || transform.position.Any(max, (a, b) => a > b))
            {
                ToInvoke.Invoke();
            }
        }

        public void DestroyThisGameObject()
        {
            Destroy(gameObject);
        }

        public void SetPosition(Transform target)
        {
            transform.position = target.position;
        }
        public void SetRotation(Transform target)
        {
            transform.rotation = target.rotation;
        }

        private Rigidbody rb;

        public void ResetVelocity()
        {
            rb.velocity = Vector3.zero;
        }

    }
}