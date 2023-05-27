namespace Sand.Systems.FallingSand;

[Element]
public class EmptyCell : Cell
{
	public EmptyCell()
	{
		CellColor = Color.Transparent;
		Density = 0;
		Heat = 293;
	}
}
