using UnityEngine;
using VContainer;

public class HeadBlobber : MonoBehaviour
{
    [SerializeField] public float walkingBobbingSpeed = 10f;
    [SerializeField] public float bobbingAmount = 0.025f;
    private float defaultPositionY = 0;
    private float timer = 0;
    private Transform _camPos;

    [Inject]
    public void Construct()
    {

    }

   
    public void Startup(Transform camPos)
    {
        _camPos = camPos;
        defaultPositionY = _camPos.transform.position.y;
    }

    public void Blob(bool IsMovingOnTheGround)
    {
        if (IsMovingOnTheGround)
        {
            timer += Time.deltaTime * walkingBobbingSpeed;

            float newY = defaultPositionY + Mathf.Sin(timer) * bobbingAmount;
            // ИСПОРАВЛЕНО: Используем Vector3 и сохраняем текущий localPosition.z
            _camPos.transform.localPosition = new Vector3(_camPos.transform.localPosition.x, newY, _camPos.transform.localPosition.z);
        }
        else
        {
            timer = 0;
            float smoothY = Mathf.Lerp(_camPos.transform.localPosition.y, defaultPositionY, Time.deltaTime * walkingBobbingSpeed);
            _camPos.transform.localPosition = new Vector3(_camPos.transform.localPosition.x, smoothY, _camPos.transform.localPosition.z);
        }
    }
}
