using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A GameController which includes pre-built smooth changing capabilities between Scenes.
/// </summary>
[RequireComponent(typeof(Animator))]
public class GameController : MonoBehaviour
{
    [Tooltip("Is this GameController part of a scene that is a menu or gameplay?")]
    [SerializeField] private bool IsMenu = false;

    #region FadeInOut fields

    [Tooltip("Build-index of the scene this scene returns (loads) on exit. -1 to exit.")]
    [SerializeField] private int ReturnToScene = 0;

    [Tooltip("Do we use fade-in and fade-out during scene changes and exit?")]
    [SerializeField] private bool UseFades = true;

    private Animator FadeAnimator;

    private static readonly string AnimatorsChangeTrigger = "SceneChange";

    private int NextScene;
    [Space]

    #endregion

    #region SaveLoad fields

    [Header("SaveLoad system control")]

    [Tooltip("Button which can be pressed to continue the game where it left off. Only displayed if old save data exists.")]
    [SerializeField] Button ContinueButton;

    [Tooltip("Disable this on menus and enable on actual game levels. Only works on non-menu GameController.")]
    [SerializeField] private bool LoadSaveOnSceneLoad = false;

    [Tooltip("Do we automatically save the game on exit? Only works on non-menu GameController.")]
    [SerializeField] private bool SaveOnExit = false;
    
    #endregion


    //Root folder of save files.
    private static readonly string SAVE_ROOT = "Saves/";
    private static readonly string SAVEFILE = "savefile";

    private SaveableLoadable[] CurrentSaveableObjects;

    private static SaveData CurrentSave;
    private static bool IsInitialized = false;


    private void Start()
    {
        FadeAnimator = GetComponent<Animator>();

        if (!IsInitialized)
        {
            Debug.Log("GameController not yet initialized");

            //As the data is stored in a static field, it needs to be loaded only once.
            CurrentSave = GenericSerializer.DeserializeReferenceType<SaveData>(SAVE_ROOT, SAVEFILE);

            //DEBUG
            if (CurrentSave == null)
            {
                Debug.Log("No current save data exists");
            }
            else
            {
                if(CurrentSave.Data?.Count == 0)
                {
                    Debug.Log("Savedata is loaded, but no previous save data exists");
                }
                else
                {
                    Debug.Log("Savedata is loaded, previous sava data exists");
                }
            }
            IsInitialized = true;
        }
        else
        {
            Debug.Log("GameController is already initialized");
        }

        //List of all active GameObjects with SaveableLoadable component.
        CurrentSaveableObjects = FindObjectsOfType<SaveableLoadable>();
        if (CurrentSaveableObjects.Length == 0)
            Debug.Log("No objects are saved.");
        else
            foreach (var obj in CurrentSaveableObjects)
                Debug.Log($"'{obj.name}' is saved.");

        /*
         * TODO: load saved data if the flag exists.
         */

        if (LoadSaveOnSceneLoad)
            LoadGame();

        if (IsMenu)
        {
            if (ContinueButton == null)
                Debug.LogError("GameController needs to have access to a Continue-button if it is marked as a menu.");
            else
            {
                if (CurrentSave != null)
                    ContinueButton.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            if (SceneManager.GetActiveScene().buildIndex != ReturnToScene)
                LoadScene(ReturnToScene);
        }
    }


    /* Non-monobehaviour */


    public void LoadScene(int sceneBuildIndex)
    {
        NextScene = sceneBuildIndex;
        if(UseFades)
            FadeAnimator.SetTrigger(AnimatorsChangeTrigger);
        else
            LoadNextScene();
    }

    public void Exit() => LoadScene(-1);

    //This is called by the Animator at the end of fade, therefore public.
    public void LoadNextScene()
    {
        if (NextScene == SceneManager.GetActiveScene().buildIndex)
        {
            Debug.LogWarning($"Trying to load the same scene '{NextScene}' again. Is this wanted behaviour?");
            return;
        }
        else
        {
            if (SaveOnExit)
                SaveGame();
            if (NextScene < 0)
            {
                Application.Quit();
            }
            else
                SceneManager.LoadScene(NextScene);
        }
    }

    public void NewGame(int buildindex)
    {
        CurrentSave = null;
        LoadScene(buildindex);
    }

    public void LoadGame()
    {
        foreach (var obj in CurrentSaveableObjects)
        {
            if (CurrentSave?.Data != null)
            {
                if (CurrentSave.Data.TryGetValue(obj.name, out Dictionary<string, SerializableData> data))
                    obj.Load(data);
            }
            else
                obj.LoadDefault();
        }
    }

    //This has side-effects (compare down)
    public void SaveGame()
    {
        CurrentSave = CreateSaveData();
        GenericSerializer.Serialize(CurrentSave, SAVE_ROOT, SAVEFILE);
    }

    //This doesn't have side-effects (compare up)
    private SaveData CreateSaveData()
    {
        //Object level dictionary
        var objdata = new Dictionary<string, Dictionary<string, SerializableData>>();
        foreach (var obj in CurrentSaveableObjects)
        {
            //name of the GameObject
            Debug.Log(obj.name);

            //Component level dictionary
            var compdata = new Dictionary<string, SerializableData>();
            foreach (var comp in obj.GetSaveableComponents())
            {
                //name of the component
                Debug.Log(comp.GetType().Name);
                compdata.Add(comp.GetType().Name.ToString(), comp.GetData());
            }
            objdata.Add(obj.name, compdata);
        }
        return new SaveData(SceneManager.GetActiveScene().buildIndex, objdata);
    }


    private void ToggleContinueButton() => ContinueButton?.gameObject.SetActive(true);

    [System.Serializable]
    private class SaveData
    {
        public readonly int SceneIndex;
        public readonly Dictionary<string, Dictionary<string, SerializableData>> Data;

        public SaveData(int index, Dictionary<string, Dictionary<string, SerializableData>> data)
        {
            this.SceneIndex = index;
            this.Data = data;
        }
    }
}
