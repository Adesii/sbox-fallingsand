namespace Sand.Systems.FallingSand;

[Element]
public class EmptyCell : Cell
{
	public EmptyCell()
	{
		color = Color.Transparent;
		Density = 0;
	}
}
