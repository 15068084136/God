using UnityEngine;
using System.Collections;
using PEProtocal;

public class VendorWin : Inventory {

    public int[] itemIdArray;

    private Player player;

    protected override void InitWin()
    {
        base.InitWin();
        InitShop();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    protected override void ClearWin()
    {
        base.ClearWin();
        ClearShopItem();
    }

    private void InitShop()
    {
        foreach (int itemId in itemIdArray)
        {
            StoreItem(itemId);
        }
    }
    /// <summary>
    /// 主角出售物品
    /// </summary>
    public void SellItem()
    {
        int sellAmount = 1;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            sellAmount = 1;
        }
        else
        {
            sellAmount = InventorySys.instance.PickedItem.Amount;
        }

        int coinAmount = InventorySys.instance.PickedItem.Item.SellPrice * sellAmount;
        GameMsg msg = new GameMsg{
            cmd = (int)Command.reqSellItem,
            reqSellItem = new ReqSellItem{
                earnCoin = coinAmount
            }
        };
        netService.SendMsg(msg);
        InventorySys.instance.RemoveItem(sellAmount);
    }

    public void ClickCloseBtn(){
        SetWinState(false);
    }

    // 清空商店物品
    public void ClearShopItem(){
        foreach (var item in slotList)
        {

            if(item.transform.childCount > 0){
                Destroy(item.transform.GetChild(0).gameObject);
            }
        }
    }
}
