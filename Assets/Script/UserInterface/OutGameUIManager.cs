using MFFrameWork.MFSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MFFrameWork
{
    public class OutGameUIManager : MonoBehaviour
    {

        Action _startButtonAction;

        void Start()
        {
            var startButton = transform.Find("StartButton").GetComponent<Button>();

            _startButtonAction += () => MainSystem.Instanse.LoadScene(SceneKind.InGame);
            Button.ButtonClickedEvent onClickEvent = new Button.ButtonClickedEvent();
            onClickEvent.AddListener(() => _startButtonAction?.Invoke());
            
            startButton.onClick = onClickEvent;
        }
    }
}
