using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] candies;
    public GameObject[,] allCandies;
    public GameObject destroyEffect;
    private BackgroundTile[,] allTiles;
    private FindMatches findMatches;
    public int baseScoreValue = 100;
    private int scoreChainValue = 1;
    private ScoreManager scoreManager;
    private GoalManager goalManager;
    private SoundManager soundManager;
    public int[] scoreGoals;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allCandies = new GameObject[width, height];
        Setup();
        currentState = GameState.pause;
    }

    private void Setup()
    {
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j<height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent=this.transform;
                backgroundTile.name="( " + i + ", " + j + " )";
                int candyToUse = Random.Range(0, candies.Length);

                while (MatchesAt(i, j, candies[candyToUse]))
                {
                    candyToUse = Random.Range(0, candies.Length);
                }

                GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                candy.GetComponent<Candy>().row = j;
                candy.GetComponent<Candy>().column = i;
                candy.transform.parent = this.transform;
                candy.name= "( " + i + ", " + j + " )";
                allCandies[i, j] = candy;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject candy)
    {
        if (column > 1 && row > 1)
        {
            if (allCandies[column - 1, row].tag == candy.tag && allCandies[column - 2, row].tag == candy.tag)
            {
                return true;
            }
            if (allCandies[column, row - 1].tag == candy.tag && allCandies[column, row - 2].tag == candy.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allCandies[column, row - 1].tag == candy.tag && allCandies[column, row - 2].tag == candy.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allCandies[column - 1, row].tag == candy.tag && allCandies[column - 2, row].tag == candy.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (goalManager!= null)
        {
            goalManager.CompareGoal(allCandies[column, row].tag.ToString());
            goalManager.UpdateGoals();
        }

        if (allCandies[column, row].GetComponent<Candy>().isMatched)
        {
            findMatches.currentMatches.Remove(allCandies[column, row]);
            Instantiate(destroyEffect, allCandies[column, row].transform.position, Quaternion.identity);
            Destroy(allCandies[column, row]);
            allCandies[column, row] = null;
        }

        if(soundManager != null)
        {
            soundManager.PlayDestroyNoise();
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j<height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreseRowCo());
        scoreManager.IncreaseScore(baseScoreValue * scoreChainValue);
    }

    private IEnumerator DecreseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCandies[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allCandies[i, j].GetComponent<Candy>().row -= nullCount;
                    allCandies[i, j] = null;
                }
            }
            nullCount=0;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j<height; j++)
            {
                if (allCandies[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject candy = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                    allCandies[i, j] = candy;
                    candy.GetComponent<Candy>().row = j;
                    candy.GetComponent<Candy>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i<width; i++)
        {
            for (int j = 0; j<height; j++)
            {
                if (allCandies[i, j] != null)
                {
                    if (allCandies[i, j].GetComponent<Candy>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            scoreChainValue ++;
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
        scoreChainValue = 1;
    }
}

