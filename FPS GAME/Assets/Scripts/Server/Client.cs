using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
namespace GameServer
{
    public class Client : MonoBehaviour
    {
        public static Client instance;
        public static int dataBufferSize = 4096;
        public string ip = "92.65.158.186";
        public int port = 3334;
        public TCP tcp;
        public UDP udp;

        [HideInInspector] public int id;

        private delegate void PacketHandler(Packet _Packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        [HideInInspector] public GameObject playerPrefab;
        [HideInInspector] public GameObject otherPlayerPrefab;

        private Button connectButton;

        public bool debugLogs = true;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exist, destroying object!");
                Destroy(gameObject);
            }
        }

        private void OnValidate()
        {
            Debug.unityLogger.logEnabled = debugLogs;
        }

        private void Start()
        {
            playerPrefab = Resources.Load<GameObject>("Prefabs/PlayerVariants/PlayerAvatar");
            otherPlayerPrefab = Resources.Load<GameObject>("Prefabs/PlayerVariants/OtherPlayer");
            IntitalizeClientData();
            tcp = new TCP();
            udp = new UDP();
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Socket is disconnected!");
            tcp.Disconnect(); // Disconnect when the game is closed
        }

        void OnLevelWasLoaded(int level)
        {
            if (level == 0)
            {
                connectButton = GameObject.Find("Battle Button").GetComponent<Button>();
                connectButton.onClick.AddListener(ConnectToServer);
            }
        }

        public void LoadLevel()
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        public void ConnectToServer()
        {
            tcp.Connect();
        }

        private void IntitalizeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ServerPackets.joinGame, ClientHandle.JoinGame },
                { (int)ServerPackets.putPlayerInGame, ClientHandle.PutPlayerInGame },
                { (int)ServerPackets.playerPosition, ClientHandle.PlayerMovement },
            };

            Debug.Log("Intitalize client data!");
        }

        public class TCP
        {
            public TcpClient socket;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _Result)
            {
                socket.EndConnect(_Result);

                if (!socket.Connected)
                {
                    Disconnect();
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet _Packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_Packet.ToArray(), 0, _Packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via TCP: {_ex}");
                    Disconnect();
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
                }
            }

            private bool HandleData(byte[] _Data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_Data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0) //If a packed is smaller then 0 we return
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            try
                            {
                                packetHandlers[_packetId](_packet);
                            }
                            catch (Exception ex)
                            {
                                Debug.Log($"ERROR {ex}");
                                Debug.Log(_packetId + " Packet incoming");
                            }
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            //Disconnects from the server and cleans up the TCP connection.
            public void Disconnect()
            {
                if (socket != null)
                {
                    socket.Client.Shutdown(SocketShutdown.Both);
                    stream = null;
                    receivedData = null;
                    receiveBuffer = null;
                    socket = null;
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Sockets", UnityEngine.SceneManagement.LoadSceneMode.Single);
                }
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            public void Connect(int _LocalPort)
            {
                socket = new UdpClient(_LocalPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallBack, null);

                using (Packet _Packet = new Packet())
                {
                    SendData(_Packet);
                }
            }

            public void SendData(Packet _Packet)
            {
                try
                {
                    _Packet.InsertInt(instance.id);
                    if (socket != null)
                    {
                        socket.BeginSend(_Packet.ToArray(), _Packet.Length(), null, null);
                    }
                }
                catch (Exception _EX)
                {
                    Debug.Log($"Error sending data to server via UDP: {_EX}");
                    instance.tcp.Disconnect();
                }
            }

            public void ReceiveCallBack(IAsyncResult _Result)
            {
                try
                {
                    byte[] _Data = socket.EndReceive(_Result, ref endPoint);
                    socket.BeginReceive(ReceiveCallBack, null);

                    if (_Data.Length < 4)
                    {
                        instance.tcp.Disconnect();
                        return;
                    }

                    HandleData(_Data);
                }
                catch (Exception _EX)
                {
                    Debug.Log($"Error on receive UDP Data: {_EX}");
                    Disconnect();
                }
            }

            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _PacketLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_PacketLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _PacketId = _packet.ReadInt();
                        packetHandlers[_PacketId](_packet); // Call appropriate method to handle the packet
                    }
                });
            }
            //Disconnects from the server and cleans up the UDP connection
            private void Disconnect()
            {
                instance.tcp.Disconnect();

                endPoint = null;
                socket = null;
            }
        }
    }
}