using System.Net.Mime;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.EventSystems;

public class PhotoWallBrick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public int count;
    public Vector2 scale = new Vector2(3, 3);
    public float time = 0.5f;
    public PhotoWallLine photoWallLine;
    public RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        photoWallLine = transform.GetComponentInParent<PhotoWallLine>();
        rawImage = transform.GetComponent<RawImage>();
        GetComponent<Button>().onClick.AddListener(OnClickEvent);
    }

    public bool isDrag;
    private Image img;//实例化后的对象
    Vector3 offPos;//存储按下鼠标时的图片-鼠标位置差
    Vector3 arragedPos; //保存经过整理后的向量，用于图片移动
    /// <summary>
    /// 开始拖拽的时候
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (count == 1)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.GetComponent<RectTransform>(), Input.mousePosition
    , eventData.enterEventCamera, out arragedPos))
            {
                offPos = transform.position - arragedPos;
            }
            isDrag = true;
        }

    }
    /// <summary>
    /// 拖拽中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag && count == 1)
        {
            // Debug.Log(offPos + "   |   " + Input.mousePosition);
            transform.position = offPos + Input.mousePosition;
        }
        
    }
    /// <summary>
    /// 拖拽结束，图片停留在结束位置
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDrag && count == 1)
        {
            transform.position = offPos + Input.mousePosition;
        }

        isDrag = false;
    }


    public void OnClickEvent()
    {
        if (isDrag)
        {
            return;
        }
        count++;

        if (count == 1)
        {
            photoWallLine.photoWallCtrl.SetBrick(transform);
            transform.DOScale(scale, time);

            Canvas tmp_canvas = gameObject.AddComponent<Canvas>();
            tmp_canvas.overrideSorting = true;
            tmp_canvas.sortingOrder = 5;
            gameObject.AddComponent<GraphicRaycaster>();
            
        }
        else if(count > 1)
        {
            CloseFun();
        }
    }


    private void CloseFun()
    {
        photoWallLine.photoWallCtrl.curSelect = null;
        photoWallLine.photoWallCtrl.OpenDoEndPos();
        transform.DOScale(Vector2.one, time).onComplete = () =>
        {
            Destroy(transform.GetComponent<GraphicRaycaster>());
            Destroy(transform.GetComponent<Canvas>());
            
            photoWallLine.photoWallCtrl.Play();
        };
        count = 0;
    }

    public void P_CloseFun()
    {

        photoWallLine.photoWallCtrl.OpenDoEndPos();
        transform.DOScale(Vector2.one, time).onComplete=()=>{
            Destroy(transform.GetComponent<GraphicRaycaster>());
            Destroy(transform.GetComponent<Canvas>());
            
        };
        count = 0;
    }
}
