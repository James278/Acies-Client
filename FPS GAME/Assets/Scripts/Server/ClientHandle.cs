using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Net;
using System.Collections.Generic;
namespace GameServer
{
    public class ClientHandle : MonoBehaviour
    {
        private static Client client;

        public static List<PlayerData> playerDataList = new List<PlayerData>();

        private static bool canSpawnLocalPlayer = true;

        private void Awake()
        {
            client = GetComponent<Client>();
        }

        //Spawn new client in the scene
        public static void JoinGame(Packet _Packet)
        {
            try
            {
                int _ID = _Packet.ReadInt();
                client.id = _ID;
                client.udp.Connect(((IPEndPoint)client.tcp.socket.Client.LocalEndPoint).Port);
                client.LoadLevel();
            }
            catch (Exception _EX)
            {
                Debug.Log(_EX);
            }
        }

        public static void PutPlayerInGame(Packet _Packet)
        {
            int _ID = _Packet.ReadInt();
            Vector3 position = _Packet.ReadVector3();
            Quaternion rotation = _Packet.ReadQuaterion();

            if (canSpawnLocalPlayer == true) //Spawn local player
            {
                GameObject player = Instantiate(client.playerPrefab, position, rotation);
                PlayerData playerData = player.GetComponent<PlayerData>();
                playerData.id = _ID;
                playerDataList.Add(playerData);
                playerData.localPlayer = true;
                canSpawnLocalPlayer = false;
            }
            else if (canSpawnLocalPlayer == false) //Spawn non local players
            {
                GameObject player = Instantiate(client.otherPlayerPrefab, position, rotation);
                PlayerData playerData = player.GetComponent<PlayerData>();
                playerData.id = _ID;
                playerData.SetName();
                playerDataList.Add(playerData);
            }
        }

        public static void PlayerMovement(Packet _Packet)
        {
            int _ID = _Packet.ReadInt();
            Vector3 _Position = _Packet.ReadVector3();
            Quaternion _Rotation = _Packet.ReadQuaterion();

            foreach (PlayerData playerData in playerDataList)
            {
                if (playerData.id == _ID && _ID != client.id)
                {
                    playerData.transform.position = _Position;
                    playerData.transform.rotation = _Rotation;
                }
            }
        }
    }
}