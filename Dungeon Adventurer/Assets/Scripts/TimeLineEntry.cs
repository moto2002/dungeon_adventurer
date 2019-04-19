using UnityEngine;
using UnityEngine.UI;

public class TimeLineEntry : MonoBehaviour
{
    [SerializeField] Image charIcon;

    int _speed;
    public int Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    int _id;
    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }


    public void SetData(Sprite icon, int id, int speed)
    {
        charIcon.sprite = icon;
        Id = id;
        Speed = speed;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector2(0f, 0f);
    }

    public void Move(float xMove)
    {

        transform.localPosition = new Vector3(xMove, 0f);
    }
}
