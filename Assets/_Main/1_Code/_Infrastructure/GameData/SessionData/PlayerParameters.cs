using System.Collections.Generic;

public class PlayerParameters : GameParametersData
{
    public IGameParameter<float> Health => _playerHealth;
    public IGameParameter<float> Speed => _pmSpeed;
    public IGameParameter<float> SprintSpeed => _pmSprintspeed;
    public IGameParameter<float> CrouchSpeed => _pmCrouchspeed;
    public IGameParameter<float> MaxAcceleration => _pmMaxacceleration;
    public IGameParameter<float> MaxAirAcceleration => _pmMaxairacceleration;
    public IGameParameter<float> JumpHeight => _pmJumpheight;
    public IGameParameter<float> CrouchHeight => _pmCrouchheight;
    public IGameParameter<int> MaxAirJumps => _pmMaxairjumps;
    public IGameParameter<float> MaxGroundAngle => _pmMaxgroundangle;
    public IGameParameter<float> MaxStairsAngle => _pmMaxstairsangle;
    public IGameParameter<float> MaxSnapSpeed => _pmMaxsnapspeed;
    public IGameParameter<float> ProbeDistance => _pmProbedistance;
    public IGameParameter<float> SafeFallSpeed => _pmSafefallspeed;
    public IGameParameter<float> FallDamageMultiplier => _pmFalldamagemultiplier;
    public IGameParameter<bool> NoClip => _pmNoclip;

    private readonly GameParameterFloat _playerHealth = new("player_health", 100.0f, 0f, 100f);
    private readonly GameParameterFloat _pmSpeed = new("pm_speed", 4.0f, 0f, 50f);
    private readonly GameParameterFloat _pmSprintspeed = new("pm_sprintspeed", 8.0f, 0f, 100f);
    private readonly GameParameterFloat _pmCrouchspeed = new("pm_crouchspeed", 2.0f, 0f, 20f);
    private readonly GameParameterFloat _pmMaxacceleration = new("pm_maxacceleration", 100.0f, 0f, 500f);
    private readonly GameParameterFloat _pmMaxairacceleration = new("pm_maxairacceleration", 1.0f, 0f, 10f);
    private readonly GameParameterFloat _pmJumpheight = new("pm_jumpheight", 1.0f, 0f, 10f);
    private readonly GameParameterFloat _pmCrouchheight = new("pm_crouchheight", 1.0f, 0f, 5f);
    private readonly GameParameterInt _pmMaxairjumps = new("pm_maxairjumps", 0, 0, 10);
    private readonly GameParameterFloat _pmMaxgroundangle = new("pm_maxgroundangle", 45.0f, 0f, 90f);
    private readonly GameParameterFloat _pmMaxstairsangle = new("pm_maxstairsangle", 80.0f, 0f, 90f);
    private readonly GameParameterFloat _pmMaxsnapspeed = new("pm_maxsnapspeed", 50.0f, 0f, 200f);
    private readonly GameParameterFloat _pmProbedistance = new("pm_probedistance", 1.0f, 0.1f, 10f);
    private readonly GameParameterFloat _pmSafefallspeed = new("pm_safefallspeed", 14.0f, 0f, 100f);
    private readonly GameParameterFloat _pmFalldamagemultiplier = new("pm_falldamagemultiplier", 10.0f, 0f, 100f);
    private readonly GameParameterBool _pmNoclip = new("pm_noclip", false);


    public PlayerParameters()
    {
        DisplayName = "PlayerData";

        Parameters = new Dictionary<string, IGameParameter>
        {
            { _playerHealth.Name, _playerHealth },
            { _pmSpeed.Name, _pmSpeed },
            { _pmSprintspeed.Name, _pmSprintspeed },
            { _pmCrouchspeed.Name, _pmCrouchspeed },
            { _pmMaxacceleration.Name, _pmMaxacceleration },
            { _pmMaxairacceleration.Name, _pmMaxairacceleration },
            { _pmJumpheight.Name, _pmJumpheight },
            { _pmCrouchheight.Name, _pmCrouchheight },
            { _pmMaxairjumps.Name, _pmMaxairjumps },
            { _pmMaxgroundangle.Name, _pmMaxgroundangle },
            { _pmMaxstairsangle.Name, _pmMaxstairsangle },
            { _pmMaxsnapspeed.Name, _pmMaxsnapspeed },
            { _pmProbedistance.Name, _pmProbedistance },
            { _pmSafefallspeed.Name, _pmSafefallspeed },
            { _pmFalldamagemultiplier.Name, _pmFalldamagemultiplier },
            { _pmNoclip.Name, _pmNoclip }
        };

        //InitializeParameters();
    }


}
