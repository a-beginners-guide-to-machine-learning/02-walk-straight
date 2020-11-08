using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength { get; set; } = 1;
    public float TimeAlive { get; set; }
    public float DistanceTraveled { get; set; }
    public DNA DNA { get; set; }

    private ThirdPersonCharacter _character;
    private Vector3 _startPosition;
    private Vector3 _move;
    private bool _jump;
    private bool _alive = true;

    public void Init()
    {
        // Initialize DNA
        // 0 Forward
        // 1 back
        // 2 left
        // 3 right
        // 4 jump
        // 5 crouch

        TimeAlive = 0;
        DNA = new DNA(DNALength, 6);
        _character = GetComponent<ThirdPersonCharacter>();
        _startPosition = transform.position;
        _alive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("dead"))
        {
            _alive = false;
        }
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // Read DNA
        float vertical = 0;
        float horizontal = 0;
        bool crouch = false;

        if (DNA.GetGene(0) == 0) vertical = 1;
        else if (DNA.GetGene(0) == 1) vertical = -1;
        else if (DNA.GetGene(0) == 2) horizontal = -1;
        else if (DNA.GetGene(0) == 3) horizontal = 1;
        else if (DNA.GetGene(0) == 4) _jump = true;
        else if (DNA.GetGene(0) == 5) crouch = true;

        _move = vertical * Vector3.forward + horizontal * Vector3.right;
        _character.Move(_move, crouch, _jump);
        _jump = false;

        if (_alive)
        {
            TimeAlive += Time.deltaTime;
            DistanceTraveled = Vector3.Distance(transform.position, _startPosition);
        }
    }
}
