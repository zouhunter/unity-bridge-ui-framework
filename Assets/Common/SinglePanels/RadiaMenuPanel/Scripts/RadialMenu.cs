using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class RadialMenu : MonoBehaviour, IPointerDownHandler
{
    enum State
    {
        Deactivated,
        Activating,
        Active,
        Deactivating,
    }

    // State of the menu
    State m_State = State.Deactivated;

    // Radial layout - positions the elements
    RadialLayout m_RadLayout;

    // Prefab buttons that will populate the menu
    public RectTransform m_DefaultMenuObject;

    public RadialSlider    m_SliderPrefab;
    public RadialMenuObject m_ButtonPrefab;
    public RadialMenuObject m_TogglePrefab;
    public RadialList       m_ListPrefab;

    public Color m_FGCol;
    public Color m_BGCol;
    public Color m_HLCol;
    public Color m_TextCol;

    RadialMenuObject m_SelectedMenuObject;

    // Name displayed on the menu
    public string m_MenuName = "Menu";
    Text m_MenuText;

    // Main menu button
    protected Button MainButton;

    // List of buttons
    public List<RadialMenuObject> m_MenuObjects = new List<RadialMenuObject>();


    // Size in pixels of buttons
    public float m_ButtonSizeMain = 60;
    public float m_ButtonSizeChild = 60;

    // Radius in pixels that the buttons will move out too
    public float m_Radius = 100;   

    // Area within which no selection will register, giving you a way to cancel the action
    public float m_DeadZone = 30;

    // Display the name of the last selected element or menu name after selection
    public bool m_DisplaySelectedName = false;

    // Target radius for the buttons
    float m_TargetRadius = 0;

    // Smoothing on the radius lerp
    public float m_Smoothing = 8;

    // Current index of the selected element
    int m_SelectedIndex = 0;

    // flag for element being selected or not
    bool m_OptionSelected = false;

    // Anlge and range for the layout
    public float m_StartAngle = 0;
    public float m_AngleRange = 360;


    // Unity event which fires off the selected index
    [System.Serializable]
    public class SelectionEvent : UnityEvent<int> { }
    public SelectionEvent OnSelected;

    // Test array of events
    public SelectionEvent[] OnSelectedEvents;
       
    GameObject m_ObjectToMessage;

    public bool m_HideInactive = false;

	void Start () 
    {
        MainButton = GetComponent<Button>();
        m_MenuText = GetComponentInChildren<Text>();
        m_MenuText.text = m_MenuName;

        m_RadLayout = new GameObject("Radial Layout").AddComponent<RadialLayout>();
        m_RadLayout.transform.SetParent(transform);
        m_RadLayout.transform.localPosition = Vector3.zero;
        m_RadLayout.MaxAngle = m_AngleRange;
        m_RadLayout.StartAngle = m_StartAngle;

        int index = m_RadLayout.transform.GetSiblingIndex();
        m_RadLayout.transform.SetSiblingIndex(index - 1);

        SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
	}


    // Transition to pooling later
    public void Reset()
    {
        foreach (RadialMenuObject obj in m_MenuObjects)
            Destroy(obj.gameObject);

        m_MenuObjects.Clear();
    }

    void Update()
    {
        AdjustAngleBasedOnScreenPos();

        // Update selection colours
       
        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            if (!m_MenuObjects[i].gameObject.activeSelf)
                continue;

          
        }

        if (m_State == State.Activating || m_State == State.Active)
        {
           
        }
       

        if( m_State == State.Activating )
        {
            m_RadLayout.UpdateFDistance(Mathf.Lerp(m_RadLayout.fDistance, m_TargetRadius, Time.deltaTime * m_Smoothing));

             float norm = m_RadLayout.fDistance / m_Radius;
             SetAllObjectFades( Mathf.Pow( norm, 5) );

             if (Mathf.Abs(m_RadLayout.fDistance - m_TargetRadius) < 2)
             {
                 m_State = State.Active;
             }
        }
        else if (m_State == State.Active )
        {
            if (Vector3.Distance(Input.mousePosition, transform.position) < m_DeadZone)
            {
                m_MenuText.text = m_MenuName;
                m_OptionSelected = false;
            }
            else
            {
                m_SelectedIndex = FindClosestButtonIndex();
                m_OptionSelected = true;
            }

           // if (Input.GetMouseButtonUp(1))
           //     DeactivateMenu();
           
        }
        else if (m_State == State.Deactivating )
        {
            float norm = ( m_Radius - m_RadLayout.fDistance ) / m_Radius;
            norm = 1 - norm;
            SetAllObjectFades(Mathf.Pow(norm, 5));

            m_RadLayout.UpdateFDistance(Mathf.Lerp(m_RadLayout.fDistance, m_TargetRadius, Time.deltaTime * m_Smoothing));

            if (Mathf.Abs(m_RadLayout.fDistance - m_TargetRadius) < 2)
            {                
                m_State = State.Deactivated;
                for (int i = 0; i < m_MenuObjects.Count; i++)
                {
                    m_MenuObjects[i].gameObject.SetActive(false);
                }

                if (m_HideInactive)
                    transform.position = Vector3.right * Screen.width * 3;
            }
        }
    }

    public void SetSelectedMenuObject( RadialMenuObject obj)
    {
        m_SelectedMenuObject = obj;

        m_SelectedMenuObject.Fade( 1 );

        foreach( RadialMenuObject menuObj in m_MenuObjects )
            if( menuObj != m_SelectedMenuObject ) menuObj.Fade( .1f );

        Color col = m_FGCol;
        col.a = .1f;
        this.GetComponent<Image>().color = col;

        Color fontCol = m_TextCol;
        fontCol.a = .1f;
        m_MenuText.color = fontCol;
    }

    public void AddSlider(string name, float rangeMin, float rangeMax, float initialValue,UnityAction<object> onSlider)
    {
        RadialSlider slider = Instantiate( m_SliderPrefab );
        slider.Init(this,name, onSlider);
        slider.m_Range = new Vector2(rangeMin, rangeMax);
        slider.ScaledVal = initialValue;
        slider.m_Text.text = name;

        slider.transform.SetParent(m_RadLayout.transform);

        m_MenuObjects.Add(slider);

        foreach (RadialMenuObject menuObj in m_MenuObjects)
            menuObj.SetPallette(m_FGCol, m_BGCol, m_HLCol, m_TextCol);
    }

    public void AddList(string name,string[] names, UnityAction<object> onSelect)
    {
        RadialList list = Instantiate(m_ListPrefab);
        list.Init(this,name, onSelect);
        list.GenerateMenu(name, names);
        list.transform.SetParent(m_RadLayout.transform);

        m_MenuObjects.Add(list);
    }

    public void SetPosition( Vector3 pos )
    {
      //  MainButton.siz
        pos.x = Mathf.Clamp(pos.x, m_ButtonSizeMain / 2, Screen.width - m_ButtonSizeMain / 2 );
        pos.y = Mathf.Clamp(pos.y, m_ButtonSizeMain / 2, Screen.height - m_ButtonSizeMain / 2);

        transform.position = pos;
    }

    void AdjustAngleBasedOnScreenPos()
    {
        // Check if radius is outside of screen
        bool leftClip = false;
        bool rightClip = false;
        bool topClip = false;
        bool bottomClip = false;

        if ((transform.position - (Vector3.right * m_Radius)).x - (m_ButtonSizeChild/2f) < 0 ) leftClip = true;
        if ((transform.position + (Vector3.right * m_Radius)).x + (m_ButtonSizeChild / 2f) > Screen.width) rightClip = true;
        if ((transform.position + (Vector3.up * m_Radius)).y + (m_ButtonSizeChild / 2f) > Screen.height) topClip = true;
        if ((transform.position - (Vector3.up * m_Radius)).y - (m_ButtonSizeChild / 2f) < 0) bottomClip = true;


        if (leftClip && !topClip && !bottomClip )
        {
            m_RadLayout.StartAngle = 90;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (rightClip && !topClip && !bottomClip)
        {
            m_RadLayout.StartAngle = 270;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (topClip && !rightClip && !leftClip)
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if (bottomClip && !rightClip && !leftClip)
        {
            m_RadLayout.StartAngle = 180;
            m_RadLayout.MaxAngle = 180;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
        else if ( bottomClip && rightClip )
        {
            m_RadLayout.StartAngle = 180;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (bottomClip && leftClip)
        {
            m_RadLayout.StartAngle = 90;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (topClip && rightClip)
        {
            m_RadLayout.StartAngle = 270;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else if (topClip && leftClip)
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 90;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild / 2);
        }
        else
        {
            m_RadLayout.StartAngle = 0;
            m_RadLayout.MaxAngle = 360;
            SetButtonSize(m_ButtonSizeMain, m_ButtonSizeChild);
        }
    }

    int FindClosestButtonIndex()
    {
        int closestButtonIndex = 0;
        float closestDist = float.MaxValue;
        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            float dist = Vector3.Distance(Input.mousePosition, m_MenuObjects[i].transform.position);
            if( dist < closestDist )
            {
                closestDist = dist;
                closestButtonIndex = i;
            }
        }

        return closestButtonIndex;
    }

    void SetButtonSize( float mainSize, float childSize )
    {
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,   mainSize);
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,     mainSize);

        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            m_MenuObjects[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childSize);
            m_MenuObjects[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childSize);
        }
    }

    void SetAllObjectFades( float fade )
    {
        Color col = m_FGCol;
        col.a = fade;
        this.GetComponent<Image>().color = col;

        Color fontCol = m_TextCol;
        fontCol.a = fade;
        m_MenuText.color = fontCol;
       

        foreach (RadialMenuObject rO in m_MenuObjects)
            rO.Fade(fade);
    }

    public void DisengageSelection()
    {
        SetAllObjectFades(1);
    }
	
    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_State == State.Deactivated )
        {
            ActivateMenu();
            Debug.Log(this.gameObject.name + " Was Clicked.");
        }
        else if( m_State == State.Active )
        {
            DeactivateMenu();
            //print("here");
        }
    }
    
    public void ActivateMenu()
    {
        //print("Menu activated");

        for (int i = 0; i < m_MenuObjects.Count; i++)
        {
            m_MenuObjects[i].gameObject.SetActive(true);
        }

        m_State = State.Activating;

        m_TargetRadius = m_Radius;
    }

    void DeactivateMenu()
    {
        if (m_OptionSelected)
        {
            OnSelected.Invoke(m_SelectedIndex);
        }

        m_State = State.Deactivating;

        if ( !m_DisplaySelectedName )
            m_MenuText.text = m_MenuName;

        //print("Deactivating,    Name set to : " + m_MenuText.text);

        m_TargetRadius = 0;
    }

    RectTransform CreateNewButton()
    {
        RectTransform newBtn = Instantiate(m_DefaultMenuObject) as RectTransform;
        newBtn.transform.SetParent(m_RadLayout.transform);
        newBtn.gameObject.SetActive(false);

        newBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_ButtonSizeChild);
        newBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_ButtonSizeChild);

        newBtn.name = "Rad Btn " + m_MenuObjects.Count;

        return newBtn;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_Radius);
    }
}
