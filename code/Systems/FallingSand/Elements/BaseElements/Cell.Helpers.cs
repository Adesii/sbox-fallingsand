using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected static bool FinalizeMove( Sandworker worker, Cell cell, Vector2 NewVel )
	{
		if ( NewVel.Length <= 1f + float.Epsilon )
		{
			cell.Velocity = NewVel;
			return true;
		}
		var (pos, vel, moved, HitSomething) = CheckPosVelocity( worker, cell, NewVel );
		if ( moved )
		{
			worker.MoveCell( cell.Position, pos, true );
			cell.Velocity = vel;
			return false;
		}
		else if ( HitSomething )
		{
			cell.Velocity = 0;
			return !moved;
		}

		return !moved;
	}

	protected static (Vector2Int pos, Vector2 vel, bool moved, bool HitSomething) CheckPosVelocity( Sandworker worker, Cell cell, Vector2 newVel )
	{
		if ( newVel.Length < 0.8f )
		{
			return (cell.Position, newVel, false, false);
		}
		Vector2Int currentPos = cell.Position;
		Vector2 vel = newVel;
		bool hitsomething = false;
		SandUtils.PointToPointFunction( cell.Position, cell.Position + newVel, ( pos ) =>
		{
			if ( hitsomething ) return;
			if ( SandUtils.CanSwap( worker, cell, pos ) && !hitsomething )
			{
				currentPos = pos;
			}
			else
			{
				hitsomething = true;
			}
		} );
		return (currentPos, newVel, cell.Position != currentPos, hitsomething);
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
