using System;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;

[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour {
    
    [BoxGroup("References")] public RectTransform codexTransform;
    [BoxGroup("References")] [HideInInspector] public Book ControledBook;
    
    [BoxGroup("Page Flipping")] public float PageFlipTime = 1;
    [BoxGroup("Page Flipping")] public float acceleratedFlipTime = 0.2f;
    [BoxGroup("Page Flipping")] public int AnimationFramesCount = 40;
    
    [BoxGroup("Codex Movement")] public Vector2 offset;
    [BoxGroup("Codex Movement")] public float codexLerp = 0.17f;
    
    //Controls
    

    
    [Foldout("Deprecated")] public FlipMode Mode;
    [Foldout("Deprecated")] public float TimeBetweenPages = 1;
    [Foldout("Deprecated")] public float DelayBeforeStarting = 0;
    [Foldout("Deprecated")] public bool AutoStartFlip=true;
    
    
    private bool isFlipping;
    // Use this for initialization
    void Start () {
        if (!ControledBook)
            ControledBook = GetComponent<Book>();
        if (AutoStartFlip)
            StartFlipping();
        ControledBook.OnFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
        
	}

    private void Update()
    {
        if (CharacterInputManager.Instance.showCodex)
        {
            codexTransform.anchoredPosition = Vector2.Lerp(codexTransform.anchoredPosition,Vector2.zero, codexLerp);
        }
        else
        {
            codexTransform.anchoredPosition = Vector2.Lerp(codexTransform.anchoredPosition, offset, codexLerp);
        }

        
    }

    void PageFlipped()
    {
        isFlipping = false;
    }
	public void StartFlipping()
    {
        StartCoroutine(FlipToEnd(PageFlipTime));
    }
    public void FlipRightPage(float flipTime)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage >= ControledBook.TotalPageCount) return;
        isFlipping = true;
        float frameTime = flipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl)*2 / AnimationFramesCount;
        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
    }
    public void FlipLeftPage(float flipTime)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage <= 0) return;
        isFlipping = true;
        float frameTime = flipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
    }
    IEnumerator FlipToEnd(float flipTime)
    {
        yield return new WaitForSeconds(DelayBeforeStarting);
        float frameTime = flipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2)*0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y)*0.9f;
        //y=-(h/(xl)^2)*(x-xc)^2          
        //               y         
        //               |          
        //               |          
        //               |          
        //_______________|_________________x         
        //              o|o             |
        //           o   |   o          |
        //         o     |     o        | h
        //        o      |      o       |
        //       o------xc-------o      -
        //               |<--xl-->
        //               |
        //               |
        float dx = (xl)*2 / AnimationFramesCount;
        switch (Mode)
        {
            case FlipMode.RightToLeft:
                while (ControledBook.currentPage < ControledBook.TotalPageCount)
                {
                    StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;
            case FlipMode.LeftToRight:
                while (ControledBook.currentPage > 0)
                {
                    StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(TimeBetweenPages);
                }
                break;
        }
    }
    IEnumerator FlipRTL(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc + xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);

        ControledBook.DragRightPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }
        ControledBook.ReleasePage();
    }
    IEnumerator FlipLTR(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        ControledBook.DragLeftPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < AnimationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookLTRToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x += dx;
        }
        ControledBook.ReleasePage();
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
        for (int i = 0; i < pagesAmount; i++)
        {
            if (flipSide)
            {
                FlipLeftPage(acceleratedFlipTime);
            }
            else
            {
                FlipRightPage(acceleratedFlipTime);
            }
            Debug.Log("Flip");
            yield return new WaitUntil(() => isFlipping == false);
        }
    }

    public void JumpToBookMark(int index)
    {
        var pageDiff = Mathf.Abs(index - ControledBook.currentPage);
        
        FlipXPages(Mathf.CeilToInt(pageDiff * 0.5f), index <= ControledBook.currentPage);
    }

    public void PlayerInputFlipPages(Vector2 input)
    {
        if (input.x > 0.5f)
        {
            FlipRightPage(PageFlipTime);
        }
        else if (input.x < -0.5f)
        {
            FlipLeftPage(PageFlipTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flipSide"> true = left, false = right </param>
    public void PlayerInputNavigateBookmarks(bool flipSide)
    {
        
        if (!flipSide)
        {
            foreach (var bookMark in ControledBook.bookMarks)
            {
                if (bookMark.index > ControledBook.currentPage)
                {
                    JumpToBookMark(bookMark.index);
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
                    JumpToBookMark(ControledBook.bookMarks[i].index);
                    return;
                }
                
            }
        }
    }
} 
