using UnityEngine;

public class Player : Singleton<Player>
{
    [Header("Config")]
    public PlayerAttack PlayerAttack { get; private set; }

    private PlayerAnimations animations;

    protected override void Awake()
    {
        base.Awake();
        PlayerAttack = GetComponent<PlayerAttack>();
        animations = GetComponent<PlayerAnimations>();
    }

    public void RespawnPlayer()
    {
        SceneChangeManager.Instance.LoadCheckpoint();
        animations.ResetPlayer();
    }
}
