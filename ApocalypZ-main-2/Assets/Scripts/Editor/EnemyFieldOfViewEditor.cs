using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyFieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyController fieldOfView = (EnemyController)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.viewRadius);

        Vector3 viewAngle01 = DirectionFromAngle(fieldOfView.transform.eulerAngles.y, -fieldOfView.viewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fieldOfView.transform.eulerAngles.y, fieldOfView.viewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngle01 * fieldOfView.viewRadius);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngle02 * fieldOfView.viewRadius);

        if (fieldOfView.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fieldOfView.transform.position, fieldOfView.playerRef.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
