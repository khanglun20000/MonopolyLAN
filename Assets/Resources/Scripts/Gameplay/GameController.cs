using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public Transform[] Tiles;

    public RectTransform[] spawnPointsUI;

    public List<NetworkPlayer> players;

    private int iActivePlayer;
    
    bool gameStarted = false;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new List<NetworkPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (players.Count > 0)
        {
            CheckPlayersReady();
        }
    }

    bool CheckPlayersReady()
    {
        bool playersReady = true;
        foreach (var player in players)
        {
            playersReady &= player.ready;
        }

        if (playersReady && !gameStarted)
        {
            FindObjectOfType<UI_Status>().EnablePoint();
            if (players[0].isServer)
            {
                Debug.Log("startgame");
                players[0].StartGame();
            }
            gameStarted = true;
        }

        return playersReady;
    }

    public void RegisterNetworkPlayer(NetworkPlayer player)
    {
        players.Add(player);
    }

    public void DeregisterNetworkPlayer(NetworkPlayer player)
    {
        players.Remove(player);
    }

    public IEnumerator SvAlterTurns()
    {
        players[iActivePlayer].SvTurnEnd();

        yield return new WaitForEndOfFrame();
        iActivePlayer = (iActivePlayer + 1) % players.Count;

        players[iActivePlayer].SvTurnStart();
    }
    public IEnumerator SvBonusTurn()
    {
        players[iActivePlayer].SvTurnEnd();
        yield return new WaitForEndOfFrame();
        players[iActivePlayer].SvTurnStart();
    }
}