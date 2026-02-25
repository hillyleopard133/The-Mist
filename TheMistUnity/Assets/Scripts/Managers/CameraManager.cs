using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera followPlayerCamera;
    [SerializeField] private CinemachineVirtualCamera combatCamera;
    [SerializeField] private CinemachineVirtualCamera BSPCamera;

    private PlayerActions actions;
    
    protected override void Awake()
    {
        base.Awake();
        if (actions == null)
        {
            actions = new PlayerActions();
        }
    }
    
    private void Start()
    {
        actions.BSP.Camera.performed += ctx => ToggleBSPCam();
    }

    public void SetCameraSize(float size)
    {
        followPlayerCamera.m_Lens.OrthographicSize = size;
    }

    public void ToggleCombatCamera()
    {
        combatCamera.gameObject.SetActive(!combatCamera.gameObject.activeSelf);
    }

    private void ToggleBSPCam()
    {
        BSPCamera.gameObject.SetActive(!BSPCamera.gameObject.activeSelf);
    }
    
    private void OnEnable()
    {
        if (actions != null)
        {
            actions.BSP.Enable();
        }
    }

    private void OnDisable()
    {
        if (actions != null)
        {
            actions.BSP.Disable();
        }
    }
}
