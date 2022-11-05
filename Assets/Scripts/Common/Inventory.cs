using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using PEProtocal;

public class Inventory : WinRoot
{
    protected Slot[] slotList;

    private float targetAlpha = 1;

    private float smoothing = 4;

    protected PlayerData pd;

    public CanvasGroup canvasGroup;

    protected override void InitWin()
    {
        base.InitWin();
        slotList = GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < .01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    #region Slot相关方法
    // 通过ID存储一个物品
    public bool StoreItem(int id)
    {
        Item item = resService.GetItemById(id);
        return StoreItem(item);
    }
    public bool StoreItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("要存储的物品的id不存在");
            return false;
        }
        if (item.Capacity == 1)
        {
            Slot slot = FindEmptySlot();
            if (slot == null)
            {
                Debug.LogWarning("没有空的物品槽");
                return false;
            }
            else
            {
                slot.StoreItem(item);//把物品存储到这个空的物品槽里面
            }
        }
        else
        {
            // 寻找相同物品ID的物品槽，且这个Slot不能装满
            Slot slot = FindSameIdSlot(item);
            if (slot != null)
            {
                // 找到了这个物品槽
                slot.StoreItem(item);
            }
            else
            {
                // 没有找到这个相同ID的物品槽，则找空的物品槽
                Slot emptySlot = FindEmptySlot();
                if (emptySlot != null)
                {
                    emptySlot.StoreItem(item);
                }
                else
                {
                    Debug.LogWarning("没有空的物品槽");
                    return false;// 存储失败
                }
            }
        }
        return true;// 除了空物品槽找不到的情况为false
    }

    /// <summary>
    /// 这个方法用来找到一个空的物品槽
    /// </summary>
    /// <returns></returns>
    private Slot FindEmptySlot()
    {
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    // 找到跟添加物品同一个ID的物品槽
    private Slot FindSameIdSlot(Item item)
    {
        foreach (Slot slot in slotList)
        {
            // Slot要有子物体ItemUI， 并且要得到这个物品槽当前的ID进行比较，并且这个物品槽还不能满
            if (slot.transform.childCount >= 1 && slot.GetItemId() == item.ID &&slot.IsFilled()==false )
            {
                return slot;
            }
        }
        return null;
    }
    #endregion

    #region 背包显示方法
    public void Show()
    {
        // 存在射线检测，可以放物品
        canvasGroup.blocksRaycasts = true;
        targetAlpha = 1;
    }
    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        targetAlpha = 0;
    }
    public void DisplaySwitch()
    {
        if (targetAlpha == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    #endregion

    #region save and load
    public void SaveInventory()
    {
        pd = GameRoot.instance.PlayerData;
        StringBuilder sb = new StringBuilder();
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                sb.Append(itemUI.Item.ID + ","+itemUI.Amount+"-");
            }
            else
            {
                sb.Append("0-");
            }
        }
        PlayerPrefs.SetString(pd.id + this.gameObject.name, sb.ToString());
    }
    public void LoadInventory()
    {
        // 如果当前有物体存在，先销毁物体
        foreach (Slot slot in slotList)
        {
            if(slot.transform.childCount > 0){
                return;
            }
        }

        pd = GameRoot.instance.PlayerData;
        if (PlayerPrefs.HasKey(pd.id + this.gameObject.name) == false) return;
        string str = PlayerPrefs.GetString(pd.id + this.gameObject.name);
        //print(str);
        string[] itemArray = str.Split('-');
        for (int i = 0; i < itemArray.Length-1; i++)
        {
            string itemStr = itemArray[i];
            if (itemStr != "0")
            {
                //print(itemStr);
                string[] temp = itemStr.Split(',');
                int id = int.Parse(temp[0]);
                Item item = resService.GetItemById(id);
                int amount = int.Parse(temp[1]);
                for (int j = 0; j < amount; j++)
                {
                    slotList[i].StoreItem(item);
                }
            }
        }
    }
    #endregion
}
