using UnityEngine;
using System.Collections;

public class ChestWin : Inventory {

    protected override void InitWin()
    {
        base.InitWin();
        InventorySys.instance.LoadChest();
    }

    public void ClickCloseBtn(){
        InventorySys.instance.SaveChest();
        SetWinState(false);
    }
}
