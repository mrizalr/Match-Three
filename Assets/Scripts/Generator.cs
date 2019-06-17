using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public int width, height;
    public GameObject gridPrefabs;
    public GameObject[] dots;
    private GridHandler[,] allTiles;
    public GameObject[,] allDots;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new GridHandler[width, height];
        allDots = new GameObject[width, height];
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
    }
}
