using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocator : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] Transform rotator;
    [SerializeField] Transform cannon;
    [SerializeField] ParticleSystem projectileParticles;
    [SerializeField] float range = 15f;

    Transform target;

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();
        AimWeapon();
    }

    void AimWeapon()
    {
        if (target == null) return;

        float targetDistance = Vector3.Distance(transform.position, target.position);

        AimRotator();
        AimCannon();

        if (targetDistance < range)
        {
            Attack(true);
        }
        else
        {
            Attack(false);
        }
    }

    void AimRotator()
    {
        Vector3 directionToTarget = target.position - rotator.position;
        directionToTarget.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rotator.rotation = targetRotation;
    }

    void AimCannon()
    {
        Vector3 directionToTarget = target.position - cannon.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        float targetAngleX = targetRotation.eulerAngles.x;

        if (targetAngleX > 180) targetAngleX -= 360;

        targetAngleX = Mathf.Clamp(targetAngleX, -90f, 90f);

        cannon.localRotation = Quaternion.Euler(targetAngleX, 0, 0);
    }

    void FindClosestTarget()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        Transform closestTarget = null;

        float maxDistance = Mathf.Infinity;

        foreach (EnemyController enemy in enemies)
        {
            float targetDistance = Vector3.Distance(transform.position, enemy.transform.position);

            if (targetDistance < maxDistance)
            {
                closestTarget = enemy.transform;
                maxDistance = targetDistance;
            }
        }

        target = closestTarget;
    }

    void Attack(bool isActive)
    {
        var emissionModule = projectileParticles.emission;
        emissionModule.enabled = isActive;

        if (isActive && projectileParticles.isStopped)
        {
            projectileParticles.Play();
        }
        else if (!isActive && projectileParticles.isPlaying)
        {
            projectileParticles.Stop();
        }
    }
}
