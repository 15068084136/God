using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using PEProtocal;

public class VendorSlot : Slot {
    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right &&InventorySys.instance.IsPickedItem==false )
        {
            if (transform.childCount > 0)
            {
                Item currentItem = transform.GetChild(0).GetComponent<ItemUI>().Item;
                // 获取所购买物品的信息，并传递出去
                InventorySys.instance.GetItemByBuy(currentItem);
                //transform.parent.parent.SendMessage("BuyItem", currentItem);
                GameMsg msg = new GameMsg{
                    cmd = (int)Command.reqBuyItem,
                    reqBuyItem = new ReqBuyItem{
                        sellCoin = currentItem.SellPrice
                    }
                };
                NetService netService = InventorySys.instance.GetNetService();
                netService.SendMsg(msg);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left && InventorySys.instance.IsPickedItem == true)
        {
            InventorySys.instance.vendorWin.SellItem();
        }
        
    }
}
