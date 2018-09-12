using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper {

	static public Vector3 GetBallisticMotion(float angle, float speed, float gravity, float time, float direction = 1f, float height = 0f)
    {
        float radAngle = angle * Mathf.Deg2Rad;

        Vector3 vec = Vector3.zero;
        vec.x = Mathf.Cos(radAngle) * speed * time * direction;
        vec.y = -1f / 2f * gravity * time * time + Mathf.Sin(radAngle) * speed * time + height;

        return vec;
    }

    static public float CubicLerp(float startValue, float duration, float timeOffset, bool clamp = true)
    {
        float startTime = -duration / 2f;
        float currentTime = startTime + timeOffset;
        float coeff = 1f / CubicPower(startTime);

        float lerp = coeff * CubicPower(currentTime);

        if (clamp)
        {
            if (lerp < -1f)
                lerp = -1f;
            else if (lerp > 1f)
                lerp = 1f;
        }

        return startValue * lerp;
    }

    static public float CubicPower(float x)
    {
        return x * x * x;
    }
}
