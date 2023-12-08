using toio;
using UnityEngine;

public class ToioMoveAway : ToioBehaviour
{
    Vector2 targetPos;
    public float MaxSpeed = 100f;
    public int Tolerance = 30; // 許容範囲　default 8。大きめにすることで目標地点近辺で止まらないようにする
    public int RotateTime = 140; // default 250 切り返し早めるために回転速度を早める
    float _maxTime = 2f;
    float _enterTime;
    
    public override void OnEnter()
    {
        base.OnEnter();
        CubeOrderBalancer.Instance.ClearOrder(cube);
        cube.PlayPresetSound(1, order: Cube.ORDER_TYPE.Strong); // selected
        if (face.Connected)
        {
            face.SetExpression(ToioFace.Expression.Angry);
        }
        _enterTime = Time.time;
        SetTargetPos();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!cube.isGrounded)
        {
            cube.PlayPresetSound(5, order: Cube.ORDER_TYPE.Strong); // Mat out
            controller.ChangeBehaviour<ToioStay>();
            return;
        }

        // CubeOrderBalancerのsendQueueに2つ以上ためっているというログを回避するためにIsIdleを確認
        if (cubeManager.synced && CubeOrderBalancer.Instance.IsIdle(cube))
        {
            Movement mv = cubeHandle.Move2Target(targetPos, maxSpd: MaxSpeed, rotateTime: RotateTime, tolerance: Tolerance).Exec();
            if (mv.reached)
            {
                face.SetExpression(ToioFace.Expression.Happy);
                controller.ChangeBehaviour<ToioMoveRandomPoints>();
                return;
            }

            if (Time.time - _enterTime > _maxTime)
            {
                face.SetExpression(ToioFace.Expression.Neutral);
                controller.ChangeBehaviour<ToioMoveRandomPoints>();
                return;
            }
        }
    }

    void SetTargetPos()
    {
        var forward = new Vector2((float)cubeHandle.dir.x, (float)cubeHandle.dir.y);
        var borderRect = cubeHandle.borderRect;

        float dot = 1f;
        Vector2 resultPos = Vector2.zero;
        // 数回試行して、一番逆向きの方向を選択
        for (int i = 0; i < 6; i++)
        {
            var tempPos = new Vector2(
                Random.Range(borderRect.xMin, borderRect.xMax),
                Random.Range(borderRect.yMin, borderRect.yMax)
            );

            var tempDot = Vector2.Dot(forward, tempPos);

            if (tempDot < dot)
            {
                dot = tempDot;
                resultPos = tempPos;
            }
        }
        targetPos = resultPos;
    }
}
