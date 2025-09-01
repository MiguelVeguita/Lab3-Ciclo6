using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))] 
public class EnemyAI : NetworkBehaviour
{
    public float Speed = 5.0f;
    private Rigidbody rb;
    private Transform targetPlayer;
    private float checkTimer = 0f;
    private const float checkInterval = 1.0f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!IsServer) return;

        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            FindClosestPlayer();
            checkTimer = 0f;
        }

        if (targetPlayer != null)
        {
            transform.LookAt(targetPlayer);
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        if (targetPlayer == null)
        {
            rb.linearVelocity = Vector3.zero; 
            return;
        }
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        Vector3 moveVelocity = direction * Speed;
        moveVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = moveVelocity;
    }

    void FindClosestPlayer()
    {
        Debug.Log("--- Buscando al jugador más cercano usando TAGS ---");

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("No se encontraron objetos con la etiqueta 'Player' en la escena.");
            targetPlayer = null; 
            return;
        }

        Debug.Log($"Se encontraron {players.Length} jugadores.");

        Transform closestPlayer = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log($"Distancia al jugador en {player.transform.position}: {distance}");

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        if (closestPlayer != null)
        {
            targetPlayer = closestPlayer;
            Debug.Log($"¡Nuevo objetivo fijado! El jugador más cercano está a {closestDistance} unidades.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Die();
        }
    }

    private void Die()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}