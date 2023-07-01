using UnityEngine;

public class SwordObj : AttackObj
{
    private float _timer;

    protected override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > 0.15f)
        {
            _timer = 0f;
            Recycle();
        }
    }
}
