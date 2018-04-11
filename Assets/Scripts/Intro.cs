using Assets.Scripts.Global;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public RectTransform ImageProgress;
    public Image ImageMaskAll;
    public Image ImageLogo;
    public Image ImageCloud1;
    public Image ImageCloud2;
    public RectTransform Cloud1;
    public RectTransform Cloud2;
    public float MoveSpeed = 0.3f;
    public int NextScense;
    public GameObject PanelError;
    public Text TextError;
    public Text TextProgress;

    private bool movingCloud = true;
    private bool fadeOut = false;
    private float alpha = 1;
    private bool errored = false;
    private bool finished = false;
    private bool allFadeout = false;

    void ErrExitButton_Clicked()
    {
        GlobalMediator.ExitGame();
    }
    void ErrReloadButton_Clicked()
    {
        errored = false;
        StartCoroutine(GameInit());
        PanelError.SetActive(false);
    }
    void Start()
    {     
        GlobalSettings.StartInIntro = true;
        GlobalSettings.GlobalSettingsInit();
        GlobalMediator.GlobalMediatorInitialization();
        GlobalModLoader.GlobalModLoaderInitialization();
        StartCoroutine(GameInit());
    }
    void Update()
    {
        if (movingCloud)
        {
            if (alpha > 0)
            {
                alpha -= MoveSpeed * Time.deltaTime;
                ImageCloud1.color = new Color(1, 1, 1, alpha);
                ImageCloud2.color = new Color(1, 1, 1, alpha);

                Cloud1.offsetMax = new Vector2(Cloud1.offsetMax.x - 0.45f, Cloud1.offsetMax.y - 0.45f);
                Cloud1.offsetMin = new Vector2(Cloud1.offsetMin.x - 0.65f, Cloud1.offsetMin.y - 0.35f);

                Cloud2.offsetMax = new Vector2(Cloud2.offsetMax.x + 0.45f, Cloud2.offsetMax.y - 0.45f);
                Cloud2.offsetMin = new Vector2(Cloud2.offsetMin.x + 0.65f, Cloud2.offsetMin.y - 0.35f);
            }
            else
            {
                ImageCloud1.color = new Color(1, 1, 1, 0);
                ImageCloud2.color = new Color(1, 1, 1, 0);
                alpha = 1;
                movingCloud = false;
                fadeOut = true;
            }
        }
        else if (fadeOut)
        {
            if (alpha > 0)
            {
                alpha -= 1 * Time.deltaTime;
                ImageLogo.color = new Color(1, 1, 1, alpha);
            }
            else
            {
                alpha = 0;
                fadeOut = false;
                if (errored)
                    PanelError.SetActive(true);
                else if (finished)
                    StartCoroutine(InitMenu());
            }
        }
        else if (allFadeout)
        {
            if (alpha < 1)
            {
                alpha += 1 * Time.deltaTime;
                ImageMaskAll.color = new Color(0, 0, 0, alpha);
            }
            else
            {
                alpha = 1;
                ImageMaskAll.color = new Color(0, 0, 0, alpha);                
                allFadeout = false;
            }
        }
    }
    IEnumerator GameInit()
    {
        TextProgress.text = "加载 UI ...";

        //等待 UI 加载完成
        yield return new WaitUntil(GlobalMediator.IsUILoadFinished);

        TextProgress.text = "加载中 ...";

        GlobalModLoader.GameInit(GameInitFeedBack, this);
    }
    IEnumerator InitMenu()
    {
        alpha = 0;
        allFadeout = true;
        yield return new WaitForSeconds(1f);
        yield return  SceneManager.LoadSceneAsync(NextScense, LoadSceneMode.Additive);
        gameObject.SetActive(false);
    }
    void GameInitFeedBack(bool finished, string status, float progress, bool error = false)
    {
        if (error)
        {
            TextError.text = status;
            errored = true;
        }
        else
        {
            if (finished)
            {
                this.finished = true;
                TextProgress.text = "一切就绪，即将开始!";
                ImageProgress.sizeDelta = new Vector2(500, ImageProgress.sizeDelta.y);
                ImageProgress.anchoredPosition = new Vector2(250, 0);
                if (alpha == 0)
                {
                    if (errored)
                        PanelError.SetActive(true);
                    else if (finished)
                        SceneManager.LoadScene(NextScense);
                }
            }
            else
            {
                ImageProgress.sizeDelta = new Vector2(500 * progress, ImageProgress.sizeDelta.y);
                ImageProgress.anchoredPosition = new Vector2(500 * progress / 2, 0);
                TextProgress.text = status;
            }
        }
    }
}
