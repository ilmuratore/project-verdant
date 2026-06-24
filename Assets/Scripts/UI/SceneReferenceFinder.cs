using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneReferenceFinder
{
    public static bool IsSceneInstance(GameObject obj)
    {
        return obj != null && obj.scene.IsValid() && obj.scene == SceneManager.GetActiveScene();
    }

    public static bool IsSceneInstance(Component component)
    {
        return component != null && IsSceneInstance(component.gameObject);
    }

    public static GameObject ResolveSceneObject(GameObject current, Transform searchRoot, string objectName)
    {
        if (IsSceneInstance(current)) return current;
        if (string.IsNullOrWhiteSpace(objectName)) return current;

        if (searchRoot != null)
        {
            Transform child = FindChildRecursive(searchRoot, objectName);
            if (child != null) return child.gameObject;
        }

        return FindGameObjectInActiveScene(objectName);
    }

    public static Transform ResolveSceneTransform(Transform current, string objectName)
    {
        if (current != null && current.gameObject.scene.IsValid()) return current;
        GameObject found = FindGameObjectInActiveScene(objectName);
        return found != null ? found.transform : null;
    }

    public static T ResolveComponentInChildren<T>(T current, Transform searchRoot, string objectName) where T : Component
    {
        if (current != null && current.gameObject.scene.IsValid()) return current;
        if (searchRoot == null) return FindComponentByObjectName<T>(objectName);

        if (!string.IsNullOrWhiteSpace(objectName))
        {
            Transform child = FindChildRecursive(searchRoot, objectName);
            if (child != null)
            {
                T component = child.GetComponent<T>();
                if (component != null) return component;
            }
        }

        return searchRoot.GetComponentInChildren<T>(true);
    }

    public static T FindComponentInActiveScene<T>() where T : Component
    {
        T[] all = Resources.FindObjectsOfTypeAll<T>();
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (T item in all)
        {
            if (item == null) continue;
            if (item.gameObject.scene == activeScene) return item;
        }

        return null;
    }

    public static T FindComponentByObjectName<T>(string objectName) where T : Component
    {
        GameObject obj = FindGameObjectInActiveScene(objectName);
        return obj != null ? obj.GetComponent<T>() : null;
    }

    public static GameObject FindGameObjectInActiveScene(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName)) return null;

        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] roots = activeScene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            if (root.name == objectName) return root;

            Transform found = FindChildRecursive(root.transform, objectName);
            if (found != null) return found.gameObject;
        }

        return null;
    }

    public static Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent == null || string.IsNullOrWhiteSpace(childName)) return null;

        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;

            Transform found = FindChildRecursive(child, childName);
            if (found != null) return found;
        }

        return null;
    }
}
