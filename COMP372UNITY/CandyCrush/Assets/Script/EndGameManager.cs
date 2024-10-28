using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public int counterMoveValue;
    public int counterTimeValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public TMP_Text moveCounter;
    public TMP_Text timeCounter;
    public EndGameRequirements requirements;
    public int currentMovesCounterValue;
    public int currentTimesCounterValue;
    private float timerSeconds;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        SetupGame();
    }

    void SetupGame()
    {
        currentMovesCounterValue = requirements.counterMoveValue;
        currentTimesCounterValue = requirements.counterTimeValue;
        moveCounter.text = currentMovesCounterValue.ToString();
        timeCounter.text = currentTimesCounterValue.ToString();
    }

    public void DecreaseTimeCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentTimesCounterValue--;
            timeCounter.text = currentTimesCounterValue.ToString();
            if (currentTimesCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void DecreaseMoveCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentMovesCounterValue--;
            moveCounter.text = currentMovesCounterValue.ToString();
            if (currentMovesCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;

        currentTimesCounterValue = 0;
        timeCounter.text = currentTimesCounterValue.ToString();

        currentMovesCounterValue = 0;
        moveCounter.text = currentMovesCounterValue.ToString();

        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Retry()
    {
        SceneManager.LoadScene("Level1Scene");
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTimesCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseTimeCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
