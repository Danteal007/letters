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
    AudioClip[] storyClips;
    AudioSource audioSource;
    Game game;
    Text guiText;

    private void Start()
    {
        isStatic = true;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        game = mainCamera.GetComponent<Game>();
        audioSource = mainCamera.GetComponent<AudioSource>();
        storyClips = game.storyClips;
        letters = game.letters;
        guiText = GetComponent<Text>();
    }

    public void OnClick()
    {        
        if (text.Equals(mainCamera.GetComponent<Game>().letter))
        {
            //Случай если нажата правильная буква
            if (isStatic && !game.isAnimated) {
                GetComponent<AudioSource>().Play();
                game.points++;
                game.tries = 0;
                audioSource.clip = game.points != 6 ? storyClips[3] : storyClips[6];
                audioSource.PlayDelayed(1);
                StopAllCoroutines();
                StartCoroutine(MoveLetter(lastPlace));
                StartCoroutine(ChangeLetterColor(Color.red));
            }
        }
        else
        {
            //Случай если нажата неправильная буква
            game.points = 0;
            audioSource.clip = storyClips[game.tries % 3];
            game.tries ++;
            audioSource.Play();
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
            yield return null;
        }
    }

    public IEnumerator Blink()// Моргание буквы
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
