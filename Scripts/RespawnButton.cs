using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnButton : MonoBehaviour
{

    // Start is called before the first frame update
    public void RespawnPlayer(Transform player)
    {
        player.position = new Vector3(0, 0, 0);
        Debug.Log("RespawnPlayer");
    }
}
