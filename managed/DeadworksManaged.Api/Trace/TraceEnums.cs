namespace DeadworksManaged.Api;

/// <summary>Shape type used for VPhys2 trace queries.</summary>
public enum RayType_t : byte {
	Line = 0,
	Sphere,
	Hull,
	Capsule,
	Mesh,
}

/// <summary>Collision group that determines which objects interact with each other in the physics simulation.</summary>
public enum CollisionGroup : byte {
	Always = 0,
	Nonphysical,
	Trigger,
	ConditionallySolid,
	FirstUser,
	Default = FirstUser,
	Debris,
	InteractiveDebris,
	Interactive,
	Player,
	BreakableGlass,
	Vehicle,
	PlayerMovement,
	NPC,
	InVehicle,
	Weapon,
	VehicleClip,
	Projectile,
	DoorBlocker,
	PassableDoor,
	Dissolving,
	Pushaway,
	NPCActor,
	NPCScripted,
	PZClip,
	Props,
	LastSharedCollisionGroup,
	// Deadlock collision groups
	CitadelAbilityProjectile = 25,
	CitadelBullet = 26,
	CitadelTimeWarp = 27,
	CitadelBreakableDebris = 28,
	CitadelMinion = 29,
	MaxAllowed = 64,
}

/// <summary>Individual content/interaction layers used to build <see cref="MaskTrace"/> bitmasks for trace queries.</summary>
public enum InteractionLayer : sbyte {
	// Base engine layers (0-30)
	ContentsSolid = 0,
	ContentsHitbox,
	ContentsTrigger,
	ContentsSky,
	FirstUser,
	ContentsPlayerClip = FirstUser,
	ContentsNpcClip,
	ContentsBlockLos,
	ContentsBlockLight,
	ContentsLadder,
	ContentsPickup,
	ContentsBlockSound,
	ContentsNoDraw,
	ContentsWindow,
	ContentsPassBullets,
	ContentsWorldGeometry,
	ContentsWater,
	ContentsSlime,
	ContentsTouchAll,
	ContentsPlayer,
	ContentsNpc,
	ContentsDebris,
	ContentsPhysicsProp,
	ContentsNavIgnore,
	ContentsNavLocalIgnore,
	ContentsPostProcessingVolume,
	ContentsUnusedLayer3,
	ContentsCarriedObject,
	ContentsPushaway,
	ContentsServerEntityOnClient,
	ContentsCarriedWeapon,
	ContentsStaticLevel,
	// Deadlock layers (31-63)
	FirstModSpecific,
	CitadelTeamAmber = FirstModSpecific,  // 31
	CitadelTeamSapphire,                  // 32
	CitadelTeamNeutal,                    // 33
	CitadelAbility,                       // 34
	CitadelBullet,                        // 35
	CitadelProjectile,                    // 36
	CitadelUnitHero,                      // 37
	CitadelUnitTrooper,                   // 38
	CitadelUnitNeutral,                   // 39
	CitadelUnitBuilding,                  // 40
	CitadelUnitProp,                      // 41
	CitadelUnitMinion,                    // 42
	CitadelUnitBoss,                      // 43
	CitadelUnitGoldOrb,                   // 44
	CitadelUnitWorldProp,                 // 45
	CitadelUnitTrophy,                    // 46
	CitadelUnitZipline,                   // 47
	CitadelMantleHidden,                  // 48
	CitadelObscured,                      // 49
	CitadelTimeWarp,                      // 50
	CitadelFoliage,                       // 51
	CitadelTransparent,                   // 52
	CitadelBlockCamera,                   // 53
	CitadelMantleable,                    // 54
	CitadelWalkable,                      // 55
	CitadelTempMovementBlocker,           // 56
	CitadelBlockMantle,                   // 57
	CitadelSkyclip,                       // 58
	CitadelValidPingTarget,               // 59
	CitadelCameraCanPassThrough,          // 60
	CitadelAbilityTrigger,                // 61
	CitadelPortalTrigger,                 // 62
	CitadelPortalEnvironment,             // 63
	NotFound = -1,
}

/// <summary>Bitmask combining <see cref="InteractionLayer"/> values to specify which content layers a trace interacts with.</summary>
[Flags]
public enum MaskTrace : ulong {
	Empty = 0ul,
	// Base engine layers
	Solid = 1ul << InteractionLayer.ContentsSolid,
	Hitbox = 1ul << InteractionLayer.ContentsHitbox,
	Trigger = 1ul << InteractionLayer.ContentsTrigger,
	Sky = 1ul << InteractionLayer.ContentsSky,
	PlayerClip = 1ul << InteractionLayer.ContentsPlayerClip,
	NpcClip = 1ul << InteractionLayer.ContentsNpcClip,
	BlockLos = 1ul << InteractionLayer.ContentsBlockLos,
	BlockLight = 1ul << InteractionLayer.ContentsBlockLight,
	Ladder = 1ul << InteractionLayer.ContentsLadder,
	Pickup = 1ul << InteractionLayer.ContentsPickup,
	BlockSound = 1ul << InteractionLayer.ContentsBlockSound,
	NoDraw = 1ul << InteractionLayer.ContentsNoDraw,
	Window = 1ul << InteractionLayer.ContentsWindow,
	PassBullets = 1ul << InteractionLayer.ContentsPassBullets,
	WorldGeometry = 1ul << InteractionLayer.ContentsWorldGeometry,
	Water = 1ul << InteractionLayer.ContentsWater,
	Slime = 1ul << InteractionLayer.ContentsSlime,
	TouchAll = 1ul << InteractionLayer.ContentsTouchAll,
	Player = 1ul << InteractionLayer.ContentsPlayer,
	Npc = 1ul << InteractionLayer.ContentsNpc,
	Debris = 1ul << InteractionLayer.ContentsDebris,
	PhysicsProp = 1ul << InteractionLayer.ContentsPhysicsProp,
	NavIgnore = 1ul << InteractionLayer.ContentsNavIgnore,
	NavLocalIgnore = 1ul << InteractionLayer.ContentsNavLocalIgnore,
	PostProcessingVolume = 1ul << InteractionLayer.ContentsPostProcessingVolume,
	UnusedLayer3 = 1ul << InteractionLayer.ContentsUnusedLayer3,
	CarriedObject = 1ul << InteractionLayer.ContentsCarriedObject,
	Pushaway = 1ul << InteractionLayer.ContentsPushaway,
	ServerEntityOnClient = 1ul << InteractionLayer.ContentsServerEntityOnClient,
	CarriedWeapon = 1ul << InteractionLayer.ContentsCarriedWeapon,
	StaticLevel = 1ul << InteractionLayer.ContentsStaticLevel,
	// Deadlock layers
	CitadelTeamAmber = 1ul << InteractionLayer.CitadelTeamAmber,
	CitadelTeamSapphire = 1ul << InteractionLayer.CitadelTeamSapphire,
	CitadelTeamNeutal = 1ul << InteractionLayer.CitadelTeamNeutal,
	CitadelAbility = 1ul << InteractionLayer.CitadelAbility,
	CitadelBullet = 1ul << InteractionLayer.CitadelBullet,
	CitadelProjectile = 1ul << InteractionLayer.CitadelProjectile,
	CitadelUnitHero = 1ul << InteractionLayer.CitadelUnitHero,
	CitadelUnitTrooper = 1ul << InteractionLayer.CitadelUnitTrooper,
	CitadelUnitNeutral = 1ul << InteractionLayer.CitadelUnitNeutral,
	CitadelUnitBuilding = 1ul << InteractionLayer.CitadelUnitBuilding,
	CitadelUnitProp = 1ul << InteractionLayer.CitadelUnitProp,
	CitadelUnitMinion = 1ul << InteractionLayer.CitadelUnitMinion,
	CitadelUnitBoss = 1ul << InteractionLayer.CitadelUnitBoss,
	CitadelUnitGoldOrb = 1ul << InteractionLayer.CitadelUnitGoldOrb,
	CitadelUnitWorldProp = 1ul << InteractionLayer.CitadelUnitWorldProp,
	CitadelUnitTrophy = 1ul << InteractionLayer.CitadelUnitTrophy,
	CitadelUnitZipline = 1ul << InteractionLayer.CitadelUnitZipline,
	CitadelMantleHidden = 1ul << InteractionLayer.CitadelMantleHidden,
	CitadelObscured = 1ul << InteractionLayer.CitadelObscured,
	CitadelTimeWarp = 1ul << InteractionLayer.CitadelTimeWarp,
	CitadelFoliage = 1ul << InteractionLayer.CitadelFoliage,
	CitadelTransparent = 1ul << InteractionLayer.CitadelTransparent,
	CitadelBlockCamera = 1ul << InteractionLayer.CitadelBlockCamera,
	CitadelMantleable = 1ul << InteractionLayer.CitadelMantleable,
	CitadelWalkable = 1ul << InteractionLayer.CitadelWalkable,
	CitadelTempMovementBlocker = 1ul << InteractionLayer.CitadelTempMovementBlocker,
	CitadelBlockMantle = 1ul << InteractionLayer.CitadelBlockMantle,
	CitadelSkyclip = 1ul << InteractionLayer.CitadelSkyclip,
	CitadelValidPingTarget = 1ul << InteractionLayer.CitadelValidPingTarget,
	CitadelCameraCanPassThrough = 1ul << InteractionLayer.CitadelCameraCanPassThrough,
	CitadelAbilityTrigger = 1ul << InteractionLayer.CitadelAbilityTrigger,
	CitadelPortalTrigger = 1ul << InteractionLayer.CitadelPortalTrigger,
	CitadelPortalEnvironment = 1ul << InteractionLayer.CitadelPortalEnvironment,
}

/// <summary>Bitmask controlling which object sets (static, dynamic, locatable) are included in a trace query.</summary>
[Flags]
public enum RnQueryObjectSet : byte {
	Static = 1 << 0,
	Keyframed = 1 << 1,
	Dynamic = 1 << 2,
	Locatable = 1 << 3,
	AllGameEntities = Keyframed | Dynamic | Locatable,
	All = Static | AllGameEntities,
}

/// <summary>Flags controlling which collision callbacks are enabled on a physics object.</summary>
[Flags]
public enum CollisionFunctionMask_t : byte {
	EnableSolidContact = 1 << 0,
	EnableTraceQuery = 1 << 1,
	EnableTouchEvent = 1 << 2,
	EnableSelfCollisions = 1 << 3,
	IgnoreForHitboxTest = 1 << 4,
	EnableTouchPersists = 1 << 5,
}

/// <summary>String comparison mode used when matching entity designer names in trace results.</summary>
public enum NameMatchType {
	Exact = 0,
	StartsWith = 1,
	EndsWith = 2,
	Contains = 3,
}
