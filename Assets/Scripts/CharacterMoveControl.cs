using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveControl : MonoBehaviour
{
    public struct CharacterCheckPointData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float energy;
        public bool boostEnabled;
        public float boostCDRemain;
    }
    public CharacterCheckPointData GetCheckPointData()
    {
        CharacterCheckPointData res;
        res.position = transform.position;
        res.rotation = transform.rotation;
        res.energy = normalizedEnergy;
        res.boostEnabled = boostEnabled;
        res.boostCDRemain = boostCDRemain;
        return res;
    }
    public void ApplyCheckPointData(CharacterCheckPointData data)
    {
        transform.position = data.position;
        transform.rotation = data.rotation;
        normalizedEnergy = data.energy;
        rb.velocity = Vector3.zero;
        lastPosition = transform.position;
        worldVelocity = Vector3.zero;
        // boostEnabled = data.boostEnabled;
        boostCDRemain = data.boostCDRemain;
    }
    public float gravity = 9.8f;
    [Header("On Ground")]
    public float groundHorizontalAccelFactor = 1;
    public float groundHorizontalDragFactor = 1;
    [Header("On Air")]
    public float airVerticalAccelFactor = 1;
    public float airVerticalDragFactor = 1;
    public float airHorizontalAccelFactor = 1;
    public float airHorizontalDragFactor = 1;
    [Header("Boost")]
    public bool boostEnabled = false;
    public void SetBoolstEnabled(bool b)
    {
        boostEnabled = b;
    }
    public float boostCDTime = 5;
    public float maxChargeDuration = 2;
    public float inChargingAccelFactor = 0.3f;
    public float boostFactor = 1;
    public float boostDuration = 1;
    public float boostAirDragFactor = 0.1f;
    [Header("Energy")]
    public float totalEnergy = 100;
    public float energyConsume = 5;
    public float currentEnergy
    {
        get
        {
            return normalizedEnergy * totalEnergy;
        }
        set
        {
            this.normalizedEnergy = value / totalEnergy;
            if (this.normalizedEnergy > 1)
                this.normalizedEnergy = 1;
            else if (this.normalizedEnergy < 0)
                this.normalizedEnergy = 0;
        }
    }
    private float normalizedEnergy = 1.0f;
    [Header("Other")]
    public float leavePlatformVecocityFactor = 1;
    public Animator animator;
    public GameObject fire;
    void Start()
    {
        cld = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        default_attach = transform.parent;
        lastPosition = transform.TransformPoint(Vector3.zero);
        alert = transform.Find("EnergyAlert").gameObject;
        alert.gameObject.SetActive(false);
    }
    public float boostCDRemain { get; private set; }
    Rigidbody rb;
    Camera cm;
    bool charging = false;
    bool boosting = false;
    Vector3 lastPosition;
    Vector3 worldVelocity;
    GameObject alert;

    Vector2 DragFuncion(Vector2 vel, float drag_factor)
    {
        var m = -vel.magnitude;
        return vel * m * drag_factor;
    }

    float DragFunction(float speed, float drag_factor)
    {
        return speed * speed * drag_factor;
    }
    void LateUpdate()
    {
        worldVelocity = (transform.TransformPoint(Vector3.zero) - lastPosition) / Time.deltaTime;
        lastPosition = transform.TransformPoint(Vector3.zero);
    }
    void Update()
    {
        Vector3 vel = transform.InverseTransformDirection(rb.velocity);
        onGround = GroundDetect();
        float forwardFactor;
        float rightwardFactor;
        float forwardDrag;
        float rightwardDrag;
        float upwardFactor = airVerticalAccelFactor;
        if (onGround)
        {
            var drag = DragFuncion(new Vector2(vel.z, vel.x), groundHorizontalDragFactor) * Mathf.Sign(transform.localScale.z);
            forwardDrag = drag.x;
            rightwardDrag = drag.y;
            forwardFactor = groundHorizontalAccelFactor;
            rightwardFactor = groundHorizontalAccelFactor;
        }
        else
        {
            var drag = DragFuncion(new Vector2(vel.z, vel.x), airHorizontalDragFactor) * Mathf.Sign(transform.localScale.z);
            forwardDrag = drag.x;
            rightwardDrag = drag.y;
            forwardFactor = airHorizontalAccelFactor;
            rightwardFactor = airHorizontalAccelFactor;
        }
        float accelForward = (float)(Input.GetAxisRaw("Vertical") * forwardFactor * forwardFactor);
        float accelRightward = (float)(Input.GetAxisRaw("Horizontal") * rightwardFactor * forwardFactor);
        Vector2 horizontalAccel = new Vector2(accelRightward, accelForward).normalized;
        accelForward *= Mathf.Abs(horizontalAccel.y);
        accelRightward *= Mathf.Abs(horizontalAccel.x);

        if (charging)
        {
            accelRightward *= inChargingAccelFactor;
            accelForward *= inChargingAccelFactor;
        }
        accelForward += forwardDrag;
        accelRightward += rightwardDrag;

        float upwardDrag = vel.y > 0 ?
            -DragFunction(vel.y, airVerticalDragFactor) :
            DragFunction(vel.y, airVerticalDragFactor);

        float accelUpward = 0;
        accelUpward += (float)(Input.GetAxisRaw("Jump") * upwardFactor);
        alert.SetActive(false);
        if (accelUpward > 0)
        {
            if (normalizedEnergy > 0)
            {
                fire.SetActive(true);
                normalizedEnergy -= (energyConsume / totalEnergy) * Time.deltaTime;
                if (normalizedEnergy < 0)
                    normalizedEnergy = 0;
            }
            else
            {
                accelUpward = 0;
                alert.SetActive(true);
                fire.SetActive(false);
            }
        }
        else
        {
            fire.SetActive(false);
        }
        accelUpward += upwardDrag;
        /// charge/boost
        if (((boostCDRemain -= Time.deltaTime) < 0))
            boostCDRemain = 0;
        float boost = Input.GetAxis("Boost");
        if (Input.GetAxisRaw("Boost") > 0)
        {
            StartBoost();
        }
        else
        {
            /// may break `StartBoost::Charging` coroutine
            charging = false;
        }
        rb.AddRelativeForce(accelRightward, accelUpward - gravity, accelForward, ForceMode.Acceleration);
        animator.SetBool("Land", onGround);
        animator.SetFloat("speed_vertical", vel.y);
        animator.SetFloat("speed", new Vector2(vel.z, vel.x).magnitude);
    }

    void StartBoost()
    {
        if (!boostEnabled || boostCDRemain > 0)
            return;
        float chargingDuration = 0;
        IEnumerator Charging()
        {
            while (charging)
            {
                chargingDuration += Time.deltaTime;
                if (chargingDuration > maxChargeDuration)
                    chargingDuration = maxChargeDuration;
                yield return null;
            }
            boosting = true;
            yield return StartCoroutine(Boosting());
        }
        IEnumerator Boosting()
        {
            var saved = airHorizontalDragFactor;
            boostCDRemain = boostCDTime;
            airHorizontalDragFactor *= boostAirDragFactor;
            /// do boost
            var boost_vel = transform.TransformDirection(0, 0, boostFactor * chargingDuration);
            boost_vel.x *= Mathf.Sign(transform.localScale.x);
            boost_vel.y *= Mathf.Sign(transform.localScale.y);
            boost_vel.z *= Mathf.Sign(transform.localScale.z);
            rb.velocity += boost_vel;
            yield return new WaitForSeconds(boostDuration);
            airHorizontalDragFactor = saved;
            boosting = false;
        }
        if (charging == false && boosting == false)
        {
            charging = true;
            StartCoroutine(Charging());
        }
    }

    public void WWW(string desc)
    {

    }
    public void StartEnergyChange(float delta, float time)
    {
        if (time <= 0 || delta <= 0)
            return;
        float spd = delta / time;
        IEnumerator change()
        {
            while (time > 0)
            {
                float dd = Time.deltaTime;
                if (time < dd)
                    dd = time;
                currentEnergy += dd * spd;
                yield return null;
                time -= dd;
            }
        }
        StartCoroutine(change());
    }

    Collider cld;
    bool GroundDetect()
    {
        var bounds = cld.bounds;
        var pos = bounds.center;

        pos.y -= bounds.extents.y;
        pos.y += 0.01f;
        RaycastHit res;
        if (Physics.Raycast(pos, Vector3.down, out res, 0.02f))
        {
            transform.SetParent(res.transform);
            return res.transform.CompareTag("Platform");
        }
        else if (transform.parent != default_attach)
        {
            transform.SetParent(default_attach);
            if (Input.GetAxisRaw("Jump") > 0)
            {
                rb.velocity += worldVelocity;
            }
        }
        return false;
    }

    bool onGround;
    Transform default_attach;
}
