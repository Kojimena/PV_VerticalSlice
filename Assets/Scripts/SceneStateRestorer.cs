using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-10000)]

public class SceneStateRestorer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(RestorePlayerNextFrame());
    }

    private IEnumerator RestorePlayerNextFrame()
    {
        yield return null;

        if (PersistenceManager.Instance == null || PersistenceManager.Instance.data == null)
            yield break;

        var data     = PersistenceManager.Instance.data;

        if (!data.hasCheckpoint)
            yield break;

        var savedScene = data.currentSceneName;
        var thisScene  = SceneManager.GetActiveScene().name;
        if (string.IsNullOrEmpty(savedScene) || savedScene != thisScene)
            yield break;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.transform.position = data.playerPosition;

        if (player.transform.position.y < -5f)
        {
            var spawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
            if (spawn) player.transform.position = spawn.transform.position;
        }

        if (cc) cc.enabled = true;

        Debug.Log($"[Restore] Player colocado en {player.transform.position}");
    }
}