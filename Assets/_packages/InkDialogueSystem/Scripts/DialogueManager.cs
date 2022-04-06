using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Globals Ink File")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [Header("Components")]
    [SerializeField] private Animator portraitAnimator;

    [Header("Choices UI")]
    [SerializeField] private List<GameObject> choiceObjects;

    [Header("Timing Params")]
    [SerializeField] private float typingSpeed;
    [SerializeField] private float delayBeforeExit;

    private TextMeshProUGUI[] choicesText;

    private Story _currentStory;
    private static bool _isDialoguePlaying;
    private Coroutine _displayLineCoroutine;
    private bool _canContinueToNextLine;
    private bool _canMakeChoice;
    private bool _isDeactivatingChoice;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";

    private static DialogueVariables dialogueVariables;

    private void Awake()
    {
        dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }

    private void Start()
    {
        _isDialoguePlaying = false;
        _canMakeChoice = false;
        _canContinueToNextLine = false;
        _isDeactivatingChoice = false;

        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choiceObjects.Count];

        for (int i = 0; i < choiceObjects.Count; i++)
        {
            choicesText[i] = choiceObjects[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public static bool IsDialoguePlaying()
    {
        return _isDialoguePlaying;
    }

    private void CheckDialogueInput(TextAsset json)
    {
        if (!_isDialoguePlaying)
        {
            EnterDialogueMode(json);
        }
        else
        {
            if (!_canContinueToNextLine)
            {
                if (!_isDeactivatingChoice)
                    SkipDisplayLineCoroutine();
            }
            else
            {
                if (_currentStory.currentChoices.Count == 0)
                {
                    ContinueStory();
                }
            }
        }
    }

    private void EnterDialogueMode(TextAsset json)
    {
        _currentStory = new Story(json.text);
        _isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        dialogueVariables.StartListening(_currentStory);

        displayNameText.text = "???";
        portraitAnimator.Play("default");

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(delayBeforeExit);

        dialogueVariables.StopListening(_currentStory);

        _isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue)
        {
            if (_displayLineCoroutine != null)
            {
                StopCoroutine(_displayLineCoroutine);
            }
            _displayLineCoroutine = StartCoroutine(DisplayLine(_currentStory.Continue()));
            HandleInkTags(_currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        HideChoices();

        continueIcon.SetActive(false);

        _canContinueToNextLine = false;


        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        bool isAddingRichText = false;

        foreach (char letter in line.ToCharArray())
        {
            if (letter == '<' || isAddingRichText)
            {
                isAddingRichText = true;
                if (letter == '>')
                {
                    isAddingRichText = false;
                }
            }
            else
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
                _isDeactivatingChoice = false;
            }
        }

        continueIcon.SetActive(true);
        DisplayChoices();

        _canContinueToNextLine = true;
    }

    private void SkipDisplayLineCoroutine()
    {
        Debug.Log("Test");
        if (_displayLineCoroutine != null)
        {
            StopCoroutine(_displayLineCoroutine);
        }

        dialogueText.maxVisibleCharacters = _currentStory.currentText.Length;

        continueIcon.SetActive(true);
        DisplayChoices();

        _canContinueToNextLine = true;
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = _currentStory.currentChoices;

        if (currentChoices.Count > choiceObjects.Count)
        {
            Debug.LogWarning("There are more choices in JSON file than there are UI button objects.\n"
                            + "Current choices in file: " + currentChoices.Count + "\n"
                            + "Current choices in UI: " + choiceObjects.Count);
        }

        for (int i = 0; i < currentChoices.Count; i++)
        {
            choiceObjects[i].gameObject.SetActive(true);
            choicesText[i].text = currentChoices[i].text;
        }

        for (int i = currentChoices.Count; i < choiceObjects.Count; i++)
        {
            choiceObjects[i].gameObject.SetActive(false);
        }

        if (currentChoices.Count > 0)
        {
            StartCoroutine(SelectFirstChoice());
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choice in choiceObjects)
        {
            choice.SetActive(false);
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForSeconds(delayBeforeExit);
        EventSystem.current.SetSelectedGameObject(choiceObjects[0].gameObject);
        _canMakeChoice = true;
    }

    private void HandleInkTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');

            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    if (displayNameText != null)
                        displayNameText.text = tagValue;
                    break;

                case PORTRAIT_TAG:
                    if (portraitAnimator != null)
                        portraitAnimator.Play(tagValue);
                    break;
                    
                default:
                    Debug.LogWarning("Tag " + tag + " is not currently being handled. Create the appropriate const in script.");
                    break;
            }
        }
    }

    public void MakeChoice(int index)
    {
        if (_canMakeChoice)
        {
            _canMakeChoice = false;
            _isDeactivatingChoice = true;
            Debug.Log("1");
            _currentStory.ChooseChoiceIndex(index);
            EventSystem.current.SetSelectedGameObject(null);
            if (_canContinueToNextLine)
            {
                ContinueStory();
            }
            else
            {
                SkipDisplayLineCoroutine();
            }
        }
    }

    public static Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variablesDictionary.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }

    public static void SetVariableState(string variableName, Ink.Runtime.Object variableValue)
    {
        if (dialogueVariables.variablesDictionary.ContainsKey(variableName))
        {
            dialogueVariables.variablesDictionary.Remove(variableName);
            dialogueVariables.variablesDictionary.Add(variableName, variableValue);
        }
        else
        {
            Debug.LogWarning("Tried to update variable that wasn't initialized by globals.ink: " + variableName);
        }
    }

    private void OnEnable()
    {
        DialogueTrigger.onDialogue += CheckDialogueInput;
    }

    private void OnDisable()
    {
        DialogueTrigger.onDialogue -= CheckDialogueInput;
    }
}
