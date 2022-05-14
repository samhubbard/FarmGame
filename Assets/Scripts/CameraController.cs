using Cinemachine;
using FishNet.Object;
using UnityEngine;

namespace FarmGame
{
    public class CameraController : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                Camera cam = Camera.main;

                cam.transform.parent = transform;
                cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (IsOwner)
            {
                Camera cam = Camera.main;
                cam.transform.parent = null;
            }
        }
    }
}