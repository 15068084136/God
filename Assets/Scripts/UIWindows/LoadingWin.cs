using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载界面
/// </summary>
public class LoadingWin : WinRoot
{
    #region UIDefine
    public Text tips;
    // 加载进度条
    public Image loadImage;
    public Image pointImage;
    // 加载进度数字
    public Text progressTxt;
    public Image LoadBg;// 加载界面
    #endregion

    private float loadImageWidth;

    /// <summary>
    /// 初始化加载页面的进度条
    /// </summary>
    protected override void InitWin(){
        base.InitWin();

        RandomLoadBg();

        loadImageWidth = loadImage.GetComponent<RectTransform>().sizeDelta.x;

        SetText(tips, "感谢观看！！！！");
        SetText(progressTxt, "0%");
        loadImage.fillAmount = 0;
        pointImage.transform.localPosition = new Vector3(-(loadImageWidth/2), 0, 0);
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    public void SetProgress(float progress){
        SetText(progressTxt, (int) (progress * 100) + "%");
        loadImage.fillAmount = progress;

        float posX = progress * loadImageWidth - (loadImageWidth/2);
        pointImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }

    // 随机加载界面背景
    public void RandomLoadBg(){
        int index = (int)Random.Range(1, 3);
        switch (index)
        {
            case 1:
                Sprite sprite = resService.LoadSprite(Constants.loadMainCityImage, true);
                LoadBg.sprite = sprite;
                break;
            case 2:
                Sprite sprite1 = resService.LoadSprite(Constants.loadFuBenImage, true);
                LoadBg.sprite = sprite1;
                break;
        }
    }
}
