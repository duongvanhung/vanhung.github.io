using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void OnNotify (object key, object data);
}

public interface ISubject
{
    void RegisterObserver (object key, IObserver observer);
    void SendMessage (object key);
    void SendMessage (object key, object data);
    void RemoveRegister (object key, IObserver observer);
    void RemoveAllRegister (IObserver observer);
}

public abstract class SingleSubject<T> : SingletonMono<T>, ISubject where T : MonoBehaviour
{
    Dictionary<object, List<IObserver>> observers = new Dictionary<object, List<IObserver>>();

    public void RegisterObserver (object key, IObserver observer) {
        if (!observers.ContainsKey(key)) {
            List<IObserver> actions = new List<IObserver>();
            actions.Add(observer);
            observers.Add(key, actions);
        }
        else {
            observers[key].Add(observer);
        }
    }

    public void SendMessage (object key) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, null);
            }
        }
    }
    public void SendMessage (object key, object data) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, data);
            }
        }
    }

    public void RemoveRegister (object key, IObserver observer) {
        if (observers.ContainsKey(key)) {
            for (int i = 0; i < observers[key].Count; i++) {
                if (observers[key][i] == observer) {
                    observers[key].RemoveAt(i);
                }
            }
        }
    }

    public void RemoveAllRegister (IObserver observer) {
        foreach (KeyValuePair<object, List<IObserver>> item in observers) {
            for (int i = 0; i < item.Value.Count; i++) {
                if (item.Value[i] == observer) {
                    item.Value.RemoveAt(i);
                }
            }
        }
    }
}

public abstract class SubjectMono : MonoBehaviour, ISubject
{
    Dictionary<object, List<IObserver>> observers = new Dictionary<object, List<IObserver>>();

    public void RegisterObserver (object key, IObserver observer) {
        if (!observers.ContainsKey(key)) {
            List<IObserver> actions = new List<IObserver>();
            actions.Add(observer);
            observers.Add(key, actions);
        }
        else {
            observers[key].Add(observer);
        }
    }

    public void SendMessage (object key) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, null);
            }
        }
    }
    public void SendMessage (object key, object data) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, data);
            }
        }
    }

    public void RemoveRegister (object key, IObserver observer) {
        if (observers.ContainsKey(key)) {
            for (int i = 0; i < observers[key].Count; i++) {
                if (observers[key][i] == observer) {
                    observers[key].RemoveAt(i);
                }
            }
        }
    }

    public void RemoveAllRegister (IObserver observer) {
        foreach (KeyValuePair<object, List<IObserver>> item in observers) {
            for (int i = 0; i < item.Value.Count; i++) {
                if (item.Value[i] == observer) {
                    item.Value.RemoveAt(i);
                }
            }
        }
    }
}

public abstract class Subject : ISubject
{
    protected static Dictionary<object, List<IObserver>> observers = new Dictionary<object, List<IObserver>>();

    public void RegisterObserver (object key, IObserver observer) {
        if (!observers.ContainsKey(key)) {
            List<IObserver> actions = new List<IObserver>();
            actions.Add(observer);
            observers.Add(key, actions);
        }
        else {
            observers[key].Add(observer);
        }
    }

    public void SendMessage (object key) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, null);
            }
        }
    }
    public void SendMessage (object key, object data) {
        if (observers.ContainsKey(key)) {
            foreach (IObserver observer in observers[key]) {
                observer.OnNotify(key, data);
            }
        }
    }

    public void RemoveRegister (object key, IObserver observer) {
        if (observers.ContainsKey(key)) {
            for (int i = 0; i < observers[key].Count; i++) {
                if (observers[key][i] == observer) {
                    observers[key].RemoveAt(i);
                }
            }
        }
    }

    public void RemoveAllRegister (IObserver observer) {
        foreach (KeyValuePair<object, List<IObserver>> item in observers) {
            for (int i = 0; i < item.Value.Count; i++) {
                if (item.Value[i] == observer) {
                    item.Value.RemoveAt(i);
                }
            }
        }
    }
}