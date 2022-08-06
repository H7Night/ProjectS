using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform targetTransform;
    MapGenerator mapGenerator;
    private bool hasTarget;
    [SerializeField] float smoothLerpSpeed;//用作相机Lerp移动
    [SerializeField] float minX, maxX, minZ, maxZ;//相机移动范围。

    // private void Awake()
    // {
    //     FindPlayer();
    // }

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        //动态获取摄像机范围
        minX = -mapGenerator.currentMap.mapSize.x/2 + 4f ;
        maxX = mapGenerator.currentMap.mapSize.x/2 - 4f ;
        minZ = -mapGenerator.currentMap.mapSize.y/2 + 2f;
        maxZ = mapGenerator.currentMap.mapSize.y/2 - 2f;
    }
    private void Update() 
    {
        FindPlayer();
    }
    private void LateUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (hasTarget)
        {
            FollowSmooth();
            LimitCamera();
        }
    }

    void FindPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            targetTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            hasTarget = true;
        }
        else
        {
            hasTarget = false;
        }
    }

    void FollowSmooth()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetTransform.position.x, 
                                                                              transform.position.y, 
                                                                              targetTransform.position.z), smoothLerpSpeed * Time.deltaTime);
    }

    void LimitCamera()
    {
        if (hasTarget)
        {
            //Math.Clamp -- 返回在 min 和 max 的 value 含(首尾) 
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX),
                                                                transform.position.y,
                                             Mathf.Clamp(transform.position.z, minZ, maxZ));
        }
        else
        {
            transform.position = new Vector3(0, 0, 0);
        }
    }
}
