using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField] TextAsset WordsFile;
    [SerializeField] GameSettings main_gs;    

    [Space]
    [SerializeField] GameObject char_prefab;    
    [SerializeField] Transform placeToSpawn;

    [Space]    
    [SerializeField] List<string> currentWord;
    [SerializeField] List<GameObject> chars_ui_prefabs;

    [Space]
    [SerializeField] List<string> usedWords;
    List<string> allWords = new List<string>();

    [Space]
    [SerializeField] GameObject[] keysForAnswers;
    [SerializeField] string[] alpabet;

    [Space]
    [SerializeField] TMP_Text score_text;
    [SerializeField] TMP_Text tries_text;

    [Space]
    [SerializeField] GameObject victory_panel;
    [SerializeField] GameObject defeat_panel;
    [SerializeField] GameObject warning_panel;
    [SerializeField] GameObject alphabet_panel;

    int tries;
    int score;

    int wordlength;

    int randomIndex;    

    readonly char[] separators = new char[] { ' ', '[', ']', '_', '*', ':', '.', '!', '?', ',', '\n', ';', '\u0027', '-', '\u0060', ')', '(' , '\u0022', '1','2','3','4','5','6','7','8','9','0'};

    void Start()
    {
        SetupGame();        
    }

    public void SetupGame()
    {
        victory_panel.SetActive(false);
        defeat_panel.SetActive(false);
        warning_panel.SetActive(false);
        alphabet_panel.SetActive(true);

        usedWords.Clear();
        allWords.Clear();

        foreach (GameObject go in chars_ui_prefabs)
        {
            Destroy(go);
        }

        chars_ui_prefabs.Clear();

        foreach (GameObject go in keysForAnswers)
        {
            go.transform.Find("Red").gameObject.SetActive(false);

            go.transform.Find("Green").gameObject.SetActive(false);
        }       

        tries = main_gs.tries;
        score = 0;

        SetupWords();

        UpdateScore();        
    }

    void SetupWords()
    {
        allWords.AddRange(WordsFile.text.Split(separators, System.StringSplitOptions.RemoveEmptyEntries));       

        GetRandomWord();
    }        

    private void GetRandomWord()
    {
        if (allWords.Count > 1)
        {
            randomIndex = Random.Range(0, allWords.Count);

            CheckRandomWord(allWords[randomIndex], randomIndex);
        }
        else
        {
            victory_panel.SetActive(true);
        }
    }

    void CheckRandomWord(string word, int i)
    {
        if (word.Length >= main_gs.characterLimit && !isThisWordUsed(word.ToUpper())) 
        {
            allWords.RemoveAt(i);            
            SetNewWord(word.ToUpper());
            warning_panel.SetActive(false);
            alphabet_panel.SetActive(true);
        }
        else
        {
            allWords.RemoveAt(i);
            StartCoroutine(SearchStuff());
            warning_panel.SetActive(true);
            alphabet_panel.SetActive(false);
        }
    }

    void SetNewWord(string word)
    {
        ClearOldWord();

        usedWords.Add(word);

        char[] arr = word.ToCharArray();

        foreach (char ch in arr)
        {
            currentWord.Add(ch.ToString());

            GameObject go = Instantiate(char_prefab, placeToSpawn);

            go.transform.Find("Char").GetComponent<TMP_Text>().text = ch.ToString();

            chars_ui_prefabs.Add(go);
        }

        wordlength = word.Length;
    }

    void ClearOldWord()
    {
        currentWord.Clear();

        foreach (GameObject go in chars_ui_prefabs)
        {
            Destroy(go);
        }

        chars_ui_prefabs.Clear();
    }    

    public void Buttons(int i)
    {
        bool itHadIt = false;

        for (int w = 0; w < currentWord.Count; w++)
        {
            if(currentWord[w] == alpabet[i])
            {                
                itHadIt = true;

                chars_ui_prefabs[w].transform.Find("Close").gameObject.SetActive(false);

                keysForAnswers[i].transform.Find("Green").gameObject.SetActive(true);

                wordlength--;
            }
        }

        if (!itHadIt)
        {
            keysForAnswers[i].transform.Find("Red").gameObject.SetActive(true);
            tries--;
        }

        UpdateScore();

        CheckGuessState();
    }
    
    void CheckGuessState()
    {
        if(wordlength <= 0)
        {
            GetNextWord();
        }

        if(tries < 0)
        {
            defeat_panel.SetActive(true);
        }
    }

    void GetNextWord()
    {
        score += tries;

        foreach (GameObject go in keysForAnswers)
        {
            go.transform.Find("Red").gameObject.SetActive(false);

            go.transform.Find("Green").gameObject.SetActive(false);
        }

        tries = main_gs.tries;

        GetRandomWord();
        UpdateScore();
    }


    void UpdateScore()
    {
        score_text.text = "Score: " + score.ToString();
        tries_text.text = "Tries: " + tries.ToString();
    }

    bool isThisWordUsed(string word)
    {
        for (int i = 0; i < usedWords.Count; i++)
        {
            if(usedWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator SearchStuff()
    {
        yield return new WaitForEndOfFrame();
        GetRandomWord();
    }

    public void ResetTheGame()
    {
        SetupGame();
    }
}