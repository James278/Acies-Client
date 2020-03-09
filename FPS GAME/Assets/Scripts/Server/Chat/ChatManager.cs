using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace GameServer
{
    public class ChatManager : MonoBehaviour
    {
        public GameObject chatPrefab;
        public GameObject chatBar;
        public Transform chatHolder;

        public List<GameObject> chatList;

        public float maxLenght = 10f;
        public float maxWidth = 10f;

        private Client client;

        bool active = false;

        private void Awake()
        {
            client = FindObjectOfType<Client>();
        }

        public void NewChatMessage(string _Message, string _UserName)
        {
            GameObject message = Instantiate(chatPrefab, chatHolder);
            chatList.Add(message);
            message.transform.Find("Text Message").GetComponent<Text>().text = _Message; //Update the message
            message.transform.Find("UserName background").GetComponentInChildren<Text>().text = _UserName; //Updates the username
        }

        public void DisconnectRequest()
        {
            if (client.tcp.socket != null)
            {
                client.tcp.Disconnect();
            }
            else UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu Sockets", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (active)
                {
                    if (chatBar.GetComponentInChildren<InputField>().text != string.Empty)
                    {
                        ClientSend.Chat(chatBar.GetComponentInChildren<InputField>().text);
                        NewChatMessage(chatBar.GetComponentInChildren<InputField>().text, Client.instance.id.ToString());
                        active = false;
                        chatBar.SetActive(active);
                        chatBar.GetComponentInChildren<InputField>().text = string.Empty;
                        return;
                    }
                }

                active = !active; //Make bool turn yes or no each time its called
                chatBar.SetActive(active);
                chatBar.GetComponentInChildren<InputField>().text = string.Empty; //Clears chatbar
                chatBar.GetComponentInChildren<InputField>().Select(); //Focus on the chatbar
            }
        }
    }
}
