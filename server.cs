//-----------------------------------------------------------------------------
// Torque Game Engine 
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Load dts shapes and merge animations
datablock TSShapeConstructor(NoFaceDts)
{
	baseShape  = "./noface_m.dts";
   sequence0  = "./noface_root.dsq root";

   sequence1  = "./noface_move.dsq run";
   sequence2  = "./noface_move.dsq walk";
   sequence3  = "./noface_move.dsq back";
   sequence4  = "./noface_move.dsq side";

   sequence5  = "./noface_crouch.dsq crouch";
   sequence6  = "./noface_crouched.dsq crouchRun";
   sequence7  = "./noface_crouched.dsq crouchBack";
   sequence8  = "./noface_crouched.dsq crouchSide";

   sequence9  = "./noface_look.dsq look";
   sequence10 = "./noface_headlook.dsq headside";
   sequence11 = "./noface_root.dsq headUp";

   sequence12 = "./noface_root.dsq jump";
   sequence13 = "./noface_root.dsq standjump";
   sequence14 = "./noface_root.dsq fall";
   sequence15 = "./noface_root.dsq land";

   sequence16 = "./noface_engulf.dsq armAttack";
   sequence17 = "./noface_root.dsq armReadyLeft";
   sequence18 = "./noface_root.dsq armReadyRight";
   sequence19 = "./noface_root.dsq armReadyBoth";
   sequence20 = "./noface_root.dsq spearready";  
   sequence21 = "./noface_root.dsq spearThrow";

   sequence22 = "./noface_root.dsq talk";  

   sequence23 = "./noface_crouch.dsq death1"; 
   
   sequence24 = "./noface_root.dsq shiftUp";
   sequence25 = "./noface_root.dsq shiftDown";
   sequence26 = "./noface_root.dsq shiftAway";
   sequence27 = "./noface_root.dsq shiftTo";
   sequence28 = "./noface_root.dsq shiftLeft";
   sequence29 = "./noface_root.dsq shiftRight";
   sequence30 = "./noface_root.dsq rotCW";
   sequence31 = "./noface_root.dsq rotCCW";

   sequence32 = "./noface_root.dsq undo";
   sequence33 = "./noface_root.dsq plant";

   sequence34 = "./noface_root.dsq sit";

   sequence35 = "./noface_root.dsq wrench";

   sequence36 = "./noface_root.dsq activate";
   sequence37 = "./noface_root.dsq activate2";

   sequence38 = "./noface_root.dsq leftrecoil";
};    

datablock PlayerData(NoFaceArmor)
{
   renderFirstPerson = false;
   emap = false;
   
   className = Armor;
   shapeFile = "./noface_m.dts";
   cameraMaxDist = 6;
   cameraTilt = 0.261;//0.174 * 2.5; //~25 degrees
   cameraVerticalOffset = 1.3;
     
   cameraDefaultFov = 90.0;
   cameraMinFov = 5.0;
   cameraMaxFov = 120.0;
   
   //debrisShapeName = "~/data/shapes/player/debris_player.dts";
   //debris = NoFaceDebris;

   aiAvoidThis = true;

   minLookAngle = -1.5708;
   maxLookAngle = 1.5708;
   maxFreelookAngle = 3.0;

   mass = 140;
   drag = 0.1;
   density = 0.7;
   maxDamage = 280;
   maxEnergy =  10;
   repairRate = 0.33;

   rechargeRate = 0.4;

   runForce = 80 * 140;
   runEnergyDrain = 0;
   minRunEnergy = 0;
   maxStepHeight= "1";
   maxForwardSpeed = 7.5;
   maxBackwardSpeed = 5;
   maxSideSpeed = 5;

   maxForwardCrouchSpeed = 7;
   maxBackwardCrouchSpeed = 5;
   maxSideCrouchSpeed = 5;

   maxForwardProneSpeed = 0;
   maxBackwardProneSpeed = 0;
   maxSideProneSpeed = 0;

   maxForwardWalkSpeed = 5;
   maxBackwardWalkSpeed = 4;
   maxSideWalkSpeed = 4;

   maxUnderwaterForwardSpeed = 15;
   maxUnderwaterBackwardSpeed = 6;
   maxUnderwaterSideSpeed = 6;

   jumpForce = 0 * 140; //8.3 * 90;
   jumpEnergyDrain = 0;
   minJumpEnergy = 0;
   jumpDelay = 0;

   minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

   minImpactSpeed = 250;
   speedDamageScale = 3.8;

   boundingBox			= vectorScale("1.2 1.2 2.8", 4);
   crouchBoundingBox	= vectorScale("1.2 1.2 0.3", 4);
   
   pickupRadius = 0.75;
   
   // Foot Prints
   //decalData   = NoFaceFootprint;
   //decalOffset = 0.25;
	
   jetEmitter = "";
   jetGroundEmitter = "";
   jetGroundDistance = 4;
  
   //footPuffEmitter = LightPuffEmitter;
   footPuffNumParts = 10;
   footPuffRadius = 0.25;

   //dustEmitter = LiftoffDustEmitter;

   splash = PlayerSplash;
   splashVelocity = 4.0;
   splashAngle = 67.0;
   splashFreqMod = 300.0;
   splashVelEpsilon = 0.60;
   bubbleEmitTime = 0.1;
   splashEmitter[0] = PlayerFoamDropletsEmitter;
   splashEmitter[1] = PlayerFoamEmitter;
   splashEmitter[2] = PlayerBubbleEmitter;
   mediumSplashSoundVelocity = 10.0;   
   hardSplashSoundVelocity = 20.0;   
   exitSplashSoundVelocity = 5.0;

   // Controls over slope of runnable/jumpable surfaces
   runSurfaceAngle  = 85;
   jumpSurfaceAngle = 86;

   minJumpSpeed = 20;
   maxJumpSpeed = 30;

   horizMaxSpeed = 0;
   horizResistSpeed = 33;
   horizResistFactor = 0.35;

   upMaxSpeed = 80;
   upResistSpeed = 25;
   upResistFactor = 0.3;
   
   footstepSplashHeight = 0.35;

   //NOTE:  some sounds commented out until wav's are available

   JumpSound			= "";

   // Footstep Sounds
//   FootSoftSound        = NoFaceFootFallSound;
//   FootHardSound        = NoFaceFootFallSound;
//   FootMetalSound       = NoFaceFootFallSound;
//   FootSnowSound        = NoFaceFootFallSound;
//   FootShallowSound     = NoFaceFootFallSound;
//   FootWadingSound      = NoFaceFootFallSound;
//   FootUnderwaterSound  = NoFaceFootFallSound;
   //FootBubblesSound     = FootLightBubblesSound;
   //movingBubblesSound   = ArmorMoveBubblesSound;
   //waterBreathSound     = WaterBreathMaleSound;

   //impactSoftSound      = ImpactLightSoftSound;
   //impactHardSound      = ImpactLightHardSound;
   //impactMetalSound     = ImpactLightMetalSound;
   //impactSnowSound      = ImpactLightSnowSound;
   
   impactWaterEasy      = Splash1Sound;
   impactWaterMedium    = Splash1Sound;
   impactWaterHard      = Splash1Sound;
   
   groundImpactMinSpeed    = 10.0;
   groundImpactShakeFreq   = "4.0 4.0 4.0";
   groundImpactShakeAmp    = "1.0 1.0 1.0";
   groundImpactShakeDuration = 0.8;
   groundImpactShakeFalloff = 10.0;
   
   //exitingWater         = ExitingWaterLightSound;

   // Inventory Items
	maxItems   = 10;	//total number of bricks you can carry
	maxWeapons = 5;		//this will be controlled by mini-game code
	maxTools = 5;
	
	uiName = "No-Face";
	rideable = false;
		lookUpLimit = 0.585398;
		lookDownLimit = 0.385398;

	canRide = false;
	showEnergyBar = false;
	paintable = true;

	brickImage = brickImage;	//the imageData to use for brick deployment
};

datablock PlayerData(NoFaceCrouchArmor : NoFaceArmor) {
   maxStepHeight = 0;
};

datablock AudioProfile(noface_goreDeath)
{
   filename    = "./splat.wav";
   description = AudioClose3d;
   preload = true;
};



function NoFaceArmor::onAdd(%this,%obj)
{
   // Vehicle timeout
   %obj.mountVehicle = true;

   // Default dynamic armor stats
   %obj.setRepairRate(0);
   %obj.setShapeNameDistance(0);
}



//called when the driver of a player-vehicle is unmounted
function NoFaceArmor::onDriverLeave(%obj, %player)
{
	//do nothing
}

datablock ItemData(NoFaceConsumeItem : GunItem) {
   shapeFile = "base/data/shapes/empty.dts";
   uiName = "Consume";

   iconName = "";

   doColorShift = true;
   colorShiftColor = "0.2 0.2 0.2 1";

   image = NoFaceConsumeImage;
   canDrop = true;
};

datablock ShapeBaseImageData(NoFaceConsumeImage)
{
   shapeFile = "base/data/shapes/empty.dts";
   mountPoint = 0;
   emap = true;

   className = "WeaponImage";
   item = NoFaceConsumeItem;
   isGrapplingHook = true;

   stateName[0]                     = "Activate";
   stateTimeoutValue[0]             = 0.1;
   stateTransitionOnTimeout[0]      = "Ready";

   stateName[1]                     = "Ready";
   stateTimeoutValue[1]             = 0.1;
   stateTransitionOnTriggerDown[1]  = "Consume";
   stateTransitionOnTimeout[1]      = "Status";

   stateName[2]                     = "Consume";
   statetimeoutValue[2]             = 0.1;
   stateScript[2]                   = "onConsume";
   stateTransitionOnTimeout[2]      = "Status";
   stateWaitForTimeout[2]           = true;

   stateName[3]                     = "Status";
   stateTimeoutValue[3]             = 0.1;
   stateScript[3]                   = "checkStatus";
   stateTransitionOnTriggerDown[3]  = "Consume";
   stateTransitionOnTimeout[3]      = "Ready";
};

function NoFaceConsumeImage::onMount(%this, %obj, %slot) {
   if (strPos(%obj.getDatablock().getName(), "NoFace") < 0) {
      centerprint(%obj.client, "Only No-Face can use this item!", 3);
      %obj.unmountImage(%slot);
      return;
   }
   return parent::onMount(%this, %obj, %slot);
}

$Noface_CaptureDistance = 4;
$Noface_ConsumeDelay = 20000;

function NoFaceConsumeImage::onConsume(%this, %obj, %slot) {
   if (getRealTime() - %obj.lastConsumedTargetTime < $Noface_ConsumeDelay) {
      centerprint(%obj.client, "You need to wait " @ mCeil(($Noface_ConsumeDelay - getRealTime() + %obj.lastConsumedTargetTime)/1000) @ " seconds before consuming again.", 1);
      return;
   }
   %start = getWords(%obj.getEyeTransform(), 0, 2);
   %end = vectorAdd(vectorScale(%obj.getEyeVector(), $Noface_CaptureDistance), %start);
   %mask = $TypeMasks::PlayerObjectType;
   %ray = containerRayCast(%start, %end, %mask, %obj);

   %target = getWord(%ray, 0);
   if (isObject(%target)) {
      %obj.isConsumingTarget = 1;
      %obj.target = %target;
      %target.isBeingConsumed = 1;
      %target.consumer = %obj;
      %obj.lastConsumedTargetTime = getRealTime();

      %target.emote(alarmProjectile);
      %target.playthread(2, activate2);
      %target.playthread(0, sit);
      %target.setVelocity("0 0 0");
      %obj.schedule(1000, mountObject, %target, 6);
      %obj.schedule(1000, playThread, 0, armAttack);
      schedule(1200, 0, scaleDown, %target, 0.1);

      spyTarget(%obj, %target.client);
      spyTarget(%obj, %obj.client);

      %target.scheduleNoQuota(1800, kill);
      %target.scheduleNoQuota(1800, delete);
      schedule(1300, 0, serverPlay3D, noface_goreDeath, %obj.getHackPosition());
      %obj.schedule(1300, spawnExplosion, grimSmokeProjectile, 0.5);
      %obj.scheduleNoQuota(4000, playThread, 0, root);

      schedule(4000, 0, eval, "if (" @ %obj @ ".isConsumingTarget) " @ %obj @ ".damage = 0;");
      schedule(4000, 0, eval, %target @ ".isBeingConsumed = 0;");
      schedule(4000, 0, eval, %obj @ ".isConsumingTarget = 0;");
      schedule(4000, 0, unSpyTarget, %obj, %obj.client);
      return;
   }
}

function NoFaceConsumeImage::checkStatus(%this, %obj, %slot) {
   if (getRealTime() - %obj.lastConsumedTargetTime < $Noface_ConsumeDelay) {
      %obj.client.bottomprint("You need to wait " @ mCeil(($Noface_ConsumeDelay - getRealTime() + %obj.lastConsumedTargetTime)/1000) @ " seconds before consuming again.", 3, 0);
   } else {
      clearBottomprint(%obj.client);
   }
}

package NoFace {
   function Observer::onTrigger(%this, %obj, %triggerNum, %val) {
      if (isObject(%pl = %obj.getControllingClient().player) && (%pl.isBeingConsumed || %pl.isConsumingTarget)) {
         return;
      }
      return parent::onTrigger(%this, %obj, %triggerNum, %val);
   }

   function Armor::onTrigger(%this, %obj, %triggerNum, %val) {
      if (strPos(%obj.getDatablock().getName(), "NoFace") >= 0 && (%triggerNum == 4 || %triggerNum == 2) && %val == 1) {
         if (%triggerNum == 2 && getRealTime() - %obj.lastJumpTime > 2500) {
            %obj.setVelocity(vectorAdd(%obj.getVelocity(), "0 0 18"));
            %obj.lasJumpTime = getRealTime();
            %obj.lastJumpTime = getRealTime();
            return;
         } else if (%triggerNum == 4 && getRealTime() - %obj.lastDashTime > 5000) {
            %obj.setMaxForwardSpeed(18);
            %obj.schedule(500, setMaxForwardSpeed, 17);
            %obj.schedule(600, setMaxForwardSpeed, 16);
            %obj.schedule(650, setMaxForwardSpeed, 15);
            %obj.schedule(700, setMaxForwardSpeed, 14);
            %obj.schedule(750, setMaxForwardSpeed, 13);
            %obj.schedule(800, setMaxForwardSpeed, 12);
            %obj.schedule(850, setMaxForwardSpeed, 11);
            %obj.schedule(900, setMaxForwardSpeed, 10);
            %obj.schedule(933, setMaxForwardSpeed, 9);
            %obj.schedule(966, setMaxForwardSpeed, 8);
            %obj.schedule(1000, setMaxForwardSpeed, %obj.getDatablock().maxForwardSpeed);
            %obj.lastDashTime = getRealTime();
            return;
         } else if (%triggerNum == 2) {
            %obj.client.bottomprint("<just:center>\c7Jump is on cooldown...", 1, 1);
            return;
         } else if (%triggerNum == 4) {
            %obj.client.bottomprint("<just:center>\c7Dash is on cooldown...", 1, 1);
            return;
         }
      } else if (strPos(%obj.getDatablock().getName(), "NoFace") >= 0 && %triggerNum == 3) {
         if (%val) {
            %obj.schedule(400, setDatablock, NoFaceCrouchArmor);
         } else if (%obj.isCrouched()) {
            %obj.schedule(400, setDatablock, NoFaceArmor);
         }
      }
      if (!%obj.isCrouched() && %obj.getDatablock().getName() $= "NoFaceCrouchArmor" && %triggerNum != 3) {
         %obj.schedule(400, setDatablock, NoFaceArmor);
      }
      return parent::onTrigger(%this, %obj, %triggerNum, %val);
   }
};
activatePackage(NoFace);

function spyTarget(%obj, %client) {
   if (!isObject(%client) || !isObject(%obj)) {
      return;
   }

   %client.setControlObject(%client.camera);
   %client.camera.oldControlObject = %client.camera.getControlObject();
   %client.camera.setControlObject(%client.camera);
   %client.camera.setMode(Corpse, %obj);
}

function unSpyTarget(%obj, %client) {
   if (!isObject(%client) || !isObject(%obj)) {
      return;
   }

   %client.camera.setControlObject(%client.camera.oldControlObject);
   %client.setControlObject(%client.player);
   %client.camera.setMode(Observer);
}

function scaleDown(%target, %endScale) {
   if (getWord(%target.getScale(), 0) < %endScale) {
      return;
   }
   %scale = getWord(%target.getScale(), 0) - 0.0001;
   %target.setScale(%scale SPC %scale SPC %scale);

   schedule(0, 0, scaleDown, %target, %endScale);
}