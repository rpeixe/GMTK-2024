using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject portrait;
    public Sprite[] close;
    public Sprite[] open;
    public (int, string)[] lines { get; set; }
    public float textSpeed = 0.1f;
    public Image portraitImage;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
    }

    public void InitDialog()
    {
        portraitImage = portrait.GetComponent<Image>();
        textComponent.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index].Item2)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].Item2;
            }
        }
    }
    public void StartDialogue()
    {
        gameObject.SetActive(true);
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        var charArr = lines[index].Item2.ToCharArray();
        foreach (char c in charArr)
        {
            if (c % 2 == 0 || c == charArr[charArr.Length-1])
            {
                portraitImage.sprite = close[lines[index].Item1];
            }
            else
            {
                portraitImage.sprite = open[lines[index].Item1];
            }
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            portraitImage.sprite = close[lines[index].Item1];
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
