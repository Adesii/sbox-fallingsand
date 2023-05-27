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
			return false;
		}
		if ( !SandUtils.IsAir( other ) )
		{
			float absy = Math.Abs( Velocity.y );
			if ( absy > 1 )
			{
				Velocity.x += Velocity.x == 0 ? ((absy / 6f) * Game.Random.Int( -1, 1 )) : 0;
				//Log.Info( Velocity );
			}
			else
			{
				Velocity.x *= 0.9f;
			}
			//convert some of the velocity to horizontal
			//Log.Info( "Same type" );
			//FinalizeMove( worker, this, Position, Velocity );
			Velocity.y = 0;
			//Velocity.x *= 0.5f;
			return false;
		}

		Velocity += Vector2.Down * 0.96f;

		/* if ( finalresult )
		{
			float absy = Math.Abs( Math.Max( Velocity.y / 10, 1f ) );
			//convert some of the velocity to horizontal
			Velocity.x += Velocity.x == 0 ? ((absy) * Game.Random.Int( -1, 1 )) : 0;
			Velocity.y = 0;
			Log.Info( Velocity );
			finalresult = FinalizeMove( worker, this, Position, Velocity );
		} */
		//FinalizeMove( worker, this, Position, Velocity );
		return true;
	}

	protected bool MoveDirection( Sandworker worker, Vector2Int dir1, Vector2Int dir2, float Vel1 = 1, float Vel2 = 1 )
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

		if ( left )
		{
			bool boyouend = (leftcell?.Density ?? 0) > Density;
			if ( boyouend )
			{
				QuickSwap( worker, Position, Position + dir1, this, leftcell );
				//leftcell.Velocity = ((Vector2)(dir1 * Vel1)).Perpendicular;
				return true;
			}
			Velocity += (Vector2)(dir1 * Vel1);
		}
		if ( right )
		{
			bool boyouend = (rightcell?.Density ?? 0) > Density;
			if ( boyouend )
			{
				QuickSwap( worker, Position, Position + dir2, this, rightcell );
				//rightcell.Velocity = ((Vector2)(dir2 * Vel2)).Perpendicular;
				return true;
			}
			Velocity += (Vector2)(dir2 * Vel2);
		}
		return (left || right);
	}
}
