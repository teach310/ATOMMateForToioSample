using toio;
using UnityEngine;

public class ATOMMateForToioScene : MonoBehaviour
{
    [SerializeField] ToioFace _toioFace;
    CubeManager _cubeManager;
    Cube _cube;

    async void Start()
    {
        _cubeManager = new CubeManager(ConnectType.Real);
        _cube = await _cubeManager.SingleConnect();
    }

    void Update()
    {
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
