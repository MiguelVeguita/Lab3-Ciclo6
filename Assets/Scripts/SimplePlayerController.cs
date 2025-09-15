using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
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

    public NetworkVariable<FixedString32Bytes> accoundID = new();
    public NetworkVariable<int> health = new();
    public NetworkVariable<int> attack = new();
    public int maxHealth = 100;

   
    public void SetData(PlayerData playerData)
    {
        accoundID.Value = playerData.accountID;
        health.Value = playerData.health;
        attack.Value = playerData.attack;
        transform.position = playerData.position;
    }
    void Start()
    {
        health.Value = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    public override void OnNetworkDespawn()
    {
        GameManager.Instance.playerStatesByAccoundID[accoundID.Value.ToString()]
    = new PlayerData(accoundID.Value.ToString(),
    transform.position,
    health.Value,
    attack.Value);

        print("me desconecte papay "+NetworkManager.Singleton.LocalClientId);
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                Vector3 direction = (hit.point - firepoint.position);
                direction.y = 0; 
                direction.Normalize(); 
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
        health.Value -= damage;

        if (health.Value <= 0)
        {
            health.Value = 0;
            Debug.Log($"El jugador {accoundID} ha sido derrotado.");
        }
    }

    [Rpc(SendTo.Server)]
    public void SHOOTRpc(Vector3 direction) 
    {
        GameObject proj = Instantiate(proyectil, firepoint.position, Quaternion.LookRotation(direction));

        Proyectibles proyectilScript = proj.GetComponent<Proyectibles>();
       /* if (proyectilScript != null)
        {
            proyectilScript.SetDamage(attack.Value);
        }*/
        proj.GetComponent<NetworkObject>().Spawn(true);

        proj.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

}
public class PlayerData
{
    public string accountID;
    public Vector3 position;
    public int health;
    public int attack;

    public PlayerData(string id, Vector3 pos, int hp, int atk)
    {
        accountID = id;
        position = pos;
        health = hp;
        attack = atk;
    }
}