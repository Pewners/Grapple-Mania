using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class DebugExtensionManager : MonoBehaviour
{
    public static DebugExtensionManager instance;

    private Transform tr;

    [Header("Markers")]
    private GameObject markerPref;
    private List<GameObject> activeMarkers;
    public float markerLifetime = 1f;

    [Header("Line Renderers")]
    private GameObject lineRendererPref;
    private List<LineRendererData> activeLines;

    [Header("Popups")]
    private GameObject popupPref_debug;
    private GameObject popupPref_damage;
    private List<GameObject> activePopups;
    public float popupLifetime = 1f;

    private List<PopupData> popupQueue;
    public float timeBetweenPopups = 0.1f;
    private float nextPopupTimer;

    [Header("Damage Popups")]
    public float popupOffsetForward = 1f;
    public float popupOffsetRight = 1f;
    public float popupOffsetUpward = 2f;

    public enum PopupType
    {
        Debug,
        Damage
    }

    private GameObject player;

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

        tr = transform;
        activeMarkers = new List<GameObject>();
        activeLines = new List<LineRendererData>();
        activePopups = new List<GameObject>();
        popupQueue = new List<PopupData>();
    }

    private void Start()
    {
        LoadReferences();
    }

    private void Update()
    {
        if(popupQueue.Count > 0)
        {
            float timerBoost = Time.deltaTime * popupQueue.Count * 0.3f;

            nextPopupTimer -= Time.deltaTime + timerBoost;

            if(nextPopupTimer <= 0)
            {
                SpawnPopupReal(popupQueue[0]);
                popupQueue.RemoveAt(0);
                nextPopupTimer = timeBetweenPopups;
            }
        }
    }

    private void LoadReferences()
    {
        if (Resources.Load<GameObject>("Marker") != null)
            markerPref = Resources.Load<GameObject>("Marker");
        else
            Debug.LogError("MarkerPref could not be found! Markers cannot be spawned.");

        if (Resources.Load<GameObject>("LineRendererPref") != null)
            lineRendererPref = Resources.Load<GameObject>("LineRendererPref");
        else
            Debug.LogError("LineRendererPref could not be found! Line Renderers cannot be spawned.");

        if (Resources.Load<GameObject>("Popup_Debug") != null)
            popupPref_debug = Resources.Load<GameObject>("Popup_Debug");
        else
            Debug.LogError("DebugPopup could not be found! Debug Popups cannot be spawned.");

        if (Resources.Load<GameObject>("Popup_Damage") != null)
            popupPref_damage = Resources.Load<GameObject>("Popup_Damage");
        else
            Debug.LogError("DamagePopup could not be found! Damage Popups cannot be spawned.");

        if (PlayerReferences.instance.gameObject != null)
            player = PlayerReferences.instance.gameObject;
        else
            Debug.LogError("Player could not be found! Damage Popups won't work correctly.");
    }

    public void SpawnPopup(Vector3 worldPosition, PopupType popupType, string content, Color color = default, bool dontDissappear = false)
    {
        PopupData data = new PopupData();
        data.worldPosition = worldPosition;
        data.popupType = popupType;
        data.content = content;
        data.dontDissappear = dontDissappear;
        data.color = color == default ? Color.red : color;
        popupQueue.Add(data);
    }

    private void SpawnPopupReal(PopupData data)
    {
        GameObject popupObj = null;

        if (data.popupType == PopupType.Debug) popupObj = Instantiate(popupPref_debug, data.worldPosition, Quaternion.identity);
        if (data.popupType == PopupType.Damage)
        {
            popupObj = Instantiate(popupPref_damage, data.worldPosition, Quaternion.identity);

            popupObj.transform.LookAt(player.transform);
            popupObj.transform.position = data.worldPosition + popupObj.transform.right * -popupOffsetRight
                                                        + popupObj.transform.forward * popupOffsetForward
                                                        + popupObj.transform.up * popupOffsetUpward;

            // flip popup
            popupObj.transform.localRotation = Quaternion.Euler(0, popupObj.transform.localRotation.eulerAngles.y + 180f, 0);
        }

        Popup popup = popupObj.GetComponent<Popup>();

        popup.Setup(data.content, data.color);

        activePopups.Add(popupObj);
        Invoke(nameof(DestroyNextPopup), 1f);
    }

    private void DestroyNextPopup()
    {
        ///Destroy(activePopups[0]); handled by Popup script
        activePopups.RemoveAt(0);
    }

    public void PlaceMarker(Vector3 worldPosition, float scale, string color = "red", bool dontDissappear = false)
    {
        // perform some safety checks
        GameObject marker = null;

        if(markerPref != null)
        {
            marker = Instantiate(markerPref, worldPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Couldn't find object to instantiate!");
            return;
        }

        marker.transform.SetParent(tr);
        marker.transform.localScale = Vector3.one * scale;

        // set color
        marker.GetComponent<MeshRenderer>().material = GetColorMaterial(color);

        activeMarkers.Add(marker);
        Invoke(nameof(DestroyNextMarker), markerLifetime);
    }

    private void DestroyNextMarker()
    {
        Destroy(activeMarkers[0]);
        activeMarkers.RemoveAt(0);
    }

    public void CreateLine(string lineIdentifier, Vector3 startPos, Vector3 endPos, string color = "red", bool displayAsGizmo = false)
    {
        print("DebugManager: creating line... " + lineIdentifier + " " + startPos + " " + endPos);

        LineRenderer lineRenderer = null;

        if (!displayAsGizmo)
            lineRenderer = Instantiate(lineRendererPref).GetComponent<LineRenderer>();

        LineRendererData lineRendererData = new LineRendererData(lineIdentifier, displayAsGizmo, lineRenderer, GetColor(color), startPos, endPos);

        if(!displayAsGizmo)
            lineRendererData.lineRenderer.material = GetColorMaterial(color);

        activeLines.Add(lineRendererData);
        UpdateLine(lineIdentifier, startPos, endPos);

        print("DebugManager: created new line");
    }

    public void UpdateLine(string lineIdentifier, Vector3 startPos, Vector3 endPos)
    {
        LineRendererData data = GetLineRendererData(lineIdentifier);

        if (data == null)
            return;

        if (!data.displayAsGizmo)
        {
            data.lineRenderer.positionCount = 2;
            data.lineRenderer.SetPosition(0, startPos);
            data.lineRenderer.SetPosition(1, endPos);
        }
        else
        {
            data.startPoint = startPos;
            data.endPoint = endPos;
        }
    }

    public void DeleteLine(string lineIdentifier)
    {
        for (int i = 0; i < activeLines.Count; i++)
        {
            if (activeLines[i].identifier == lineIdentifier)
            {
                Destroy(activeLines[i].lineRenderer.gameObject);
                activeLines.RemoveAt(i);
            }
        }
    }

    private LineRendererData GetLineRendererData(string identifier)
    {
        for (int i = 0; i < activeLines.Count; i++)
        {
            if (activeLines[i].identifier == identifier)
                return activeLines[i];
        }

        return null;
    }

    #region Draw Gizmos

    private void OnDrawGizmos()
    {
        // draw lines
        if(activeLines != null)
        {
            for (int i = 0; i < activeLines.Count; i++)
            {
                if (activeLines[i].displayAsGizmo)
                {
                    LineRendererData data = activeLines[i];
                    Gizmos.color = data.color;
                    Gizmos.DrawLine(data.startPoint, data.endPoint);
                }
            }
        }
    }

    #endregion

    #region Getters

    public Material GetColorMaterial(string color)
    {
        string colorMaterialName = "Mat_" + char.ToUpper(color[0]) + color.Substring(1);
        Material mat = null;
        if (Resources.Load<Material>(colorMaterialName) != null)
        {
            mat = Resources.Load<Material>(colorMaterialName);
            return mat;
        }
        else
        {
            Debug.LogError("Color could not be found");
            return null;
        }
    }

    public Color GetColor(string color)
    {
        if (color == "red") return Color.red;
        else if (color == "blue") return Color.blue;
        else if (color == "green") return Color.green;
        else if (color == "yellow") return Color.yellow;
        else if (color == "black") return Color.black;
        else if (color == "white") return Color.white;
        else return Color.white;
    }

    #endregion

    private class PopupData
    {
        public Vector3 worldPosition;
        public PopupType popupType;
        public string content;
        public bool dontDissappear;
        public Color color;
    }

    private class LineRendererData
    {
        public string identifier;
        public bool displayAsGizmo;
        public LineRenderer lineRenderer;
        public Color color;
        public Vector3 startPoint;
        public Vector3 endPoint;

        public LineRendererData(string identifier, bool displayAsGizmo, LineRenderer lineRenderer, Color color, Vector3 startPoint, Vector3 endPoint)
        {
            this.identifier = identifier;
            this.displayAsGizmo = displayAsGizmo;
            this.lineRenderer = lineRenderer;
            this.color = color;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }
    }
}
