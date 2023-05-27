using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	public struct HitResult
	{
		public Vector2Int Position;
		public Vector2 Velocity;
		public bool Moved;
		public bool HitSomething;

		public Cell HitCell;

		public HitResult( Vector2Int pos, Vector2 vel, bool moved, bool hitsomething, Cell hitcell = null )
		{
			Position = pos;
			Velocity = vel;
			Moved = moved;
			HitSomething = hitsomething;
			HitCell = hitcell;
		}
	}

	protected virtual bool FinalizeMove( Sandworker worker, Cell cell, Vector2 NewVel )
	{
		return FinalizeMove( worker, cell, NewVel, out _ );
	}
	protected virtual bool FinalizeMove( Sandworker worker, Cell cell, Vector2 NewVel, out Vector2Int newPos )
	{
		/* if ( NewVel.Length <= 1f + float.Epsilon )
		{
			cell.Velocity = NewVel;
			newPos = cell.Position;
			return true;
		} */
		var HitResult = CheckPosVelocity( worker, cell, NewVel );
		if ( HitResult.Moved )
		{
			worker.MoveCell( cell.Position, HitResult.Position, true );
			cell.Velocity = HitResult.Velocity;
			newPos = HitResult.Position;
			return false;
		}
		else if ( HitResult.HitSomething )
		{
			cell.Velocity = 0;
			newPos = cell.Position;
			//cell.OnHit( worker, HitResult.HitCell );
			return !HitResult.Moved;
		}

		//cell.OnHit( worker, HitResult.HitCell );
		newPos = cell.Position;
		return !HitResult.Moved;
	}


	protected static HitResult CheckPosVelocity( Sandworker worker, Cell cell, Vector2 newVel )
	{
		if ( newVel.Length < 0.8f )
		{
			return new( cell.Position, newVel, false, false );
		}
		Vector2Int currentPos = cell.Position;
		Vector2 vel = newVel;
		bool hitsomething = false;
		Cell hitcell = null;
		SandUtils.PointToPointFunction( cell.Position, cell.Position + newVel, ( pos ) =>
		{
			if ( hitsomething ) return;
			if ( SandUtils.CanSwap( worker, cell, pos, out var thehitcell ) && (cell.Position != pos) && !hitsomething )
			{
				currentPos = pos;
				hitcell = thehitcell;
			}
			else if ( cell.Position != pos )
			{
				hitcell = worker.GetCell( pos + vel );
				hitsomething = true;
			}
		} );
		return new( currentPos, newVel, cell.Position != currentPos, hitsomething, hitcell );
	}

	protected static Vector2 GetGravityAtPosition( Sandworker worker, Cell cell )
	{
		// do a simple gravity towards 0,0 for now
		Vector2 gravityPosition = Vector2.Zero;
		Vector2 pos = cell.Position.vec;
		Vector2 gravity = gravityPosition - pos;
		gravity = gravity.Normal;
		return gravity;
	}

	protected static Vector2Int GetGravityCheckDir( Vector2 gravity )
	{
		//return Vector2Int.Down;
		Vector2Int dir = Vector2Int.Zero;
		// snap the gravity to a direction in 8 directions
		if ( gravity.x >= 0.5f )
		{
			dir.x = 1;
		}
		else if ( gravity.x < -0.5f )
		{
			dir.x = -1;
		}
		if ( gravity.y >= 0.5f )
		{
			dir.y = 1;
		}
		else if ( gravity.y < -0.5f )
		{
			dir.y = -1;
		}



		return dir;
	}
}
