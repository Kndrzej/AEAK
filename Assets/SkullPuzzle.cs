using UnityEngine;
using UnityEngine.InputSystem;

public class SkullPuzzle : MonoBehaviour
{
    [Header("Puzzle Pieces")]
    [SerializeField] private Transform[] skullTransforms;
    [SerializeField] private Transform opheliaEyes;

    [Header("Puzzle Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private int pressesToSolve = 1;
    
    private Transform rotatingSkull = null;
    private float targetY = 0f;
    private int[] skullPressCounts;

    private bool[] skullSolved;
    private bool puzzleSolved = false;

    private void Awake()
    {
        skullPressCounts = new int[skullTransforms.Length];
        skullSolved = new bool[skullTransforms.Length];
    }

    private void Update()
    {
        SmoothRotate();
    }



    public void TryRotateFromRaycast(Transform hitTransform)
    {
        for (int i = 0; i < skullTransforms.Length; i++)
        {
            Transform skull = skullTransforms[i];
            if (hitTransform.IsChildOf(skull))
            {
                StartRotation(skull, i);
                return;
            }
        }
    }

    private void StartRotation(Transform skull, int index)
    {
        if (skullSolved[index])
            return;

        rotatingSkull = skull;
        targetY = (skull.localEulerAngles.y + 90f) % 360f;

        skullPressCounts[index]++;

        if (skullPressCounts[index] >= pressesToSolve)
        {
            skullSolved[index] = true;

            if (skull.childCount > 0)
            {
                skull.GetChild(0).gameObject.SetActive(true);
            }

            CheckPuzzleSolved();
        }
    }

    private void CheckPuzzleSolved()
    {
        if (puzzleSolved) return;

        foreach (bool solved in skullSolved)
        {
            if (!solved)
                return;
        }

        puzzleSolved = true;
        OnPuzzleSolved();
    }

    private void SmoothRotate()
    {
        if (rotatingSkull == null) return;

        float currentY = rotatingSkull.localEulerAngles.y;
        float newY = Mathf.MoveTowardsAngle(currentY, targetY, rotateSpeed * Time.deltaTime);
        rotatingSkull.localEulerAngles = new Vector3(rotatingSkull.localEulerAngles.x, newY, rotatingSkull.localEulerAngles.z);

        if (Mathf.Approximately(newY, targetY))
        {
            rotatingSkull.localEulerAngles = new Vector3(rotatingSkull.localEulerAngles.x, targetY, rotatingSkull.localEulerAngles.z);
            rotatingSkull = null;

            if (IsPuzzleSolved())
            {
                OnPuzzleSolved();
            }
        }
    }

    private bool IsPuzzleSolved()
    {
        foreach (int count in skullPressCounts)
        {
            if (count < pressesToSolve)
                return false;
        }
        return true;
    }

    private void OnPuzzleSolved()
    {    
       opheliaEyes.gameObject.SetActive(true);   
    }
}

