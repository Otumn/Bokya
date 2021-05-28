using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Otumn.Bokya
{
    public class LinkedEvent : MonoBehaviour
    {
        public UnityEvent linkedEvent;

        public void InvokeLinkedEvent()
        {
            linkedEvent.Invoke();
        }
    }
}
