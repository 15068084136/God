using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI事件监控
/// </summary>
public class PEListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    // 定义事件
    public Action<object> onClick;
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;

    // 参数
    public object args;

    public void OnDrag(PointerEventData eventData)
    {
        if(onDrag != null){
            onDrag(eventData);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(onClick != null){
            onClick(args);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(onClickDown != null){
            onClickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(onClickUp != null){
            onClickUp(eventData);
        }
    }
}
