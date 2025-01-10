using MFFrameWork.Utilities;
using UnityEngine;

namespace MFFrameWork.MFSystem
{
    public class MainSystem : SingletonMonoBehaviour<MainSystem>
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void HideCursor(bool frag)
        {
            Cursor.visible = frag;
            if (frag)
                Cursor.lockState = CursorLockMode.None;
            else 
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
