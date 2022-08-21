using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Timers;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
    //Start is called before the first frame update
    private DesignManager designManager;
    public TMP_Text totalPriceText;
    private float totalValue;
    public float BetValue;
    private float AllBetValue = 0;
    public TMP_Text[] BetTexts;
    private float[] BetValues = new float[4] { 0, 0, 0, 0 };
    public TMP_Text BetBtnText;
    public TMP_Text ClearBtnText;
    public Button BetBtn;
    public Button ClearBtn;
    public static APIForm apiform;
    public static Globalinitial _global;
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    BetPlayer _player;
    int n = 1;
    public bool clickflag = true;
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];
        float i_balance = float.Parse(usersInfo["amount"]);
        totalValue = i_balance;
        totalPriceText.text = totalValue.ToString("F2");
    }
    void Start()
    {
        _player = new BetPlayer();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    GameReady("Ready");
#endif
        BetBtnText.text = "Deal";
        ClearBtnText.text = "Clear";
        totalValue = 10000f;
        totalPriceText.text = totalValue.ToString("F2");
        StartCoroutine(firstServer());
        designManager = FindObjectOfType<DesignManager>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator firstServer()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", "_player.username");
        form.AddField("token", "_player.token");
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/CardOder", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                designManager.cardOrderArray = apiform.cardOder;
                StartCoroutine(designManager.CardOder());
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
        }
    }
    public void BetOrRebet()
    {
        if (BetValue > 0)
        {
            BetBtnAction();
            // if (totalValue >= BetValue * n)
            // {
            //     if (totalValue >= 5)
            //     {

            //     }
            //     else
            //     {
            //         StartCoroutine(alert("Insufficient balance!", "other"));
            //     }
            // }
            // else
            // {
            //     StartCoroutine(alert("Insufficient balance!", "other"));
            // }
        }
        else
        {
            StartCoroutine(alert("Set balance!", "other"));
        }
    }
    private void BetBtnAction()
    {
        switch (BetBtnText.text)
        {
            case "Deal":
                StartCoroutine(designManager.CardThrow(0, 5));
                n = 1;
                BetValues[0] = BetValues[0] + BetValue;
                AllBetValue = AllBetValue + BetValues[0];
                BetBtnText.text = "1X";
                ClearBtnText.text = "Fold";
                break;
            case "1X":
                StartCoroutine(designManager.CardRotate(3, 4));
                n = 1;
                BetValues[1] = BetValues[1] + BetValue;
                AllBetValue = AllBetValue + BetValues[1];
                BetBtnText.text = "2X";
                break;
            case "2X":
                StartCoroutine(designManager.CardRotate(4, 5));
                n = 2;
                BetValues[2] = BetValues[2] + BetValue * 2;
                AllBetValue = AllBetValue + BetValues[2];
                BetBtnText.text = "3X";
                break;
            case "3X":
                StartCoroutine(designManager.CardRotate(5, 6));
                n = 3;
                BetValues[3] = BetValues[3] + BetValue * 3;
                AllBetValue = AllBetValue + BetValues[3];
                StartCoroutine(Server());
                BetBtnText.text = "Repeat";
                ClearBtnText.text = "Clear";
                break;
            case "Repeat":
                n = 1;
                BetValues = new float[4] { BetValue, 0, 0, 0 };
                AllBetValue = BetValue;

                StartCoroutine(firstServer());
                BetBtnText.text = "1X";
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            BetTexts[i].text = BetValues[i].ToString();
        }
        StartCoroutine(UpdateCoinsAmount(totalValue, totalValue - BetValue * n));
    }
    public void NewOrFold()
    {
        StartCoroutine(designManager.CardRotate(3, 6));
        switch (ClearBtnText.text)
        {
            case "Clear":
                ClearBtnText.text = "Fold";
                break;
            case "Fold":
                BetBtnText.text = "Repeat";
                ClearBtnText.text = "Clear";
                break;
        }
    }
    public IEnumerator Server()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", "_player.username");
        form.AddField("token", "_player.token");
        form.AddField("betAmount", AllBetValue.ToString());
        form.AddField("amount", totalValue.ToString("F2"));
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/betting", form);
        yield return new WaitForSeconds(0.00001f);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                yield return new WaitForSeconds(0.5f);
                UnityEngine.Debug.Log(strdata);
                // if (apiform.activeArray.Length > 0)
                // {
                //     if (apiform.msg != "")
                //     {
                //         StartCoroutine(alert(apiform.msg, "win"));
                //     }
                //     for (int i = 0; i < apiform.activeArray.Length; i++)
                //     {
                //         string name = "card" + (apiform.activeArray[i] + 1);
                //         GameObject.Find(name).GetComponent<SpriteRenderer>().material.color = Color.yellow;
                //     }
                // }

                // StartCoroutine(UpdateCoinsAmount(totalValue, apiform.total));
            }
            else
            {
                StartCoroutine(alert(apiform.serverMsg, "other"));
                StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + AllBetValue));
            }
        }
        else
        {
            StartCoroutine(alert("Can't find server!", "other"));
            StartCoroutine(UpdateCoinsAmount(totalValue, totalValue + AllBetValue));
        }
    }
    public IEnumerator alert(string msg, string state)
    {
        if (state == "win")
        {
            AlertController.isWin = true;
        }
        else
        {
            AlertController.isLose = true;
        }
        GameObject.Find("alert").GetComponent<TMP_Text>().text = msg;
        yield return new WaitForSeconds(3f);
        AlertController.isWin = false;
        AlertController.isLose = false;
        yield return new WaitForSeconds(0.5f);
    }
    private IEnumerator UpdateCoinsAmount(float preValue, float changeValue)
    {
        // Animation for increasing and decreasing of coins amount
        const float seconds = 0.2f;
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            totalPriceText.text = Mathf.Floor(Mathf.Lerp(preValue, changeValue, (elapsedTime / seconds))).ToString();
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        totalValue = changeValue;
        totalPriceText.text = totalValue.ToString();
    }
    public IEnumerator cardActiveClear()
    {
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator BetClear()
    {
        yield return new WaitForSeconds(0.1f);
    }
    public IEnumerator betformat()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
public class BetPlayer
{
    public string username;
    public string token;
}