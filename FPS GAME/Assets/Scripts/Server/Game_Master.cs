﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace GameServer
{
    public class Game_Master : MonoBehaviour
    {
        private void Awake()
        {
            ClientSend.PlayerIsLoaded();
        }
    }

}