using UnityEngine;

public class ControllerFactory : MonoBehaviour
{
    public GameObject ControllerPrefab;
    
    public PlayerController CreatePlayerController(Transform SpawnPoint)
    {
        GameObject newController = Instantiate(ControllerPrefab,SpawnPoint);
        return newController.GetComponent<PlayerController>();
    }
}