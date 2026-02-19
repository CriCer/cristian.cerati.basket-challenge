using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum GameState{
        Start,
        Playing,
        Finished,
        Paused
    }

    public float gameTime = 60;

    [SerializeField]
    TMP_Text startText = null;
    //[SerializeField]
    //slider

    float remainingTime;
    GameState gameState = GameState.Start;

    void Start()
    {
        StartCoroutine(StartGameTimer());
    }

    public void Update()
    {
        if (gameState == GameState.Playing)
        {
            if (remainingTime <= 0)
            {
                EndGame();
            }
            else
            {
                remainingTime -= Time.deltaTime;
                startText.text = remainingTime.ToString();
            }
        }
        
    }

    public void EndGame()
    {
        gameState = GameState.Finished;
        //remove inputs
    }

    public IEnumerator StartGameTimer()
    {
        startText.text = "3";
        yield return new WaitForSeconds(1);
        startText.text = "2";
        yield return new WaitForSeconds(1);
        startText.text = "1";
        yield return new WaitForSeconds(1);
        startText.text = "GO";
        remainingTime = gameTime;
        gameState = GameState.Playing;
        yield return new WaitForSeconds(1);
        startText.text = "";
        
    }
}
