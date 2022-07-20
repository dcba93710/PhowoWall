using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
public class PhotoWallCtrl : MonoBehaviour
{
    public static PhotoWallCtrl Instance;
    public List<Texture2D> texture2Ds;

    public Transform curSelect;
    public float minDistance = 400;
    public float doEndPosTime = 0.5f;

    public float range = 1200;
    public float repulsion = 15;
    public float targetAttractive = 3;

    [HideInInspector]
    public List<PhotoWallBrick> photoes;

    private PhotoWallLine[] photoWallLines;
    private Dictionary<PhotoWallBrick, Vector3> curMovePhotoes;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        curMovePhotoes = new Dictionary<PhotoWallBrick, Vector3>();
        photoWallLines = transform.GetComponentsInChildren<PhotoWallLine>();

        int num = texture2Ds.Count / photoWallLines.Length;
        int rem = texture2Ds.Count % photoWallLines.Length;

        int index = 0;
        for (int i = 0; i < photoWallLines.Length; i++)
        {
            if (i < photoWallLines.Length - 1)
            {
                for (int j = 0; j < num; j++)
                {
                    photoWallLines[i].texture2Ds.Add(texture2Ds[index]);
                    index++;
                }
            }
            else
            {
                num += rem;
                for (int j = 0; j < num; j++)
                {
                    photoWallLines[i].texture2Ds.Add(texture2Ds[index]);
                    index++;
                }
            }

            photoWallLines[i].Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (curSelect != null)
        {
            Avoid(curSelect,minDistance);
        }
    }


    private void Avoid(Transform forceAnchor_, float Max)
    {

        foreach (var item in curMovePhotoes)
        {
            if (item.Key.transform != forceAnchor_)
            {
                var velocity = Vector3.zero;
                Vector3 vec = item.Key.transform.position;
                var len = (vec - forceAnchor_.position).magnitude;  //计算距离
                if (len < Max)
                {
                    var rate = (Max - len) / range;
                    var intensity = repulsion * rate;     //根据距离来计算力的强度（如果没有这一步，会出现抖动）
                    velocity += (item.Key.transform.position - forceAnchor_.position) * intensity * Time.deltaTime;  //计算斥力
                }
                velocity += (item.Value - item.Key.transform.position) * Time.deltaTime * targetAttractive;
                item.Key.transform.position += velocity;
            }

        }
    }
    public void OpenDoEndPos()
    {
        foreach (var item in curMovePhotoes)
        {
            if (item.Key.transform.position != item.Value && item.Key.transform != curSelect)
            {
                item.Key.transform.DOMove(item.Value, doEndPosTime);
            }
        }
    }
    public void SetBrick(Transform tran)
    {
        if (curSelect != null)
        {
            curSelect.GetComponent<PhotoWallBrick>().P_CloseFun();
            curSelect = tran;
            Pause();
        }
        else
        {
            curSelect = tran;
            curMovePhotoes.Clear();
            foreach (var item in photoes)
            {
                curMovePhotoes.Add(item, item.transform.position);
            }
            Pause();
        }
        
    }
    public void Pause()
    {
        foreach (var item in photoWallLines)
        {
            item.isPlay = false;
        }
    }
    public void Play()
    {
        foreach (var item in photoWallLines)
        {
            item.isPlay = curSelect == null;
        }
        // gameObject.SetActive(true);
    }

}
