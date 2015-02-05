using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NetworkSync
{

    public class DisableOnClient : MonoBehaviour
    {
        public List<Behaviour> scripts;

        public void OnConnectedToServer()
        {
            scripts.ForEach(script => script.enabled = false);
        }

        public void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            scripts.ForEach(script => script.enabled = true);
        }
    }

}