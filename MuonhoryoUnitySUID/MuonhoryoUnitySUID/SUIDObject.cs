

using UnityEngine;

namespace MuonhoryoLibrary.Unity.SUID
{

    [AddComponentMenu("")]
    public sealed class SUIDObject:MonoBehaviour
    {
        [SerializeField][HideInInspector] private int ID = -1;
        public int ID_
        {
            get => ID;
            set
            {
                if (Application.isPlaying)
                {
                    throw new System.Exception("Trying to change SUID without using SUIDEditorTools.");
                }
                ID = value;
            }
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                throw new  System.Exception("Destroying SUIDObject in play mode.");
            }
        }
    }
}
