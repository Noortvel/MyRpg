using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyRpg
{

    public class GameAdmin : MonoBehaviour
    {
        public enum GameMode
        {
            None,
            Server,
            Client
        }
        public static GameMode GAME_MODE = GameMode.None;

        [SerializeField]
        private Character characterPrefab;
        [SerializeField]
        private Transform spawnPoint;
        [SerializeField]
        public GameObject cameraOnHintPrefab;
        [SerializeField]
        public PlayerController defaultPlayerController;

        private Queue<NetworkCryptor.NetCommand> messages;
        private List<Character> connectedCharcters;
        private GameObject ccamera;


        private bool isIntancenate = false;

        public void AddMessageToQueue(NetworkCryptor.NetCommand message)
        {
            messages.Enqueue(message);
        }
        public GameObject CameraOnHintPrefab
        {
            get { return cameraOnHintPrefab; }
        }
        public Transform SpawnPoint
        {
            get { return spawnPoint; }
        }
        private GServer gServer;
        private GClient gClient;
        private void Awake()
        {

            System.Net.IPAddress address;
            int port = 8080;
            address = System.Net.IPAddress.Parse("127.0.0.1");
            if (GAME_MODE == GameMode.Server)
            {
                gServer = new GServer(address, port, this);
                Quaternion camRot = Quaternion.Euler(45f, 0, 0);
                Instantiate(cameraOnHintPrefab, spawnPoint.transform.position, camRot);
                isIntancenate = true;
            }
            else if(GAME_MODE == GameMode.Client)
            {
                gClient = new GClient(address, port, this);
                if (gClient.isConnected)
                {
                    messages = new Queue<NetworkCryptor.NetCommand>();
                    connectedCharcters = new List<Character>();
                    ccamera = GameAdmin.Instantiate(CameraOnHintPrefab, SpawnPoint.position, CameraOnHintPrefab.transform.rotation);
                    isIntancenate = true;
                }
            }
            else
            {
                Debug.Log("GameMode: None, do u forget set it or it editor?");
            }

        }
        private void Update()
        {
            if (isIntancenate)
            {
                if (messages != null && messages.Count > 0)
                {
                    while (messages.Count > 0)
                    {
                        ExecuteCommand(messages.Dequeue());
                    }
                }
            }
        }
        private void ExecuteCommand(NetworkCryptor.NetCommand cmd)
        {
            if(cmd is NetworkCryptor.PlayerInstanceInfo)
            {
                var inf = (NetworkCryptor.PlayerInstanceInfo) cmd;
                var character = Instantiate(characterPrefab, inf.position, transform.rotation);
                character.netId = inf.id;
                if (connectedCharcters.Count == 0)
                {
                    var controller = gameObject.AddComponent<PlayerController>();
                    var cam = ccamera.transform.GetChild(0).GetComponent<Camera>();
                    controller.Initialize(character, cam, gClient.connectedClient);
                }
                connectedCharcters.Add(character);
            }
            if(cmd is NetworkCryptor.PlayerMoveToInfo)
            {
                var inf = (NetworkCryptor.PlayerMoveToInfo)cmd;
                DLogger.WriteLineToScreen("Game Admin Move to:");
                Character character = null;
                if (inf != null)
                {
                    character = connectedCharcters.Find(fc => fc.netId == inf.id);
                    DLogger.WriteLineToScreen("TO: " + " id: " + inf.id + " dest " + inf.destonation);
                }
                if (character != null)
                {
                    character.MoveTo(inf.destonation);
                }
            }
            if(cmd is NetworkCryptor.PlayerDestroyInfo)
            {
                var inf = (NetworkCryptor.PlayerDestroyInfo)cmd;
                Character character = null;
                if (inf != null)
                {
                    character = connectedCharcters.Find(fc => fc.netId == inf.id);
                    if(character != null)
                    {
                        connectedCharcters.Remove(character);
                        Destroy(character.gameObject);
                    }
                }
            }

        }
        private void OnApplicationQuit()
        {
            if (isIntancenate)
            {
                if (gServer != null)
                {
                    gServer.CloseConnections();
                }
                if (gClient != null)
                {
                    gClient.CloseConnection();
                }
            }
        }
    }
}