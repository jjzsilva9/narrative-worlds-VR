using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRBookController : MonoBehaviour
{
    public TMP_Text pageText;

    [TextArea(8, 20)]
    public string[] pages;

    private int currentPage = 0;

    void Start()
    {
        ShowPage();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            NextPage();
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            PreviousPage();
        }
    }

    public void SetPages(string[] newPages)
    {
        pages = newPages;
        currentPage = 0;
        ShowPage();
    }

    public void ResetBook()
    {
        currentPage = 0;
        ShowPage();
    }

    public void NextPage()
    {
        if (pages == null || pages.Length == 0) return;

        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (pages == null || pages.Length == 0) return;

        if (currentPage > 0)
        {
            currentPage--;
            ShowPage();
        }
    }

    private void ShowPage()
    {
        if (pageText == null) return;

        if (pages == null || pages.Length == 0)
        {
            pageText.text = "No pages assigned.";
            return;
        }

        pageText.text = "Page " + (currentPage + 1) + "\n\n" + pages[currentPage];
    }
}