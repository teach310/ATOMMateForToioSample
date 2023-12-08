using toio;
using UnityEngine;

public class ATOMMateForToioScene : MonoBehaviour
{
    [SerializeField] ToioFace _toioFace;
    CubeManager _cubeManager;
    Cube _cube;
    ToioBehaviourController _controller;

    async void Start()
    {
        _cubeManager = new CubeManager(ConnectType.Real);
        _cube = await _cubeManager.SingleConnect();
        SetupHandle(_cubeManager.handles[0]);
        _controller = new ToioBehaviourController(_cubeManager, _cubeManager.handles[0]);
        _controller.Face = _toioFace;
        _controller.OnEnter();
    }

    void SetupHandle(CubeHandle handle)
    {
        // defaultだとマットからはみ出しがちのため、少し狭める
        var matRect = new RectInt(45, 45, 410, 410);
        handle.SetBorderRect(matRect, margin: 30);
    }

    void Update()
    {
        _controller?.OnUpdate();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_cube == null) return;
            if (!_toioFace.Connected) return;

            var expression = (ToioFace.Expression)Random.Range(0, 6);
            _toioFace.SetExpression(expression);
            PlaySound(expression);
        }
    }

    void PlaySound(ToioFace.Expression expression)
    {
        switch (expression)
        {
            case ToioFace.Expression.Happy:
                _cube.PlayPresetSound(0);
                break;
            case ToioFace.Expression.Angry:
                _cube.PlayPresetSound(1);
                break;
            case ToioFace.Expression.Sad:
                _cube.PlayPresetSound(2);
                break;
            case ToioFace.Expression.Doubt:
                _cube.PlayPresetSound(3);
                break;
            case ToioFace.Expression.Sleepy:
                _cube.PlayPresetSound(4);
                break;
            case ToioFace.Expression.Neutral:
                _cube.PlayPresetSound(5);
                break;
        }
    }
}
