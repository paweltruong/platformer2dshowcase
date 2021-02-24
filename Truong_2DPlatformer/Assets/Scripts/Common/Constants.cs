using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Constants
{
    public const string PlayerTag = "Player";

    public const float HelpMessage_ShortDuration = 1.5f;

    public class AnimationParameters
    {
        public const string Trigger_Jump = "Jump";
        public const string Trigger_LayDown = "LayDown";
        public const string Trigger_StandUp = "StandUp";

        public const string Param_Velocity = "Velocity";
        public const string Param_IsMoving = "IsMoving";
        public const string Param_IsGrounded = "IsGrounded";
        public const string Param_IsFrozen = "IsFrozen";


        public const string Trigger_Eat = "Eat";
        public const string Trigger_Fly = "Fly";

        public const string State_Character_Idle = "Character_Idle";
    }
}