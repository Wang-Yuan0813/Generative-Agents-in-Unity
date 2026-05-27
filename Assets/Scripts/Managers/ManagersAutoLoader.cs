//THIS SCRIPT CAN BE USED TO AUTO LOAD MANAGERS IN THE SCENE
//IT WILL AUTO LOAD ALL MANAGERS IN THE SCENE(IF MANAGERS DON'T EXIST)
using UnityEngine;
public static class ManagersAutoLoader
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureManagersExist()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("[ManagersAutoLoader] GameManager not found, loading ManagersRoot");
            var prefab = Resources.Load<GameObject>("Managers/ManagersRoot"); 
            if (prefab != null)
            {
                var root = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(root);
            }
        }
    }
}

