using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGUVirtualOffice
{
    [CreateAssetMenu(menuName = "Toolkit/avPlayerInputManager")]
    public class avPlayerInputManager:ScriptableObject
    {
        public static PlayerInput pInput;

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            Debug.Log("I Initialized player input settings");
            pInput = new PlayerInput();
            pInput.Player.Enable();
        }

        public static Vector2 GetMoveDir() {
            return pInput.Player.MoveDir.ReadValue<Vector2>();
        }

        public void SetPlayerActive(bool isActive)
        {
            if (isActive)
            {
                pInput.Player.Enable();
            }
            else {
                pInput.Player.Disable();
            }
            

        }


    }
}