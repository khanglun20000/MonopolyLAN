using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Status : MonoBehaviour
{
    public static UI_Status instance;

    [SerializeField] private RectTransform[] points;

    private void Awake()
    {
        if (!instance)
            instance = this;
        NetworkPlayer.OnSetDisplayName += DisplayName;
        NetworkPlayer.OnDisplayMoney += DisplayMoney;
    }
    public void EnablePoint()
    {
        for (int i = 0; i < GameController.Instance.players.Count; i++)
        {
            points[i].gameObject.SetActive(true);
        }
    }


    public void DisplayName(string playerName)
    {
        points[0].GetComponentInChildren<TextMeshProUGUI>().text = playerName;
    }
    public void DisplayMoney(int playerMoney)
    {
        points[0].GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = playerMoney.ToString();
    }
}
