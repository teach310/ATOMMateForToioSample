using toio;

public class ToioBehaviour
{
    protected ToioBehaviourController controller;

    public void SetController(ToioBehaviourController controller)
    {
        this.controller = controller;
    }

    protected CubeManager cubeManager => controller.CubeManager;
    protected CubeHandle cubeHandle => controller.CubeHandle;
    protected Cube cube => controller.Cube;
    protected ToioFace face => controller.Face;

    public virtual void OnEnter()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {
    }
}
