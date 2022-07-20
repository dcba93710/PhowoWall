using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
public class PhotoWallLine : MonoBehaviour
{   
    private RectTransform thisRect;
    private Vector2 v2Dir;
    private int curTexIndex;
    private Vector2 endPos;  //图片到达的终点
    
    
    
    [HideInInspector]
    public List<Texture2D> texture2Ds;  //要使用的图片数量  
    public Transform endTarget;
    public PhotoWallCtrl photoWallCtrl;
    public float speed = 50;
    public float offect;  //图片之间的间隔
    public int count;       //创建的图片数量
    public float HeightLimit;  //高度限制

    [Range(-1, 1)]
    public int dir;         //移动方向
    public bool isPlay;     //是否移动
    
    public void Init()
    {
        photoWallCtrl = transform.GetComponentInParent<PhotoWallCtrl>();
        thisRect = GetComponent<RectTransform>();
        v2Dir = new Vector2(dir, 0);
        float lastX = 0;
        for (int i = 0; i < count; i++)
        {
            RawImage raw = new GameObject().AddComponent<RawImage>();
            Button button = raw.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            if (curTexIndex >= texture2Ds.Count)
            {
                curTexIndex = 0;
            }
            raw.name = (curTexIndex + 1).ToString() + ":" + photoWallCtrl.texture2Ds[curTexIndex].name;
            raw.texture = texture2Ds[curTexIndex];
            curTexIndex++;
            raw.SetNativeSize();
            
            raw.transform.SetParent(transform);

            RectTransform rect = raw.GetComponent<RectTransform>();
            rect.sizeDelta = CalculateWH(rect.sizeDelta);

            rect.anchoredPosition = new Vector2(lastX + rect.sizeDelta.x / 2 * dir, 0);

            lastX = rect.anchoredPosition.x + (rect.sizeDelta.x / 2 + offect) * dir;
            

            photoWallCtrl.photoes.Add(raw.gameObject.AddComponent<PhotoWallBrick>());
        }
        endPos = endTarget.position;

        isPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlay)
        {
            transform.Translate(speed * Time.deltaTime * v2Dir);


            RectTransform tmp_rect = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
            if (Math.Abs(tmp_rect.position.x) > Math.Abs(endPos.x) + tmp_rect.sizeDelta.x / 2)
            {
                curTexIndex++;
                if (curTexIndex >= texture2Ds.Count)
                {
                    curTexIndex = 0;
                }
                RawImage tmp_raw = tmp_rect.GetComponent<RawImage>();
                tmp_raw.texture = texture2Ds[curTexIndex];
                tmp_raw.SetNativeSize();

                tmp_rect.sizeDelta = CalculateWH(tmp_rect.sizeDelta);
                RectTransform zeroRect = transform.GetChild(0).GetComponent<RectTransform>();

                float newPosX = zeroRect.anchoredPosition.x - (zeroRect.sizeDelta.x / 2 + tmp_rect.sizeDelta.x / 2 + offect) * dir;
                tmp_rect.anchoredPosition = new Vector2(newPosX, tmp_rect.anchoredPosition.y);

                tmp_rect.transform.SetAsFirstSibling();
                tmp_rect.name = (curTexIndex + 1) + ":" + photoWallCtrl.texture2Ds[curTexIndex].name;
            }

        }
    }


    /// <summary>
    /// 通过高度限制来重新计算图片的宽高
    /// </summary>
    public Vector2 CalculateWH(Vector2 size)
    {
        float rate = 0;
        if (size.y > HeightLimit)
        {
            rate = size.y / HeightLimit;

            return size / rate;
        }
        else
        {
            rate = HeightLimit / size.y;
            return size * rate;
        }

        
    }
}
