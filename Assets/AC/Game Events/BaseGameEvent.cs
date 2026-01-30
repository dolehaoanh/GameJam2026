
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseGameEvent<T> : ScriptableObject
{
    private UnityEvent<T> _gameEvent = new UnityEvent<T>();
    // Start is called before the first frame update
    public void RaiseEvent(T eventData)
    {
        _gameEvent?.Invoke(eventData);
    }

    public void AddListener(UnityAction<T> listener)
    {
        _gameEvent.AddListener(listener);
    }

    public void RemoveListener(UnityAction<T> listener)
    {
        _gameEvent.RemoveListener(listener);
    }
}

public abstract class BaseGameEvent : ScriptableObject
{
    private UnityEvent _gameEvent = new UnityEvent();
    // Start is called before the first frame update
    public void RaiseEvent()
    {
        _gameEvent?.Invoke();
    }

    public void AddListener(UnityAction listener)
    {
        _gameEvent.AddListener(listener);
    }

    public void RemoveListener(UnityAction listener)
    {
        _gameEvent.RemoveListener(listener);
    }
}
