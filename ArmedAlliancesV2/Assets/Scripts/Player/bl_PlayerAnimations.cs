//////////////////////////////////////////////////////////////////////////////
// bl_PlayerAnimations.cs
//
// - was ordered to encourage TPS player animations using legacy animations,
//  and heat look controller from Unity technologies.
//
//                           Lovatto Studio
/////////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class bl_PlayerAnimations : MonoBehaviour
{

    [HideInInspector]
    public bool m_Update = true;
    [Header("Animations Name")]
    public _Animations m_Animations;
    [Header("Animations Settings")]
    [Range(0.2f, 5)]
    public float WalkForwardSpeed = 1.5f;
    [Range(0.2f, 5)]
    public float WalkBackWardSpeed = 1.5f;
    [Range(0.2f, 5)]
    public float WalkLeftSpeed = 1.5f;
    [Range(0.2f, 5)]
    public float WalkRightSpeed = 1.5f;
    [Range(0.2f, 5)]
    public float RunSpeed = 1.5f;
    [Space(5)]
    [Header("Heat Look")]
    public Transform rootNode;
    public Transform mTarget = null;
    public BendingSegment[] segments;
    public NonAffectedJoints[] nonAffectedJoints;
    public Vector3 headLookVector = Vector3.forward;
    public Vector3 headUpVector = Vector3.up;
    public Vector3 target = Vector3.zero;
    public float effect = 1;
    public bool overrideAnimation = false;
    [HideInInspector]
    public bool grounded = true;
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public Vector3 localVelocity = Vector3.zero;
    public float turnSpeed;

    [HideInInspector]
    public float movementSpeed;
    [HideInInspector]
    public float lastYRotation;


    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        SetupSegments();
        m_Animations.anim[m_Animations.walkForward].wrapMode = WrapMode.Loop;
        m_Animations.anim[m_Animations.walkForward].layer = 1;
        m_Animations.anim[m_Animations.walkForward].weight = 1;
        m_Animations.anim[m_Animations.walkBackwards].layer = 1;
        m_Animations.anim[m_Animations.strafeLeft].layer = 1;
        m_Animations.anim[m_Animations.strafeRight].layer = 1;
        m_Animations.anim[m_Animations.walkForward].speed = WalkForwardSpeed;
        m_Animations.anim[m_Animations.walkBackwards].speed = WalkBackWardSpeed;
        m_Animations.anim[m_Animations.strafeLeft].speed = WalkLeftSpeed;
        m_Animations.anim[m_Animations.strafeRight].speed = WalkRightSpeed;

        m_Animations.anim[m_Animations.runForward].layer = 1;
        m_Animations.anim[m_Animations.runForward].speed = RunSpeed;

        m_Animations.anim[m_Animations.standingJump].layer = 1;
        m_Animations.anim[m_Animations.standingJump].wrapMode = WrapMode.Loop;
        m_Animations.anim[m_Animations.standingJump].blendMode = AnimationBlendMode.Blend;
        m_Animations.anim[m_Animations.runJump].layer = 1;
        m_Animations.anim[m_Animations.runJump].wrapMode = WrapMode.Once;
        m_Animations.anim[m_Animations.runJump].blendMode = AnimationBlendMode.Blend;

        m_Animations.anim[m_Animations.idleAim].layer = 1;

        m_Animations.anim[m_Animations.crouchIdle].layer = 1;
        m_Animations.anim[m_Animations.crouchWalkForward].layer = 1;
        m_Animations.anim[m_Animations.crouchWalkLeft].layer = 1;
        m_Animations.anim[m_Animations.crouchWalkRight].layer = 1;
        m_Animations.anim[m_Animations.crouchWalkBackwards].layer = 1;

        m_Animations.anim[m_Animations.turnAnim].layer = 1;
        m_Animations.anim[m_Animations.turnAnimCrouch].layer = 1;
    }
    /// <summary>
    /// 
    /// </summary>
    void SetupSegments()
    {
        if (rootNode == null)
        {
            rootNode = transform;
        }

        // Setup segments
        foreach (BendingSegment segment in segments)
        {
            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);
            segment.referenceLookDir =
                parentRotInv * rootNode.rotation * headLookVector.normalized;
            segment.referenceUpDir =
                parentRotInv * rootNode.rotation * headUpVector.normalized;
            segment.angleH = 0;
            segment.angleV = 0;
            segment.dirUp = segment.referenceUpDir;

            segment.chainLength = 1;
            Transform t = segment.lastTransform;
            while (t != segment.firstTransform && t != t.root)
            {
                segment.chainLength++;
                t = t.parent;
            }

            segment.origRotations = new Quaternion[segment.chainLength];
            t = segment.lastTransform;
            for (int i = segment.chainLength - 1; i >= 0; i--)
            {
                segment.origRotations[i] = t.localRotation;
                t = t.parent;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!m_Update)
            return;

        ControllerInfo();
        Animate();
    }
    /// <summary>
    /// 
    /// </summary>
    void ControllerInfo()
    {
        localVelocity = rootNode.InverseTransformDirection(velocity);
        localVelocity.y = 0;

        turnSpeed = Mathf.DeltaAngle(lastYRotation, transform.rotation.eulerAngles.y);
        movementSpeed = velocity.magnitude;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private float HorizontalAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

    }
    /// <summary>
    /// 
    /// </summary>
    void Animate()
    {
        if (grounded)
        {

            if (movementSpeed < 1.0f)
            {
                m_Animations.anim.CrossFade(m_Animations.idleAim);
                if (turnSpeed > 0.1f || turnSpeed < -0.1f)
                {
                    if (state == 0)
                    {
                        m_Animations.anim.Blend(m_Animations.turnAnim, 0.2f);
                    }
                    else if (state == 1)
                    {
                        m_Animations.anim.Blend(m_Animations.turnAnimCrouch, 0.2f);
                    }
                }
            }

            if (state == 0)
            { //Walk and Run

                if (localVelocity.z > 1f)
                { //Forward

                    if (movementSpeed < 5.0f)
                    {
                        m_Animations.anim.CrossFade(m_Animations.walkForward, 0.2f, PlayMode.StopAll);
                    }
                    else
                    {
                        m_Animations.anim.CrossFade(m_Animations.runForward, 0.2f);
                    }

                    return;
                }
                else if (localVelocity.z < -1f)
                { //Backward

                    if (movementSpeed < 5.0f)
                    {
                        m_Animations.anim.CrossFade(m_Animations.walkBackwards, 0.2f);
                    }

                }
                else if (localVelocity.x > 1f)
                {

                    if (movementSpeed < 5.0f)
                    {
                        m_Animations.anim.CrossFade(m_Animations.strafeRight, 0.2f);
                    }
                }
                else if (localVelocity.x < -1f)
                {

                    if (movementSpeed < 5.0f)
                    {
                        m_Animations.anim.CrossFade(m_Animations.strafeLeft, 0.2f);
                    }

                }
                else
                {
                    m_Animations.anim.CrossFade(m_Animations.idleAnim, 0.3f);
                }

            }
            else if (state == 1)
            { //Crouch
                if (localVelocity.z > 0.2f)
                {
                    m_Animations.anim.CrossFade(m_Animations.crouchWalkForward, 0.2f);
                }
                else if (localVelocity.z < -0.2f)
                {
                    m_Animations.anim.CrossFade(m_Animations.crouchWalkBackwards, 0.2f);

                }
                else if (localVelocity.x < -0.2f)
                {
                    m_Animations.anim.CrossFade(m_Animations.crouchWalkLeft, 0.2f);

                }
                else if (localVelocity.x > 0.2f)
                {
                    m_Animations.anim.CrossFade(m_Animations.crouchWalkRight, 0.2f);
                }
                else
                {
                    m_Animations.anim.CrossFade(m_Animations.crouchIdle, 0.3f);
                }
            }

        }
        else
        {
            m_Animations.anim.CrossFade(m_Animations.runJump, 0.15f, PlayMode.StopAll);
            m_Animations.anim.CrossFadeQueued(m_Animations.standingJump, 0.1f, QueueMode.CompleteOthers);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LateUpdate()
    {
        if (!m_Update)
            return;
        if (Time.timeScale == 0)
            return;


        if (mTarget != null)
        {
            target = mTarget.position;
        }
        // Remember initial directions of joints that should not be affected
        Vector3[] jointDirections = new Vector3[nonAffectedJoints.Length];
        for (int i = 0; i < nonAffectedJoints.Length; i++)
        {
            foreach (Transform child in nonAffectedJoints[i].joint)
            {
                jointDirections[i] = child.position - nonAffectedJoints[i].joint.position;
                break;
            }
        }

        // Handle each segment
        foreach (BendingSegment segment in segments)
        {
            Transform t = segment.lastTransform;
            if (overrideAnimation)
            {
                for (int i = segment.chainLength - 1; i >= 0; i--)
                {
                    t.localRotation = segment.origRotations[i];
                    t = t.parent;
                }
            }

            Quaternion parentRot = segment.firstTransform.parent.rotation;
            Quaternion parentRotInv = Quaternion.Inverse(parentRot);

            // Desired look direction in world space
            Vector3 lookDirWorld = (target - segment.lastTransform.position).normalized;

            // Desired look directions in neck parent space
            Vector3 lookDirGoal = (parentRotInv * lookDirWorld);

            // Get the horizontal and vertical rotation angle to look at the target
            float hAngle = bl_UtilityHelper.AngleAroundAxis(
                segment.referenceLookDir, lookDirGoal, segment.referenceUpDir
            );

            Vector3 rightOfTarget = Vector3.Cross(segment.referenceUpDir, lookDirGoal);

            Vector3 lookDirGoalinHPlane =
                lookDirGoal - Vector3.Project(lookDirGoal, segment.referenceUpDir);

            float vAngle = bl_UtilityHelper.AngleAroundAxis(
                lookDirGoalinHPlane, lookDirGoal, rightOfTarget
            );

            // Handle threshold angle difference, bending multiplier,
            // and max angle difference here
            float hAngleThr = Mathf.Max(
                0, Mathf.Abs(hAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(hAngle);

            float vAngleThr = Mathf.Max(
                0, Mathf.Abs(vAngle) - segment.thresholdAngleDifference
            ) * Mathf.Sign(vAngle);

            hAngle = Mathf.Max(
                Mathf.Abs(hAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(hAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(hAngle) * Mathf.Sign(segment.bendingMultiplier);

            vAngle = Mathf.Max(
                Mathf.Abs(vAngleThr) * Mathf.Abs(segment.bendingMultiplier),
                Mathf.Abs(vAngle) - segment.maxAngleDifference
            ) * Mathf.Sign(vAngle) * Mathf.Sign(segment.bendingMultiplier);

            // Handle max bending angle here
            if (!segment.OnlyVertical)
            {
                hAngle = Mathf.Clamp(hAngle, -segment.maxBendingAngle, segment.maxBendingAngle);
            }
            else
            {
                hAngle = Mathf.Clamp(hAngle, -1, 1);
            }
            vAngle = Mathf.Clamp(vAngle, -segment.maxBendingAngle, segment.maxBendingAngle);

            Vector3 referenceRightDir =
                Vector3.Cross(segment.referenceUpDir, segment.referenceLookDir);

            // Lerp angles
            if (!segment.OnlyVertical)
            {
                segment.angleH = Mathf.Lerp(
                    segment.angleH, hAngle, Time.deltaTime * segment.responsiveness
                );
            }
            else
            { //Use this when find the transform only move in vertical.
              // segment.angleH = 0.0f;
                segment.angleH = Mathf.Lerp(
                    segment.angleH, (hAngle / 2.5f), Time.deltaTime * segment.responsiveness
                );
            }
            segment.angleV = Mathf.Lerp(
                segment.angleV, vAngle, Time.deltaTime * segment.responsiveness
            );

            // Get direction
            lookDirGoal = Quaternion.AngleAxis(segment.angleH, segment.referenceUpDir)
                * Quaternion.AngleAxis(segment.angleV, referenceRightDir)
                * segment.referenceLookDir;

            // Make look and up perpendicular
            Vector3 upDirGoal = segment.referenceUpDir;
            Vector3.OrthoNormalize(ref lookDirGoal, ref upDirGoal);

            // Interpolated look and up directions in neck parent space
            Vector3 lookDir = lookDirGoal;
            segment.dirUp = Vector3.Slerp(segment.dirUp, upDirGoal, Time.deltaTime * 5);
            Vector3.OrthoNormalize(ref lookDir, ref segment.dirUp);

            // Look rotation in world space
            Quaternion lookRot = (
                (parentRot * Quaternion.LookRotation(lookDir, segment.dirUp))
                * Quaternion.Inverse(
                    parentRot * Quaternion.LookRotation(
                        segment.referenceLookDir, segment.referenceUpDir
                    )
                )
            );

            // Distribute rotation over all joints in segment
            Quaternion dividedRotation =
                Quaternion.Slerp(Quaternion.identity, lookRot, effect / segment.chainLength);
            t = segment.lastTransform;
            for (int i = 0; i < segment.chainLength; i++)
            {
                t.rotation = dividedRotation * t.rotation;
                t = t.parent;
            }
        }

        // Handle non affected joints
        for (int i = 0; i < nonAffectedJoints.Length; i++)
        {
            Vector3 newJointDirection = Vector3.zero;

            foreach (Transform child in nonAffectedJoints[i].joint)
            {
                newJointDirection = child.position - nonAffectedJoints[i].joint.position;
                break;
            }

            Vector3 combinedJointDirection = Vector3.Slerp(
                jointDirections[i], newJointDirection, nonAffectedJoints[i].effect
            );

            nonAffectedJoints[i].joint.rotation = Quaternion.FromToRotation(
                newJointDirection, combinedJointDirection
            ) * nonAffectedJoints[i].joint.rotation;
        }

        //for rotation
        lastYRotation = transform.rotation.eulerAngles.y;
    }

    [System.Serializable]
    public class BendingSegment
    {
        public Transform firstTransform;
        public Transform lastTransform;
        public float thresholdAngleDifference = 0;
        public float bendingMultiplier = 0.6f;
        public float maxAngleDifference = 30;
        public float maxBendingAngle = 80;
        public float responsiveness = 5;
        public bool OnlyVertical = false;
        internal float angleH;
        internal float angleV;
        internal Vector3 dirUp;
        internal Vector3 referenceLookDir;
        internal Vector3 referenceUpDir;
        internal int chainLength;
        internal Quaternion[] origRotations;
    }

    [System.Serializable]
    public class _Animations
    {
        public Animation anim;
        [Space(5)]

        public string idleAnim = "";
        public string idleAim = "";
        //Walk
        public string walkForward = "";
        public string walkBackwards = "";
        public string strafeLeft = "";
        public string strafeRight = "";
        //Run
        public string runForward = "";

        //Crouch
        public string crouchIdle = "";
        public string crouchWalkForward = "";
        public string crouchWalkBackwards = "";
        public string crouchWalkLeft = "";
        public string crouchWalkRight = "";
        //Jump
        public string runJump = "";
        public string standingJump = "";
        public string turnAnim;
        public string turnAnimCrouch = "";
    }
    [System.Serializable]
    public class NonAffectedJoints
    {
        public Transform joint;
        public float effect = 0;
    }
}