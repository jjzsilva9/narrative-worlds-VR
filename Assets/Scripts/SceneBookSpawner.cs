using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns the VRBook prefab when the scene loads and populates it with
/// the chapter text defined for this scene in NarrativeChapters.
/// Add this to an empty GameObject in each environment scene (Mordor, GollumCave).
/// Assign the book prefab and a spawn point transform in the Inspector.
/// </summary>
public class SceneBookSpawner : MonoBehaviour
{
    [Tooltip("The VRBook prefab to spawn")]
    [SerializeField] private GameObject bookPrefab;

    private void Start()
    {
        SpawnBook();
    }

    private void SpawnBook()
    {
        if (bookPrefab == null)
        {
            Debug.LogWarning("SceneBookSpawner: Book prefab not assigned.");
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        string[] pages = NarrativeChapters.GetPages(sceneName);

        if (pages == null)
        {
            Debug.LogWarning($"SceneBookSpawner: No chapter text defined for scene \"{sceneName}\".");
            return;
        }

        GameObject book = Instantiate(bookPrefab, transform.position, transform.rotation);

        if (book.TryGetComponent(out VRBookController bookController))
            bookController.SetPages(pages);
    }
}
