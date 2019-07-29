using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public string letter;
    public Transform letter_prefab;
    public Vector3[] markers;
    public Transform[] letters;
    public Vector3[] targets;
    public GameObject canvas;

    public AudioClip[] audioClips;
    public AudioClip[] storyClips;

    public float aspectRatio;
    public bool isAnimated = false;
    string abc = "абвгдежзийклмнпрсту";
    string Xabc;

    public int points = 0;
    public int tries = 0;

    void Start()
    {
        letter = abc[Random.Range(0,abc.Length-1)].ToString();
        Xabc = abc.Replace(letter,"");

        aspectRatio = (float)Screen.width / (float)Screen.height;
        markers = new Vector3[3];

        for (int i = 0; i < 3; i++)
        {
            markers[i] = new Vector3(200*(i+1)-400, 320/aspectRatio, 0);
            
        }

        Debug.Log(Screen.width);
        letters = new Transform[20];
        Color color = Color.white;

        for (int i = 0; i < 6; i++)
        {
            Transform shadow = Instantiate(letter_prefab);
            shadow.transform.SetParent(canvas.transform);
            shadow.GetComponent<RectTransform>().localPosition = markers[i / 2] + (i % 2 == 0 ? new Vector3(26, 0, 0) : new Vector3(-26, 0, 0));
            shadow.GetComponent<Text>().color = Color.white;
            shadow.GetComponent<Letter>().isBigSize = i % 2 == 0 ? true : false;
            shadow.GetComponent<Text>().text = shadow.GetComponent<Letter>().isBigSize ? letter : letter.ToUpper();
            shadow.GetComponent<Text>().fontStyle = (FontStyle)(i / 2);
        }

        for (int i = 0; i < letters.Length; i++)
        {
            letters[i] = Instantiate(letter_prefab);
            letters[i].transform.SetParent(canvas.transform);

            RectTransform rt = letters[i].GetComponent<RectTransform>();
            Letter let = letters[i].GetComponent<Letter>();
            Text textGui = letters[i].GetComponent<Text>();

            rt.localPosition = i < 6 ? markers[i / 2] + (i % 2 == 0 ? new Vector3(26, 0, 0) : new Vector3(-26, 0, 0)) : new Vector3(0,0,0);

            let.lastPlace = letters[i].transform.localPosition;

            letters[i].GetComponent<Text>().fontStyle = i > 5 ? (FontStyle)Random.Range(0,2) : (FontStyle)(i/2);
            let.isBigSize = i % 2 == 0 ? true : false;
            let.text = i > 5 ? Xabc[Random.Range(0, 18)].ToString() : letter;
            Debug.Log(abc.IndexOf(let.text));
            letters[i].GetComponent<AudioSource>().clip = audioClips[abc.IndexOf(let.text)];
            textGui.text = i > 5 ? let.text : (let.isBigSize ? letter : letter.ToUpper());
            textGui.color = i < 6 ? Color.red : Color.grey;
            letters[i].gameObject.SetActive(i > 5 ? false : true);


        }
        
        StartCoroutine(StartGame());
    }

    public void LocateAllLetters() //Задает буквам случайные позиции
    {
        targets = new Vector3[20];

        for (int i = 0; i < letters.Length; i++)
        {
            int x = i % 5;
            int y = i / 5;

            targets[i] = new Vector3(Random.Range(x * 160 + 26, (x + 1) * 160 - 26), Random.Range(y * (160 / aspectRatio) + 26, (y + 1) * (160 / aspectRatio) - 26), 0);
            targets[i] += new Vector3(-400, -400 / aspectRatio);       
        }

        for (int i = targets.Length - 1; i >= 1; i--)
        {
            int j = Random.Range(0, i);

            Vector3 tmp = targets[j];
            targets[j] = targets[i];
            targets[i] = tmp;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            letters[i].GetComponent<Letter>().target = targets[i];
        }
    }

    IEnumerator StartGame() // Основная последовательность действий
    {
        GetComponent<AudioSource>().clip = storyClips[4];
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(3);
        Debug.Log("Game Started");
        yield return StartCoroutine(MixLetters());
        StartCoroutine(MixLetters());
        yield return null;
    }

    public IEnumerator Wrong(Letter letter)
    {
        isAnimated = true;
        StopAllCoroutines();
        

        StartCoroutine(letter.ChangeLetterColor(Color.black));
        for (int i = 0; i < 6; i++)
        {
            StartCoroutine(letters[i].GetComponent<Letter>().ChangeLetterColor(Color.red));
        }
        yield return StartCoroutine(letter.Blink());
        StopAllCoroutines();

        int k = 0;
        while (k != points)
        {
            k = 0;
            for (int i = 0; i < 6; i++)
            {
                if (letters[i].GetComponent<Text>().color == Color.red)
                {
                    k++;
                }
            }
            yield return null;
        }

        StartCoroutine(MixLetters());
    }

    IEnumerator MixLetters()
    {
        isAnimated = true;
        LocateAllLetters();
        int k = 0;
        for (int i = 0; i < letters.Length; i++)
        {
            Text l = letters[i].GetComponent<Text>();
            letters[i].GetComponent<Button>().enabled = false;
            letters[i].GetComponent<Letter>().StopAllCoroutines();
            StartCoroutine(letters[i].GetComponent<Letter>().ChangeLetterColor(Color.grey));
            StartCoroutine(letters[i].GetComponent<Letter>().MoveLetter(targets[i]));
        }
        while (k < letters.Length)
        {
            k = 0;
            for (int i = 0; i< letters.Length; i++)
            {
                if (letters[i].GetComponent<Letter>().isStatic)
                    k++;
                yield return null;
            }
            Debug.Log("k = "+k);
            
        }

        for (int i = 0; i < letters.Length; i++)
        {
            letters[i].GetComponent<Button>().enabled = true;
        }
        isAnimated = false;
    }

}
