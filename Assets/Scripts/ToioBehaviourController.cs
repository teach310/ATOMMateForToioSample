using System.Collections.Generic;
using toio;

public class ToioBehaviourController
{
    readonly Dictionary<string, ToioBehaviour> _behaviours = new Dictionary<string, ToioBehaviour>();

    public CubeManager CubeManager { get; }
    public CubeHandle CubeHandle { get; }
    public Cube Cube => CubeHandle.cube;

    public ToioBehaviour CurrentBehaviour { get; private set; }

    public ToioBehaviourController(CubeManager cm, CubeHandle cubeHandle)
    {
        CubeManager = cm;
        CubeHandle = cubeHandle;
        _behaviours.Add(typeof(ToioStay).Name, new ToioStay());
        _behaviours.Add(typeof(ToioMoveRandomPoints).Name, new ToioMoveRandomPoints());

        foreach (var behaviour in _behaviours.Values)
        {
            behaviour.SetController(this);
        }
        CurrentBehaviour = _behaviours[typeof(ToioStay).Name];
    }

    public void ChangeBehaviour<T>() where T : ToioBehaviour
    {
        CurrentBehaviour.OnExit();
        CurrentBehaviour = _behaviours[typeof(T).Name];
        CurrentBehaviour.OnEnter();
    }

    public void OnEnter()
    {
        CurrentBehaviour.OnEnter();
    }

    public void OnUpdate()
    {
        CurrentBehaviour.OnUpdate();
    }
}
