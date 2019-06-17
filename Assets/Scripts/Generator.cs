using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Generator : MonoBehaviour
{
    public int width, height;
    public GameState currentState = GameState.move;
    public GameObject gridPrefabs;
    public GameObject[] dots;
    private GridHandler[,] allTiles;
    public GameObject[,] allDots;
    public SwipeDots swiper;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GridHandler[width, height];
        allDots = new GameObject[width, height];
        swiper = GameObject.FindObjectOfType<SwipeDots>();
        SettingUp();
    }

    void SettingUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 gridPos = new Vector2(i, j);
                GameObject gridGO = Instantiate(gridPrefabs, gridPos, Quaternion.identity) as GameObject;
                gridGO.transform.parent = this.transform;
                gridGO.name = "(" + i + "," + j + ")";

                //dotsGenerate
                int randomIdx = Random.Range(0, dots.Length);

                int maxIterations = 0;
                while (MatchesAt(i,j,dots[randomIdx]) && maxIterations < 100)
                {
                    randomIdx = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[randomIdx], gridPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }
        }
    }

    bool MatchesAt(int column, int row, GameObject dots)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column -1, row].tag == dots.tag && allDots[column - 2, row].tag == dots.tag)
            {
                return true;
            }
            if (allDots[column, row-1].tag == dots.tag && allDots[column, row-2].tag == dots.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == dots.tag && allDots[column, row - 2].tag == dots.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column-1, row].tag == dots.tag && allDots[column-2, row].tag == dots.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void DestroyDotsAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<SwipeDots>().isMatch)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyDotsGO()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    DestroyDotsAt(i, j);
                }
            }
        }
        StartCoroutine(decraseRow());
    }

    IEnumerator decraseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<SwipeDots>().row -= nullCount;
                    allDots[i, j].GetComponent<SwipeDots>().prevCol = allDots[i, j].GetComponent<SwipeDots>().column;
                    allDots[i, j].GetComponent<SwipeDots>().prevRow = allDots[i, j].GetComponent<SwipeDots>().row;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);

        StartCoroutine(fillBoardCounter());
    }

    void RefillDots()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] == null)
                {
                    Vector2 newDotPos = new Vector2(i, j);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject newDot = Instantiate(dots[dotToUse], newDotPos, Quaternion.identity);
                    newDot.transform.parent = this.transform;
                    allDots[i, j] = newDot;
                }
            }
        }
    }

    bool MatchedOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i,j] != null)
                {
                    if (allDots[i,j].GetComponent<SwipeDots>().isMatch)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator fillBoardCounter()
    {
        RefillDots();
        yield return new WaitForSeconds(.5f);

        while (MatchedOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyDotsGO();
        }

        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }

    public void MoveDots()
    {
        if (swiper.angleOfTouch <= 45 && swiper.angleOfTouch > -45 && swiper.column < width - 1)
        {
            swiper.otherDot = allDots[swiper.column + 1, swiper.row];
            swiper.otherDot.GetComponent<SwipeDots>().column -= 1;
            swiper.column += 1;
        }
        else if (swiper.angleOfTouch > 45 && swiper.angleOfTouch <= 135 && swiper.row < height - 1)
        {
            swiper.otherDot = allDots[swiper.column, swiper.row + 1];
            swiper.otherDot.GetComponent<SwipeDots>().row -= 1;
            swiper.row += 1;
        }
        else if (swiper.angleOfTouch > 135 || swiper.angleOfTouch <= -135 && swiper.column > 0)
        {
            swiper.otherDot = allDots[swiper.column - 1, swiper.row];
            swiper.otherDot.GetComponent<SwipeDots>().column += 1;
            swiper.column -= 1;
        }
        else if (swiper.angleOfTouch < -45 && swiper.angleOfTouch >= -135 && swiper.row > 0)
        {
            swiper.otherDot = allDots[swiper.column, swiper.row - 1];
            swiper.otherDot.GetComponent<SwipeDots>().row += 1;
            swiper.row -= 1;
        }

        StartCoroutine(reverseDots());
    }

    public IEnumerator reverseDots()
    {
        yield return new WaitForSeconds(.6f);
        if (swiper.otherDot != null)
        {
            if (!swiper.isMatch && !swiper.otherDot.GetComponent<SwipeDots>().isMatch)
            {
                swiper.otherDot.GetComponent<SwipeDots>().row = swiper.row;
                swiper.otherDot.GetComponent<SwipeDots>().column = swiper.column;
                swiper.row = swiper.prevRow;
                swiper.column = swiper.prevCol;
                yield return new WaitForSeconds(.5f);
                currentState = GameState.move;
            }
            else
            {
                DestroyDotsGO();
            }
            swiper.otherDot = null;
        }
    }
}
