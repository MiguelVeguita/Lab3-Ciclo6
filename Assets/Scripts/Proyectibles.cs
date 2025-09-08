using UnityEngine;
using Unity.Netcode;

public class Proyectibles : NetworkBehaviour
{
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
        // La l�gica de da�o solo se ejecuta en el servidor
        if (!IsServer) return;

        // Comprobamos si chocamos con un objeto con la etiqueta "Enemy"

        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("�Colisi�n con un 'Enemy' detectada!");

            EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamageRpc(damage);
                // A�ADE ESTA L�NEA PARA QUE SE DESTRUYA TRAS EL PRIMER GOLPE
                SimpleDespaw();
                return; // Salimos de la funci�n para no ejecutar m�s c�digo
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            // Obtenemos el script del enemigo y llamamos a su funci�n para recibir da�o
           
        }

        // El proyectil se destruye al chocar con cualquier cosa (excepto el jugador que lo dispar�)
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
