using System.Collections;
using System.Collections.Generic;
using System.Linq;
using toio;
using UnityEngine;

public class ToioMoveRandomPoints : ToioBehaviour
{
    Vector2 targetPos;
    public float MaxSpeed = 60f;
    public int Tolerance = 30; // 許容範囲　default 8。大きめにすることで目標地点近辺で止まらないようにする
    Queue<int> distanceQueue = new Queue<int>(); // 誤作動があるため直近3回分の距離を記録しておく
    int _latestDistanceFrame = 0;

    public override void OnEnter()
    {
        base.OnEnter();
        SetTargetPos();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (face.Connected)
        {
            if (UpdateDistanceIfNeeded() && IsObstacleFront())
            {
                controller.ChangeBehaviour<ToioMoveAway>();
                return;
            }
        }

        if (!cube.isGrounded)
        {
            cube.PlayPresetSound(5, order: Cube.ORDER_TYPE.Strong); // Mat out
            controller.ChangeBehaviour<ToioStay>();
            return;
        }

        if (cubeManager.synced)
        {
            Movement mv = cubeHandle.Move2Target(targetPos, maxSpd: MaxSpeed, tolerance: Tolerance).Exec();
            if (mv.reached)
            {
                face.SetExpression(ToioFace.Expression.Neutral);
                SetTargetPos();
            }
        }
    }

    bool UpdateDistanceIfNeeded()
    {
        if (_latestDistanceFrame == face.LatestDistanceUpdateFrame)
        {
            return false;
        }

        distanceQueue.Enqueue(face.Distance);
        if (distanceQueue.Count > 3)
        {
            distanceQueue.Dequeue();
        }
        _latestDistanceFrame = face.LatestDistanceUpdateFrame;
        return true;
    }

    bool IsObstacleFront()
    {
        var average = distanceQueue.Average();
        return 1 < average && average < 150;
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
