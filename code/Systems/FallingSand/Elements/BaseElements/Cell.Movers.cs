using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected bool MoveDown( Sandworker worker )
	{
		//using var _b = Profile.Scope( "MoveDown" );
		var other = worker.GetCell( Position + Vector2Int.Down );

		bool boyouend = (other?.Density ?? 0) > Density;
		if ( boyouend && !SandUtils.IsAir( other ) )
		{
			QuickSwap( worker, Position, Position + Vector2Int.Down, this, other );
			return true;
		}
		if ( GetType() != other?.GetType() )
		{
			var oldvel = Velocity;
			oldvel += Vector2Int.Down;
			return FinalizeMove( worker, Position, oldvel );
		}
		return true;
	}

	protected bool MoveDirection( Sandworker worker, Vector2Int dir1, Vector2Int dir2, int Vel1 = 1, int Vel2 = 1 )
	{
		Cell leftcell = worker.GetCell( Position + dir1 );
		Cell rightcell = worker.GetCell( Position + dir2 );
		bool left = SandUtils.IsAir( leftcell ) || (leftcell?.Density ?? 0) > Density;
		bool right = SandUtils.IsAir( rightcell ) || (rightcell?.Density ?? 0) > Density;
		if ( left && right )
		{
			left = Game.Random.Float() > 0.5f;
			right = !left;
		}


		var oldvel = Velocity;

		if ( left )
		{
			bool boyouend = (leftcell?.Density ?? 0) > Density;
			if ( boyouend )
			{
				QuickSwap( worker, Position, Position + dir1, this, leftcell );
			}
			oldvel += dir1 * Vel1;
		}
		else if ( right )
		{
			bool boyouend = (rightcell?.Density ?? 0) > Density;
			if ( boyouend )
			{
				QuickSwap( worker, Position, Position + dir2, this, rightcell );
			}
			oldvel += dir2 * Vel2;
		}
		if ( left || right )
		{
			return FinalizeMove( worker, Position, oldvel );
		}

		return true;
	}

	protected bool MoveSides( Sandworker worker )
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
			return FinalizeMove( worker, Position, oldvel );
		}

		return true;
	}
}
