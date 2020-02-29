using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace GameServer
{
    public class ServerGUIManager : MonoBehaviour
    {
        public static ServerGUIManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exist, destroying object!");
                Destroy(gameObject);
            }
        }
    }
}