using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class SetNetManIP : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log(string.Format("NetworkManager.singleton.networkAddress: {0}", NetworkManager.singleton.networkAddress));

    }

}
