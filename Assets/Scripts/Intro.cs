using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public RectTransform ImageProgress;
    public Image ImageLogo;
    public Image ImageCloud1;
    public Image ImageCloud2;
    public RectTransform Cloud1;
    public RectTransform Cloud2;
    public float MoveSpeed = 0.3f;
    public int NextScense;

    private bool movingCloud = true;
    private bool fadeOut = false;
    private float alpha = 1;
    private bool errored = false;
    private bool finished = false;

    void Start()
    {
        GlobalMediator.GlobalMediatorInitialization();
        GlobalModLoader.GlobalModLoaderInitialization();

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
                //if (finished)
                //j加载menu场景
                SceneManager.LoadScene(NextScense);
            }
        }
    }
}
