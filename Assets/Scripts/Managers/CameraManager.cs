using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void SetCameraSize(float size)
    {
        virtualCamera.m_Lens.OrthographicSize = size;
    }
    
}
