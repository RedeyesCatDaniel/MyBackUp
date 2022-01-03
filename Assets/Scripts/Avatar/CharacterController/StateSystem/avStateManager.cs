using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LGUVirtualOffice
{
    public class avStateManager : MonoBehaviour
    {
        public avState[] states;
        private avState currentState;
        public int currentIndex = -1;
        private void Start()
        {
            currentState = states[0];
            currentState.OnEnter();
        }

        public void EnterState(int index) {
            if (currentIndex != index)
            {
                avState state = states[index];
                if(currentState != null) currentState.OnExit();
                state.OnEnter();
                currentState = state;
                currentIndex = index;
                Debug.Log("I changed my state");
            }
            else {
               // Debug.Log(index);
            }
            
        }
    }

    [System.Serializable]
    public class avState {
        public string StateName;
        public UnityEvent OnStateEnter;
        public UnityEvent OnStateExit;

        public void OnEnter()
        {

            OnStateEnter.Invoke();
           // Debug.Log($"I enter the state {StateName}");
        }

        public void OnExit() {
            OnStateExit.Invoke();
            Debug.Log($"I exit the state {StateName}");
        }
    }
}