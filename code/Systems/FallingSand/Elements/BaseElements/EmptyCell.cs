namespace Sand.Systems.FallingSand;

public class EmptyCell : Cell
{
	public EmptyCell()
	{
		color = Color.Transparent;
		Velocity = Vector2Int.Zero;
		Density = 0;
	}
}
