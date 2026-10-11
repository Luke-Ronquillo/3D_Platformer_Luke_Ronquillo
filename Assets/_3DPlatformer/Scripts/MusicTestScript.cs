using UnityEngine;

public class MusicTestScript : MonoBehaviour
{ 
    void Start()
    {
        MusicManagerScript.instance.PlayMusic("Test1", 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MusicManagerScript.instance.PlayMusic("Test2", 0.5f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MusicManagerScript.instance.PlayMusic("Test1", 0.5f);
        }
    }
}
