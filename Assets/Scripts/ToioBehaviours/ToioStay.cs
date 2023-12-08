using toio;

public class ToioStay : ToioBehaviour
{
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (controller.Cube.isGrounded)
        {
            cube.PlayPresetSound(4, order: Cube.ORDER_TYPE.Strong); // Mat in
            controller.ChangeBehaviour<ToioMoveRandomPoints>();
        }
    }
}
