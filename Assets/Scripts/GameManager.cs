using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct BackboardBonus
{
    public int bonusPoints;
    public int weight;
}

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
    float timeAfterShot = 0.5f;
    [SerializeField]
    TMP_Text backboardBonusText;
    [SerializeField]
    BackboardBonus[] backboardBonuses;
    [SerializeField]
    float backboardBonusTime = 5f;
    [SerializeField]
    [Tooltip("Percentage of backboard bonus spawning divided by 100 (0 is never, 1 is always)")]
    [Range(0, 1)]
    float bonusSpawnChance = 0.2f;

    int score = 0;
    int currentBackboardExtraPoints = 0;
    float backboardBonusTimer = 0.0f;
    float remainingTime = 0.0f;
    GameState gameState = GameState.Start;
    float bonusSpawnTimer;

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
            //Game timer
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

            //Backboard bonus timer
            if (backboardBonusTimer > 0f)
            {
                backboardBonusTimer -= Time.deltaTime;

                if (backboardBonusTimer <= 0f)
                {
                    currentBackboardExtraPoints = 0;

                    if (!backboardBonusText) return;
                    backboardBonusText.text = "";
                }
            }
            else
            {
                bonusSpawnTimer += Time.deltaTime;

                if (bonusSpawnTimer >= 1f)
                {
                    bonusSpawnTimer = 0f;
                    TrySpawnBackboardBonus();
                }
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
                AddScore(backboardShotScore + currentBackboardExtraPoints);
                StartCoroutine(WaitAndSetNewPosition());
                shotText = "Backboard shot! +" + (backboardShotScore + currentBackboardExtraPoints);
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

    int GetRandomBackboardBonus()
    {
        int totalWeight = 0;

        foreach (var bonus in backboardBonuses)
            totalWeight += bonus.weight;

        int randomValue = Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (var bonus in backboardBonuses)
        {
            currentWeight += bonus.weight;

            if (randomValue < currentWeight)
                return bonus.bonusPoints;
        }

        return backboardBonuses[0].bonusPoints;
    }

    public void StartBackboardBonus()
    {
        currentBackboardExtraPoints = GetRandomBackboardBonus();
        backboardBonusTimer = backboardBonusTime;

        if (!backboardBonusText) return;
        backboardBonusText.text = "+" + currentBackboardExtraPoints;
    }

    void TrySpawnBackboardBonus()
    {
        if (Random.value <= bonusSpawnChance)
            StartBackboardBonus();
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
