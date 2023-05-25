using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected static bool FinalizeMove( Sandworker worker, Vector2Int NewPos, Vector2Int NewVel )
	{
		var (pos, vel, moved) = CheckPosVelocity( worker, NewPos, NewVel );
		if ( moved )
		{
			worker.MoveCell( NewPos, pos );
		}
		worker.SetCellVelocity( NewPos, vel );
		return !moved;
	}

	protected static (Vector2Int pos, Vector2Int vel, bool moved) CheckPosVelocity( Sandworker worker, Vector2Int newPos, Vector2Int newVel )
	{
		/*if ( newVel.x == 0 && newVel.y == 0 ) return (newPos, newVel, false);

		if ( newVel.Length <= 1 )
		{
			//Log.Info( worker.IsEmpty( newPos + newVel ) );
			if ( worker.IsEmpty( newPos + newVel ) )
			{
				return (newPos + newVel, Vector2Int.Zero, true);
			}
		} */

		Vector2Int currentPos = newPos;
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
		return (currentPos, currentPos - newPos, newPos != currentPos);
	}

	public static void QuickSwap( Sandworker worker, Vector2Int pos, Vector2Int pos2, Cell a, Cell b )
	{
		worker.SetCell( pos, ref b, true );
		worker.SetCell( pos2, ref a, true );

		worker.PingChunk( pos );
		worker.PingChunk( pos2 );
	}
}
