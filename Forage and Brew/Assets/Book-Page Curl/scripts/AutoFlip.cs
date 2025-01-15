using System;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using UnityEngine.Events;

[RequireComponent(typeof(Book))]
public class AutoFlip : Singleton<AutoFlip>
{
    [BoxGroup("References")] public RectTransform codexTransform;
    [BoxGroup("References")] public RectTransform codexProportions;
    [BoxGroup("References")] public Book ControledBook;

    [BoxGroup("Page Flipping")] public float PageFlipTime = 1;
    [BoxGroup("Page Flipping")] public float acceleratedFlipTime = 0.2f;
    [BoxGroup("Page Flipping")] public int AnimationFramesCount = 40;

    [BoxGroup("Codex Movement")] public Vector2 offset;
    [BoxGroup("Codex Movement")] public float codexLerp = 0.17f;
    [BoxGroup("Codex Movement")] public bool isNavigatingPages;
    [BoxGroup("Codex Movement")] public Vector3 aimedCodexPos;

    [BoxGroup("Codex Movement")] public float zoomIntensity = 2;
    [BoxGroup("Codex Movement")] public float yCursorSpeed = 1;
    [BoxGroup("Codex Movement")] public float xCursorSpeed = 1;


    //Controls


    [Foldout("Deprecated")] public FlipMode Mode;
    [Foldout("Deprecated")] public bool AutoStartFlip = true;
    public bool isDissolving { get; set; }


    private bool isFlipping;

    // Use this for initialization
    public override void Awake()
    {
        base.Awake();
        if (!ControledBook)
            ControledBook = GetComponent<Book>();
        var index = Array.IndexOf(ControledBook.bookPages.ToArray(), ControledBook.dummyOrderPage);
        ControledBook.bookMarks[0].index = index;
        ControledBook.bookMarks[1].index = index;
        ControledBook.bookMarks[2].index = index + 1;

        ControledBook.bookPages.RemoveAt(index);
        ControledBook.bookPages.RemoveAt(index);

        index = Array.IndexOf(ControledBook.bookPages.ToArray(), ControledBook.dummyOrderPage);
        ControledBook.bookMarks[^1].index = index + 1;

        ControledBook.bookPages.RemoveAt(index);
        ControledBook.bookPages.RemoveAt(index);
    }

    void Start()
    {
        ControledBook.OnFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
        CharacterInputManager.Instance.OnNavigationChange.AddListener(ChangeCodexNavigationType);
        Cursor.lockState = CursorLockMode.Confined;
        proportions = new Vector2(codexProportions.rect.width, codexProportions.rect.height);


        GameDontDestroyOnLoadManager.Instance.OnNewIngredientCollected.AddListener(ControledBook.StoreNewIngredient);
        ControledBook.SetupIngredientDisplays();
    }

    private void Update()
    {
        CodexNavigation();
    }

    private void CodexNavigation()
    {
        if (CharacterInputManager.Instance.showCodex)
        {
            codexTransform.anchoredPosition = Vector2.Lerp(codexTransform.anchoredPosition, aimedCodexPos,
                isNavigatingPages ? codexLerp * 0.3f : codexLerp);
        }
        else
        {
            codexTransform.anchoredPosition = Vector2.Lerp(codexTransform.anchoredPosition, offset, codexLerp);
            aimedCodexPos = Vector3.zero;
        }

        if (isNavigatingPages)
        {
            codexTransform.localScale = Vector2.Lerp(codexTransform.localScale, Vector2.one * zoomIntensity, codexLerp);
        }
        else
        {
            codexTransform.localScale = Vector2.Lerp(codexTransform.localScale, Vector2.one, codexLerp);
        }
    }

    void PageFlipped()
    {
        isFlipping = false;
    }

    public void FlipRightPage(float flipTime, int pageFlips)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage >= ControledBook.TotalPageCount) return;
        isFlipping = true;
        float frameTime = flipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx, pageFlips));
    }

    public void FlipLeftPage(float flipTime, int pageFlips)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage <= 0) return;
        isFlipping = true;
        float frameTime = flipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx, pageFlips));
    }

    IEnumerator FlipRTL(float xc, float xl, float h, float frameTime, float dx, int pageFlips)
    {
        float x = xc + xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);

        ControledBook.DragRightPageToPoint(new Vector3(x, y, 0), pageFlips - 1);
        ControledBook.SetupRTLFlip();
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }

        ControledBook.ReleasePage(pageFlips);
    }

    IEnumerator FlipLTR(float xc, float xl, float h, float frameTime, float dx, int pageFlips)
    {
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        ControledBook.DragLeftPageToPoint(new Vector3(x, y, 0), pageFlips - 1);
        ControledBook.SetupLTRFlip();
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookLTRToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x += dx;
        }

        ControledBook.ReleasePage(pageFlips);
    }

    /// <summary>
    /// Flip multiple pages in given direction
    /// </summary>
    /// <param name="pagesAmount"></param>
    /// <param name="flipSide"> true = left, false = right </param>
    /// <returns></returns>
    public void FlipXPages(int pagesAmount, bool flipSide)
    {
        StartCoroutine(FlipXPagesCoroutine(pagesAmount, flipSide));
    }

    IEnumerator FlipXPagesCoroutine(int pagesAmount, bool flipSide)
    {
        if (flipSide)
        {
            FlipLeftPage(acceleratedFlipTime, pagesAmount);
        }
        else
        {
            FlipRightPage(acceleratedFlipTime, pagesAmount);
        }

        for (int i = 0; i < pagesAmount; i++)
        {
            yield return new WaitUntil(() => isFlipping == false);
        }
    }

    public void FlipToPageIndex(int index)
    {
        if (index % 2 == 1)
        {
            index++;
        }

        var pageDiff = Mathf.Abs(index - ControledBook.currentPage);

        if (pageDiff == 0) return;
        
        FlipXPages(Mathf.CeilToInt(pageDiff * 0.5f), index <= ControledBook.currentPage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="navType">false = global codex navigation, true = page navigation</param>
    public void ChangeCodexNavigationType(bool navType)
    {
        isNavigatingPages = navType;
        if (!isNavigatingPages)
        {
            aimedCodexPos = Vector3.zero;
        }
    }

    private Vector2 proportions;

    public void PlayerInputFlipPages(Vector2 input)
    {
        if (isNavigatingPages)
        {
            aimedCodexPos -= new Vector3(input.x * xCursorSpeed, input.y * yCursorSpeed, 0);
            aimedCodexPos.x = Mathf.Clamp(aimedCodexPos.x, -proportions.x,
                proportions.x);

            aimedCodexPos.y = Mathf.Clamp(aimedCodexPos.y, -proportions.y,
                proportions.y);
            return;
        }

        if (input.x > 0.5f)
        {
            FlipRightPage(PageFlipTime, 1);
        }
        else if (input.x < -0.5f)
        {
            FlipLeftPage(PageFlipTime, 1);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flipSide"> true = left, false = right </param>
    public void PlayerInputNavigateBookmarks(bool flipSide)
    {
        if (isFlipping)
            return;

        if (!flipSide)
        {
            foreach (var bookMark in ControledBook.bookMarks)
            {
                if (bookMark.index > ControledBook.currentPage)
                {
                    FlipToPageIndex(bookMark.index);
                    return;
                }
            }
        }
        else
        {
            for (int i = ControledBook.bookMarks.Length - 1; i >= 0; i--)
            {
                if (ControledBook.bookMarks[i].index < ControledBook.currentPage)
                {
                    FlipToPageIndex(ControledBook.bookMarks[i].index);
                    return;
                }
            }
        }
    }
    
    public void HandleNewRecipes()
    {
        if (CodexContentManager.instance.pageIndexesToCheck[^1].Item1 % 2 == 1)
        {
            ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1].Item1 + 1);
        }
        else
        {
            ControledBook.JumpToPage(CodexContentManager.instance.pageIndexesToCheck[^1].Item1);
        }
        
        
        CharacterInputManager.Instance.EnterCodexMethod();
            
        CharacterInputManager.Instance.DisableCodexInputs();
        CharacterInputManager.Instance.DisableMoveInputs();

        StartCoroutine(PresentNewCodexContent());
    }

    IEnumerator PresentNewCodexContent()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = CodexContentManager.instance.pageIndexesToCheck.Count - 1; i >= 0; i--)
        {
            FlipToPageIndex(CodexContentManager.instance.pageIndexesToCheck[i].Item1);
            isDissolving = true;
            yield return new WaitWhile(() => isFlipping);
            if (CodexContentManager.instance.pageIndexesToCheck[i].Item2 != null)
            {
                CodexContentManager.instance.pageIndexesToCheck[i].Item2.StartDissolve();
                yield return new WaitWhile(() => isDissolving);
            }
            
        }
        
        CharacterInputManager.Instance.EnableCodexInputs();
        CharacterInputManager.Instance.EnableCodexExitInput();
        CharacterInputManager.Instance.EnableMoveInputs();
    }
}