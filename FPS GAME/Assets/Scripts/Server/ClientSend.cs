using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
namespace GameServer
{
    public class ClientSend : MonoBehaviour
    {

        #region TCP

        private static void SendTCPData(Packet _Packet)
        {
            {
                _Packet.WriteLength();
                Client.instance.tcp.SendData(_Packet);
            }
        }

        #endregion

        #region TCP Packets

        public static void PlayerIsLoaded()
        {
            using (Packet _Packet = new Packet((int)ClientPackets.playerIsLoaded))
            {
                SendTCPData(_Packet);
            }
        }

        public static void Chat(string message)
        {
            using (Packet _Packet = new Packet((int)ClientPackets.chat))
            {
                _Packet.Write(message);

                SendTCPData(_Packet);
            }
        }

        #endregion

        #region UDP

        public static void SendUDPData(Packet _Packet)
        {
            _Packet.WriteLength();
            Client.instance.udp.SendData(_Packet);
        }

        #endregion

        #region UDP Packets

        public static void SendMovement(Vector3 _Position, Quaternion _Rotation)
        {
            using (Packet _Packet = new Packet((int)ClientPackets.playerMovement))
            {
                _Packet.Write(_Position);
                _Packet.Write(_Rotation);

                SendUDPData(_Packet);
            }
        }

        #endregion
        
    }
}