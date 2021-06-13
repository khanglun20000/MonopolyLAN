using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private Transform spawnPoint;

    private void Awake()
    {
        spawnPoint = FindObjectOfType<GameController>().Tiles[0];
    }

    public override void OnStartServer() => NetworkManagerLobby.OnServerReadied += SpawnPlayer;

    [ServerCallback]
    private void OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPoint.position + new Vector3(0,8f,0), spawnPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, playerInstance);
    }
}
