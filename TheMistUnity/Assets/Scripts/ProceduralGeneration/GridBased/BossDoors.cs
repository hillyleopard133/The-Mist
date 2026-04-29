using UnityEngine;

public class BossDoors : MonoBehaviour
{
    [SerializeField] private GameObject[] doors;

    public void OpenBossDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
        }
    }
}
