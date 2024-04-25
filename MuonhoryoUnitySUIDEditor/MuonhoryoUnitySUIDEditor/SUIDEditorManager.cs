
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MuonhoryoLibrary.Unity.SUID.Editor
{
    [ExecuteInEditMode]
    public sealed class SUIDEditorManager:MonoBehaviour
    {
        internal static SUIDEditorManager Instance_ { get; private set; }

        [SerializeField][HideInInspector] private SUIDManager Owner;
        [SerializeField][HideInInspector] private bool IsInitialized = false;
        [SerializeField][HideInInspector] private string OwnedSceneName;
        [SerializeField][HideInInspector] private List<SUIDObjectEditorInitializer> ObjectsInScene = null;

        public int InSceneObjectsCount_ => ObjectsInScene.Count;
        private void Awake()
        {
            var objs = GameObject.FindObjectsOfType<SUIDEditorManager>();
            if (objs.Length > 1)
            {
                Debug.LogError("Can't place more than one SUIDManager in scene.");
                DestroyImmediate(this);
            }
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
                return;

            gameObject.hideFlags = HideFlags.None;
            if (Owner != null &&
                gameObject.activeInHierarchy &&
                ValidateScene())
            {
                DestroyImmediate(Owner);
            }
        }
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            Instance_ = this;
            OwnedSceneName = SceneManager.GetActiveScene().name;
            if (!IsInitialized)
            {
                IsInitialized = true;
                ObjectsInScene = new List<SUIDObjectEditorInitializer>();
                gameObject.hideFlags =gameObject.hideFlags |HideFlags.NotEditable;
                RunSUIDObjDelayedInitializations();
            }
            else
            {
                UpdateSceneNameForSUIDObjs();
            }
            if (Owner == null)
            {
                if (!TryGetComponent(out Owner))
                {
                    Owner = gameObject.AddComponent<SUIDManager>();
                }
            }
        }
        internal void RunSUIDObjDelayedInitializations()
        {
            Comparison<SUIDObjectEditorInitializer> comparison = (x, y) =>
            {
                if (x.ID_ < 0) return 1;
                else return SUIDObjectEditorInitializer.GetComparison()(x, y);
            };
            var objs = new List<SUIDObjectEditorInitializer>
                (GameObject.FindObjectsOfType<SUIDObjectEditorInitializer>());
            objs.Sort(comparison);
            foreach (var obj in objs)
                obj.InitializeSUIDObject();
        }
        private void UpdateSceneNameForSUIDObjs()
        {
            foreach (var obj in ObjectsInScene)
                obj.UpdateSceneName(OwnedSceneName);
        }

        internal SUIDObjectEditorInitializer GetObjectByID(int id)
        {
            if (id > 0 && id <= ObjectsInScene.Count)
            {
                return ObjectsInScene[id - 1];
            }
            else return null;
        }
        public bool ValidateScene()
        {
            return SceneManager.GetActiveScene().name == OwnedSceneName;
        }

        private void SortList()
        {
            ObjectsInScene.Sort(SUIDObjectEditorInitializer.GetComparison());
        }
        internal void AddObject(SUIDObjectEditorInitializer obj)
        {
            if (ObjectsInScene.Contains(obj))
                throw new Exception("SUIDObject \"" + obj + "\" is already contained in object's list.");

            ObjectsInScene.Add(obj);
            SortList();
        }
        internal void RemoveObject(int id)
        {
            if (id > InSceneObjectsCount_ || id < 1)
                throw new IndexOutOfRangeException("Haven't object by ID=" + id);

            ObjectsInScene.RemoveAt(id - 1);

            for (int i = id - 1; i < InSceneObjectsCount_; i++)
            {
                ObjectsInScene[i].ID_--;
            }
            SortList();
        }


        [ContextMenu("DeleteManager")]
        public void DeleteManager()
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
    }
}
