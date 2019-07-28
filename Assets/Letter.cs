using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    public string text;
    public bool isBigSize;
    public Vector3 lastPlace;
    public bool isStatic;
    GameObject mainCamera;
    Transform[] letters;
    public Vector3 target;
    Game game;
    Text guiText;

    private void Start()
    {
        isStatic = true;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        game = mainCamera.GetComponent<Game>();
        letters = game.letters;
        guiText = GetComponent<Text>();
    }

    public void OnClick()
    {
        Debug.Log("sss");
        if (text.Equals(mainCamera.GetComponent<Game>().letter))
        {
            if (isStatic) {
                StopAllCoroutines();
                StartCoroutine(MoveLetter(lastPlace));
                StartCoroutine(ChangeLetterColor(Color.red));
            }
        }
        else
        {
            //Случай если нажата неправильная буква
            StartCoroutine(game.Wrong(this));
        }
            
    }

    

    public IEnumerator MoveLetter(Vector3 target)// Плавное перемещение буквы в заданную точку
    {
        
        isStatic = false;
        while (transform.localPosition != target)
        {
            transform.gameObject.SetActive(true);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, Time.deltaTime * 150);

            yield return null;

        }
        isStatic = true;
    }

    public IEnumerator ChangeLetterColor(Color color)// Плавная смена цвета
    {
        while (guiText.color != color)
        {
            guiText.color = Color.Lerp(guiText.color, color, Time.deltaTime);
            Debug.Log("Color Changing to " + color.ToString());
            yield return null;
        }
        Debug.Log("Color changed - "+ color.ToString());
    }

    public IEnumerator Blink()
    {
        for (int i = 0; i < 5; i++)
        {
            guiText.enabled = false;
            yield return new WaitForSeconds(0.3f);
            yield return null;
            guiText.enabled = true;
            yield return new WaitForSeconds(0.3f);
            yield return null;
        }
    }
}
