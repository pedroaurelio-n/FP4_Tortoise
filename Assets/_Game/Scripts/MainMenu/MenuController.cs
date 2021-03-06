using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    [SerializeField] private Page initialPage;
    [SerializeField] private GameObject firstFocusItem;

    private Canvas _rootCanvas;
    private Stack<Page> _pageStack = new Stack<Page>();

    private void Awake()
    {
        _rootCanvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        GameManager.Instance.LockMouse(false);
        Time.timeScale = 1;

        if (firstFocusItem != null)
        {
            EventSystem.current.SetSelectedGameObject(firstFocusItem);
        }

        if (initialPage != null)
        {
            PushPage(initialPage);
        }
    }

    /*private void OnCancel()
    {
        if (_rootCanvas.enabled && _rootCanvas.gameObject.activeInHierarchy)
        {
            if (_pageStack.Count != 0)
            {
                PopPage();
                Debug.Log("a");

                EventSystem.current.SetSelectedGameObject(_pageStack.Peek().onCancelSelectedObject);
            }
        }
    }*/

    public void StartGame()
    {
        FadeManager.StartFadeIn(delegate { FadeManager.DelayAfterFadeIn(2f, delegate{ SceneManager.LoadScene(2); }); });
    }

    public void Credits()
    {
        FadeManager.StartFadeIn(delegate { FadeManager.DelayAfterFadeIn(2f, delegate{ SceneManager.LoadScene(1); }); });
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public bool IsPageInStack(Page page)
    {
        return _pageStack.Contains(page);
    }

    public bool IsPageOnTopOfStack(Page page)
    {
        return _pageStack.Count > 0 && page == _pageStack.Peek();
    }

    public void PushPage(Page page)
    {
        page.Enter(true);

        if (_pageStack.Count > 0)
        {
            var currentPage = _pageStack.Peek();

            if (currentPage.ExitOnNewPagePush)
                currentPage.Exit(false);
        }

        _pageStack.Push(page);
    }

    public void PopPage()
    {
        if (_pageStack.Peek().isAnimating)
            return;
            
        if (_pageStack.Count > 1)
        {
            var page = _pageStack.Pop();
            page.Exit(true);

            var newCurrentPage = _pageStack.Peek();

            if (newCurrentPage.ExitOnNewPagePush)
            {
                newCurrentPage.Enter(false);
            }
        }
        else
        {
            Debug.LogWarning("Trying to the only page in the stack!");
        }
    }

    public void PopAllPages()
    {
        for (int i = 1; i < _pageStack.Count; i++)
        {
            PopPage();
        }
    }

    public void SetSelectedGameObject(GameObject selectedObject)
    {
        EventSystem.current.SetSelectedGameObject(selectedObject);
    }
}
