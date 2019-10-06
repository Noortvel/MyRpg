using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRpg
{
    public class PlayerController : MonoBehaviour
    {
        private GameObject cameraHint;
        private Character controllerCharacter;
        private Camera cameraComp;
        private GTcpClientOnClient gcClient;
        private bool isInit = false;

        public void Initialize(Character controllerCharacter,Camera cameraComp, GTcpClientOnClient gcClient)
        {
            this.controllerCharacter = controllerCharacter;
            this.cameraComp = cameraComp;
            this.gcClient = gcClient;
            isInit = true;
        }
        void Update()
        {
            if (isInit)
            {
                InputUpdate();
            }
        }
        private void InputUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastCheck();
            }
        }
        private void RaycastCheck()
        {
            //print("123");
            Vector3 screenplane = Input.mousePosition;
            screenplane.z = 10;
            Vector3 point = cameraComp.ScreenToWorldPoint(screenplane);
            Vector3 sorc = cameraComp.transform.position;
            RaycastHit info;
            Vector3 dir = point - sorc;
            if (Physics.Raycast(sorc, dir, out info, 40f))
            {
                if (info.transform.tag == "Terrain")
                {
                    controllerCharacter.MoveTo(info.point);
                    var inf = new NetworkCryptor.PlayerMoveToInfo(controllerCharacter.netId, info.point);
                    gcClient.SendAsync(inf.toBytes());
                }
                if (info.transform.tag == "Character")
                {
                    var obj = info.transform.gameObject;
                }
            }
        }
    }

}