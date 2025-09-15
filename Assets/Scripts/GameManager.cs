using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject playerPrefab;
    public GameObject buffPrefab;
    public GameObject enemyPrefab;

    public float BuffSpawnCount = 4;
    public float currentBuffCount = 0;

    public float EnemySpawnCount = 10;
    public float currentEnemyCount = 0;

    public Dictionary<string, PlayerData> playerStatesByAccoundID = new();

    public Action OnConnection;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this; 
        }
    }
    void Start()
    {

    }
    [Rpc(SendTo.Server)]
    public void RegisterPlayerServerRpc(string accountID, ulong clientID)
    {
        PlayerData dataToSpawn;
        if (!playerStatesByAccoundID.TryGetValue(accountID, out PlayerData data))
        {
            Debug.Log($"Registrando nuevo jugador: {accountID} con ClientID: {clientID}");

            PlayerData NewData = new PlayerData(accountID, Vector3.zero, 100, 5);
            playerStatesByAccoundID[accountID] = NewData;
            dataToSpawn = NewData;
            //Instanciar al player con al data!!!!!
        }
        else
        {
            Debug.Log($"Jugador {accountID} ha regresado. Cargando datos.");
            dataToSpawn = data;
        }
        SpawnPlayerServer(clientID, dataToSpawn);

    }
    public void SpawnPlayerServer(ulong ID, PlayerData data)
    {
        if (!IsServer) return;
        GameObject player = Instantiate(playerPrefab);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(ID, true);

        player.GetComponent<SimplePlayerController>().SetData(data);
    }

    
    public override void OnNetworkSpawn()
    {
        print("CurrentPlayer" + NetworkManager.Singleton.ConnectedClients.Count);
      
        if (IsServer)
        {

            NetworkManager.Singleton.OnClientDisconnectCallback += HandleDisconnect;
        }
        OnConnection?.Invoke();
        //SpawnPlayerRpc(NetworkManager.Singleton.LocalClientId);
    }
    public override void OnNetworkDespawn()
    {
       
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleDisconnect;
        }
    }
    private void HandleDisconnect(ulong clientId)
    {
        print(clientId + " Desconetado");
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
            /*if (currentBuffCount > BuffSpawnCount)
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
            }*/
        }
    }
}
