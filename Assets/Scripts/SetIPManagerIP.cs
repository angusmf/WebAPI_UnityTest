using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class SetIPManagerIP : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

            Debug.Log(string.Format("IPManager.GetIP(ADDRESSFAM.IPv4): {0}", IPManager.GetIP(ADDRESSFAM.IPv4)));

    }

}
