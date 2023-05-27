using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected bool MoveDown( Sandworker worker )
	{
		if ( !SandUtils.CanSwap( worker, this, Position + Vector2Int.Down ) )
		{
			float absy = Math.Abs( Velocity.y );
			if ( absy > 1 )
			{
				Velocity.x += Velocity.x == 0 ? ((absy / 10f) * Game.Random.Int( -1, 1 )) : 0;
			}
			else
			{
				Velocity.x *= 0.9f;
			}
			Velocity.y = 0;
			return false;
		}

		Velocity += Vector2.Down * 0.96f;
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
			Velocity += (Vector2)(dir1 * Vel1);
		}
		if ( right )
		{
			Velocity += (Vector2)(dir2 * Vel2);
		}
		return left || right;
	}
}
