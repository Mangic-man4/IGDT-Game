using UnityEngine;

public class DisableOnBossDeath : MonoBehaviour
{
    [SerializeField] private MimicBossController boss;
    //[SerializeField] private GameObject targetObject; // optional if you want to disable something else

    private void Start()
    {
        if (boss == null)
        {
            boss = FindObjectOfType<MimicBossController>();
        }

        if (boss != null)
        {
            boss.OnDied += DisableSelf;
        }
        else
        {
            Debug.LogWarning("[DisableOnBossDeath] Boss not found.");
        }
    }

    private void DisableSelf()
    {
        Debug.Log("[DisableOnBossDeath] Boss died, disabling object.");
        gameObject.SetActive(false); // Or targetObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (boss != null)
        {
            boss.OnDied -= DisableSelf;
        }
    }

}
