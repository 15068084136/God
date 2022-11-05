using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PEProtocal;

public class KnapsackWin : Inventory
{
    #region UIDefine
    public Text coinTxt;
    public Text DiaTxt;
    #endregion

    protected override void InitWin()
    {
        base.InitWin();
        InventorySys.instance.LoadKnapsack();
        RefreshUI();
    }

    public void ClickCloseBtn(){
        InventorySys.instance.SaveKnapsack();
        SetWinState(false);
    }

    public void RefreshUI(){
        pd = GameRoot.instance.PlayerData;
        SetText(coinTxt, pd.coin);
        SetText(DiaTxt, pd.diamond);
    }
}
