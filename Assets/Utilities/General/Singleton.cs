  using UnityEngine;

  namespace Utilities {

    // Generic singleton base class
    public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
      private static T instance;
      public static T Instance {
        get {
          // Return existing instance
          if (instance != null) {
            return instance;
          }
          else {
            // Find any existing instances in scene
            T[] managers = Object.FindObjectsOfType(typeof(T)) as T[];

            if (managers.Length == 0) {
              // Create new instance if none found
              GameObject go = new GameObject(typeof(T).Name, typeof(T));
              instance = go.GetComponent<T>();
              return instance;
            }
            else if (managers.Length == 1) {
              // Use existing instance
              instance = managers[0];
              instance.gameObject.name = typeof(T).Name;
              return instance;
            }
            else {
              // Warning if multiple found, use last one
              Debug.LogWarning($"Multiple {typeof(T).Name} singletons found. Using last one.");
              instance = managers[managers.Length-1];
              instance.gameObject.name = typeof(T).Name;
              return instance;
            }
          }
        }
        set {
          instance = value as T;
        }
      }
    }

  }
