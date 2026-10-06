using UnityEngine;

public class KillScript : MonoBehaviour
{
    [SerializeField] float killTime;
    private void Start()
    {
        Destroy(gameObject, killTime);
    }
}
