using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectScript : MonoBehaviour
{

    public RectTransform panel;
    public Button[] bttn;
    public RectTransform center; //objeto de refenceia del centrode la UI

    public GameObject nextBtn;
    public GameObject previousBtn;


    public float[] distance; //valor de la distancia
    public float[] distRepo;
    public bool dragging = false; //scrollRect dragging status
    public int bttnDistance;
    public int minBttnnum;
    public int bttnLength;
    public int BtnIndex = 0;
    private bool targetnearbttn = true;//

    public Vector2 newPosition;
    


    private void Start()
    {
        bttnLength = bttn.Length;
        distance = new float[bttnLength];
        distRepo = new float[bttnLength];

        bttnDistance = (int)Mathf.Abs(bttn[1].GetComponent<RectTransform>().anchoredPosition.x - bttn[0].GetComponent<RectTransform>().anchoredPosition.x);
        
    }

    private void Update()
    {
        for (int i = 0; i < bttn.Length; i++)
        {
            distRepo[i] = center.GetComponent<RectTransform>().position.x - bttn[i].GetComponent<RectTransform>().position.x;
            distance[i] = Mathf.Abs(distRepo[i]);// calcula el centro de la scena del primer button(itemcard)

            if (distRepo[i] > 3500)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX + (bttnLength * bttnDistance), curY);
                bttn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }
            if (distRepo[i] < -3500)
            {
                float curX = bttn[i].GetComponent<RectTransform>().anchoredPosition.x;
                float curY = bttn[i].GetComponent<RectTransform>().anchoredPosition.y;

                Vector2 newAnchoredPos = new Vector2(curX - (bttnLength * bttnDistance), curY);
                bttn[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPos;
            }
        }
        if (targetnearbttn)
        {
            float minDistance = Mathf.Min(distance);

            for (int a = 0; a < bttn.Length; a++)
            {
                if (minDistance == distance[a])
                {
                    minBttnnum = a;
                }
            }
        }
        

        if (!dragging)
        {
            // LerpToBttn(minBttnnum * -bttnDistance);
            LerpToBttn(-bttn[minBttnnum].GetComponent<RectTransform>().anchoredPosition.x);
        }

        void LerpToBttn(float position)
        {
            float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 10f);
            newPosition = new Vector2(newX, panel.anchoredPosition.y);

            panel.anchoredPosition = newPosition;

        }
       




    }

    public void nextCard()
    {
        targetnearbttn = false;
        BtnIndex += 1;
        Debug.Log(BtnIndex);
        minBttnnum = BtnIndex;
        if (BtnIndex == bttn.Length)
        {
            BtnIndex = -1;
        }
    }

    public void previousCard()
    {
        targetnearbttn = false;
        BtnIndex -= 1;
        minBttnnum = BtnIndex;
        if (BtnIndex == 0)
        {
            BtnIndex = bttn.Length;
        }
        if (BtnIndex == -1)
        {
            BtnIndex += 1;
        }
    }
     
    public void StartDrag()
    {
        dragging = true;
        targetnearbttn = true;
    }

    public void EndDrag()
    {
        dragging = false;

    }



}
