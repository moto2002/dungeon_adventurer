using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleCharacterPather : UIBehaviour
{
    const int speedMulti = 2;

    [SerializeField] Vector3[] routePoints;
    [SerializeField] float walkSpeed;
    [SerializeField] Animator animator;

    int _currentTargetIndex = 0;
    bool _waiting = true;

    protected override void Awake()
    {
        animator.SetFloat("Blend", walkSpeed);
        Invoke("StartWalking", Random.Range(0, 13));
    }

    private void Update()
    {
        if (_waiting) return;

        float step = walkSpeed * Time.deltaTime * speedMulti;
        transform.position = Vector3.MoveTowards(transform.position, routePoints[_currentTargetIndex], step);
        transform.LookAt(routePoints[_currentTargetIndex], Vector3.up);
        if (Vector3.Distance(transform.position, routePoints[_currentTargetIndex]) < 0.001f)
        {
            _currentTargetIndex++;
            if (_currentTargetIndex >= routePoints.Length)
            {
                _waiting = true;
                Invoke("StartWalking", Random.Range(2, 5));
            }
        }
    }

    void StartWalking()
    {
        transform.position = routePoints[0];
        _currentTargetIndex = 1;
        _waiting = false;
    }




}
