using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected static bool FinalizeMove( Sandworker worker, Cell cell, Vector2Int NewPos, Vector2 NewVel )
	{
		if ( NewVel.Length <= 2f + float.Epsilon )
		{
			cell.Velocity = NewVel;
			return true;
		}
		var (pos, vel, moved, HitSomething) = CheckPosVelocity( worker, NewPos, NewVel );
		if ( moved )
		{
			worker.MoveCell( NewPos, pos );
			cell.Velocity = vel;
			return false;
		}
		else if ( HitSomething )
		{
			cell.Velocity = 0;
			return !moved;
		}
		/* if ( HitSomething )
			cell.Velocity = 0;
		else
			cell.Velocity = vel; */
		//cell.Velocity = 0;

		//Log.Info( $"Moved: {moved} HitSomething: {HitSomething} NewVel: {NewVel} Vel: {vel} Pos: {pos} NewPos: {NewPos}" );
		return !moved;
	}

	protected static (Vector2Int pos, Vector2 vel, bool moved, bool HitSomething) CheckPosVelocity( Sandworker worker, Vector2Int newPos, Vector2 newVel )
	{
		if ( newVel.Length < 0.8f )
		{
			return (newPos, newVel, false, false);
		}
		Vector2Int currentPos = newPos;
		Vector2 vel = newVel;
		bool hitsomething = false;
		SandUtils.PointToPointFunction( newPos, newPos + newVel, ( pos ) =>
		{
			if ( hitsomething ) return;
			if ( worker.IsEmpty( pos ) && !hitsomething )
			{
				currentPos = pos;
			}
			else
			{
				hitsomething = true;
			}
		} );
		return (currentPos, newVel, newPos != currentPos, hitsomething);
	}

	public static void QuickSwap( Sandworker worker, Vector2Int pos, Vector2Int pos2, Cell a, Cell b )
	{
		worker.SetCell( pos, ref b, true );
		worker.SetCell( pos2, ref a, true );

		worker.PingChunk( pos );
		worker.PingChunk( pos2 );
	}
}
