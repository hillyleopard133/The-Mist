using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineCamera followPlayerCamera;
    [SerializeField] private CinemachineCamera combatCamera;
    [SerializeField] private CinemachineCamera BSPCamera;

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
        followPlayerCamera.Lens.OrthographicSize = size;
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
