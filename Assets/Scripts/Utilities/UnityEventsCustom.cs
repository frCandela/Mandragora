using UnityEngine.Events;

[System.Serializable]
public class UnityEventInt : UnityEvent<int> {}

[System.Serializable]
public class UnityEventBool : UnityEvent<bool> {}

[System.Serializable]
public class UnityEventInputBool : UnityEvent<InputButtons, bool> {}