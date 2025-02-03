using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
  public class GameManager : Singleton<GameManager>
  {
    private void Update() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        UIManager.Instance.ToggleOptionsMenu();
      }
    }
  }
}
