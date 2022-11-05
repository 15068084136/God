using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolTipWin : WinRoot {

    public Text toolTipText;
    public Text contentText;
    public CanvasGroup canvasGroup;

    private float targetAlpha = 0 ;

    public float smoothing = 2;

    protected override void InitWin()
    {
        base.InitWin();
    }

    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha,smoothing*Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < 0.05f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    public void Show(string text)
    {
        InventorySys.instance.SetToolTipWinState();
        toolTipText.text = text;
        contentText.text = text;
        targetAlpha = 1;
    }
    public void Hide()
    {
        targetAlpha = 0;
        
    }
    public void SetLocalPotion(Vector3 position)
    {
        transform.localPosition = position;
    }
	
}
