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
}
