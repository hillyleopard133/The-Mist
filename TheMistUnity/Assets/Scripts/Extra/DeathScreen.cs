using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public void FadeInEvent()
    {
        UIManager.Instance.ShowDeathScreenContent();
    }

    public void FadeOutEvent()
    {
        UIManager.Instance.DeactivateDeathScreen();
    }
}