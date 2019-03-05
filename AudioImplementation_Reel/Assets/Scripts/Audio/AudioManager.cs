using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    public GameObject playerGO;


    private AudioSource ambiance2DSource;
    //State
    public enum location
    { 
        village,
        forest
    }

    private enum houses
    {
        tavern,
        quietHouse,
        forge
    }

    private location currentLocation = location.village;
    //Ambiance Zones
    private class zoneClass
    {
        public GameObject zoneGameObject;
        public GameObject sourceObj;
        public AudioSource sourceForAmbiance;
        public Renderer zoneRenderer;
        public Ambiance ambianceAudio;

    }
    private Dictionary<location, zoneClass> allZones = new Dictionary<location, zoneClass>();


    //ForestAnimalStuff
    private Dictionary<location, List<ForestAnimals>> soundsBasedOnLocation = new Dictionary<location, List<ForestAnimals>>();
    private RangedFloat rangeOfTimeForRandomizedAmb;
    private class RandomizerGameObjects
    {
        public AudioSource source;
        public GameObject gameObjectOfFAS;
        public OcclusionCalculation occlusionCalculation;
    }
    private Queue<RandomizerGameObjects> spatialRandomizerSources = new Queue<RandomizerGameObjects>();

    private void Awake()
    {
        if (AudioManager.Instance == null) Instance = this;
        else Destroy(gameObject);
        AudioInitialization();
    }

    private void Start()
    {
        AudioStart();
    }

    private void AudioInitialization()
    {
        //Zone Init
        zoneClass forestZone = new zoneClass();
        zoneClass villageZone = new zoneClass();
        playerGO = GameObject.Find("Camera");
        villageZone.zoneGameObject = GameObject.Find("VillageAudioZone");
        villageZone.zoneRenderer = villageZone.zoneGameObject.GetComponent<Renderer>();
        villageZone.ambianceAudio = Resources.Load<Ambiance>("Audio/SFX/Amb/Village");
        villageZone.sourceObj = Instantiate(Resources.Load<GameObject>("Prefab/AmbianceObj"));
        villageZone.sourceForAmbiance = villageZone.sourceObj.GetComponent<AudioSource>();
        allZones.Add(location.village, villageZone);

        forestZone.zoneGameObject = GameObject.Find("ForestAudioZone");
        forestZone.zoneRenderer = forestZone.zoneGameObject.GetComponent<Renderer>();
        forestZone.ambianceAudio = Resources.Load<Ambiance>("Audio/SFX/Amb/Forest");
        forestZone.sourceObj = Instantiate(Resources.Load<GameObject>("Prefab/AmbianceObj"));
        forestZone.sourceForAmbiance = forestZone.sourceObj.GetComponent<AudioSource>();
        allZones.Add(location.forest, forestZone);

        ambiance2DSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < 4; i++)
        {
            RandomizerGameObjects forestAnimal = new RandomizerGameObjects();
            forestAnimal.gameObjectOfFAS = Instantiate(Resources.Load<GameObject>("Prefab/PointSource"));
            forestAnimal.source = forestAnimal.gameObjectOfFAS.GetComponent<AudioSource>();
            forestAnimal.occlusionCalculation = forestAnimal.gameObjectOfFAS.GetComponent<OcclusionCalculation>();
            spatialRandomizerSources.Enqueue(forestAnimal);
        }

        List<ForestAnimals> allForestAnimals = new List<ForestAnimals>();
        foreach (ForestAnimals sfx in Resources.LoadAll<ForestAnimals>("Audio/SFX/ForestAnimals"))
        {
            sfx.Init();
            allForestAnimals.Add(sfx);
        }
        soundsBasedOnLocation.Add(location.forest, allForestAnimals);

        List<ForestAnimals> allVillageRandoms = new List<ForestAnimals>();
        foreach (ForestAnimals sfx in Resources.LoadAll<ForestAnimals>("Audio/SFX/VillageRandom"))
        {
            sfx.Init();
            allVillageRandoms.Add(sfx);
        }
        soundsBasedOnLocation.Add(location.village, allVillageRandoms);

    }

    private void AudioStart ()
    {
        allZones[currentLocation].ambianceAudio.Play(allZones[currentLocation].sourceForAmbiance);
        StartCoroutine(StartSpatializedAmbiance());
    }

    private IEnumerator StartSpatializedAmbiance()
    {
        RandomizerGameObjects tempSource = spatialRandomizerSources.Dequeue();
        spatialRandomizerSources.Enqueue(tempSource);

        yield return new WaitForSecondsRealtime(Random.Range(0, 5));
        tempSource.gameObjectOfFAS.transform.position = RandomizePositionAroundPlayer();
        soundsBasedOnLocation[currentLocation][Random.Range(0, soundsBasedOnLocation[currentLocation].Count)].Play(tempSource.source);
        StartCoroutine(StartSpatializedAmbiance());
    }

    private Vector3 RandomizePositionAroundPlayer ()
    {
        Vector3 vectorToReturn = new Vector3(0, 0, 0);

        vectorToReturn.x = Random.Range(10, 30) * RandomSign();
        vectorToReturn.y = Mathf.Max(Random.Range(0, 10), Random.Range(0, 10));
        vectorToReturn.z = Random.Range(10, 30) * RandomSign(); 
        vectorToReturn = vectorToReturn + playerGO.transform.position;
        return vectorToReturn;
    }

    private float RandomSign ()
    {
        if (Random.Range(0, 99) >= 50)
        {
            return -1;
        }
        else return 1;

    }

    public void CaculateOcclusionAndConeAttenuation (GameObject objectSource, AudioLowPassFilter lowPassForOcclusion, AudioSource sourceForOcclusion)
    {
        bool isOccluded = false;
        CalculateOcclusionOnly(objectSource, lowPassForOcclusion, sourceForOcclusion, isOccluded);

        float angle = Vector3.Angle((playerGO.transform.position - objectSource.transform.position), objectSource.transform.forward);
        Debug.DrawRay(objectSource.transform.position, (playerGO.transform.position - objectSource.transform.position));
        if (angle > 120)
        {
            sourceForOcclusion.volume /= 2.5f;
            lowPassForOcclusion.cutoffFrequency = ReturnLowPassFreq(isOccluded, true);
        }
        else if (angle > 70)
        {
            sourceForOcclusion.volume /= 1.6f;
            lowPassForOcclusion.cutoffFrequency = ReturnLowPassFreq(isOccluded, false);
        }

    }

    private float ReturnLowPassFreq (bool isOccluded, bool highAngle)
    {
        if (isOccluded)
        {
            if (highAngle)
            {
                return 2500;
            }
            return 3500;
        }
        else
        {
            if (highAngle)
            {
                return 14000;
            }
            return 16000;
        }

    }


    public void CalculateOcclusionOnly (GameObject objectSource, AudioLowPassFilter lowPassForOcclusion, AudioSource sourceForOcclusion)
    {
        Ray ray = new Ray(objectSource.transform.position, (playerGO.transform.position - objectSource.transform.position));
        RaycastHit hit;
        Debug.DrawRay(objectSource.transform.position, (playerGO.transform.position - objectSource.transform.position), Color.green);
        if (Physics.Raycast(ray, out hit, 30))
        {
            if (hit.collider.tag != "Player")
            {
                lowPassForOcclusion.cutoffFrequency = 5000;
                sourceForOcclusion.volume = .5f;
            }
            else
            {
                lowPassForOcclusion.cutoffFrequency = 22000;
                sourceForOcclusion.volume = 1f;
            }

        }
        else
        {
            lowPassForOcclusion.cutoffFrequency = 22000;
            sourceForOcclusion.volume = 1f;
        }

    }

    public void CalculateOcclusionOnly(GameObject objectSource, AudioLowPassFilter lowPassForOcclusion, AudioSource sourceForOcclusion, bool returnIfOccluded)
    {
        Ray ray = new Ray(objectSource.transform.position, (playerGO.transform.position - objectSource.transform.position));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 30))
        {
            if (hit.collider.tag != "Player")
            {
                lowPassForOcclusion.cutoffFrequency = 5000;
                sourceForOcclusion.volume = .5f;
                returnIfOccluded = true;
            }
            else
            {
                lowPassForOcclusion.cutoffFrequency = 22000;
                sourceForOcclusion.volume = 1f;
            }

        }
        else
        {
            lowPassForOcclusion.cutoffFrequency = 22000;
            sourceForOcclusion.volume = 1f;
        }

    }

    private void Update()
    {
        UpdateClosestZone();
        if (Input.GetKeyDown(KeyCode.A))
        {
            Resources.Load<Ambiance>("Audio/SFX/Amb/Forest").Play(gameObject.AddComponent<AudioSource>());
        }
    }

    private void UpdateClosestZone()
    {
        float distance = 100;
        switch (currentLocation)
        {
            case (location.forest):
                allZones[location.village].sourceObj.transform.position = allZones[location.village].zoneRenderer.bounds.ClosestPoint(playerGO.transform.position);
                distance = Vector3.Distance (allZones[location.village].sourceObj.transform.position, playerGO.transform.position);
                AmbianceProxLogic(allZones[location.village], distance);
                break;
            case (location.village):
                allZones[location.forest].sourceObj.transform.position = allZones[location.forest].zoneRenderer.bounds.ClosestPoint(playerGO.transform.position);
                distance = Vector3.Distance(allZones[location.forest].sourceObj.transform.position, playerGO.transform.position);
                AmbianceProxLogic(allZones[location.forest], distance);
                break;
        }

    }

    private void AmbianceProxLogic (zoneClass zoneToUpdate, float distance)
    {
        if (distance < 40)
        {
            if (!zoneToUpdate.sourceForAmbiance.isPlaying)
            {
                zoneToUpdate.sourceForAmbiance.Play();
            }
            zoneToUpdate.sourceForAmbiance.spatialize = true;
            zoneToUpdate.sourceForAmbiance.spatialBlend = Mathf.Pow(distance/40, 1/2);
        }
        else if (distance > 40 && zoneToUpdate.sourceForAmbiance.isPlaying)
        {
            zoneToUpdate.sourceForAmbiance.Stop();
            return;
        }
    }


    public void CurrentLocation_Forest ()
    {
        currentLocation = location.forest;
        allZones[location.forest].sourceForAmbiance.spatialBlend = 0;
    }

    public void CurrentLocation_Village()
    {
        currentLocation = location.village;
        allZones[location.village].sourceForAmbiance.spatialBlend = 0;
    }
}
