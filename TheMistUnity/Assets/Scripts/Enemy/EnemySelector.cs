using UnityEngine;

public class EnemySelector : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject selectorSprite;

    private EnemyBrainRPG _enemyBrainRpg;

    private void Awake()
    {
        _enemyBrainRpg = GetComponent<EnemyBrainRPG>();
    }

    private void EnemySelectedCallback(EnemyBrainRPG enemySelected)
    {
        //enemyBrain.GetComponent<EnemyHealth>().UpdateSelectedEnemy(enemySelected);
        if (enemySelected == _enemyBrainRpg)
        {
            selectorSprite.SetActive(true);
            //UIManager.Instance.UpdateEnemyInfoPanel(enemySelected);
        }
        else
        {
            selectorSprite.SetActive(false);
        }
    }

    public void NoSelectionCallback()
    {
        selectorSprite.SetActive(false);
        //UIManager.Instance.CloseEnemyInfoPanel();
    }

    private void OnEnable()
    {
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoSelectionCallback;
    }

    private void OnDisable()
    {
        SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent -= NoSelectionCallback;
    }

}