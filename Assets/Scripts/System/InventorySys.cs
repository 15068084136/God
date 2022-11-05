using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

/// <summary>
/// 背包业务系统
/// </summary>
public class InventorySys : SystemRoot
{
    public static InventorySys instance;

    // 物品信息列表
    private List<Item> itemList = new List<Item>();

    #region WinDefine
    public ItemUI pickedItemWin;
    public ToolTipWin toolTipWin;
    public KnapsackWin knapsackWin;
    public ChestWin chestWin;
    public VendorWin vendorWin;
    #endregion

    public override void InitSys(){
        base.InitSys();
        instance = this;
        itemList = resService.itemList;
        // toolTip只有一个
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        pickedItemWin.Hide();
    }

    #region ToolTip
    private bool isToolTipShow = false;// ToolTip是否在显示状态

    private Vector2 toolTipPosionOffset = new Vector2(10, -10);// ToolTip的偏移
    #endregion

    #region PickedItem
    private bool isPickedItem = false;// 当前是否选中了物体

    public bool IsPickedItem
    {
        get
        {
            return isPickedItem;
        }
    }
    public ItemUI PickedItem
    {
        get
        {
            return pickedItemWin;
        }
    }
    #endregion

    private Canvas canvas;

    // private void Start() {
    //     LitJsonTest();
    // }

    // public void LitJsonTest(){
    //     itemList = new List<Item>();
    //     TextAsset itemText = Resources.Load<TextAsset>("Items");
    //     string itemsStr = itemText.text;
    //     JsonData itemsJson = JsonMapper.ToObject(itemsStr);
    //     foreach (JsonData item in itemsJson)
    //     {
    //         Debug.Log(item["name"].ToString() + ":" + (int)item["id"]);
    //     }
    // }

    void Update()
    {
        if (isPickedItem)
        {
            //如果我们捡起了物品，我们就要让物品跟随鼠标
            Vector2 position;
            // 得到鼠标在画布上的局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            pickedItemWin.SetLocalPosition(position);
        }else if (isToolTipShow)
        {
            //控制提示面板跟随鼠标
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
            // 设置ToolTip的相对位置
            toolTipWin.SetLocalPotion(position+toolTipPosionOffset);
        }

        //物品丢弃的处理  点击鼠标左键，并且鼠标上有东西，并且鼠标下方没有物体
        if (isPickedItem && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)==false)
        {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }

    #region ToolTipWin
    // 展示ToolTip
    public void ShowToolTip(string content)
    {
        // 如果当前鼠标上有物体拿着，就不允许ToolTip显示
        if (this.isPickedItem) return;
        isToolTipShow = true;
        toolTipWin.Show(content);
    }

    public void HideToolTip()
    {
        isToolTipShow = false;
        toolTipWin.Hide();
    }
    public void SetToolTipWinState(bool isActive = true){
        toolTipWin.SetWinState(isActive);
    }
    #endregion

    #region PickupItemWin
    //捡起物品槽指定数量的物品
    public void PickupItem(Item item,int amount)
    {
        PickedItem.SetItem(item, amount);// 将ItemUI上的信息复制到了PickedItem上
        isPickedItem = true;

        PickedItem.Show();
        this.toolTipWin.Hide();// 在拿起物品后将ToolTip隐藏
        //如果我们捡起了物品，我们就要让物品跟随鼠标
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
        pickedItemWin.SetLocalPosition(position);
    }

    /// <summary>
    /// 从手上拿掉一个物品放在物品槽里面
    /// </summary>
    public void RemoveItem(int amount=1)
    {
        PickedItem.ReduceAmount(amount);
        if (PickedItem.Amount <= 0)
        {
            isPickedItem = false;
            PickedItem.Hide();
        }
    }
    #endregion

    #region Save and Load
    public void SaveKnapsack()
    {
        knapsackWin.SaveInventory();
        //CharacterPanel.Instance.SaveInventory();
        //Forge.Instance.SaveInventory();
    }
    // 保存仓库数据
    public void SaveChest(){
        chestWin.SaveInventory();
    }

    public void LoadKnapsack()
    {
        knapsackWin.LoadInventory();
        //CharacterPanel.Instance.LoadInventory();
        //Forge.Instance.LoadInventory();
    }

    public void LoadChest(){
        chestWin.LoadInventory();
    }
    #endregion

    #region KnapsackWIN
    public void OpenKnapsackWin(){
        knapsackWin.SetWinState();
    }
    #endregion

    #region ChestWin
    public void OpenChestWin(){
        chestWin.SetWinState();
    }
    #endregion

    #region VendorWin
    public void OpenVendorWin(){
        vendorWin.SetWinState();
    }
    public Item currentBuyItem;

    // 获取当前所购买的物品
    public Item GetItemByBuy(Item item){
        currentBuyItem = item;
        return currentBuyItem;
    }
    
    public void RspBuyItem(GameMsg msg){
        RspBuyItem data = msg.rspBuyItem;
        GameRoot.instance.SetPlayerDataByBuyItem(data);
        // 购买物品
        knapsackWin.StoreItem(currentBuyItem);
        // 刷新背包UI
        knapsackWin.RefreshUI();
    }

    public void RspSellItem(GameMsg msg){
        RspSellItem data = msg.rspSellItem;
        GameRoot.instance.SetPlayerDataBySellItem(data);
        // 刷新背包UI
        knapsackWin.RefreshUI();
    }
    #endregion

    // 获得网络服务系统
    public NetService GetNetService(){
        return netService;
    }
}
