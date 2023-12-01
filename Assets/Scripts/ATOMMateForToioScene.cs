using UnityEngine;

public class ATOMMateForToioScene : MonoBehaviour
{
    [SerializeField] ToioFace _toioFace;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _toioFace.SetExpression((ToioFace.Expression)Random.Range(0, 6));
        }
    }
}
