using System;

namespace Sand.Systems.FallingSand.Elements;

[Element]
public class Coal : MovableSolid, IFlamable
{
	protected override bool ShouldBounceToSide { get => true; set => base.ShouldBounceToSide = value; }
	protected override float HorizontalConversion => 40f;


	public override float Inertia { get; set; } = 0.85f;

	public float Flammability => 0.3f;

	public Coal()
	{
		CellColor = Color.FromBytes( 30, 30, 30, 255 ).Darken( 0.5f ).Lighten( Game.Random.Float( 0.6f, 1.4f ) );
		Density = 1;
	}

}
