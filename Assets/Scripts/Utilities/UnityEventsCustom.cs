using UnityEngine.Events;

[System.Serializable]
public class UnityEventInt : UnityEvent<int> {}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool> {}

[System.Serializable]
public class UnityEventMTK_Interactable : UnityEvent<MTK_Interactable> {}

[System.Serializable]
public delegate void VoidDelegate();