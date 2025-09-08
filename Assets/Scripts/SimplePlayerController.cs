using UnityEngine;
using Unity.Netcode;
public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;
    public NetworkVariable<int> Life;
    public float JumpForce = 5;
    public float Speed = 10;
    private Animator animator;
    private Rigidbody rb;
    public LayerMask groundLayer;

    public GameObject proyectil;
    public Transform firepoint;

    public float projectileSpeed = 15f;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
   
    public void Update()
    {
        if (!IsOwner) return;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float VelX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
            float VelY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;
            UpdatePositionRpc(VelX, VelY);
        }
        CheckGroundRpc();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpTriggerRpc("Jump");

        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            // 1. Crear un rayo desde la cámara hacia la posición del ratón
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 2. Lanzar el rayo. Usamos una LayerMask para asegurarnos de que solo choque con el suelo.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // 3. Calcular la dirección desde el punto de disparo al punto de impacto del rayo
                Vector3 direction = (hit.point - firepoint.position);
                direction.y = 0; // Ignoramos la altura para que el disparo sea horizontal
                direction.Normalize(); // La convertimos en un vector de dirección puro

                // 4. Llamar al RPC enviando la dirección calculada
                SHOOTRpc(direction);
            }
        }
    }
    [Rpc(SendTo.Server)]
    public void UpdatePositionRpc(float x, float y)
    {
        transform.position += new Vector3(x, 0, y);
    }
    [Rpc(SendTo.Server)]
    public void JumpTriggerRpc(string animationName)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        animator.SetTrigger(animationName);
    }
    [Rpc(SendTo.Server)]
    public void CheckGroundRpc()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer))
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("Grounded", true);
        }
    }
    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage)
    {
        Life.Value -= damage;

        if (Life.Value <= 0)
        {
            Life.Value = 0;
            Debug.Log($"El jugador {PlayerID.Value} ha sido derrotado.");
        }
    }

    [Rpc(SendTo.Server)]
    public void SHOOTRpc(Vector3 direction) // Ahora recibe la dirección
    {
        // 5. Usar la dirección para la rotación y la fuerza
        GameObject proj = Instantiate(proyectil, firepoint.position, Quaternion.LookRotation(direction));
        proj.GetComponent<NetworkObject>().Spawn(true);

        proj.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }
}