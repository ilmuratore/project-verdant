using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Utility used by UI managers to resolve scene instances even when menu/HUD objects are prefabs.
/// It avoids using prefab assets from the Project window as runtime references.
/// </summary>
public static class SceneReferenceFinder
{
    public static bool IsSceneInstance(GameObject obj)
    {
        return obj != null && obj.scene.IsValid() && obj.scene.isLoaded;
    }

    public static bool IsSceneInstance(Component component)
    {
        return component != null && IsSceneInstance(component.gameObject);
    }

    public static GameObject ResolveSceneObject(GameObject current, Transform searchRoot, string objectName)
    {
        if (IsSceneInstance(current)) return current;

        if (searchRoot != null && IsSceneInstance(searchRoot.gameObject))
        {
            Transform child = FindChildRecursive(searchRoot, objectName);
            if (child != null) return child.gameObject;
        }

        return FindGameObjectInActiveScene(objectName);
    }

    public static Transform ResolveSceneTransform(Transform current, string objectName)
    {
        if (IsSceneInstance(current)) return current;

        GameObject found = FindGameObjectInActiveScene(objectName);
        return found != null ? found.transform : null;
    }

    public static T ResolveComponentInChildren<T>(T current, Transform searchRoot, string objectName) where T : Component
    {
        if (IsSceneInstance(current)) return current;

        if (searchRoot != null && IsSceneInstance(searchRoot.gameObject))
        {
            Transform child = FindChildRecursive(searchRoot, objectName);
            if (child != null)
            {
                T component = child.GetComponent<T>();
                if (component != null) return component;
            }
        }

        GameObject foundObject = FindGameObjectInActiveScene(objectName);
        return foundObject != null ? foundObject.GetComponent<T>() : null;
    }

    public static T FindComponentInActiveScene<T>() where T : Component
    {
        T[] all = Resources.FindObjectsOfTypeAll<T>();

        foreach (T component in all)
        {
            if (IsSceneInstance(component))
            {
                return component;
            }
        }

        return null;
    }

    public static GameObject FindGameObjectInActiveScene(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName)) return null;

        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid()) return null;

        GameObject[] roots = activeScene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            Transform found = FindChildRecursive(root.transform, objectName);
            if (found != null) return found.gameObject;
        }

        return null;
    }

    public static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null || string.IsNullOrWhiteSpace(childName)) return null;

        if (parent.name == childName) return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform found = FindChildRecursive(child, childName);

            if (found != null) return found;
        }

        return null;
    }
}
