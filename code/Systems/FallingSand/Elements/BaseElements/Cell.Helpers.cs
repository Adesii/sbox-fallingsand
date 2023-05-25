using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected static bool FinalizeMove( Sandworker worker, Vector2Int NewPos, Vector2Int NewVel )
	{
		var (pos, vel, moved) = CheckPosVelocity( worker, NewPos, NewVel );
		worker.MoveCell( NewPos, pos );
		worker.SetCellVelocity( NewPos, vel );
		return !moved;
	}

	protected static (Vector2Int pos, Vector2Int vel, bool moved) CheckPosVelocity( Sandworker worker, Vector2Int newPos, Vector2Int newVel )
	{
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
	}
}
