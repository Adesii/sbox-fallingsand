using Sand.util;

namespace Sand.Systems.FallingSand;

public partial class Cell
{
	protected bool MoveDown( Sandworker worker )
	{
		Vector2 down = Vector2.Down; //GetGravityAtPosition( worker, this );
		if ( !SandUtils.CanSwap( worker, this, Position + GetGravityCheckDir( down ) ) )
		{
			// Convert Vertical Velocity to Horizontal Velocity on impact
			if ( ShouldBounceToSide )
			{
				float absy = Math.Abs( Velocity.y );
				if ( absy > 1 )
				{
					Velocity += down.Perpendicular * (absy / HorizontalConversion) * Game.Random.Int( -1, 1 );
				}
				else
				{
					Velocity.x *= 0.9f;
				}
				Velocity.y = 0;
			}
			return false;
		}

		Velocity += down * 2f;
		return true;
	}

	protected bool MoveDirection( Sandworker worker, Vector2 dir1, Vector2 dir2, float Vel1 = 1, float Vel2 = 1 )
	{
		Cell leftcell = worker.GetCell( Position + dir1.SnapToGrid( 1 ) );
		Cell rightcell = worker.GetCell( Position + dir2.SnapToGrid( 1 ) );
		bool left = SandUtils.IsAir( leftcell ) || (leftcell?.Density ?? 0) > Density;
		bool right = SandUtils.IsAir( rightcell ) || (rightcell?.Density ?? 0) > Density;
		if ( left && right )
		{
			left = Game.Random.Float() > 0.5f;
			right = !left;
		}

		if ( left )
		{
			Velocity += dir1 * Vel1;
		}
		if ( right )
		{
			Velocity += dir2 * Vel2;
		}
		return left || right;
	}
}
