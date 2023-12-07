using toio;

public class ToioStay : ToioBehaviour
{
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (controller.Cube.isGrounded)
        {
            controller.ChangeBehaviour<ToioMoveRandomPoints>();
        }
    }
}
