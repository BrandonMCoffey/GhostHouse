using UnityEngine;

public class ResetData : MonoBehaviour
{
    public void ResetInteractableData() {
        DataManager.Instance.ResetData();
    }
}