using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct HapticSettings
{
    public float strength;
    public float time;
}

public class GamepadHaptics : MonoBehaviour
{
    [SerializeField] private bool enableHaptics = false;
    [Header("Combat")] 
    [SerializeField] private HapticSettings successfulAttack;
    [SerializeField] private HapticSettings receivedDamage;

    private float playerWidth = 1;
    
    public void SetPlayerWidth(float width) { playerWidth = width; }

    public void SuccessfullyAttacked(Vector2 sourcePoint)
    {
        var percentage = Mathf.Round(GetLeftRightPercentage(transform.position.x, sourcePoint.x));
        StartCoroutine(HapticCoroutine(
            successfulAttack.time,
            successfulAttack.strength * percentage,
            successfulAttack.strength * (1 - percentage)));
    }

    public void ReceivedDamage(Vector2 sourcePoint)
    {
        var percentage = GetLeftRightPercentage(transform.position.x, sourcePoint.x);
        //TODO: improve this someday
        if(percentage <= 0.55f && percentage >= 0.45f)
            StartCoroutine(HapticCoroutine(
                receivedDamage.time, 
                receivedDamage.strength, 
                receivedDamage.strength));
        else
            StartCoroutine(HapticCoroutine(
                receivedDamage.time, 
                percentage * receivedDamage.strength, 
                receivedDamage.strength - percentage));
    }
    
    public static void Pause() { Gamepad.current.PauseHaptics(); }
    public static void Resume() { Gamepad.current.ResumeHaptics(); }

    private IEnumerator HapticCoroutine(float time, float leftMotor, float rightMotor)
    {
        if (!enableHaptics) yield break;

        if (Gamepad.current == null) yield break;
        Gamepad.current.SetMotorSpeeds(rightMotor, leftMotor);
        
        yield return new WaitForSeconds(time);
        
        if (Gamepad.current == null) yield break; 
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    /// <summary>
    /// Identifies side from which damage came, so that
    /// motors can rumble accordingly
    /// <returns>
    ///     0 = left side <br/> 1 = right side, <br/>
    ///     (0, 1) if in between
    /// </returns> 
    /// </summary>
    private float GetLeftRightPercentage(float playerX, float sourceX)
    {
        sourceX -= playerX;
        float leftBound = -playerWidth/2;
        float rightBound = playerWidth/2;
        if (sourceX <= leftBound) return 0;
        if (sourceX >= rightBound) return 1;
        return sourceX/playerWidth + 0.5f;
    }

    private void OnDestroy()
    {
        Gamepad.current.ResetHaptics();
    }
}
