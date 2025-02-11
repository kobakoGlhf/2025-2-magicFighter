using System.Collections.Generic;
using UnityEngine;

namespace MFFrameWork
{
    public class CharacterAnimation
    {
        static readonly Dictionary<AnimationKind, string> ClipName = new Dictionary<AnimationKind, string>()
        {
            {AnimationKind.Move, "Move" },
            {AnimationKind.Jump, "Jump" },
            {AnimationKind.Attack, "Attack" }
        };
        static readonly Dictionary<AnimationPropertys, string> PropertysName = new Dictionary<AnimationPropertys, string>()
        {
            {AnimationPropertys.MoveSpeed,"MoveSpeed" },
            {AnimationPropertys.MoveX, "MoveX" },
            {AnimationPropertys.MoveY, "MoveY" },
            {AnimationPropertys.IsGround, "IsGround" },
            {AnimationPropertys.AttackTrigger,"AttackTrigger" },
            {AnimationPropertys.DamageTrigger,"DamageTrigger" },
            {AnimationPropertys.JumpTrigger,"JumpTrigger" },
            {AnimationPropertys.DushTrigger,"DushTrigger" },
            {AnimationPropertys.IsLookMode, "IsLook" }
        };

        Animator _animator;
        public void SetAnimator(Animator animator) => _animator = animator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="changeMod"></param>
        /// <param name="duration">アニメーションの遷移時間。ChangeModeがCrossFadeの時のみ使用される</param>
        public void AnimationChange(AnimationKind kind, ChangeAnimation changeMod, float duration = 1)
        {
            switch (changeMod)
            {
                case ChangeAnimation.Defalt:
                    _animator.Play(ClipName[kind]);
                    break;
                case ChangeAnimation.CrossFade:
                    _animator.CrossFade(ClipName[kind], duration);
                    break;
            }
        }
        public void SetFloat(AnimationPropertys kind, float value)
        {
            _animator.SetFloat(PropertysName[kind], value);
        }
        public void SetTrigger(AnimationPropertys kind)
        {
            _animator.SetTrigger(PropertysName[kind]);
        }
        public void SetBool(AnimationPropertys kind, bool frag)
        {
            _animator.SetBool(PropertysName[kind], frag);
        }
    }
    public enum ChangeAnimation
    {
        Defalt,
        CrossFade
    }
    public enum AnimationKind
    {
        Move,
        Jump,
        Attack
    }
    public enum AnimationPropertys
    {
        MoveSpeed,
        MoveX,
        MoveY,

        IsGround,
        IsLookMode,

        AttackTrigger,
        DamageTrigger,
        JumpTrigger,
        DushTrigger
    }
}
