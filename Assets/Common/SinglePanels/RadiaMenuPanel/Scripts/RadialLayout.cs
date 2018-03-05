using UnityEngine;
using UnityEngine.EventSystems;
public class RadialLayout : UIBehaviour
{
	public float fDistance;
	[Range(0f,360f)]
	public float MinAngle, MaxAngle, StartAngle;
    protected DrivenRectTransformTracker m_Tracker;

    protected override void OnEnable() { base.OnEnable(); CalculateRadial(); }

	protected override void OnValidate()
	{
		base.OnValidate();
		CalculateRadial();
	}

    public void UpdateFDistance( float fdist )
    {
        fDistance = fdist;
        CalculateRadial();
    }

	void CalculateRadial()
	{
		m_Tracker.Clear();
		if (transform.childCount == 0)
			return;

        int activeChildren = 0;
        for (int i = 0; i < transform.childCount; i++)
		{
			 if( transform.GetChild(i).gameObject.activeSelf )
                 activeChildren++;
		}

		float fOffsetAngle = ((MaxAngle - MinAngle)) / (activeChildren - 1);

        if( MaxAngle - MinAngle == 360 )
            fOffsetAngle = ((MaxAngle - MinAngle)) / (activeChildren);
		
		float fAngle = StartAngle;
        for (int i = 0; i < activeChildren; i++)
		{
			RectTransform child = (RectTransform)transform.GetChild(i);
			if (child != null)
			{
				//Adding the elements to the tracker stops the user from modifiying their positions via the editor.
				m_Tracker.Add(this, child, 
				              DrivenTransformProperties.Anchors |
				              DrivenTransformProperties.AnchoredPosition |
				              DrivenTransformProperties.Pivot);
				Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
				child.localPosition = vPos * fDistance;
				//Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
				child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
				fAngle -= fOffsetAngle;
			}
		}
		
	}
}