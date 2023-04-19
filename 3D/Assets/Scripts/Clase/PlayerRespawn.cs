using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Scene") == SceneManager.GetActiveScene().name)
        {
            transform.position = (new Vector3(PlayerPrefs.GetFloat("CKX"), PlayerPrefs.GetFloat("CKY"), PlayerPrefs.GetFloat("CKZ")));
        }
    }

    public void ReachedCheckPoint(string Scene, float x, float y, float z)
    {
        PlayerPrefs.SetString("Scene", Scene);
        PlayerPrefs.SetFloat("CKX", x);
        PlayerPrefs.SetFloat("CKY", y);
        PlayerPrefs.SetFloat("CKZ", z);
    }
}
