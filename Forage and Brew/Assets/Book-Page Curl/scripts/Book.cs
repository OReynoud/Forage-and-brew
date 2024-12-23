//The implementation is based on this article:http://rbarraza.com/html5-canvas-pageflip/
//As the rbarraza.com website is not live anymore you can get an archived version from web archive 
//or check an archived version that I uploaded on my website: https://dandarawy.com/html5-canvas-pageflip/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine.Events;
public enum FlipMode
{
    RightToLeft,
    LeftToRight
}
[ExecuteInEditMode]
public class Book : MonoBehaviour {
    [SerializeField]
    RectTransform BookPanel;

    
    [Serializable]
    public struct BookPage
    {
        public Sprite pageSprite;
        public RectTransform UIComponent;

        public BookPage(Sprite PageSprite, RectTransform uiComponent)
        {
            pageSprite = PageSprite;
            UIComponent = uiComponent;
        }
    }
    public List<BookPage> bookPages = new List<BookPage>();
    
    public BookMark[] bookMarks;
    public float bookmarkLerp;
    
    public bool enableShadowEffect=true;
    //represent the index of the sprite shown in the right page
    public int currentPage = 0;
    public int TotalPageCount
    {
        get { return bookPages.Count; }
    }
    public Vector3 EndBottomLeft
    {
        get { return ebl; }
    }
    public Vector3 EndBottomRight
    {
        get { return ebr; }
    }
    public float Height
    {
        get
        {
            return BookPanel.rect.height ; 
        }
    }
    [Foldout("Refs")] public Canvas canvas;
    [Foldout("Refs")] public Sprite background;
    [Foldout("Refs")] public Image ClippingPlane;
    [Foldout("Refs")] public Image NextPageClip;
    [Foldout("Refs")] public Image Shadow;
    [Foldout("Refs")] public Image ShadowLTR;
    [Foldout("Refs")] public Image Left;
    [Foldout("Refs")] public Image LeftNext;
    [Foldout("Refs")] public Image Right;
    [Foldout("Refs")] public Image RightNext;
    public UnityEvent OnFlip;
    float radius1, radius2;
    //Spine Bottom
    Vector3 sb;
    //Spine Top
    Vector3 st;
    //corner of the page
    Vector3 c;
    //Edge Bottom Right
    Vector3 ebr;
    //Edge Bottom Left
    Vector3 ebl;
    //follow point 
    Vector3 f;
    public bool pageDragging = false;
    //current flip mode
    FlipMode mode;

    void Start()
    {
        if (!canvas) canvas=GetComponentInParent<Canvas>();
        if (!canvas) Debug.LogError("Book should be a child to canvas");

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        UpdateSprites();
        CalcCurlCriticalPoints();

        float pageWidth = BookPanel.rect.width / 2.0f;
        float pageHeight = BookPanel.rect.height;
        NextPageClip.rectTransform.sizeDelta = new Vector2(pageWidth, pageHeight + pageHeight * 2);


        ClippingPlane.rectTransform.sizeDelta = new Vector2(pageWidth * 2 + pageHeight, pageHeight + pageHeight * 2);

        //hypotenous (diagonal) page length
        // float hyp = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
        // float shadowPageHeight = pageWidth / 2 + hyp;
        //
        // Shadow.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        // Shadow.rectTransform.pivot = new Vector2(1, (pageWidth / 2) / shadowPageHeight);
        //
        // ShadowLTR.rectTransform.sizeDelta = new Vector2(pageWidth, shadowPageHeight);
        // ShadowLTR.rectTransform.pivot = new Vector2(0, (pageWidth / 2) / shadowPageHeight);

        foreach (var bookMark in bookMarks)
        {
            bookMark.basePos = bookMark.UIComponent.anchoredPosition;
        }

    }

    private void CalcCurlCriticalPoints()
    {
        sb = new Vector3(0, -BookPanel.rect.height / 2);
        ebr = new Vector3(BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        ebl = new Vector3(-BookPanel.rect.width / 2, -BookPanel.rect.height / 2);
        st = new Vector3(0, BookPanel.rect.height / 2);
        radius1 = Vector2.Distance(sb, ebr);
        float pageWidth = BookPanel.rect.width / 2.0f;
        float pageHeight = BookPanel.rect.height;
        radius2 = Mathf.Sqrt(pageWidth * pageWidth + pageHeight * pageHeight);
    }

    public Vector3 transformPoint(Vector3 mouseScreenPos)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 mouseWorldPos = canvas.worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, canvas.planeDistance));
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseWorldPos);

            return localPos;
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 globalEBR = transform.TransformPoint(ebr);
            Vector3 globalEBL = transform.TransformPoint(ebl);
            Vector3 globalSt = transform.TransformPoint(st);
            Plane p = new Plane(globalEBR, globalEBL, globalSt);
            float distance;
            p.Raycast(ray, out distance);
            Vector2 localPos = BookPanel.InverseTransformPoint(ray.GetPoint(distance));
            return localPos;
        }
        else
        {
            //Screen Space Overlay
            Vector2 localPos = BookPanel.InverseTransformPoint(mouseScreenPos);
            return localPos;
        }
    }


    private void FixedUpdate()
    {
        for (int i = 0; i < bookMarks.Length; i++)
        {
            if (i == bookMarks.Length - 1)
            {
                if (bookMarks[^1].index <= currentPage)
                {
                    bookMarks[^1].UIComponent.anchoredPosition = Vector2.Lerp(
                        bookMarks[^1].UIComponent.anchoredPosition,
                        bookMarks[^1].basePos + new Vector2(bookMarks[^1].xDisplacement,0),bookmarkLerp);
                }
                else
                {
                    bookMarks[^1].UIComponent.anchoredPosition = Vector2.Lerp(
                        bookMarks[^1].UIComponent.anchoredPosition,
                        bookMarks[^1].basePos,bookmarkLerp);
                }
                return;
            }
            if (bookMarks[i].index <= currentPage && bookMarks[i+1].index > currentPage)
            {
                bookMarks[i].UIComponent.anchoredPosition = Vector2.Lerp(
                    bookMarks[i].UIComponent.anchoredPosition,
                    bookMarks[i].basePos + new Vector2(bookMarks[i].xDisplacement,0),bookmarkLerp);
            }
            else
            {
                bookMarks[i].UIComponent.anchoredPosition = Vector2.Lerp(
                    bookMarks[i].UIComponent.anchoredPosition,
                    bookMarks[i].basePos,bookmarkLerp);
            }
        }
    }



    public void UpdateBookLTRToPoint(Vector3 followLocation)
    { 
        mode = FlipMode.LeftToRight;
        f = followLocation;
        ShadowLTR.transform.SetParent(ClippingPlane.transform, true);
        ShadowLTR.transform.localPosition = new Vector3(0, 0, 0);
        ShadowLTR.transform.localEulerAngles = new Vector3(0, 0, 0);
        Left.transform.SetParent(ClippingPlane.transform, true);

        Right.transform.SetParent(BookPanel.transform, true);
        Right.transform.localEulerAngles = Vector3.zero;
        LeftNext.transform.SetParent(BookPanel.transform, true);

        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebl, out t1);
        //0 < T0_T1_Angle < 180
        clipAngle = (clipAngle + 180) % 180;

        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Left.transform.position = BookPanel.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Left.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - 90 - clipAngle);

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle - 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        LeftNext.transform.SetParent(NextPageClip.transform, true);
        Right.transform.SetParent(ClippingPlane.transform, true);
        Right.transform.SetAsFirstSibling();

        ShadowLTR.rectTransform.SetParent(Left.rectTransform, true);    
    }

    // public void SetupRTLFlip()
    // {
    //     
    //     Shadow.transform.SetParent(ClippingPlane.transform, false);
    // }
    public float lerp;
    public void UpdateBookRTLToPoint(Vector3 followLocation)
    {
        mode = FlipMode.RightToLeft;
        f = followLocation;
        // Shadow.transform.SetParent(ClippingPlane.transform, false);
        // Shadow.transform.localPosition = Vector3.zero;
        // Shadow.transform.localEulerAngles = Vector3.zero;
        Right.transform.SetParent(ClippingPlane.transform, true);

        Left.transform.SetParent(BookPanel.transform, true);
        Left.transform.localEulerAngles = Vector3.zero;
        RightNext.transform.SetParent(BookPanel.transform, true);
        c = Calc_C_Position(followLocation);
        Vector3 t1;
        float clipAngle = CalcClipAngle(c, ebr, out t1);
        if (clipAngle > -90) clipAngle += 180;

        
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);
        ClippingPlane.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);


        //FOUND THE SOLUTION HOLY FCK
        Shadow.rectTransform.pivot = ClippingPlane.rectTransform.pivot;
        Shadow.rectTransform.position = Vector3.Lerp(NextPageClip.transform.position, ClippingPlane.transform.position, lerp);
        Shadow.transform.localEulerAngles = -ClippingPlane.transform.localEulerAngles;
        ClippingPlane.transform.position = BookPanel.TransformPoint(t1);

        //page position and angle
        Right.transform.position = BookPanel.TransformPoint(c);
        float C_T1_dy = t1.y - c.y;
        float C_T1_dx = t1.x - c.x;
        float C_T1_Angle = Mathf.Atan2(C_T1_dy, C_T1_dx) * Mathf.Rad2Deg;
        Right.transform.localEulerAngles = new Vector3(0, 0, C_T1_Angle - (clipAngle + 90));

        NextPageClip.transform.localEulerAngles = new Vector3(0, 0, clipAngle + 90);
        NextPageClip.transform.position = BookPanel.TransformPoint(t1);
        RightNext.transform.SetParent(NextPageClip.transform, true);
        Left.transform.SetParent(ClippingPlane.transform, true);
        Left.transform.SetAsFirstSibling();

        Shadow.rectTransform.SetParent(Right.rectTransform, true);
    }
    private float CalcClipAngle(Vector3 c,Vector3 bookCorner,out  Vector3 t1)
    {
        Vector3 t0 = (c + bookCorner) / 2;
        float T0_CORNER_dy = bookCorner.y - t0.y;
        float T0_CORNER_dx = bookCorner.x - t0.x;
        float T0_CORNER_Angle = Mathf.Atan2(T0_CORNER_dy, T0_CORNER_dx);
        float T0_T1_Angle = 90 - T0_CORNER_Angle;
        
        float T1_X = t0.x - T0_CORNER_dy * Mathf.Tan(T0_CORNER_Angle);
        T1_X = normalizeT1X(T1_X, bookCorner, sb);
        t1 = new Vector3(T1_X, sb.y, 0);
        
        //clipping plane angle=T0_T1_Angle
        float T0_T1_dy = t1.y - t0.y;
        float T0_T1_dx = t1.x - t0.x;
        T0_T1_Angle = Mathf.Atan2(T0_T1_dy, T0_T1_dx) * Mathf.Rad2Deg;
        return T0_T1_Angle;
    }
    private float normalizeT1X(float t1,Vector3 corner,Vector3 sb)
    {
        if (t1 > sb.x && sb.x > corner.x)
            return sb.x;
        if (t1 < sb.x && sb.x < corner.x)
            return sb.x;
        return t1;
    }
    private Vector3 Calc_C_Position(Vector3 followLocation)
    {
        Vector3 c;
        f = followLocation;
        float F_SB_dy = f.y - sb.y;
        float F_SB_dx = f.x - sb.x;
        float F_SB_Angle = Mathf.Atan2(F_SB_dy, F_SB_dx);
        Vector3 r1 = new Vector3(radius1 * Mathf.Cos(F_SB_Angle),radius1 * Mathf.Sin(F_SB_Angle), 0) + sb;

        float F_SB_distance = Vector2.Distance(f, sb);
        if (F_SB_distance < radius1)
            c = f;
        else
            c = r1;
        float F_ST_dy = c.y - st.y;
        float F_ST_dx = c.x - st.x;
        float F_ST_Angle = Mathf.Atan2(F_ST_dy, F_ST_dx);
        Vector3 r2 = new Vector3(radius2 * Mathf.Cos(F_ST_Angle),
           radius2 * Mathf.Sin(F_ST_Angle), 0) + st;
        float C_ST_distance = Vector2.Distance(c, st);
        if (C_ST_distance > radius2)
            c = r2;
        return c;
    }
    public void DragRightPageToPoint(Vector3 point)
    {
        if (currentPage >= bookPages.Count) return;
        pageDragging = true;
        mode = FlipMode.RightToLeft;
        f = point;


        NextPageClip.rectTransform.pivot = new Vector2(0, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(1, 0.35f);

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(0, 0);
        Left.transform.position = RightNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        if (currentPage < bookPages.Count)
        {
            Left.sprite = bookPages[currentPage].pageSprite;
            bookPages[currentPage].UIComponent.SetParent(Left.transform);
            bookPages[currentPage].UIComponent.SetAsLastSibling();
            bookPages[currentPage].UIComponent.anchoredPosition = Vector2.zero;
        }
        else
            Left.sprite = background;
        
        Left.transform.SetAsFirstSibling();
        
        Right.gameObject.SetActive(true);
        Right.transform.position = RightNext.transform.position;
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        if (currentPage < bookPages.Count - 1)
        {
            Right.sprite = bookPages[currentPage + 1].pageSprite;
            bookPages[currentPage + 1].UIComponent.SetParent(Right.transform);
            bookPages[currentPage + 1].UIComponent.SetAsLastSibling();
            bookPages[currentPage + 1].UIComponent.anchoredPosition = Vector2.zero;
            bookPages[currentPage + 1].UIComponent.gameObject.SetActive(true);
        }
        else
            Right.sprite = background;

        if (currentPage < bookPages.Count - 2)
        {
            RightNext.sprite = bookPages[currentPage + 2].pageSprite;
            bookPages[currentPage + 2].UIComponent.SetParent(RightNext.transform);
            bookPages[currentPage + 2].UIComponent.SetAsLastSibling();
            bookPages[currentPage + 2].UIComponent.anchoredPosition = Vector2.zero;
            bookPages[currentPage + 2].UIComponent.gameObject.SetActive(true);
        }
        else
            RightNext.sprite = background;

        if (currentPage >= 2)
        {
            bookPages[currentPage - 2].UIComponent.gameObject.SetActive(false);
        }
        LeftNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) Shadow.gameObject.SetActive(true);
        UpdateBookRTLToPoint(f);
    }

    public void DragLeftPageToPoint(Vector3 point)
    {
        if (currentPage <= 0) return;
        pageDragging = true;
        mode = FlipMode.LeftToRight;
        f = point;

        NextPageClip.rectTransform.pivot = new Vector2(1, 0.12f);
        ClippingPlane.rectTransform.pivot = new Vector2(0, 0.35f);

        Right.gameObject.SetActive(true);
        Right.transform.position = LeftNext.transform.position;
        Right.sprite = bookPages[currentPage - 1].pageSprite;
        Right.transform.eulerAngles = new Vector3(0, 0, 0);
        Right.transform.SetAsFirstSibling();
        bookPages[currentPage - 1].UIComponent.SetParent(Right.transform);
        bookPages[currentPage - 1].UIComponent.SetAsLastSibling();
        bookPages[currentPage - 1].UIComponent.anchoredPosition = Vector2.zero;
        bookPages[currentPage - 1].UIComponent.gameObject.SetActive(true);
        if (currentPage + 1 < bookPages.Count)
        {
            bookPages[currentPage + 1].UIComponent.gameObject.SetActive(false);
        }

        Left.gameObject.SetActive(true);
        Left.rectTransform.pivot = new Vector2(1, 0);
        Left.transform.position = LeftNext.transform.position;
        Left.transform.eulerAngles = new Vector3(0, 0, 0);
        if (currentPage >= 2)
        {
            Left.sprite = bookPages[currentPage - 2].pageSprite;
            bookPages[currentPage - 2].UIComponent.SetParent(Left.transform);
            bookPages[currentPage - 2].UIComponent.SetAsLastSibling();
            bookPages[currentPage - 2].UIComponent.gameObject.SetActive(true);
            bookPages[currentPage - 2].UIComponent.anchoredPosition = Vector2.zero;
        }
        else
            Left.sprite = background;


        if (currentPage >= 3)
        {
            LeftNext.sprite = bookPages[currentPage - 3].pageSprite;
            bookPages[currentPage - 3].UIComponent.SetParent(LeftNext.transform);
            bookPages[currentPage - 3].UIComponent.SetAsLastSibling();
            bookPages[currentPage - 3].UIComponent.gameObject.SetActive(true);
            bookPages[currentPage - 3].UIComponent.anchoredPosition = Vector2.zero;
        }
        else
            LeftNext.sprite = background;

        RightNext.transform.SetAsFirstSibling();
        if (enableShadowEffect) ShadowLTR.gameObject.SetActive(true);
        UpdateBookLTRToPoint(f);
    }

    public void ReleasePage()
    {
        if (pageDragging)
        {
            pageDragging = false;
            TweenForward();
        }
    }
    Coroutine currentCoroutine;
    public void UpdateSprites()
    {
        if (currentPage > 0 && currentPage <= bookPages.Count)
        {
            LeftNext.sprite = bookPages[currentPage - 1].pageSprite;
            bookPages[currentPage - 1].UIComponent.SetParent(LeftNext.transform);
            bookPages[currentPage - 1].UIComponent.SetAsLastSibling();
            bookPages[currentPage - 1].UIComponent.anchoredPosition = Vector2.zero;
            bookPages[currentPage - 1].UIComponent.localRotation = Quaternion.identity;
            bookPages[currentPage - 1].UIComponent.gameObject.SetActive(true);
            if (currentPage - 3 >=1)
            {
                bookPages[currentPage - 3].UIComponent.gameObject.SetActive(false);
            }
        }
        else
            LeftNext.sprite = background;

        if (currentPage >= 0 && currentPage < bookPages.Count)
        {
            RightNext.sprite = bookPages[currentPage].pageSprite;
            bookPages[currentPage].UIComponent.SetParent(RightNext.transform);
            bookPages[currentPage].UIComponent.SetAsLastSibling();
            bookPages[currentPage].UIComponent.anchoredPosition = Vector2.zero;
            bookPages[currentPage].UIComponent.localRotation = Quaternion.identity;
            bookPages[currentPage].UIComponent.gameObject.SetActive(true);
        }
        else
            RightNext.sprite = background;

        if (currentPage + 2 < bookPages.Count)
        {
            
            bookPages[currentPage + 1].UIComponent.gameObject.SetActive(false);   
            bookPages[currentPage + 2].UIComponent.gameObject.SetActive(false);
        }
    }
    public void TweenForward()
    {
        if(mode== FlipMode.RightToLeft)
        currentCoroutine = StartCoroutine(TweenTo(ebl, 0.15f, () => { Flip(); }));
        else
        currentCoroutine = StartCoroutine(TweenTo(ebr, 0.15f, () => { Flip(); }));
    }
    void Flip()
    {
        if (mode == FlipMode.RightToLeft)
            currentPage += 2;
        else
            currentPage -= 2;
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.transform.SetParent(BookPanel.transform, true);
        LeftNext.transform.SetParent(BookPanel.transform, true);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);
        Right.transform.SetParent(BookPanel.transform, true);
        RightNext.transform.SetParent(BookPanel.transform, true);
        UpdateSprites();
        Shadow.gameObject.SetActive(false);
        ShadowLTR.gameObject.SetActive(false);
        if (OnFlip != null)
            OnFlip.Invoke();
    }
    public IEnumerator TweenTo(Vector3 to, float duration, System.Action onFinish)
    {
        int steps = (int)(duration / 0.025f);
        Vector3 displacement = (to - f) / steps;
        for (int i = 0; i < steps-1; i++)
        {
            if(mode== FlipMode.RightToLeft)
                UpdateBookRTLToPoint( f + displacement);
            else
                UpdateBookLTRToPoint(f + displacement);

            yield return new WaitForSeconds(0.025f);
        }
        if (onFinish != null)
            onFinish();
    }



}
