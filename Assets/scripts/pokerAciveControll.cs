using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pokerAciveControll : MonoBehaviour
{
    public int isActive = 0;
    private GameManager gameManager;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        StartCoroutine(pokerActive());
    }
    public IEnumerator pokerActive()
    {
        animator.SetInteger("isActive", 0);
        yield return new WaitForSeconds(0.5f);
        animator.SetInteger("isActive", isActive);
        switch (isActive)
        {
            case 0:
                gameManager.BetValue = 0;
                break;
            case 1:
                gameManager.BetValue = 100;
                break;
            case 2:
                gameManager.BetValue = 50;
                break;
            case 3:
                gameManager.BetValue = 25;
                break;
            case 4:
                gameManager.BetValue = 10;
                break;
            case 5:
                gameManager.BetValue = 5;
                break;
        }

    }
}
