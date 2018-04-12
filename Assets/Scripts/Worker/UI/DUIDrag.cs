using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * UI 拖动脚本
 * 
 */

public class DUIDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler,
    IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DUIDrag()
    {
        mouseDragPosOffist = new Vector2();
    }

    /// <summary>
    /// 是否正在拖动
    /// </summary>
    public bool isDrag
    {
        get; private set;
    }
    public bool isDraged
    {
        get
        {
            bool rs = _isDraged;
            _isDraged = false;
            return rs;
        }
    }

    public Vector2 DragOffist { get { return mouseDragPosOffist; } }
    private bool _isDraged = false;
    private Vector2 mouseDragPosOffist;
    private Vector2 mouseDownPos;

    //当鼠标按下时调用 接口对应  IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
        mouseDownPos = eventData.position;    //记录鼠标按下时的屏幕坐标
    }
    //当鼠标拖动时调用   对应接口 IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mouseDrag = eventData.position;   //当鼠标拖动时的屏幕坐标
        mouseDragPosOffist.Set(mouseDrag.x - mouseDownPos.x, mouseDrag.y - mouseDownPos.y);
        _isDraged = true;
    }
    //当鼠标抬起时调用  对应接口  IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
    }
    //当鼠标结束拖动时调用   对应接口  IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
    }
    //当鼠标进入图片时调用   对应接口   IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
    }
    //当鼠标退出图片时调用   对应接口   IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
