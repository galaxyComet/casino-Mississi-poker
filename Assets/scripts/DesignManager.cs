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
public class DesignManager : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("material")]
    private Transform CardObject;
    public Material[] cardMaterial;
    public Transform prefab;
    private GameManager gameManager;
    private float[] cardX;
    private float cardY = 0.1415f;
    private float[] cardZ;
    private float[] movecardX = new float[5] { -0.1049f, 0.0664f, -0.1937f, -0.0184f, 0.1602f };
    private float movecardY = 0.1012f;
    private float[] movecardZ = new float[5] { -0.121f, -0.121f, 0.0718f, 0.0718f, 0.0718f };
    public int[] cardOrderArray;
    void Start()
    {
        cardX = new float[5];
        cardZ = new float[5];
        gameManager = FindObjectOfType<GameManager>();
    }
    public IEnumerator CardOder()
    {
        const float seconds = 0.2f;
        float time = 0;
        float beforeX = 0.524f;
        float beforeZ = 0.2f;

        cardX[0] = beforeX;
        cardZ[0] = beforeZ;
        while (time < seconds)
        {
            for (int i = 0; i < 52; i++)
            {
                float nextX = beforeX + 0.0031f;
                float nextZ = beforeZ + 0.0029f;
                CardObject = Instantiate(prefab, Vector3.Lerp(new Vector3(beforeX, cardY, beforeZ), new Vector3(nextX, cardY, nextZ), time / seconds), Quaternion.identity);
                if (i > 0 && i < 5)
                {
                    cardX[i] = nextX;
                    cardZ[i] = nextZ;
                }
                CardObject.name = "card" + (i + 1);
                CardObject.GetComponent<SpriteRenderer>().material = cardMaterial[cardOrderArray[i]];
                CardObject.transform.localScale = new Vector3(0.008277704f, -0.006732143f, 0.5f);
                CardObject.transform.eulerAngles = new Vector3(31.426f, 48.156f, 90);
                time += Time.deltaTime;
                beforeX = nextX;
                beforeZ = nextZ;
                yield return new WaitForEndOfFrame();
            }
        }
        yield return new WaitForSeconds(1f);
    }
    void Update()
    {

    }
    public IEnumerator ThrowedCardClear(bool flag)
    {
        for (int i = 0; i < 52; i++)
        {
            string name = "card" + (i + 1);
            Destroy(GameObject.Find(name));
        }
        // StartCoroutine(gameManager.firstServer());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(CardThrow(0, 6));
        // StartCoroutine(gameManager.beginServer());
    }
    public IEnumerator CardThrow(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            float time = 0;
            const float seconds = 0.15f;
            string name = "card" + (i + 1);
            while (time < seconds)
            {
                GameObject.Find(name).transform.position = Vector3.Lerp(new Vector3(cardX[i], cardY, cardZ[i]), new Vector3(movecardX[i], movecardY, movecardZ[i]), time / seconds);
                if (i > 1 && i < 5)
                {
                    GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(30, 90, 90)), Quaternion.Euler(new Vector3(90, 90, 90)), time / seconds);
                }
                else
                {
                    GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(30, 90, 90)), Quaternion.Euler(new Vector3(-90, 90, 90)), time / seconds);
                }
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameObject.Find(name).transform.position = new Vector3(movecardX[i], movecardY, movecardZ[i]);
            if (i > 1 && i < 5)
            {
                GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(90, 90, 90));
            }
            else
            {
                GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 90));
            }
            yield return new WaitForSeconds(0.1f);
        }
        // yield return new WaitForSeconds(1.5f);
        // gameManager.betbtn.interactable = true;
        // gameManager.foldbtn.interactable = true;
    }
    public IEnumerator CardRotate(int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            float time = 0;
            const float seconds = 0.15f;
            string name = "card" + i;
            while (time < seconds)
            {
                GameObject.Find(name).transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(90, 90, 90)), Quaternion.Euler(new Vector3(-90, 90, 90)), time / seconds);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            GameObject.Find(name).transform.rotation = Quaternion.Euler(new Vector3(-90, 90, 90));
        }
        yield return new WaitForSeconds(0.1f);
    }
}
