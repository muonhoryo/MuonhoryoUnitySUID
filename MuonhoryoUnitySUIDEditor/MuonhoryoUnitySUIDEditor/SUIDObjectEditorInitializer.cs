
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace MuonhoryoLibrary.Unity.SUID.Editor
{
    [ExecuteInEditMode]
    public sealed class SUIDObjectEditorInitializer:MonoBehaviour
    {
        private static int NextNum_ => SUIDEditorManager.Instance_.InSceneObjectsCount_ + 1;

        [SerializeField][HideInInspector] private SUIDObject Owner;
        [SerializeField][HideInInspector] private string SceneName;
        [SerializeField][HideInInspector] private bool IsInitialized = false;
        [SerializeField][HideInInspector] private int ID = -1;
        public int ID_
        {
            get => ID;
            internal set
            {
                ID = value;
                Owner.ID_ = value;
            }
        }
        internal SUIDObject Owner_ => Owner;

        private void Awake()
        {
            if (SUIDEditorManager.Instance_ != null)
            {
                var obj = SUIDEditorManager.Instance_.GetObjectByID(ID_);
                if (obj != null && obj != this && obj.SceneName == SceneName)
                {
                    Debug.LogError("Can't have more than one object with same SUID.");
                    DestroyImmediate(gameObject);
                    return;
                }
            }
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
                return;

            if (Owner != null &&
                gameObject.activeInHierarchy &&
                SUIDEditorManager.Instance_.ValidateScene())
            {
                DestroyImmediate(Owner);
            }
            gameObject.hideFlags = gameObject.hideFlags & ((HideFlags)int.MaxValue ^ HideFlags.NotEditable);

            if (SUIDEditorManager.Instance_ != null)
                SUIDEditorManager.Instance_.RemoveObject(ID);

        }
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;


            if (!IsInitialized) //Script just created and need to initialize
            {

                Initialization();
            }
            else if (Owner == null) //Script is already initialized but by some reason haven't owner
            {
                Owner = GetComponent<SUIDObject>();
            }

            if (Owner != null)
                ID = Owner.ID_;
        }
        private void Initialization()
        {
            hideFlags = HideFlags.DontSaveInBuild | HideFlags.NotEditable;
            gameObject.hideFlags = gameObject.hideFlags | HideFlags.NotEditable;
            SceneName = SceneManager.GetActiveScene().name;
            IsInitialized = true;

            if (HasEditorManager()) //editor manager is exists in scene
            {
                InitializeSUIDObject();
            }
            else    //there is no editor manager in scene temporally, initialization will executed by editor manager,
                    //when it created
            {
                Debug.LogError("Haven't SUIDEditorManager in scene. " +
                    "Script didn't initialized until there is no SUIDEditorManager in scene");
            }
        }
        internal void InitializeSUIDObject()
        {
            if (!TryGetComponent(out Owner))
            {
                Owner = gameObject.AddComponent<SUIDObject>();
                ID_ = NextNum_;
                Owner.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

                SUIDEditorManager.Instance_.AddObject(this);
            }
            else
            {
                if (!HasEditorManager())
                    Debug.LogWarning("There is no SUIDEditorManager in scene.");
                else if (SUIDEditorManager.Instance_.GetObjectByID(ID_) == null) //add owner to editor manager when scene just opened
                {
                    SUIDEditorManager.Instance_.AddObject(this);
                }
            }
        }
        internal void UpdateSceneName(string newName)
        {
            this.SceneName = newName;
        }
        private bool HasEditorManager()
        {
            return SUIDEditorManager.Instance_ != null;
        }




        [ContextMenu("DeleteSUID")]
        public void DeleteSUID()
        {
            DestroyImmediate(this);
        }
        internal void SetHideFlags(HideFlags flags)
        {
            HideFlags ownerFlags = Owner.hideFlags;
            gameObject.hideFlags = flags;
            Owner.hideFlags = ownerFlags;
        }
        [ContextMenu("Unsafe edit")]
        public void UnsafeEdit()
        {
            SetHideFlags(gameObject.hideFlags & ((HideFlags)int.MaxValue ^ HideFlags.NotEditable)); //Turn on editing
        }

        public static Comparison<SUIDObjectEditorInitializer> GetComparison()
        {
            return (x, y) =>
            {
                if (x.ID_ > y.ID_) return 1;
                else if (x.ID_ < y.ID_) return -1;
                else
                    throw new Exception("Have more than one object by ID=" + x.ID_);
            };
        }
    }
}
