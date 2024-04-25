
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MuonhoryoLibrary.Unity.SUID
{
    [AddComponentMenu("")]
    public sealed class SUIDManager : MonoBehaviour,ISingltone<SUIDManager>
    {
        public static SUIDManager Instance_ { get; private set; }

        //Singltone
        SUIDManager ISingltone<SUIDManager>.Singltone 
            { get => Instance_; set => Instance_ = value; }
        void ISingltone<SUIDManager>.Destroy() => Destroy(this);

        private void Initialization()
        {
            Instance_ = this;
            var objs = GameObject.FindObjectsOfType<SUIDObject>();
            ObjectsInScene = new List<SUIDObject>(objs);
            SortList();
        }
        private void Awake()
        {
            SingltoneInitializations.InitializationForFirstExample(this,
                Initialization);
        }
        private void OnDestroy()
        {
            if (Application.isPlaying)
                throw new Exception("Destroying SUIDManager in play mode.");
        }
        //Singltone

        private List<SUIDObject> ObjectsInScene =
            new List<SUIDObject>();
        public int InSceneObjectsCount_ => ObjectsInScene.Count;

        private void SortList()
        {
            Comparison<SUIDObject> comparison = (x, y) =>
            {
                if (x.ID_ > y.ID_) return 1;
                else if (x.ID_ < y.ID_) return -1;
                else
                    throw new Exception("Have more than one object by ID=" + x.ID_);
            };
            ObjectsInScene.Sort(comparison);
        }

        public SUIDObject GetObjectByID(int id)
        {
            if (id > 0 && id <= ObjectsInScene.Count)
            {
                return ObjectsInScene[id - 1];
            }
            else return null;
        }
    }
}