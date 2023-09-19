using MarkusSecundus.MultiInput;
using MarkusSecundus.PhysicsSwordfight.Sword;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkusSecundus.PhysicsSwordfight.HotSeat
{
    public class QuickAndDirtyHotSeatSetup : MonoBehaviour
    {
        [SerializeField] Camera Player1Camera, Player2Camera;
        [SerializeField] SwordsmanAssembly Player1, Player2;

        // Start is called before the first frame update
        void Start()
        {
            SetupCameras();
            SetupSystemCursor();
            SetupInput();
        }

        void SetupCameras()
        {
            if(Display.displays.Length > 1)
            {
                Display.displays[1].Activate();
                Player2Camera.targetDisplay = 1;
            }
            else
            {
                var (p1rect, p2rect) = (Player1Camera.rect, Player2Camera.rect);
                p1rect.xMax = p2rect.xMin = 0.5f;
                (Player1Camera.rect, Player2Camera.rect) = (p1rect, p2rect);
            }
        }
        void SetupSystemCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void SetupInput()
        {
            var (i1, i2) = (Player1.GetComponent<SpecificDeviceInput>(), Player2.GetComponent<SpecificDeviceInput>());
            System.Action<IMouse> setupInputCamera = m =>
            {
                m.ShouldDrawCursor = false;
                if (IInputProvider.Instance.ActiveMice.ElementAtOrDefault(i1.MouseIndex) == m) m.Config.TargetCamera = Player1Camera;
                else if (IInputProvider.Instance.ActiveMice.ElementAtOrDefault(i2.MouseIndex) == m) m.Config.TargetCamera = Player2Camera;
            };
            IInputProvider.Instance.OnMouseActivated += setupInputCamera;
            if (IInputProvider.Instance.ActiveMice.Count > 0)
                foreach (var m in IInputProvider.Instance.ActiveMice) setupInputCamera(m);
        }
    }
}
