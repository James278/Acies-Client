using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncServers : MonoBehaviour
{
    public Text refreshServersText;

    public GameObject menu;
    public GameObject serverList;
    public GameObject serverPrefab;
    public Transform serverListPanel;

    private bool searchForServers;

    int foundServer = 0;

    //TODO Fix all code there to work with the new multiplayer code!

    public void RefreshServers()
    {
        refreshServersText.text = "Search for servers.";
        searchForServers = true;
        StartCoroutine(SearchForServersGUIUpdator(0, 10));
    }

    IEnumerator SearchForServersGUIUpdator(float counter, float timeOut)
    {
        while (searchForServers)
        {
            if (counter >= 5) { refreshServersText.text = "Search for servers."; counter = 0; }
            refreshServersText.text += ".";
            counter++;
            if (foundServer >= 10)
            {
                searchForServers = false;
                refreshServersText.gameObject.SetActive(false);
                UpdateServerList();
                break;
            }
            if (timeOut <= 0)
            {
                searchForServers = false;
                refreshServersText.text = "No servers found";
                break;
            }
            timeOut--;
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateServerList()
    {
        
    }

    public void ReturnToMenu()
    {
        serverList.SetActive(false);
        menu.SetActive(true);
        searchForServers = false;
        StopCoroutine(SearchForServersGUIUpdator(0, 10));
    }

    public void ReturnToServerList()
    {
        serverList.SetActive(true);
        menu.SetActive(false);
        RefreshServers(); // Refresh the server's on click
    }
}
