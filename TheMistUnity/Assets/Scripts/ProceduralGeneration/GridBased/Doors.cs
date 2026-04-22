using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] private GameObject topDoorOpen;
    [SerializeField] private GameObject bottomDoorOpen;
    [SerializeField] private GameObject leftDoorOpen;
    [SerializeField] private GameObject rightDoorOpen;

    [SerializeField] private GameObject topDoorBlocked;
    [SerializeField] private GameObject bottomDoorBlocked;
    [SerializeField] private GameObject leftDoorBlocked;
    [SerializeField] private GameObject rightDoorBlocked;

    public void OpenTopDoor()
    {
        topDoorOpen.SetActive(true);
        topDoorBlocked.SetActive(false);
    }

    public void OpenBottomDoor()
    {
        bottomDoorOpen.SetActive(true);
        bottomDoorBlocked.SetActive(false);
    }

    public void OpenLeftDoor()
    {
        leftDoorOpen.SetActive(true);
        leftDoorBlocked.SetActive(false);
    }

    public void OpenRightDoor()
    {
        rightDoorOpen.SetActive(true);
        rightDoorBlocked.SetActive(false);
    }
}
