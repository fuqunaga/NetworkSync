using UnityEngine;
using System.Collections;

namespace NetworkSync {


public class NetworkSyncManager<T> : NetworkSyncManagerBase
    where T : MonoBehaviour
{
#region singleton
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }
#endregion
}

public class NetworkSyncManagerBase : MonoBehaviour 
{
#region static method
    public static bool isOnline { get { return (Network.peerType == NetworkPeerType.Server) || (Network.peerType == NetworkPeerType.Client); } }

    public static GameObject InstantiateChild(MonoBehaviour parent, Object prefab) { return InstantiateChild(parent, prefab, Vector3.zero, Quaternion.identity); }
    public static GameObject InstantiateChild(MonoBehaviour parent, Object prefab, Vector3 pos, Quaternion rot)
    {
        var go = Instantiate(prefab, pos, rot) as GameObject;
        if (go) go.transform.SetParent(parent.transform);
        return go;
    }

    public static new Object Instantiate(Object prefab) { return Instantiate(prefab, Vector3.zero, Quaternion.identity); }
    public static new Object Instantiate(Object prefab, Vector3 pos, Quaternion rot)
    {
        Object obj = null;

        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                obj = Object.Instantiate(prefab, pos, rot);
                break;

            case NetworkPeerType.Server:
                obj = Network.Instantiate(prefab, pos, rot, 0);
                break;

            default:
                Debug.LogWarning("peerType[" + Network.peerType + "]: not allows instantiate.");
                break;
        }

        return obj;
    }

    public static void Destroy(GameObject go)
    {
        switch (Network.peerType)
        {
            case NetworkPeerType.Disconnected:
                Object.Destroy(go);
                break;

            case NetworkPeerType.Server:
                Network.Destroy(go);
                break;

            default:
                Debug.LogWarning("peerType[" + Network.peerType + "]: not allows destroy.");
                break;
        }
    }
    
#endregion

    public virtual string address { get { return "localhost"; } }
    public virtual int port { get { return 8632; } }

    public virtual void Start()
    {
        Network.sendRate = 60f;
    }

    public virtual NetworkConnectionError Connect()
    {
        return Network.Connect(address, port);
    }

    public virtual void Disconnect() 
    { 
        Network.Disconnect();  
    }

    public virtual NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat)
    {
        return Network.InitializeServer(connections, port, useNat);
    }

    public virtual void DebugMenu()
    {
        if (!isOnline)
        {
            if (GUILayout.Button("Connect"))
            {
                Connect();
            }
            if (GUILayout.Button("Host"))
            {
                InitializeServer(1, port, false);
            }
        }
        else
        {
            GUILayout.Label("[" + Network.peerType + "]connections:" + Network.connections.Length.ToString());
            if (GUILayout.Button("Disconnect"))
            {
                Disconnect();
            }
        }
    }
}

}