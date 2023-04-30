using MarkusSecundus.PhysicsSwordfight.Utils.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.Actions
{
    namespace MarkusSecundus.PhysicsSwordfight.Respawnability
    {
        /// <summary>
        /// Componant that takes care of part of the scene and allows it to be reloaded.
        /// </summary>
        public class AreaManager : MonoBehaviour
        {
            /// <summary>
            /// Prototype of the scene segment
            /// </summary>
            public GameObject Prototype;
            /// <summary>
            /// Currently active instance of the scene segment
            /// </summary>
            private GameObject CurrentInstance;

            // Start is called before the first frame update
            void Start()
            {
                Reload();
            }

            /// <summary>
            /// Reload the scene segment - discard all changes and start it anew
            /// </summary>
            public void Reload()
            {
                if (CurrentInstance != null) Unload();

                CurrentInstance = Prototype.InstantiateWithTransform(copyScale: true);
            }
            /// <summary>
            /// Destroy current instance of the scene segment
            /// </summary>
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
}