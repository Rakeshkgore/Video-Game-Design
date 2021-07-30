Legends of Gamers
Final Version
Team Gamer Jackets

Team Members:
Wenyuan Song — mwysong@gatech.edu — wsong75
Yi Chin Chen — ychen3243@gatech.edu — ychen3243
Rishov Sarkar — rishov.sarkar@gatech.edu — rsarkar30
Rakesh Gorrepati — rgorrepati5@gatech.edu — rgorrepati3

Start Scene File: Assets/Scenes/SampleScene.unity

How to Play:
Use WASD or the up, down, left, and right arrow keys to move in their respective directions.
Use Z to swing bat, X to dive roll forward, and C to throw rocks after gaining the ability to do so.
Use the space bar to jump.
Defeat the tutorial golem to gain the throw ability. Then figure out how to cross the bridge, where you will discover more ways to unlock abilities and fight the rhino.

Manifest:
Wenyuan: Static objects and game world design, tree animations
	Food.cs
	GetBlessed.cs
	GetHealth.cs
	DropApples.cs
	BridgeActivatation.cs
	BridgeActivatation1.cs
	BridgeActivatation2.cs
	BridgeActivatation3.cs
	MazeGone.cs
	scroll1.cs
	scroll2.cs
	scroll3.cs
	scroll4.cs
	scroll5.cs
	BaseofWall1.anim
	BaseofWall2.anim
	BaseofWall3.anim
	DescendingWall1.anim
	DescendingWall2.anim
	DescendingWall3.anim
	Maze.anim
	Maze1.anim
	Cube (51).controller
	Cube (10).controller
	Cube (9).controller
	Maze.controller
Yi Chin: Sound implementation, menu screens, tutorial text display
	GameStarter.cs
	GameQuitter.cs
	PauseMenuToggle.cs
	SceneStart.cs
	TutorialText.cs
	AppEvents/CharacterSoundEffects.cs
	AppEvents/GolemSoundEffects1.cs
	AppEvents/RhinoSoundEffects.cs
	EventManager.cs
	RockThrower.cs
	Credits.unity
	Defeat.unity
	StartGame.unity
	Victory.unity
Rishov: Rhino and golem (NPC) AI and animations, weapons collision handling, git merge conflict handling
	GolemAI.cs
	RhinoAI.cs
	EndParticlesAt.cs
	GUI/ContextualText.cs
	GUI/ObjectiveText.cs
	GUI/ShowContextualTextAfterLastParticle.cs
	GUI/ShowContextualTextOnTrigger.cs
	Invincibility.cs
	InvincibleInvisibleMaterialController.cs
	Invisibility.cs
	InvisibilitySlider.cs
	InvisibilityTrigger.cs
	LerpMaterials.cs
	NonTutorialGolemParticleSystemController.cs
	ReceiveWeaponEvents.cs
	RockController.cs
	Rotator.cs
	ShowContextualLockedText.cs
	SoundParticleController.cs
	SoundParticleEmitter.cs
	TutorialGolemParticleSystemController.cs
	Utility/FiniteStateMachine.cs
	Utility/NavMeshAgentAutoStop.cs
	Utility/TriggerCount.cs
	Weapon.cs
	WeaponDelegate.cs
	Golem.controller
	Rhino.controller
Rakesh: Main player animations, health for player/enemy, masks for animation layers.
	CharacterInputController.cs
	RootMotionControlScript.cs
	SimpleAnimatorController.controller
	EnemyHealthSlider.cs
	PlayerHealthSlider.cs
