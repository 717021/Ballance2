using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*摄像机圆周旋转脚本*/

public class RotateCam : MonoBehaviour {

    /// <summary>
    /// 绕着转的物体
    /// </summary>
    public GameObject RotateObbject;
    /// <summary>
    /// 旋转速度
    /// </summary>
    public float RotateSpeed = -5f;

    void Start()
    {

    }

    void Update()
    {
        if(RotateObbject != null)
            transform.RotateAround(RotateObbject.transform.position, new Vector3(0, 1, 0), RotateSpeed * Time.deltaTime);
    }
}
