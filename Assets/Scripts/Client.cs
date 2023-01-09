using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Client : MonoBehaviour
{
    NetworkManager manager;
    
    // Start is called before the first frame update
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void Start()
    {
#if UNITY_SERVER
#else
        if (!NetworkServer.active)
        {
            manager.StartClient();
        }
#endif
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}