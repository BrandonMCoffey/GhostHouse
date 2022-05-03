using UnityEngine;

public class ResetAllData : MonoBehaviour
{
    public void ResetGameData()
    {
        DataManager.Instance.ResetAllData();
    }
}
