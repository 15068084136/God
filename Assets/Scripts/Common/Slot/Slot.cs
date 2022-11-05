using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public GameObject itemPrefab;
    /// <summary>
    /// 把item放在自身下面
    /// 如果自身下面已经有item了，amount++
    /// 如果没有 根据itemPrefab去实例化一个item，放在下面
    /// </summary>
    /// <param name="item"></param>
    public void StoreItem(Item item)
    {
        // 如果Slot下没有子物体。则实例化一个子物体 
        if (transform.childCount == 0)
        {
            GameObject itemGameObject = Instantiate(itemPrefab) as GameObject;
            itemGameObject.transform.SetParent(this.transform);
            itemGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            itemGameObject.transform.localPosition = Vector3.zero;
            itemGameObject.GetComponent<ItemUI>().SetItem(item);
        }
        else
        {
            // 如果有子物体，则添加一个
            transform.GetChild(0).GetComponent<ItemUI>().AddAmount();
        }
    }

    /// <summary>
    /// 得到当前物品槽存储的物品类型
    /// </summary>
    /// <returns></returns>
    public Item.ItemType GetItemType()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }

    /// <summary>
    /// 得到当前这个格子物品的id
    /// </summary>
    /// <returns></returns>
    public int GetItemId()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }
    // 当前这个格子是否已经满了
    public bool IsFilled()
    {
        ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
        return itemUI.Amount >= itemUI.Item.Capacity;//当前的数量大于等于容量
    }

    #region 点击事件
    public void OnPointerExit(PointerEventData eventData)
    {
        if(transform.childCount>0)
            InventorySys.instance.HideToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.childCount > 0)
        {
            string toolTipText = transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
            InventorySys.instance.ShowToolTip(toolTipText);
        }
        
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (InventorySys.instance.IsPickedItem==false&& transform.childCount > 0)
            {
                ItemUI currentItemUI = transform.GetChild(0).GetComponent<ItemUI>();
                if (currentItemUI.Item is Equipment || currentItemUI.Item is Weapon)
                {
                    currentItemUI.ReduceAmount(1);
                    Item currentItem = currentItemUI.Item;
                    if (currentItemUI.Amount <= 0)
                    {
                        DestroyImmediate(currentItemUI.gameObject);
                        InventorySys.instance.HideToolTip();
                    }
                    CharacterPanel.Instance.PutOn(currentItem);   
                }
            }
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;
        // 自身是空 1,IsPickedItem ==true  pickedItem放在这个位置
                            // 按下ctrl      放置当前鼠标上的物品的一个
                            // 没有按下ctrl   放置当前鼠标上的物品的所有
                 //2,IsPickedItem==false  不做任何处理
        // 自身不是空 
                 //1,IsPickedItem==true
                        //自身的id==pickedItem.id  
                                    // 按下ctrl      放置当前鼠标上的物品的一个
                                    // 没有按下ctrl   放置当前鼠标上的物品的所有
                                                    //可以完全放下
                                                    //只能放下其中一部分
                        //自身的id!=pickedItem.id   pickedItem跟当前物品交换          
                 //2,IsPickedItem==false
                        //ctrl按下 取得当前物品槽中物品的一半
                        //ctrl没有按下 把当前物品槽里面的物品放到鼠标上
        // 当前存在子物体
        if (transform.childCount > 0)
        {
            ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();// 获得当前物品槽的信息

            if (InventorySys.instance.IsPickedItem == false)//当前没有选中任何物品( 当前手上没有任何物品)当前鼠标上没有任何物品
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    int amountPicked = (currentItem.Amount + 1) / 2;// 相当于在奇数的情况下，捡起的会比余下的多一个
                    InventorySys.instance.PickupItem(currentItem.Item, amountPicked);
                    int amountRemained = currentItem.Amount - amountPicked;
                    if (amountRemained <= 0)
                    {
                        // 如果余下的物品数量小于0
                        Destroy(currentItem.gameObject);//销毁当前物品
                    }
                    else
                    {
                        currentItem.SetAmount(amountRemained);// 更新剩余的数量
                    }
                }
                else
                {
                    InventorySys.instance.PickupItem(currentItem.Item,currentItem.Amount);
                    Destroy(currentItem.gameObject);//销毁当前物品
                }
            }else// 当前鼠标上有物体
            {
                //1,IsPickedItem==true
                    //自身的id==pickedItem.id  
                        // 按下ctrl      放置当前鼠标上的物品的一个
                        // 没有按下ctrl   放置当前鼠标上的物品的所有
                            //可以完全放下
                            //只能放下其中一部分
                    //自身的id!=pickedItem.id   pickedItem跟当前物品交换          
                if (currentItem.Item.ID == InventorySys.instance.PickedItem.Item.ID)
                {
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        if (currentItem.Item.Capacity > currentItem.Amount)//当前物品槽还有容量
                        {
                            currentItem.AddAmount();// 当前物品槽物品数量＋1
                            InventorySys.instance.RemoveItem();// 鼠标上的PickupItem的数量 - 1
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (currentItem.Item.Capacity > currentItem.Amount)
                        {
                            int amountRemain = currentItem.Item.Capacity - currentItem.Amount;//当前物品槽剩余的空间
                            if (amountRemain >= InventorySys.instance.PickedItem.Amount)
                            {
                                currentItem.SetAmount(currentItem.Amount + InventorySys.instance.PickedItem.Amount);
                                InventorySys.instance.RemoveItem(InventorySys.instance.PickedItem.Amount);
                            }
                            else
                            {
                                currentItem.SetAmount(currentItem.Amount + amountRemain);
                                InventorySys.instance.RemoveItem(amountRemain);
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    Item item = currentItem.Item;
                    int amount = currentItem.Amount;
                    currentItem.SetItem(InventorySys.instance.PickedItem.Item, InventorySys.instance.PickedItem.Amount);
                    InventorySys.instance.PickedItem.SetItem(item, amount);
                }

            }
        }
        else// 当前不存在子物体
        {
            // 自身是空  
                        //1,IsPickedItem ==true  pickedItem放在这个位置
                            // 按下ctrl      放置当前鼠标上的物品的一个
                            // 没有按下ctrl   放置当前鼠标上的物品所有数量
                        //2,IsPickedItem==false  不做任何处理
            // 
            if (InventorySys.instance.IsPickedItem == true)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    // 存一个
                    this.StoreItem(InventorySys.instance.PickedItem.Item);
                    // 手上减一个
                    InventorySys.instance.RemoveItem();
                }
                else
                {
                    // 将手上所有的物品全部放到当前格子里
                    for (int i = 0; i < InventorySys.instance.PickedItem.Amount; i++)
                    {
                        this.StoreItem(InventorySys.instance.PickedItem.Item);
                    }
                    InventorySys.instance.RemoveItem(InventorySys.instance.PickedItem.Amount);
                }
            }
            else
            {
                return;
            }

        }
    }
    #endregion
}
