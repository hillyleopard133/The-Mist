using UnityEngine;

public static class HelperMethods
{
    public static Camera mainCamera;
    
    public static Vector3 GetMouseWorldPosition()
    {
        if(mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;
        
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);
        
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        
        worldMousePosition.z = 0f;
        
        return worldMousePosition;
    }
}