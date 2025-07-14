using UnityEngine;

public class StoreInteraction : MonoBehaviour
{
    public GameObject storeUI;
    public GameController gameController;

    private bool canInteract = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        storeUI.SetActive(false);

        PlayerInteraction.OnPlayerInteraction += OpenStoreUI;
    }
    void OnDestroy()
    {
        PlayerInteraction.OnPlayerInteraction -= OpenStoreUI;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    private void OpenStoreUI()
    {
        if (canInteract)
        {
            storeUI.SetActive(true);
            gameController.PauseGame();
        }
    }

}
