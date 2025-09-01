using Unity.Netcode;
using UnityEngine;
public class RandomBuff : NetworkBehaviour
{
    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ulong playerID = other.GetComponent<NetworkObject>().OwnerClientId;
            AddBuffToPlayerRpc(playerID);
        }
    }
    [Rpc(SendTo.Server)]
    private void AddBuffToPlayerRpc(ulong playerID)
    {
        print("Aplicar buff a :" + playerID);
        //AddHPToPlayerRpc(playerID, 10);

        GetComponent<NetworkObject>().Despawn(true);
    }
   
}
