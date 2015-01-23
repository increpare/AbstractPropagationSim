using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

public struct AbstractMovement :IEquatable<AbstractMovement> {
	public readonly int speed;
	public readonly int torsion;
	public readonly bool passive; //passive is only meaningful in CombineMovements
	public readonly string[] causes;

	public bool Equals(AbstractMovement other) 
	{
		if (speed != other.speed||torsion!=other.torsion||passive!=other.passive) {
			return false;
		}
		if (causes.Length != other.causes.Length) {
			return false;
		}
		for (var i = 0; i < causes.Length; i++) {
			if (causes [i] != other.causes [i]) {
				return false;
			}
		}
		return true;
	}

	public static bool operator==(AbstractMovement a, AbstractMovement b){
		return a.Equals(b);
	}

	public static bool operator!=(AbstractMovement a, AbstractMovement b){
		return !a.Equals(b);
	}

	public override int GetHashCode ()
	{
		int hash = 17;

		hash = hash * 23 + speed.GetHashCode();
		hash = hash * 23 + torsion.GetHashCode();
		hash = hash * 23 + passive.GetHashCode();
		hash = hash * 23 + causes.GetHashCode();
		return hash;	
	}


	public override bool Equals(Object other2) 
	{
		if (other2 == null) {
			return false;
		}
		if (!(other2 is AbstractMovement)) {
			return false;
		}

		var other = (AbstractMovement)other2;

		if (speed != other.speed||torsion!=other.torsion||passive!=other.passive) {
			return false;
		}
		if (causes.Length != other.causes.Length) {
			return false;
		}
		for (var i = 0; i < causes.Length; i++) {
			if (causes [i] != other.causes [i]) {
				return false;
			}
		}
		return true;
	}

	public static List<AbstractMovement> Translations() 
	{
		return new List<AbstractMovement> () {
			new AbstractMovement(0,0),
			new AbstractMovement(1,0),
			new AbstractMovement(2,0),
			new AbstractMovement(3,0),
			new AbstractMovement(4,0),
		};
	}
	public static List<AbstractMovement> Rotations() 
	{
		return new List<AbstractMovement> () {
			new AbstractMovement(0,0),
			new AbstractMovement(1,0),
			new AbstractMovement(2,0),
			new AbstractMovement(3,0),
			new AbstractMovement(4,0),
			new AbstractMovement(1,1),
			new AbstractMovement(2,1),
			new AbstractMovement(3,1),
			new AbstractMovement(4,1),
			new AbstractMovement(1,-1),
			new AbstractMovement(2,-1),
			new AbstractMovement(3,-1),
			new AbstractMovement(4,-1)
		};
	}

	public int surfaceSpeed {
		get { return speed + torsion;}
	}

	public bool Causal(){
		return causes.Contains ("Player");
	}

	public bool SameAs(AbstractMovement other){
		return speed == other.speed && torsion == other.torsion;
	}

	/*
	 * id -1 = assign new
	 * id -2 = assign negative id to denote no cause
	 * */
	public AbstractMovement(int speed, int torsion,bool passive=false, string[] causes=null){
		this.speed = speed;
		this.torsion = torsion;
		this.passive = passive;
		this.causes =  causes==null?new string[0]:causes;
	}

	public AbstractMovement AddCause(string cause){
		if (causes.Contains (cause)) {
			return this;
		}
		var newCauses = new string[causes.Length + 1];
		Array.Copy (causes, newCauses, causes.Length);
		newCauses [newCauses.Length - 1] = cause;
		return new AbstractMovement (speed, torsion, passive, newCauses);
	}

	public AbstractMovement AddCause(IEnumerable<string> causes){
		var newCauses = this.causes.ToList ();
		newCauses.AddRange (causes);
		var newCausesArray = newCauses.Distinct ().ToArray();
		return new AbstractMovement (speed, torsion, passive, newCausesArray);
	}

	public bool CausedBy(string possibleCause){
		if (possibleCause == "Ground") {
			return false;
		}
		return causes.Contains (possibleCause);
	}

	//active propagation ignores torsion
	public AbstractMovement Active(){
		return new AbstractMovement (speed, 0,false,causes);
	}

	public AbstractMovement Passive(){
		return new AbstractMovement (speed, torsion,true,causes);
	}

	public AbstractMovement Mirror(){
		return new AbstractMovement (speed, -torsion, passive);
	}

	public override string ToString ()
	{
		string result = speed.ToString ();
		switch (torsion) {
		case -1:
			result += "&#8634;";
			break;
		case 0:
			break;
		case 1:
			result += "&#8635;"; 
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

	private static int Delta(IEnumerable<AbstractMovement> movements) {
		if (movements.All(m=>m.torsion==1)){
			return 1;
		} else if (movements.All(m=>m.torsion==-1)){
			return -1;
		} else {
			return 0;
		}
	}

	//move at maximum of contributing passive forces
	public static AbstractMovement EasyCombine(List<AbstractMovement> movements, bool canRoll){
		var passiveMovements = movements.Where (m => m.passive).ToList ();
		var activeMovements = movements.Where (m => !m.passive).ToList ();	

		if (passiveMovements.Count () == 0) {
			passiveMovements.Add(new AbstractMovement(0,0,true));
		}
		if (activeMovements.Count () == 0) {
			activeMovements.Add(new AbstractMovement(0,0,false));
		}

		int maxActiveSpeed =  activeMovements.Any()?activeMovements.Max (af => af.speed):0;

		string[] newCauses = movements.SelectMany (m => m.causes).Distinct ().ToArray ();

		if (canRoll) {
			var minPassiveSpeed = passiveMovements.Max (m => m.speed);
			var delta = Delta (passiveMovements);
			if (minPassiveSpeed >= maxActiveSpeed) {
				return new AbstractMovement (minPassiveSpeed, -delta, false, newCauses);
			} else {
				return new AbstractMovement (maxActiveSpeed, maxActiveSpeed>0?1:0, false, newCauses);
			}
		} else {
			var minPassiveSurfaceSpeed = passiveMovements.Max (pm => pm.surfaceSpeed);
			return new AbstractMovement (Math.Max(maxActiveSpeed,minPassiveSurfaceSpeed),0, false, newCauses);
		}					
	}

	//must pass movements that've had their passive flag considered (Active/Passive called)
	public static AbstractMovement Combine(List<AbstractMovement> movements, bool canRoll){
		var passiveMovements = movements.Where (m => m.passive).ToList ();
		var activeMovements = movements.Where (m => !m.passive).ToList ();	
						
		if (passiveMovements.Count () == 0) {
			passiveMovements.Add(new AbstractMovement(0,0,true));
		}
		if (activeMovements.Count () == 0) {
			activeMovements.Add(new AbstractMovement(0,0,false));
		}

		int maxActiveSpeed =  activeMovements.Any()?activeMovements.Max (af => af.speed):0;

		string[] newCauses = movements.SelectMany (m => m.causes).Distinct ().ToArray ();

		if (canRoll) {
			var minPassiveSpeed = passiveMovements.Min (m => m.speed);
			var delta = Delta (passiveMovements);
			if (minPassiveSpeed >= maxActiveSpeed) {
				return new AbstractMovement (minPassiveSpeed, -delta, false, newCauses);
			} else {
				return new AbstractMovement (maxActiveSpeed, maxActiveSpeed>0?1:0, false, newCauses);
			}
		} else {
			var minPassiveSurfaceSpeed = passiveMovements.Min (pm => pm.surfaceSpeed);
			return new AbstractMovement (Math.Max(maxActiveSpeed,minPassiveSurfaceSpeed),0, false, newCauses);
		}					
	}
}
