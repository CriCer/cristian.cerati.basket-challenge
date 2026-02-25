using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public enum GameState{
        Start,
        Playing,
        Finished,
        Paused
    }

    public float gameTime = 60;
    public GameObject[] ballPositions;
    public PlayerManager player;
    public InputManager inputManager;
    public UIManager uiManager;

    [SerializeField]
    int perfectShotScore = 4;
    [SerializeField]
    int normalShotScore = 2;
    [SerializeField]
    int backboardShotScore = 3;
    [SerializeField]
    private float timeAfterShot = 0.5f;

    int score = 0;

    float remainingTime;
    GameState gameState = GameState.Start;

    void Start()
    {
        StartCoroutine(StartGameTimer());


        if (!player) return;
        if (!player.ball) return;

        player.ball.onShotEnd += BallScore;

        player.NewBallPosition(GetNewPosition());

        InputManager.disableInputs = true;
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
                
                if (!uiManager) return;
                uiManager.UpdateTimeSlider(Mathf.InverseLerp(0, gameTime, remainingTime));
            }
        }
        
    }

    void StartGame()
    {
        InputManager.disableInputs = false;
        remainingTime = gameTime;
        gameState = GameState.Playing;
    }

    void EndGame()
    {
        gameState = GameState.Finished;
        InputManager.disableInputs = true;

        if (!uiManager) return;
        uiManager.ShowResultScreen(score);

        if (!player) return;
        if (!player.ball) return;
        player.ball.ResetPosition(false);
    }

    void BallScore(BallHandler.ShotResult outcome)
    {
        if (!player) return;

        string shotText = "";

        switch (outcome)
        {
            case BallHandler.ShotResult.Miss:
                if (!player.ball) return;
                player.ball.ResetPosition();
                shotText = "Miss!";
                break;

            case BallHandler.ShotResult.Perfect:
                AddScore(perfectShotScore);
                StartCoroutine(WaitAndSetNewPosition());
                shotText = "Perfect shot! +" + perfectShotScore;
                break;

            case BallHandler.ShotResult.Normal:
                AddScore(normalShotScore);
                StartCoroutine(WaitAndSetNewPosition());
                shotText = "Normal shot! +" + normalShotScore;
                break;

            case BallHandler.ShotResult.Backboard:
                AddScore(backboardShotScore);
                StartCoroutine(WaitAndSetNewPosition());
                shotText = "Backboard shot! +" + backboardShotScore;
                break;

        }

        if (!uiManager) return;

        uiManager.DisplayInfoText(shotText);
        uiManager.UpdatePowerSlider(0);
    }

    Vector3 GetNewPosition()
    {
        int i = Random.Range(0, ballPositions.Length);
        return ballPositions[i].transform.position;
    }

    void AddScore(int newScore)
    {
        score += newScore;

        if (!uiManager) return;
        uiManager.UpdateScoreText(score);
    }

    #region Coroutines
    IEnumerator StartGameTimer()
    {
        if (!uiManager) yield break;

        uiManager.UpdateInfoText("3");
        yield return new WaitForSeconds(1);
        uiManager.UpdateInfoText("2");
        yield return new WaitForSeconds(1);
        uiManager.UpdateInfoText("1");
        yield return new WaitForSeconds(1);
        uiManager.UpdateInfoText("GO");
        StartGame();
        yield return new WaitForSeconds(1);
        uiManager.UpdateInfoText("");
        
    }

    IEnumerator WaitAndSetNewPosition()
    {
        yield return new WaitForSeconds(timeAfterShot);
        player.NewBallPosition(GetNewPosition());

    }
    #endregion
}
