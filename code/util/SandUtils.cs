using Sand.Systems.FallingSand;

namespace Sand.util;

public static class SandUtils
{
	public static void PointToPointFunction( Vector2Int pos1, Vector2Int pos2, Action<Vector2Int> function )
	{
		// If the two points are the same no need to iterate. Just run the provided function
		if ( pos1 == pos2 )
		{
			function( pos1 );
			return;
		}

		int matrixX1 = pos1.x;
		int matrixY1 = pos1.y;
		int matrixX2 = pos2.x;
		int matrixY2 = pos2.y;

		int xDiff = matrixX1 - matrixX2;
		int yDiff = matrixY1 - matrixY2;
		bool xDiffIsLarger = Math.Abs( xDiff ) > Math.Abs( yDiff );

		int xModifier = xDiff < 0 ? 1 : -1;
		int yModifier = yDiff < 0 ? 1 : -1;

		int longerSideLength = Math.Max( Math.Abs( xDiff ), Math.Abs( yDiff ) );
		int shorterSideLength = Math.Min( Math.Abs( xDiff ), Math.Abs( yDiff ) );
		float slope = (shorterSideLength == 0 || longerSideLength == 0) ? 0 : ((float)shorterSideLength / longerSideLength);

		int shorterSideIncrease;
		for ( int i = 1; i <= longerSideLength; i++ )
		{
			shorterSideIncrease = (int)Math.Round( i * slope );
			int yIncrease, xIncrease;
			if ( xDiffIsLarger )
			{
				xIncrease = i;
				yIncrease = shorterSideIncrease;
			}
			else
			{
				yIncrease = i;
				xIncrease = shorterSideIncrease;
			}
			int currentY = matrixY1 + (yIncrease * yModifier);
			int currentX = matrixX1 + (xIncrease * xModifier);
			Vector2Int v = new( currentX, currentY );
			function( v );
		}
	}

	public static bool CanSwap( Sandworker worker, Cell a, Vector2Int swappos )
	{
		Cell b = worker.GetCell( swappos );
		if ( b == null )
		{
			return false;
		}
		if ( b.IsAir() )
		{
			return true;
		}
		if ( a.GetType() == b.GetType() )
		{
			return false;
		}
		return a.Density < b.Density;

	}

	internal static bool IsAir( this Cell leftcell )
	{
		return leftcell is EmptyCell;
	}
}

