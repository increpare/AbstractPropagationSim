using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;


public struct AbstractMovement {
	public readonly int speed;
	public readonly int torsion;
	public readonly bool passive; //passive is only meaningful in CombineMovements
	public readonly int[] causes;
	public readonly int id;

	public int surfaceSpeed {
		get { return speed + torsion;}
	}

	private static int idcounter=0;

	public AbstractMovement(int speed, int torsion,bool passive=false, int[] causes=null){
		this.speed = speed;
		this.torsion = torsion;
		this.passive = passive;
		this.causes =  (int[])causes.Clone();
		this.id = idcounter;
		idcounter++;
	}

	public AbstractMovement AddCause(int id){
		if (causes.Contains (id)) {
			return this;
		}
		var newCauses = new int[causes.Length + 1];
		Array.Copy (causes, newCauses, causes.Length);
		newCauses [newCauses.Length - 1] = id;
		return new AbstractMovement (speed, torsion, passive, newCauses);
	}

	public bool IsCause(AbstractMovement possibleConsequence){
		return possibleConsequence.causes.Contains (possibleConsequence.id);
	}

	//active propagation ignores torsion
	public AbstractMovement Active(){
		return new AbstractMovement (speed, 0,false);
	}

	public AbstractMovement Passive(){
		return new AbstractMovement (speed, torsion,true);
	}

	public AbstractMovement Mirror(){
		return new AbstractMovement (speed, -torsion, passive);
	}

	public override string ToString ()
	{
		string result = speed.ToString ();
		switch (torsion) {
		case -1:
			result += " CCW";
			break;
		case 0:
			break;
		case 1:
			result += " CW"; 
			break;
		}
		return result;
	}

	private static AbstractMovement MergePassiveMovements(List<AbstractMovement> movements){
		var minspeed = movements.Min (m => m.speed);
		int torsion = 0;
		if (movements.Distinct ().Count () == 1) {
			torsion = movements [0].torsion;
		}
		return new AbstractMovement (minspeed, torsion, true);
	}

	//must pass movements that've had their passive flag considered (Active/Passive called)
	public static AbstractMovement Combine(List<AbstractMovement> movements, bool canRoll){
		var passiveMovements = movements.Where (m => m.passive).ToList ();
		var activeMovements = movements.Where (m => !m.passive).ToList ();	
		int maxActiveSpeed =  activeMovements.Any()?activeMovements.Max (af => af.speed):0;

		if (passiveMovements.Count () == 0) {
			return new AbstractMovement (maxActiveSpeed, canRoll?1:0);
		}

		var result = MergePassiveMovements (passiveMovements);
		if (canRoll == false) {
			result = new AbstractMovement (result.speed + result.torsion, 0);
		} else {
			result = result.Mirror ();
		}

		//bung together the passive forces
		//if active force speed greater than passive force speed, then slide at speed of active force
		if (maxActiveSpeed > result.speed) {
			result = new AbstractMovement (maxActiveSpeed, result.surfaceSpeed==0?1:0);
		}
		return result;
	}
}
