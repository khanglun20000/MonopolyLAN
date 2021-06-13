using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [SyncVar] public bool isTurn = false;

    [SyncVar] public float turnTimer = 20;

    [SyncVar] public bool ready = false;

    public PlayerBehaviour pb;
    private void Awake()
    {
        if(isServer)
            SetupServerPlayer();
    }
    // Update is called once per frame
    void Update()
    {
        if(isTurn && isServer) CountDownTime();
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(this);

        base.OnStartClient();
        if (isServer)
        {
            StartPlayer();
        }
        GameController.Instance.RegisterNetworkPlayer(this);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        pb.SetupLocalPlayer();
    }

    [Server]
    public void SetupServerPlayer()
    {
        pb.isServer = true;
    }

    [Server]
    void CountDownTime()
    {
        turnTimer -= Time.deltaTime;
        if (turnTimer <= 0)
        {
            StartCoroutine(GameController.Instance.SvAlterTurns());
        }
    }

    [Server]
    public void StartPlayer()
    {
        ready = true;
    }

    [Server]
    public void StartGame()
    {
        if (isLocalPlayer)
        {
            SvTurnStart();
        }
    }

    [Command]
    public void CmdAlterTurns()
    {
        StartCoroutine(GameController.Instance.SvAlterTurns());
    }

    [Command]
    public void CmdBonusTurn()
    {
        StartCoroutine(GameController.Instance.SvBonusTurn());
    }

    [Server]
    public void SvTurnStart()
    {
        turnTimer = 20;
        isTurn = true;
        CltRPCTurnStart();
    }

    [Server]
    public void SvTurnEnd()
    {
        isTurn = false;
        CltRPCTurnEnd();
    }

    [ClientRpc]
    public void CltRPCTurnStart()
    {
        pb.TurnStart();
    }

    [ClientRpc]
    public void CltRPCTurnEnd()
    {
        pb.TurnEnd();
    }

    
    public override void OnStopClient()
    {
        base.OnStopClient();
        GameController.Instance.DeregisterNetworkPlayer(this);
    }

    public void OnTurnChange(bool turn)
    {
        if (isLocalPlayer)
        {
            //play turn sound 
        }
    }

}
