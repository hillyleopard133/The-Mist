using UnityEngine;

public class EnemySelector : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject selectorSprite;

    private EnemyBrain enemyBrain;

    private void Awake()
    {
        enemyBrain = GetComponent<EnemyBrain>();
    }

    private void EnemySelectedCallback(EnemyBrain enemySelected)
    {
        //enemyBrain.GetComponent<EnemyHealth>().UpdateSelectedEnemy(enemySelected);
        if (enemySelected == enemyBrain)
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