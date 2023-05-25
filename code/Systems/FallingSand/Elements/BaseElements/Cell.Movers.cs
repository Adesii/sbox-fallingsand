using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected bool MoveDown( Sandworker worker, out bool sleep )
	{
		//using var _b = Profile.Scope( "MoveDown" );
		var other = worker.GetCell( Position + Vector2Int.Down );

		bool boyouend = (other?.Density ?? 0) > Density;
		if ( boyouend )
		{
			QuickSwap( worker, Position, Position + Vector2Int.Down, this, other );
			sleep = false;
			return true;
		}
		if ( GetType() != other?.GetType() )
		{
			var oldvel = Velocity;
			oldvel += Vector2Int.Down;
			sleep = FinalizeMove( worker, Position, oldvel );
			return !sleep;
		}
		sleep = false;
		return false;
	}


	protected bool MoveDownSides( Sandworker worker, out bool sleep )
	{

		bool left = worker.IsEmpty( Position + Vector2Int.Down + Vector2Int.Left );
		bool right = worker.IsEmpty( Position + Vector2Int.Down + Vector2Int.Right );
		if ( left && right )
		{
			left = Game.Random.Float() > 0.5f;
			right = !left;
		}


		var oldvel = new Vector2Int();

		if ( left )
		{
			oldvel = Vector2Int.Left + Vector2Int.Down;
		}
		else if ( right )
		{
			oldvel = Vector2Int.Right + Vector2Int.Down;
		}
		if ( left || right )
		{
			sleep = FinalizeMove( worker, Position, oldvel );
			return !sleep;
		}

		sleep = true;

		return left || right;
	}

	protected bool MoveSides( Sandworker worker, out bool sleep )
	{
		//using var _c = Profile.Scope( "MoveDownSides" );
		bool downleft = worker.IsEmpty( Position + Vector2Int.Left );
		bool downright = worker.IsEmpty( Position + Vector2Int.Right );
		if ( downleft && downright )
		{
			downleft = Game.Random.Float() > 0.5f;
			downright = !downleft;
		}

		var oldvel = Velocity;

		if ( downleft )
		{
			oldvel = Vector2Int.Left * Game.Random.Float( 1, 5 );
			//MoveCell( pos, pos + Vector2Int.Left, false );
		}
		else if ( downright )
		{
			oldvel = Vector2Int.Right * Game.Random.Float( 1, 5 );
			//MoveCell( pos, pos + Vector2Int.Right, false );
		}
		//using var _e = Profile.Scope( "MoveDownSides::Finalize" );
		if ( downleft || downright )
		{
			sleep = FinalizeMove( worker, Position, oldvel );
			return !sleep;
		}

		sleep = true;

		return downleft || downright;
	}
}
