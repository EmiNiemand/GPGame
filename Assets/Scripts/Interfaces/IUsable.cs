using UnityEngine;

/// <summary>
///     Player can interact with/use object implementing this interface
///</summary>
/// <remarks>
///     GameObject using script implementing this interface must have
///     Collider2D set to Trigger as well as be on <b>Usable</b> layer
/// </remarks>
public interface IUsable {
    void OnEnter(GameObject user);
    void OnExit(GameObject user);
    void Use(GameObject user);
}