using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Respawnability
{
    public class AreaManager : MonoBehaviour
    {
        public GameObject Prototype;

        private GameObject CurrentInstance;

        public bool ShouldDeactivatePrototype = true;

        // Start is called before the first frame update
        void Start()
        {
            if (ShouldDeactivatePrototype)
                Prototype.SetActive(false);
            Reload();
        }


        public void Reload()
        {
            if (CurrentInstance != null) Unload();

            CurrentInstance = Prototype.InstantiateWithTransform(copyScale: true);
        }

        public void Unload()
        {
            if (CurrentInstance != null)
            {
                Destroy(CurrentInstance);
                CurrentInstance = null;
            }
        }
    }
}