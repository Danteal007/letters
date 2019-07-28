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
    public float aspectRatio;
    bool isAnimated = false;
    string[] alphabet = new string[] {"й", "ц", "у", "к", "е", "н", "г", "ш", "щ", "з", "х", "ъ", "ф", "ы", "в", "а", "п", "р", "о", "л", "д", "ж", "э", "я", "ч", "с", "м", "и", "т", "ь", "б", "ю", "ё" };
    string abc = "йцукенгшщзхъфывапролджэячсмитьбюё";

    void Start()
    {
        abc = abc.Replace(letter,"");
        aspectRatio = (float)Screen.width / (float)Screen.height;
        markers = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            markers[i] = new Vector3(200*(i+1)-400, 320/aspectRatio, 0);
            
        }


        letters = new Transform[20];
        Color color = Color.white;

        for (int i = 0; i < 6; i++)
        {
            Transform shadow = Instantiate(letter_prefab);
            shadow.transform.SetParent(canvas.transform);
            shadow.GetComponent<RectTransform>().localPosition = markers[i / 2] + (i % 2 == 0 ? new Vector3(25, 0, 0) : new Vector3(-25, 0, 0));
            shadow.GetComponent<Text>().color = Color.white;
            shadow.GetComponent<Letter>().isBigSize = i % 2 == 0 ? true : false;
            shadow.GetComponent<Text>().text = shadow.GetComponent<Letter>().isBigSize ? letter : letter.ToUpper();
            shadow.GetComponent<Text>().fontStyle = (FontStyle)(i / 2);
            //shadow
        }

        for (int i = 0; i < letters.Length; i++)
        {
            letters[i] = Instantiate(letter_prefab);
            letters[i].transform.SetParent(canvas.transform);
            letters[i].GetComponent<RectTransform>().localPosition = i < 6 ? markers[i / 2] + (i % 2 == 0 ? new Vector3(25, 0, 0) : new Vector3(-25, 0, 0)) : new Vector3(0,0,0);
            letters[i].GetComponent<Letter>().lastPlace = letters[i].transform.localPosition;
            letters[i].GetComponent<Text>().fontStyle = i > 5 ? (FontStyle)Random.Range(0,2) : (FontStyle)(i/2);
            letters[i].GetComponent<Letter>().isBigSize = i % 2 == 0 ? true : false;
            letters[i].GetComponent<Letter>().text = i > 5 ? abc[Random.Range(0, 31)].ToString() : letter;
            letters[i].GetComponent<Text>().text = i > 5 ? letters[i].GetComponent<Letter>().text : (letters[i].GetComponent<Letter>().isBigSize ? letter : letter.ToUpper());
            letters[i].GetComponent<Text>().color = i < 6 ? Color.red : Color.grey;
            letters[i].gameObject.SetActive(i > 5 ? false : true);


        }

        //LocateAllLetters();
        StartCoroutine(StartGame());


    }

    // Update is called once per frame
    void Update()
    {
        Canvas.ForceUpdateCanvases();
    }

    public void LocateAllLetters() //Задает буквам случайные позиции
    {
        targets = new Vector3[20];

        for (int i = 0; i < letters.Length; i++)
        {
            int x = i % 5;
            int y = i / 5;

            targets[i] = new Vector3(Random.Range(x * 160 + 26, (x + 1) * 160 - 26), Random.Range(y * (160 / aspectRatio) + 26, (y + 1) * (160 / aspectRatio) - 26), 0);
            //Debug.Log("Target" + i + " = " + targets[i]);
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
        yield return new WaitForSeconds(3);
        Debug.Log("Game Started");
        yield return StartCoroutine(MixLetters());
        StartCoroutine(MixLetters());
        /*LocateAllLetters();
        for (int i = 0; i < letters.Length; i++)
        {
            StartCoroutine(letters[i].GetComponent<Letter>().MoveLetter(targets[i]));
        }*/
        yield return null;
    }

    public IEnumerator Wrong(Letter letter)
    {
        StopAllCoroutines();
        StartCoroutine(letter.ChangeLetterColor(Color.black));
        for (int i = 0; i < 6; i++)
        {
            
            StartCoroutine(letters[i].GetComponent<Letter>().ChangeLetterColor(Color.red));
        }
        yield return StartCoroutine(letter.Blink());
        StopAllCoroutines();

        StartCoroutine(MixLetters());
        yield return new WaitForSeconds(4);
    }

    IEnumerator MixLetters()
    {
        LocateAllLetters();
        int k = 0;
        for (int i = 0; i < letters.Length; i++)
        {
            Text l = letters[i].GetComponent<Text>();
            
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
    }

}
