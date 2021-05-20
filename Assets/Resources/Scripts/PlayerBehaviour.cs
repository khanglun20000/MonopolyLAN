using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBehaviour : NetworkBehaviour
{
    private readonly Transform[] routes = new Transform[40]; // normal route
    private readonly Transform[] routes1 = new Transform[6]; // lucky route
    private readonly Transform[] routes2 = new Transform[6]; // unlucky route
    private readonly Transform[] routes3 = new Transform[6]; // ox1 route
    private readonly Transform[] routes4 = new Transform[6]; // ox2 route
    private int routeToGo, specRouteToGo;

    private readonly Transform[] tiles = new Transform[40]; // normal tiles
    private readonly Transform[] specTiles = new Transform[20]; // special tiles

    private int curTile, specCurTile; // index of current tile the player is on

    private int stepNum;

    private float tParam;

    private Vector3 objectPosition;

    [SerializeField] private float speedModifier;

    private bool coroutineAllowed;// once each call in update
    private bool moveAllowed; // avoid having new move while moving
    [SerializeField] private bool isStopping, isNormalTile = true, isBtw2Routes = false, isInSpecRoute;

    enum RouteTypes
    {
        normalRoutes,
        luckyRoutes,
        unluckyRoutes,
        ox1Routes,
        ox2Routes
    }
    private RouteTypes rt;

    enum PlayerTurn
    {
        isMyTurn,
        notMyTurn
    }
    [SerializeField]private PlayerTurn pt;


    private void Awake()
    {
        GameObject map = GameObject.Find("Map");
        GameObject Route = GameObject.Find("Routes");

        // get the normal route (0-39)
        for (int i = 0; i < routes.Length; i++)
        {
            routes[i] = Route.transform.GetChild(i);
        }
        // get the lucky route (40-45)
        for (int i = 0; i< routes1.Length; i++)
        {
            routes1[i] = Route.transform.GetChild(i + 40);
        }
        // get the unlucky route (46-51)
        for (int i = 0; i < routes2.Length; i++)
        {
            routes2[i] = Route.transform.GetChild(i + 46);
        }
        // get the ox1 route (52-57)
        for (int i = 0; i < routes3.Length; i++)
        {
            routes3[i] = Route.transform.GetChild(i + 52);
        }
        // get the ox2 route (58-63)
        for (int i = 0; i < routes4.Length; i++)
        {
            routes4[i] = Route.transform.GetChild(i + 58);
        }
        // get the normal tiles (0-39)
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = map.transform.GetChild(i);
        }
        // get the special tiles (40-59)
        for (int i = 0; i < specTiles.Length; i++)
        {
            specTiles[i] = map.transform.GetChild(i + 40);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        rt = RouteTypes.normalRoutes;
        curTile = 0;
        routeToGo = 0;
        specRouteToGo = 0;
        tParam = 0f;
        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Space) && moveAllowed == false && pt == PlayerTurn.isMyTurn)
            {
                moveAllowed = true;
                if (isBtw2Routes)
                {
                    // roll 1 dice
                    stepNum = Random.Range(1, 7);
                }
                else
                {
                    // roll 2 dices
                    stepNum = Random.Range(2, 13);
                }
            }
        }

        if ((curTile == 17 && isStopping) || (specCurTile >= 0 && specCurTile <= 5 && isInSpecRoute)) 
        {
            if (curTile == 17 && isStopping)
            {
                // change index according to the differences between jump out index
                specCurTile = 0;
                curTile += 6;
                isInSpecRoute = true;
            }
            isBtw2Routes = true;
            isNormalTile = false;
            rt = RouteTypes.luckyRoutes;
            if (moveAllowed && coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(specRouteToGo, routes1));
            }
        }
        else if ((curTile == 37 && isStopping) || (specCurTile >= 6 && specCurTile <= 11 && isInSpecRoute))
        {
            if (curTile == 37 && isStopping)
            {
                specCurTile = 6;
                curTile = 3;
                isInSpecRoute = true;
            }
            isBtw2Routes = true;
            isNormalTile = false;
            rt = RouteTypes.unluckyRoutes;
            if (moveAllowed && coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(specRouteToGo, routes2));
            }
        }
        else if ((curTile == 6  && isStopping)|| (specCurTile >= 12 && specCurTile <= 17 && isInSpecRoute))
        {
            if (curTile == 6 && isStopping)
            {
                specCurTile = 12;
                curTile += 2;
                isInSpecRoute = true;
            }
            isBtw2Routes = true;
            isNormalTile = false;
            rt = RouteTypes.ox1Routes;
            if (moveAllowed && coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(specRouteToGo, routes3));
            }
        }
        else if ((curTile == 26 && isStopping) || (specCurTile >= 18 && specCurTile <= 23 && isInSpecRoute))
        {
            if (curTile == 26 && isStopping)
            {
                specCurTile = 18;
                curTile += 2;
                isInSpecRoute = true;
            }
            isBtw2Routes = true;
            isNormalTile = false;
            rt = RouteTypes.ox2Routes;
            if (moveAllowed && coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(specRouteToGo, routes4));
            }
        }
        else
        {
            isBtw2Routes = false;
            isNormalTile = true;
            isInSpecRoute = false;
            rt = RouteTypes.normalRoutes;
            if (moveAllowed && coroutineAllowed)
            {
                StartCoroutine(GoByTheRoute(routeToGo, routes));
            }
        }
    }

    // follow a route
    private IEnumerator GoByTheRoute(int routeNum, Transform[] routesToGo)
    {
        coroutineAllowed = false;
        isStopping = false;

        Vector3 p0 = routesToGo[routeNum].GetChild(0).position;
        Vector3 p1 = routesToGo[routeNum].GetChild(1).position;
        Vector3 p2 = routesToGo[routeNum].GetChild(2).position;
        Vector3 p3 = routesToGo[routeNum].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }
        tParam = 0f;

        // count steps left
        stepNum--;
        if (stepNum <= 0)
        {
            moveAllowed = false;
            isStopping = true;
            pt = PlayerTurn.notMyTurn;
        }

        routeNum++;

        if (isNormalTile)
        {
            curTile++;
            routeToGo = routeNum;

            // back to first route if out of array
            if (routeToGo > routes.Length - 1)
            {
                routeToGo = 0;
            }
            // back to first tile if out of array
            if (curTile > tiles.Length - 1)
            {
                curTile = 0;
            }
        }
        else
        {
            specCurTile++;
            curTile++;
            specRouteToGo = routeNum;
            
            if(specRouteToGo > routes1.Length - 1)
            {
                specRouteToGo = 0;
                isInSpecRoute = false;
                // choose escape route when out of special route
                switch (rt)
                {
                    case RouteTypes.luckyRoutes:
                        routeToGo = 29;
                        break;
                    case RouteTypes.unluckyRoutes:
                        routeToGo = 9;
                        break;
                    case RouteTypes.ox1Routes:
                        routeToGo = 14;
                        break;
                    case RouteTypes.ox2Routes:
                        routeToGo = 34;
                        break;
                }
            }

            if (curTile > tiles.Length - 1)
            {
                curTile = 0;
            }
        }
        coroutineAllowed = true;
        
    }
}
