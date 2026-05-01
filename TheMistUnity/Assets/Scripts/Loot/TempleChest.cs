using UnityEngine;

public class TempleChest : MonoBehaviour
{
    private bool isOpened = false;
    
    private TempleManager templeManager;

    private void Start()
    {
        templeManager = TempleManager.Instance;
    }

    public void Open()
    {
        isOpened = true;
        templeManager.OpenChest();
    }
}
