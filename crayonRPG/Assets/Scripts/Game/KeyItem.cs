using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatHeight = 0.2f;

    private Vector3 startPos;
    private bool floating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.localPosition;
    }

    public void StartFloating()
    {
        floating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(floating)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AddKey();

            Destroy(gameObject);
        }
    }



}
