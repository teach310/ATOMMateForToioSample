using System.Collections;
using System.Collections.Generic;
using toio;
using UnityEngine;

public class ToioMoveRandomPoints : ToioBehaviour
{
    Vector2 targetPos;
    public float MaxSpeed = 60f;
    public int Tolerance = 30; // 許容範囲　default 8。大きめにすることで目標地点近辺で止まらないようにする

    public override void OnEnter()
    {
        base.OnEnter();
        SetTargetPos();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // TODO: 距離センサの値が近かったらAwayに遷移する

        if (!cube.isGrounded)
        {
            // TODO: SE
            controller.ChangeBehaviour<ToioStay>();
            return;
        }

        if (cubeManager.synced)
        {
            Movement mv = cubeHandle.Move2Target(targetPos, maxSpd: MaxSpeed, tolerance: Tolerance).Exec();
            if (mv.reached)
            {
                SetTargetPos();
            }
        }
    }

    void SetTargetPos()
    {
        var borderRect = cubeHandle.borderRect;
        targetPos = new Vector2(
            Random.Range(borderRect.xMin, borderRect.xMax),
            Random.Range(borderRect.yMin, borderRect.yMax)
        );
    }
}
