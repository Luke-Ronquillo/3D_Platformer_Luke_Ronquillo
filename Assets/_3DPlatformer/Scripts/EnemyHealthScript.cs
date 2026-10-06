using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class EnemyHealthScript : MonoBehaviour
{
    Animator anim;

    [Header("Set up Materials")]
    [SerializeField] SkinnedMeshRenderer _skin;
    public Material baseMat;
    public Material hurtMat;

    [Header("Setup Effects")]
    public GameObject impactEffect;
    public GameObject deathSmokeEffect;
    public Transform impactSpawnPoint;


    public int enemyHealth;
    [SerializeField] Rigidbody rb;
    bool isHurt = false;
    bool isDead = false;
    public float knockbackForce = 4;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _skin.material = baseMat;
        anim = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HitSphere" && !isHurt)
        {
            Transform _player = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 lookPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            transform.LookAt(lookPosition);
            isHurt = true;
            StartCoroutine(TakeDamage());
        }
    }
    IEnumerator TakeDamage()
    {
        Debug.Log("I took Damage");
        enemyHealth--;
        if (enemyHealth <= 0)
        {
            StartCoroutine(EnemyDeath());
            yield return null;
        }
        else
        {
            anim.SetTrigger("Hurt");
            Instantiate(impactEffect, impactSpawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.025f);
            HitStopManager.Instance.DoHitStop(0.1f, 0);
            rb.AddForce(transform.forward * -knockbackForce, ForceMode.Impulse);
            _skin.material = hurtMat;
            yield return new WaitForSeconds(0.1f);
            _skin.material = baseMat;
            isHurt = false;
        }
    }
    IEnumerator EnemyDeath()
    {
        isDead = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(1.5f);
        Instantiate(deathSmokeEffect, impactSpawnPoint.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
