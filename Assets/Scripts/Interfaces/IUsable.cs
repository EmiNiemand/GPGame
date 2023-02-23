using UnityEngine;

/// <summary>
///     Player can interact with/use object implementing this interface
///</summary>
public interface IUsable {
    void OnEnter(GameObject user);
    void OnExit(GameObject user);
    void Use(GameObject user);
}