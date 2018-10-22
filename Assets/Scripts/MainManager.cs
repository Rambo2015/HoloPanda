using HoloToolkit.Unity;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class MainManager : MonoBehaviour
{
    
    private bool m_isInitialized;
    [SerializeField]
    private float kMinAreaForComplete = 20.0f;
    [SerializeField]
    private float kMinHorizontalAreaForComplete = 10.0f;
    [SerializeField]
    private float kMinWallAreaForComplete = 5.0f;

    [SerializeField] private Panda _pandaPrefab;
    [SerializeField] private GameObject _bambooPrefab;    

    private Panda _activePanda;    

    private Vector3 _floorPosition = Vector3.zero;
    private HeadsUpDirectionIndicator _headsUpIndicator;

    private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[512];

    void Awake()
    {
        Assert.IsNotNull(_pandaPrefab);
        Assert.IsNotNull(_bambooPrefab);
    }

    void Start()
    {
        _headsUpIndicator = GameObject.FindObjectOfType<HeadsUpDirectionIndicator>();        
        SpatialUnderstanding.Instance.ScanStateChanged += Instance_ScanStateChanged;        
    }

    //private void OnTappedEvent(TappedEventArgs tappedEventArgs)
    //{
    //    if (_villain == null) { return; }
    //    _villain.KillVictim();
    //    DestroyNPCs();
    //    if (_floorPosition != Vector3.zero) { InstantiateNPCs(_floorPosition); }
    //}

    private void Instance_ScanStateChanged()
    {
        if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done) &&
    SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
        {
            FinishSetup();
        }
    }

    void Update()
    {
        // check if enough of the room is scanned
        if (!m_isInitialized && DoesScanMeetMinBarForCompletion)
        {
            // let service know we're done scanning
            SpatialUnderstanding.Instance.RequestFinishScan();
            m_isInitialized = true;
        }
    }

    public bool DoesScanMeetMinBarForCompletion
    {
        get
        {
            // Only allow this when we are actually scanning
            if ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) ||
                (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding))
            {
                return false;
            }

            // Query the current playspace stats
            IntPtr statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
            if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
            {
                return false;
            }
            SpatialUnderstandingDll.Imports.PlayspaceStats stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();

            // Check our preset requirements
            if ((stats.TotalSurfaceArea > kMinAreaForComplete) ||
                (stats.HorizSurfaceArea > kMinHorizontalAreaForComplete) ||
                (stats.WallSurfaceArea > kMinWallAreaForComplete))
            {
                return true;
            }
            return false;
        }
    }

    private void FinishSetup()
    {
        // use spatial understanding to find floor
        SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
        SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();

        // find large floor area
        IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);
        int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(
            resultsTopology.Length, resultsTopologyPtr);

        // hide mesh
        var customMesh = SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>();
        customMesh.DrawProcessedMesh = false;

        // find some topology data
        Vector3 floorPosition;

        if (locationCount > 0)
        {
            // retrieve a large floor area from spatial understanding
            floorPosition = resultsTopology[0].position;
            _floorPosition = floorPosition;
        }
        else
        {
            // just get floor in front of the player
            var inFrontOfCamera = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
            floorPosition = new Vector3(inFrontOfCamera.x, alignment.FloorYValue, 2.69f);
        }
        //if (_floorObject != null)
        {            
            InstantiateNPCs(floorPosition);
        }
    }

    private void InstantiateNPCs(Vector3 floorPosition)
    {
        if (_pandaPrefab != null)
        {
            _activePanda = Instantiate(_pandaPrefab.gameObject, floorPosition, Quaternion.identity).GetComponent<Panda>();

            if (_headsUpIndicator != null)
            {
                _headsUpIndicator.gameObject.SetActive(true);
                Debug.Log("HUD Indicator active");
                _headsUpIndicator.TargetObject = _activePanda.gameObject;
            }
        }
    }

    private void DestroyNPCs()
    {
        if (_activePanda.gameObject != null)
        {
            Destroy(_activePanda.gameObject);
        }        
    }

    private void HideNPCs()
    {        
        if (_activePanda) DestroyImmediate(_activePanda);        
    }

    public void OnReset()
    {
        HideNPCs();
        InstantiateNPCs(_floorPosition);
    }
}
