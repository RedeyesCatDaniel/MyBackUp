using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace LGUVirtualOffice
{
    public class avInputActionAssociator : MonoBehaviour
    {
        public List<InputActionPair> actions;
        private void Start()
        {
            foreach (var item in actions)
            {
                item.Associate();
            }
        }

        public void PrintInfo() {
            Debug.Log("actions associated");
        }


    }

    [System.Serializable]
    public struct InputActionPair {
        public InputActionReference action;
        public UnityEvent<InputAction.CallbackContext> Onperformed;

        public void Associate() {
            InputAction iaction = avPlayerInputManager.pInput.asset.FindAction(action.action.name);
            iaction.performed -= Act;
            iaction.performed += Act;
        }

        public void Act(InputAction.CallbackContext contex) {
            Onperformed.Invoke(contex);
        }
    }
}