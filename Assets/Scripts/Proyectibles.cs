using UnityEngine;
using Unity.Netcode;

public class Proyectibles : NetworkBehaviour
{
   // private int damage;

    public int damage = 25;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    void Start()
    {

        if (IsServer)
        {
            Invoke("SimpleDespaw", 5);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SimpleDespaw()
    {
        GetComponent<NetworkObject>().Despawn();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // Comprobamos si chocamos con un objeto con la etiqueta "Enemy"

        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("¡Colisión con un 'Enemy' detectada!");

            EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamageRpc(damage);
                SimpleDespaw();
                return; 
            }
        }
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("¡Colisión con un 'Player' detectada!");

            SimplePlayerController player = other.gameObject.GetComponent<SimplePlayerController>();
            if (player != null)
            {
                player.TakeDamageRpc(damage);
                SimpleDespaw();
                return;
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
           
        }

        if (!other.gameObject.CompareTag("Player"))
        {
            SimpleDespaw();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            SimpleDespaw();
        }
    }

}
