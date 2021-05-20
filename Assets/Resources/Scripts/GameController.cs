using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

enum GameState
{
    Setting,
    PlayerTurn,
    Action
}
public class GameController : NetworkBehaviour
{
    [SerializeField] private GameState state;
    [SerializeField] private Transform[] Tiles;
    [SerializeField] private Transform playerPf;


    private int turnNum;
    private void Awake()
    {
        state = GameState.Setting;
        

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.Setting:
                break;
            case GameState.PlayerTurn:
                RollDice();
                
                break;
            case GameState.Action:
                
                break;
        }
    }
    void RollDice()
    {

    }
}
