using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDots : MonoBehaviour
{
    public int column, row;
    private Generator gen;
    private GameObject otherDot;
    private int prevCol, prevRow;
    private int targetX, targetY;
    private Vector2 startTouchPos, finalTouchPos, tempPos;
    public float angleOfTouch;
    public bool isMatch;
    private int swipeThreshold = 1;

    // Start is called before the first frame update
    void Start()
    {
        gen = FindObjectOfType<Generator>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        prevCol = column;
        prevRow = row;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDots();
        if (isMatch)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }

        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1f)
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .5f);
            if (gen.allDots[column, row] != this.gameObject)
            {
                gen.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1f)
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .5f);
            if (gen.allDots[column, row] != this.gameObject)
            {
                gen.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    private void OnMouseDown()
    {
        startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngel();
    }

    void CalculateAngel()
    {
        if (Mathf.Abs(finalTouchPos.y - startTouchPos.y) > swipeThreshold || Mathf.Abs(finalTouchPos.x - startTouchPos.x) > swipeThreshold)
        {
            angleOfTouch = Mathf.Atan2(finalTouchPos.y - startTouchPos.y, finalTouchPos.x - startTouchPos.x) * 180 / Mathf.PI;
            MoveDots();
        }
    }

    void MoveDots()
    {
        if (angleOfTouch <= 45 && angleOfTouch > -45 && column < gen.width-1)
        {
            otherDot = gen.allDots[column + 1, row];
            otherDot.GetComponent<SwipeDots>().column -= 1;
            column += 1;
        }
        else if (angleOfTouch > 45 && angleOfTouch <= 135 && row < gen.height-1)
        {
            otherDot = gen.allDots[column, row+1];
            otherDot.GetComponent<SwipeDots>().row -= 1;
            row += 1;
        }
        else if (angleOfTouch > 135 || angleOfTouch <= -135 && column > 0)
        {
            otherDot = gen.allDots[column -1, row];
            otherDot.GetComponent<SwipeDots>().column += 1;
            column -= 1;
        }
        else if (angleOfTouch < -45 && angleOfTouch >= -135 && row > 0)
        {
            otherDot = gen.allDots[column, row - 1];
            otherDot.GetComponent<SwipeDots>().row += 1;
            row -= 1;
        }

        StartCoroutine(reverseDots());
    }

    void CheckDots()
    {
        if (column > 0 && column < gen.width-1)
        {
            GameObject leftDotsCheck = gen.allDots[column - 1, row];
            GameObject rightDotsCheck = gen.allDots[column + 1, row];
            if (leftDotsCheck != null && rightDotsCheck != null)
            {
                if (leftDotsCheck.tag == gameObject.tag && rightDotsCheck.tag == gameObject.tag)
                {
                    leftDotsCheck.GetComponent<SwipeDots>().isMatch = true;
                    rightDotsCheck.GetComponent<SwipeDots>().isMatch = true;
                    isMatch = true;
                }
            }
        }

        if (row > 0 && row < gen.height - 1)
        {
            GameObject upDotsCheck = gen.allDots[column, row+1];
            GameObject downDotsCheck = gen.allDots[column, row-1];
            if (upDotsCheck != null && downDotsCheck != null)
            {
                if (upDotsCheck.tag == gameObject.tag && downDotsCheck.tag == gameObject.tag)
                {
                    upDotsCheck.GetComponent<SwipeDots>().isMatch = true;
                    downDotsCheck.GetComponent<SwipeDots>().isMatch = true;
                    isMatch = true;
                }
            }
        }
    }

    IEnumerator reverseDots()
    {
        yield return new WaitForSeconds(.6f);
        if (otherDot != null)
        {
            if (!isMatch && !otherDot.GetComponent<SwipeDots>().isMatch)
            {
                otherDot.GetComponent<SwipeDots>().row = row;
                otherDot.GetComponent<SwipeDots>().column = column;
                row = prevRow;
                column = prevCol;
            }
            else
            {
                gen.DestroyDotsGO();
            }
            otherDot = null;
        }
    }
}
