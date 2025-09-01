using UnityEngine;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public GameObject playerPrefab;
    public GameObject buffPrefab;
    public GameObject enemyPrefab;

    public float BuffSpawnCount = 4;
    public float currentBuffCount = 0;

    public float EnemySpawnCount = 10;
    public float currentEnemyCount = 0;
    void Start()
    {

    }
   public override void OnNetworkSpawn()
    {
        print("CurrentPlayer" + NetworkManager.Singleton.ConnectedClients.Count);
        SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
    }
    [Rpc(SendTo.Server)]
    public void SpawnPlayerRpc(ulong id)
    {
        GameObject player = Instantiate(playerPrefab);
        //player.GetComponent<NetworkObject>().Spawn(true);
        player.GetComponent<SimplePlayerController>().PlayerID.Value = id;
        player.GetComponent<NetworkObject>().SpawnWithOwnership(id, true);
    }
    void Update()
    {
        if (IsServer && NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            currentBuffCount += Time.deltaTime;
            if (currentBuffCount > BuffSpawnCount)
            {
                Vector3 randomPos = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
                GameObject buff = Instantiate(buffPrefab, randomPos, Quaternion.identity);
                buff.GetComponent<NetworkObject>().Spawn(true);
                currentBuffCount = 0;
            }
            currentEnemyCount += Time.deltaTime;
            if (currentEnemyCount > EnemySpawnCount)
            {
                Vector3 randomPos = new Vector3(Random.Range(-10, 10), 1f, Random.Range(-10, 10));
                GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
                enemy.GetComponent<NetworkObject>().Spawn(true);
                currentEnemyCount = 0;
            }
        }
    }
}
