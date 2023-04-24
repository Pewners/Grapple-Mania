using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class hoverOver : MonoBehaviour
{
    int UILayer = 5;
    
    bool pointerOverItemSlot;
    bool lastPointerOverItemSlot;

    private Transform currItemSlotT;
    private string currName;
    private string currDescr;

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("pointerenter hover " + gameObject.name);

        //UiManager.instance.ShowHoverInfo("test", "testdescription", transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        print("pointerextit hover " + gameObject.name);

        //UiManager.instance.HideHoverInfo();
    }

    private void Update()
    {
        pointerOverItemSlot = IsPointerOverUIElement();

        bool stateChange = pointerOverItemSlot != lastPointerOverItemSlot;

        if (stateChange)
        {
            if (pointerOverItemSlot)
            {
                if(currName != "" && currDescr != "")
                {
                    UiManager.instance.ShowHoverInfo(currName, currDescr, currItemSlotT.position);
                    print("pointerEnteredItemSlot " + currItemSlotT.name);

                    // audio
                    //AudioManager.PlayEffect(GameAssets.EffectSound.HoverOverButton);
                }
            }
            else
            {
                UiManager.instance.HideHoverInfo();
                print("pointerExitedItemSlot " + currItemSlotT.name);
            }
        }

        lastPointerOverItemSlot = pointerOverItemSlot;

        // click item slots
        if (Input.GetKeyDown(KeyCode.Mouse0) && pointerOverItemSlot)
        {
            ///currItemSlotT.GetComponent<ItemSlot>().OnClick();
            print("Clicked on itemSlot " + currItemSlotT.name);
        }
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            ///if (curRaysastResult.gameObject.GetComponent<ItemSlot>() != null)
            ///{
            ///    currItemSlotT = curRaysastResult.gameObject.transform;
            ///    ItemSlot itemSlot = currItemSlotT.GetComponent<ItemSlot>();
            ///    currName = itemSlot.itemName;
            ///    currDescr = itemSlot.itemDescr;
            ///    return true;
            ///}
        }

        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
