using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [Header("UiView")]
    public List<UiView> allViews;
    public UiView view_ingameUi;

    private bool viewOpened;
    private Vector3 viewOpenPoint;
    public float maxDistanceToViewOpenPoint;

    private UiView activeView;
    private bool deactivateViewOverDistance;

    [Header("Interaction")]
    public GameObject interactionSignPref;

    [Header("Popups")]
    public GameObject infoPopupPref;
    public GameObject floatingTextPopupPref;
    public Transform popupContainer;

    [Header("HoverInfo")]
    public GameObject hoverInfoText;
    public Vector3 hoverInfoOffset;

    public enum PopupType
    {
        Info,
        FloatingText
    }

    private Transform player;
    private Transform cam;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if(FindObjectOfType<PlayerMovement_MLab>() != null)
            player = FindObjectOfType<PlayerMovement_MLab>().transform;

        cam = Camera.main.transform;

        HideAllViews();

        ShowIngameUi(ViewIngameUi.IngameUiType.Main);
    }

    private void Update()
    {
        if (interactionSign != null) RotateInteractionSign();
        if (deactivateViewOverDistance) CheckForPlayerDistance();
    }

    #region ViewHandling

    public void ShowView(UiView.Type viewType)
    {
        activeView = GetView(viewType);
        activeView.Show();

        FindObjectOfType<PlayerInput>().viewActive = true;

        if (activeView.stationary)
        {
            viewOpenPoint = player.position;
            deactivateViewOverDistance = true;
        }

        viewOpened = true;

        ShowCursor();
    }

    public void HideActiveView()
    {
        if (activeView == null) return;

        activeView.Hide();
        activeView = null;

        FindObjectOfType<PlayerInput>().viewActive = false;

        viewOpened = false;
        deactivateViewOverDistance = false;

        HideCursor();
    }

    public void HideAllViews()
    {
        for (int i = 0; i < allViews.Count; i++)
        {
            allViews[i].Hide();
        }
    }

    private void CheckForPlayerDistance()
    {
        float distance = Vector3.Distance(viewOpenPoint, player.position);
        if (distance > maxDistanceToViewOpenPoint)
        {
            HideActiveView();
            deactivateViewOverDistance = false;
        }
    }

    private void ShowCursor()
    {
        print("Cursor shown");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        FindObjectOfType<PlayerCam_MLab>().locked = true;
        PlayerReferences.instance.combat.locked = true;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        FindObjectOfType<PlayerCam_MLab>().locked = false;
        PlayerReferences.instance.combat.locked = false;
    }

    #endregion

    #region IngameUi

    // redirect function to viewIngameUi
    public void ShowIngameUi(ViewIngameUi.IngameUiType type)
    {
        FindObjectOfType<ViewIngameUi>().ShowIngameUi(type);
    }

    #endregion

    #region Interaction

    private GameObject interactionSign;
    public void PlaceInteractionSign(Transform interactionObject)
    {
        // remove old sign
        RemoveInteractionSign();

        // place new one
        interactionSign = Instantiate(interactionSignPref, interactionObject.position + Vector3.up * 1f, Quaternion.identity);
        interactionSign.transform.SetParent(interactionObject);
    }

    private void RotateInteractionSign()
    {
        interactionSign.transform.rotation = Quaternion.LookRotation(interactionSign.transform.position - cam.transform.position);
    }

    public void RemoveInteractionSign()
    {
        if (interactionSign != null)
        {
            Destroy(interactionSign);
            interactionSign = null;
        }
    }

    #endregion

    #region Popups

    public void ShowPopup(string content, string title = "Info", PopupType popupType = PopupType.Info)
    {
        if(popupType == PopupType.Info)
        {
            GameObject popupObj = Instantiate(infoPopupPref, popupContainer);
            InfoPopup infoPopup = popupObj.GetComponent<InfoPopup>();
            infoPopup.Setup(title, content);
        }
        else if(popupType == PopupType.FloatingText)
        {
            GameObject popupObj = Instantiate(floatingTextPopupPref, popupContainer);
            FloatingTextPopup floatingTextPopup = popupObj.GetComponent<FloatingTextPopup>();
            floatingTextPopup.Setup(content);
        }
        else
        {
            Debug.LogError("Popup type not implemented...");
        }
    }

    private GameObject currHoverPopup;
    public void ShowHoverInfo(string title, string content, Vector3 position)
    {
        currHoverPopup = Instantiate(hoverInfoText, popupContainer);
        currHoverPopup.transform.position = position + hoverInfoOffset;
        InfoPopup hoverInfo = currHoverPopup.GetComponent<InfoPopup>();
        hoverInfo.Setup(title, content);
    }

    public void HideHoverInfo()
    {
        Destroy(currHoverPopup);
        currHoverPopup = null;
    }

    #endregion

    #region Getters

    public UiView GetView(UiView.Type viewType)
    {
        // find and return correct view
        for (int i = 0; i < allViews.Count; i++)
        {
            if (allViews[i].type == viewType)
                return allViews[i];
        }

        Debug.LogError("View " + viewType + " could not be found");
        return null;
    }

    public bool IsViewActive(UiView.Type viewType)
    {
        return GetView(viewType).IsActive();
    }

    #endregion
}
