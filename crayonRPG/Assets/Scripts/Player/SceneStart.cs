using UnityEngine;

public class SceneStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Transform spawn = GameObject.Find("SpawnPoint").transform;

        player.transform.position = spawn.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
