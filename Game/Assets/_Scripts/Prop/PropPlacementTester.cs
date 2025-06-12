using UnityEngine;
using UnityEngine.UI;

public class PropPlacementTester : MonoBehaviour
{
    public PropPlacementManager propManager;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => {
            propManager.ProcessRooms();
        });
    }
}
