using UnityEngine;

public class LineFade : MonoBehaviour
{
    LineRenderer line;
    float startWidth;
    float counter;
    float percent;
    float fadeTime;
    bool activated;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        startWidth = line.widthMultiplier;
    }

    public void setup(Vector3 start, Vector3 end, float dur)
    {
        fadeTime = dur;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        activated = true;
    }

    private void FixedUpdate()
    {
        if (!activated) return;
        counter += Time.deltaTime;
        percent = counter / fadeTime;
        line.widthMultiplier = Mathf.Lerp(startWidth, 0, percent);
        if (percent > 1) Destroy(gameObject);
    }
}
